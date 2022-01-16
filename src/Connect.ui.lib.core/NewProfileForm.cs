using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using core.audiamus.aux;
using core.audiamus.aux.ex;
using core.audiamus.aux.win;
using core.audiamus.common;
using R = core.audiamus.connect.ui.Properties.Resources;
using static core.audiamus.aux.Logging;

namespace core.audiamus.connect.ui {
  public partial class NewProfileForm : Form {

    private static IReadOnlyDictionary<ERegion, string> __regions = new Dictionary<ERegion, string> {
       { ERegion.de, R.de },
       { ERegion.us, R.us },
       { ERegion.uk, R.uk },
       { ERegion.fr, R.fr },
       { ERegion.ca, R.ca },
       { ERegion.it, R.it },
       { ERegion.au, R.au },
       { ERegion.@in, R.in_ },
       { ERegion.jp, R.jp },
       { ERegion.es, R.es },
     };

    private string LoginUrl => textBox1.Text;
    private bool _ignoreFlag;
    private bool _loginUrlUsed;
    private readonly IEnumerable<IProfileKeyEx> _profiles;
    private readonly AffineSynchronizationContext _sync;
    private AudibleClient Client { get; }

    public IProfileKeyEx ProfileKey { get; private set; }

    public NewProfileForm (AudibleClient client, IEnumerable<IProfileKeyEx> profiles) {
      Log (3, this);

      InitializeComponent ();
      Client = client;
      _profiles = profiles;
      _sync = new ();

      using var _ = new ResourceGuard (x => _ignoreFlag = x);

      comBoxRegion.DataSource = Enum.GetValues<ERegion> ()
        .Select (r => getRegionName(r))
        .ToList();
      comBoxRegion.SelectedIndex = -1;

      string ttt = toolTip1.GetToolTip (panelCreateUrl2);
      foreach (Control ctrl in panelCreateUrl2.Controls)
        toolTip1.SetToolTip (ctrl, ttt);
    }

    private void enable(bool? loginUrlUsed = null) {
      if (loginUrlUsed.HasValue)
        _loginUrlUsed = loginUrlUsed.Value;
      bool noProfile = ProfileKey is null;
      panelRegion.Enabled = noProfile;
      panelCreateUrl.Enabled = comBoxRegion.SelectedIndex >= 0 && noProfile;
      panelCreateUrl1.Enabled = LoginUrl.IsNullOrWhiteSpace();
      panelCreateUrl2.Enabled = !LoginUrl.IsNullOrWhiteSpace();
      panelPasteUrl.Enabled = _loginUrlUsed && noProfile;
      panelResult.Visible = !noProfile;
      btnOK.Enabled = !noProfile;
    }

    private bool allowPreAmazonAccount () {
      ERegion? region = null;
      if (comBoxRegion.SelectedIndex >= 0)
        region = (ERegion)comBoxRegion.SelectedIndex;
      bool allowPreAmazon = allowPreAmazonAccount (region);
      return allowPreAmazon;
    }

    private static bool allowPreAmazonAccount (ERegion? region) {
      return region == ERegion.de || region == ERegion.uk || region == ERegion.us;
    } 

    private static string getRegionName (ERegion region) {
      if (!__regions.TryGetValue (region, out string name))
        return region.ToString ();
      return name;
    }

    private bool deregisterDeviceConfirmation (IProfileKeyEx key) {
      return false;
      //string msg = string.Format (R.MsgDeregisterDevice, key.DeviceName, key.Region, key.AccountName);
      //var result = MsgBox.Show (this, msg, this.Text, MessageBoxButtons.YesNo, 
      //  MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

      //return result == DialogResult.Yes;
    }

    private bool getAccountAlias (AccountAliasContext ctxt) {
      return _sync.Send (getAccountAliasSync, ctxt);

      static bool getAccountAliasSync (AccountAliasContext ctxt) {
        var dlg = new AccountAliasForm (ctxt);
        dlg.ShowDialog ();
        return true;
      }
    }


