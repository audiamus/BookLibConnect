using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using core.audiamus.aux;
using core.audiamus.aux.diagn;
using core.audiamus.aux.ex;
using core.audiamus.aux.win;
using core.audiamus.aux.win.ex;
using core.audiamus.booksdb;
using core.audiamus.connect.ui;
using core.audiamus.util;
using static core.audiamus.aux.ApplEnv;
using static core.audiamus.aux.Logging;
using R = core.audiamus.connect.app.gui.Properties.Resources;

namespace core.audiamus.connect.app.gui {
  public partial class MainForm : Form {
    private bool _ignoreFlag;
    private CancellationTokenSource _cts;
    private readonly AaxExporter _exporter;

    private IProfileAliasKey CurrentProfile { get; set; }
    private AudibleClient AudibleClient { get; set; }
    private IAudibleApi Api { get; set; }
    private AppSettings AppSettings { get; } 
    private UserSettings UserSettings { get; } 
    private AffineSynchronizationContext SynchronizationContext { get; }
    private ProgressProcessor1 ProgressProcessor1 { get; }
    private ProgressProcessor2 ProgressProcessor2 { get; }
    private readonly InteractionCallbackHandler<UpdateInteractionMessage> _interactionHandler;
    private readonly SystemMenu _systemMenu;
    private bool _initDone;
    private bool _libUpdated;
    private bool _updateAvailableFlag;
    private WaitForm _waitForm;


    public MainForm () {
      InitializeComponent ();

      Log (1, this, () => 
        $"{ApplName} {AssemblyVersion} as {(Is64BitProcess ? "64" : "32")}bit process" +
        $" on Windows {OSVersion} {(Is64BitOperatingSystem ? "64" : "32")}bit");

      this.Text = ApplEnv.AssemblyTitle;

      _systemMenu = new SystemMenu (this);
      _systemMenu.AddCommand (R.SysMenuItemAbout, onSysMenuAbout, true);
      _systemMenu.AddCommand ($"{R.SysMenuItemHelp}\tF1", onSysMenuHelp, false);
      
      _interactionHandler = new InteractionCallbackHandler<UpdateInteractionMessage> (this, updateInteractMessage);

      AppSettings = SettingsManager.GetAppSettings<AppSettings> ();

      UserSettings = SettingsManager.GetUserSettings<UserSettings> ();
      if (UserSettings?.ExportSettings is not null)
        UserSettings.ExportSettings.ChangedSettings += exportSettings_ChangedSettings;

      SynchronizationContext = new AffineSynchronizationContext();
      
      ProgressProcessor1 = new ProgressProcessor1 (progressBarConvert);
      ProgressProcessor2 = new ProgressProcessor2 (progressBarItems, progressBarDnld);


      using (new ResourceGuard (x => _ignoreFlag = x)) {
        ckBoxRefresh.Checked = UserSettings.DownloadSettings.AutoRefresh;
        ckBoxUpdLib.Checked = UserSettings.DownloadSettings.AutoUpdateLibrary;
        ckBoxOpenDlg.Checked = UserSettings.DownloadSettings.AutoOpenDownloadDialog;
        ckBoxMultiPart.Checked = UserSettings.DownloadSettings.MultiPartDownload;
        ckBoxAdult.Checked = UserSettings.DownloadSettings.IncludeAdultProducts;
        ckBoxKeepEncrypted.Checked = UserSettings.DownloadSettings.KeepEncryptedFiles;
     
        comBoxSort.DataSource = Enum.GetValues<EInitialSorting> ()
          .Select(v => v.ToString().Replace("_", ", "))
          .ToArray();
        comBoxSort.SelectedIndex = (int)UserSettings.DownloadSettings.InitialSorting;

        ckBoxExport.Checked = UserSettings.ExportSettings.ExportToAax ?? false;
      }

      convertdgvControl1.DownloadOnlyMode = true;
      convertdgvControl1.DownloadSettings = UserSettings.DownloadSettings;
      convertdgvControl1.ExportSettings = UserSettings.ExportSettings;
      btnConvert.Enabled = false;
      btnAbort.Enabled = false;
      btnProfiles.Enabled = convertdgvControl1.IsIdle;

      convertdgvControl1.SelectionChanged += convertdgvControl1_SelectionChanged;
      convertdgvControl1.IdleChanged += convertdgvControl1_IdleChanged;

      _exporter = new AaxExporter (UserSettings.ExportSettings, UserSettings.DownloadSettings);

      Cursor.Current = Cursors.WaitCursor;
    }