    private void reinit () {
      if (_ignoreFlag)
        return;

      bool allowPreAmazon = allowPreAmazonAccount ();
      ckBoxPreAmazonUsername.Enabled = allowPreAmazon;

      textBox1.Text = null;
      enable (false);
    }

    private void comBoxRegion_SelectedIndexChanged (object sender, EventArgs e) => 
      reinit ();

    private void btnCreateUrl_Click (object sender, EventArgs e) {
      Log (3, this);
      ERegion region = (ERegion)comBoxRegion.SelectedIndex;
      bool preAmazonAccount = allowPreAmazonAccount (region) && ckBoxPreAmazonUsername.Checked;

      if (_profiles is not null) {
        var names = _profiles.Where (p => p.Region == region).Select (p => p.AccountName).ToList ();
        string msgSrc;
        if (names.Count > 0) {
          if (names.Count == 1)
            msgSrc = R.MsgExistingAccount;
          else
            msgSrc = R.MsgExistingAccounts;
          string msg = string.Format (msgSrc, region, names.Combine (','));

          MsgBox.Show (this, msg, R.CptExistingProfiles, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
      }

      Uri uri = Client.ConfigBuildNewLoginUri (region, preAmazonAccount);

      textBox1.Text = uri.ToString ();

      enable ();
    }

    private void textBox1_MouseClick (object sender, MouseEventArgs e) {
      Log (3, this);
      enable (true);
    }

    private void btnOpenBrowser_Click (object sender, EventArgs e) {
      Log (3, this);
      ShellExecute.File (LoginUrl);
      enable (true);
    }

    private void btnCopy_Click (object sender, EventArgs e) {
      Log (3, this);
      Clipboard.SetText (LoginUrl);
      enable (true);
    }

    private void btnOK_Click (object sender, EventArgs e) {
      Log (3, this);
      DialogResult = DialogResult.OK;
      Close ();
    }

    private async void textBox2_TextChanged (object sender, EventArgs e) {
      using var _ = new LogGuard (3, this);

      Cursor cursor = Cursor.Current;
      Cursor.Current = Cursors.WaitCursor;
      var rg = new ResourceGuard (() => Cursor.Current = cursor);

      string finalUrl = textBox2.Text;
      bool succ = Uri.TryCreate (finalUrl, UriKind.Absolute, out Uri uri);
      if (!succ) {
        MsgBox.Show (this, R.MsgNewProfileInvalidUrl, this.Text, 
          MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      var callbacks = new Callbacks {
        DeregisterDeviceConfirmCallback = deregisterDeviceConfirmation,
        GetAccountAliasFunc = getAccountAlias
      };

      var result = await Client.ConfigParseExternalLoginResponseAsync (uri, callbacks);
      var key = result.NewProfileKey;
      string msg;
      Log (3, this, () => $"result={result.Result}");

      switch (result.Result) {
        case EAuthorizeResult.succ:
          msg = string.Format (R.MsgNewProfileSucc, key?.Region, key?.AccountName, key?.DeviceName);
          MsgBox.Show (this, msg, 
            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
          break;
        case EAuthorizeResult.authorizationFailed:
          MsgBox.Show (this, R.MsgNewProfileAuthorizationFailed, 
            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
          break;
        case EAuthorizeResult.registrationFailed:
          MsgBox.Show (this, R.MsgNewProfileRegistrationFailed, 
            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
          break;
        case EAuthorizeResult.deregistrationFailed:
          msg = string.Format (R.MsgNewProfileDeregistrationFailed, 
            key?.Region, key?.AccountName, key?.DeviceName, result.PrevDeviceName);
          MsgBox.Show (this, msg, 
            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
          break;
      }

      if (result.Result < EAuthorizeResult.succ)
        return;

      ProfileKey = key;

      lblRegion.Text = key.Region.ToString ();
      lblAccount.Text = key.AccountName;
      lblDevice.Text = key.DeviceName;
      enable ();
      
    }

    private void ckBoxPreAmazonUsername_CheckedChanged (object sender, EventArgs e) => 
      reinit ();
  }
}