    protected override void WndProc (ref Message m) {
      base.WndProc (ref m);
      _systemMenu.HandleMessage (ref m);
    }

    protected override void OnKeyDown (KeyEventArgs e) {
      if (e.Modifiers == Keys.Control) {
        //if (e.KeyCode == Keys.A) {
        //  e.SuppressKeyPress = true;
        //  selectAll ();
        //} else
        base.OnKeyDown (e);
      } else {
        if (e.KeyCode == Keys.F1) {
          e.SuppressKeyPress = true;
          onSysMenuHelp ();
        } else
          base.OnKeyDown (e);
      }
    }

    protected override void OnClosing (CancelEventArgs e) {
      using var _ = new LogGuard (3, this);
      
      base.OnClosing (e);
      UserSettings?.Save ();

      if (_updateAvailableFlag) {
        e.Cancel = true;
        handleDeferredUpdateAsync ();
      }

    }

    protected override async void OnLoad (EventArgs e) {
      using var _ = new LogGuard (3, this);

      await Task.Delay (100);

      Log (3, this, () =>
        UserSettings.Dump (EDumpFlags.byInterface | EDumpFlags.inherInterfaceAttribs | EDumpFlags.inclNullVals));


      base.OnLoad (e);
      //Cursor.Current = Cursors.WaitCursor;
      Enabled = false;


      _waitForm = new WaitForm () {
        Owner = this
      };
      _waitForm.SetStartPositionCentered ();
      _waitForm.Show ();

    }

    protected async override void OnShown (EventArgs e) {

      base.OnShown (e);
      if (_initDone)
        return;

      using var _ = new LogGuard (3, this);

      checkOnlineUpdate ();

      await init ();

      _initDone = true;
      Enabled = true;
      UseWaitCursor = false;
      //Cursor.Current = Cursors.Default;
      enable (false);

      _waitForm?.Close ();
      _waitForm = null;
    }

    private async Task init () {
      using var _ = new LogGuard (3, this);

      AudibleClient = new AudibleClient (UserSettings.ConfigSettings, UserSettings.DownloadSettings);
      Log (4, this, () => $"before wizard, {Cursor.Current}");

      await runWizardAsync ();
      //Cursor.Current = Cursors.WaitCursor;

      using (new ResourceGuard (x => {
        if (_waitForm is not null) {
          if (x)
            _waitForm.LabelTask.Text = R.MsgInitDB;
          else
            _waitForm.LabelTask.Text = null;
        }
        })) {

        Log (4, this, () => $"before db, {Cursor.Current}");
        await BookDbContextLazyLoad.StartupAsync ();
      }

      //Cursor.Current = Cursors.WaitCursor;

      _exporter.BookLibrary = AudibleClient.BookLibraryExcerpt;

      Log (4, this, () => $"before config/refresh, {Cursor.Current}");
      CurrentProfile = await AudibleClient.ConfigFromFileAsync (
        UserSettings.DownloadSettings?.Profile,
        getAccountAlias
      );
      //Cursor.Current = Cursors.WaitCursor;

      if (CurrentProfile is null)
        return;

      UserSettings.DownloadSettings.Profile = new (CurrentProfile);
      UserSettings.Save ();

      //Cursor.Current = Cursors.WaitCursor;
      Log (4, this, () => $"before init lib, {Cursor.Current}");
      await initLibraryAsync ();
      Log (4, this, () => $"all done, {Cursor.Current}");

    }

    private async Task runWizardAsync () {

      SimpleWizard wizard = new SimpleWizard {
        Text = $"{ApplEnv.AssemblyTitle} {R.CptWizard}"
      };

      // if no config add config page
      var profiles = await AudibleClient.GetProfilesAsync ();
      if (profiles.IsNullOrEmpty ())
        wizard.AddPage (new WizStepProfile (AudibleClient));

      // if no download dir add download page
      if (UserSettings.DownloadSettings.DownloadDirectory.IsNullOrWhiteSpace())
        wizard.AddPage (new WizStepDownload (setDownloadDir));

      // if no export add export page
      var es = UserSettings.ExportSettings;
      if (!es.ExportToAax.HasValue || (es.ExportToAax.Value && es.ExportDirectory is null))
        wizard.AddPage (new WizStepExport (setExportDir, UserSettings.ExportSettings));

      if (wizard.Pages.Any ())
        wizard.AddPage (new WizStepHelp (onSysMenuHelp));
      else
        return;

      Log (3, this, () => $"#pages={wizard.Pages.Count()}");
     
      _waitForm?.Hide ();
      var result = wizard.ShowDialog ();

      if (result != DialogResult.OK)
        MsgBox.Show (this, R.MsgWzardWarning, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning); 
      
      _waitForm?.Show ();
      await Task.Delay (100);
    }

    private async Task initLibraryAsync () {
      var s = UserSettings.DownloadSettings;
      using var _ = new LogGuard (3, this, () => $"settings: auto refresh={s.AutoRefresh}, auto update={s.AutoUpdateLibrary}");

      Api = AudibleClient.Api;

      Api.GetAccountAliasFunc = getAccountAlias;

      if (s.AutoUpdateLibrary && !_libUpdated) {
        if (!s.AutoRefresh)
          await Api.RefreshTokenAsyncFunc ();
        using (new ResourceGuard (x => {
          if (_waitForm is not null) {
            if (x)
              _waitForm.LabelTask.Text = R.MsgInitLib;
            else
              _waitForm.LabelTask.Text = null;
          }
        })) {

          await Api.GetLibraryAsync ();

          if (_waitForm is not null)
            _waitForm.LabelTask.Text = R.MsgDnldCoverImg;

          await Api.DownloadCoverImagesAsync ();
        }
        _libUpdated = true;
      }

      convertdgvControl1.AudibleApi = Api;
    }

    private void onSysMenuAbout () => new AboutForm () { Owner = this }.ShowDialog ();

    private void onSysMenuHelp () {
      const string PDF = ".pdf";

      string uiCultureName = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
      string neutralDocFile = ApplEnv.ApplName + PDF;

      var sb = new StringBuilder ();
      sb.Append (ApplEnv.ApplName);
      //if (uiCultureName != NeutralCultureName)
      //  sb.Append ($".{uiCultureName}");
      sb.Append (PDF);
      string localizedDocFile = sb.ToString ();

      var filename = Path.Combine (ApplEnv.ApplDirectory, localizedDocFile);
      if (!File.Exists (filename))
        filename = Path.Combine (ApplEnv.ApplDirectory, neutralDocFile);

      try {
        ShellExecute.File (filename);
      } catch (Exception) {
      }

    }

    private OnlineUpdate newOnlineUpdate () => 
      new OnlineUpdate (UserSettings.UpdateSettings, ApplEnv.ApplName, null, AppSettings.DbgOnlineUpdate);

    private async void checkOnlineUpdate () {
      var update = newOnlineUpdate ();
      await update.UpdateAsync (_interactionHandler, () => Application.Exit (), isBusyForUpdate);
    }


    private async void handleDeferredUpdateAsync () {
      var update = newOnlineUpdate ();
      await update.InstallAsync (_interactionHandler, () => Application.Exit ());
    }

    private bool isBusyForUpdate () {
      bool busy = !convertdgvControl1.IsIdle;
      if (busy)
        _updateAvailableFlag = true;
      return busy;
    }

    private string updateInteractMessage (UpdateInteractionMessage uim) {
      string msg = null;
      var pi = uim.pckInfo;
      if (!pi.DefaultApp)
        return null;
      switch (uim.Kind) {
        case EUpdateInteract.newVersAvail:
          msg = string.Format (R.MsgOnlineUpdateDownload, 
            pi.Version, prev(pi.Preview), pi.AppName, desc (pi.Desc));
          break;
        case EUpdateInteract.installNow:
          msg = string.Format (
            R.MsgOnlineUpdateInstallNow, 
            pi.Version, prev(pi.Preview), pi.AppName);
          break;
        case EUpdateInteract.installLater:
          msg = string.Format (
            R.MsgOnlineUpdateInstallLater, 
            pi.Version, prev(pi.Preview), pi.AppName);
          break;
      }
      return msg;

      static string prev (bool prv) => prv ? $" ({R.MsgOnlineUpdatePreview})" : string.Empty;
      static string desc (string dsc) => !dsc.IsNullOrWhiteSpace () ? $"\r\n\"{dsc}\"" : string.Empty;

    }


    private bool getAccountAlias (AccountAliasContext ctxt) {
      using var _ = new LogGuard (3, this);
      return SynchronizationContext.Send (getAccountAliasSync, ctxt);
      
      bool getAccountAliasSync (AccountAliasContext ctxt) {
        Log (4, this, () => $"before dlg, {Cursor.Current}");
        var dlg = new AccountAliasForm (ctxt);
        dlg.ShowDialog ();

        //if (!_initDone)
        //  Cursor.Current = Cursors.WaitCursor;

        Log (4, this, () => $"after dlg, {Cursor.Current}");
        return true;
      }
    }

    private void exportSettings_ChangedSettings (object sender, EventArgs e) {
      if (UserSettings.ExportSettings.ExportToAax.HasValue)
        ckBoxExport.Checked = UserSettings.ExportSettings.ExportToAax.Value;
    }

    private void convertdgvControl1_IdleChanged (object sender, BoolEventArgs args) {
      btnProfiles.Enabled = args.Value;
    }

    private void convertdgvControl1_SelectionChanged (object sender, EventArgs e) {
      resetProgressBars ();
      btnConvert.Enabled = convertdgvControl1.SelectedConversions.Any () &&
        Directory.Exists (UserSettings.DownloadSettings.DownloadDirectory) &&
        (!(UserSettings.ExportSettings.ExportToAax ?? false) ||
          Directory.Exists (UserSettings.ExportSettings.ExportDirectory));
    }

    private async void ckBoxRefresh_CheckedChanged (object sender, EventArgs e) {
      checkedChanged (sender, s => UserSettings.DownloadSettings.AutoRefresh = s);
      if (!_ignoreFlag && UserSettings.DownloadSettings.AutoRefresh) {
        Cursor cursor = Cursor.Current;
        Cursor.Current = Cursors.AppStarting;
        var rg = new ResourceGuard (() => Cursor.Current = cursor);
        await Api?.RefreshTokenAsyncFunc ();
      }
    }

    private async void ckBoxUpdLib_CheckedChanged (object sender, EventArgs e) {
      checkedChanged (sender, s => UserSettings.DownloadSettings.AutoUpdateLibrary = s);
      if (!_ignoreFlag && UserSettings.DownloadSettings.AutoUpdateLibrary) {
        Cursor cursor = Cursor.Current;
        Cursor.Current = Cursors.AppStarting;
        var rg = new ResourceGuard (() => Cursor.Current = cursor);
        await initLibraryAsync ();
      }
    }

    private void ckBoxOpenDlg_CheckedChanged (object sender, EventArgs e) =>
      checkedChanged (sender, s => UserSettings.DownloadSettings.AutoOpenDownloadDialog = s);

    private void ckBoxMultiPart_CheckedChanged (object sender, EventArgs e) =>
      checkedChanged (sender, s => UserSettings.DownloadSettings.MultiPartDownload = s);

    private void ckBoxAdult_CheckedChanged (object sender, EventArgs e) =>
      checkedChanged (sender, s => UserSettings.DownloadSettings.IncludeAdultProducts = s);

    private void ckBoxKeepEncrypted_CheckedChanged (object sender, EventArgs e) =>
      checkedChanged (sender, s => UserSettings.DownloadSettings.KeepEncryptedFiles = s);

    private void checkedChanged (object sender, Action<bool> setAction) {
      if (_ignoreFlag || sender is not CheckBox cb)
        return;
      setAction (cb.Checked);
      onChangedSettings ();
    } 

    private void comBoxSort_SelectedIndexChanged (object sender, EventArgs e) {
      if (_ignoreFlag)
        return;
      UserSettings.DownloadSettings.InitialSorting = (EInitialSorting)comBoxSort.SelectedIndex;
      onChangedSettings ();
    }


    private void ckBoxExport_CheckedChanged (object sender, EventArgs e) {
      if (_ignoreFlag)
        return;
      UserSettings.ExportSettings.ExportToAax = ckBoxExport.Checked;
    }


    private void onChangedSettings () {
      UserSettings.DownloadSettings.OnChange ();
      Log (3, this, () => 
        UserSettings.Dump (EDumpFlags.byInterface | EDumpFlags.inherInterfaceAttribs | EDumpFlags.inclNullVals));
    }

    private void convertAction (Book book, ConversionContext context, Action<Conversion> onNewStateCallback ) {
      if (!(UserSettings.ExportSettings.ExportToAax ?? false))
        return;
      using var _ = new LogGuard (3, this, () => book.ToString ());
      _exporter.Export (book, new SimpleConversionContext(context.Progress, context.CancellationToken), onNewStateCallback);
    }

    private async void btnConvert_Click (object sender, EventArgs e) {
      using var lg = new LogGuard (3, this);
      using var rg = new ResourceGuard (x => enable (x));

      resetProgressBars ();

      _cts = new CancellationTokenSource ();

      var context = new ConversionContext (ProgressProcessor1.Progress, _cts.Token);
      context.Init (convertdgvControl1.SelectedConversions.Count ());

      using var job = new DownloadDecryptJob<ConversionContext> (
        Api,
        UserSettings.DownloadSettings,
        convertdgvControl1.ResetDataSourceItem);
      await job.DownloadDecryptAndConvertAsync (
        convertdgvControl1.SelectedConversions,
        ProgressProcessor2.Progress,
        context,
        convertAction
      );

      timer1.Enabled = true;
    
    }


    private void resetProgressBars () {
      timer1.Enabled = false;
      progressBarItems.Value = 0;
      progressBarDnld.Value = 0;
      progressBarConvert.Value = 0;
    }

    private void btnAbort_Click (object sender, EventArgs e) {
      Log (3, this);
      _cts?.Cancel ();
      enable (false);
    }

    private void enable (bool running) {
      convertdgvControl1.Enabled = !running;
      btnConvert.Enabled = !running;
      btnAbort.Enabled = running;
      panelTop.Enabled = !running;
    }

    private void btnDnldDir_Click (object sender, EventArgs e) =>
      setDownloadDir ();

    private void btnExprtDir_Click (object sender, EventArgs e) =>
      setExportDir ();

    private bool setDownloadDir () =>
      setDirDialog (
        () => UserSettings.DownloadSettings.DownloadDirectory,
        s => UserSettings.DownloadSettings.DownloadDirectory = s,
        @"Audible\Download",
        R.CptSelectFolder + "Download"
    );

    private bool setExportDir () =>
      setDirDialog (
        () => UserSettings.ExportSettings.ExportDirectory,
        s => UserSettings.ExportSettings.ExportDirectory = s,
        @"Audible\Export",
        R.CptSelectFolder + "Export"
    );

    private bool setDirDialog (Func<string> dirSetSrc, Action<string> dirSetDst, string defSubDir, string caption) {
      string olddir = dirSetSrc();
      string newdir = setDirDialog (olddir, defSubDir, caption);
      if (!newdir.IsNullOrEmpty () && newdir != olddir) {
        dirSetDst (newdir);
        UserSettings.Save ();
        convertdgvControl1_SelectionChanged (this, EventArgs.Empty);
        return true;
      }
      return false;
    }

    private static string setDirDialog (string olddir, string defaultSubPath, string caption) {
      string dir = olddir;
      if (dir.IsNullOrWhiteSpace () ||
         !Directory.Exists (dir)) {
        string defdir = Environment.GetFolderPath (Environment.SpecialFolder.MyMusic);

        dir = Path.Combine (defdir, defaultSubPath);
        Directory.CreateDirectory (dir);
      }

      var dlg = new FolderBrowserDialog () {
        SelectedPath = dir,
        Description = caption,
        UseDescriptionForTitle = true
      };
      var result = dlg.ShowDialog ();
      if (result == DialogResult.OK)
        return dlg.SelectedPath;
      else
        return dir;
    }

    private async void btnProfiles_Click (object sender, EventArgs e) {
      var dlg = new ManageProfilesForm (AudibleClient);
      var result = dlg.ShowDialog ();
      if (result != DialogResult.OK)
        return;

      var prevProfile = CurrentProfile;
      CurrentProfile = AudibleClient.ProfileAliasKey;

      if (CurrentProfile == prevProfile)
        return;

      if (CurrentProfile is null) {
        convertdgvControl1.AudibleApi = null;
        UserSettings.DownloadSettings.Profile = null;
        UserSettings.Save ();
        return;
      }

      UserSettings.DownloadSettings.Profile = new (CurrentProfile);
      UserSettings.Save ();

      using var rg = new ResourceGuard (x => UseWaitCursor = x); 
      await initLibraryAsync ();
    }

    private void timer1_Tick (object sender, EventArgs e) {
      resetProgressBars ();
    }
  }
}
