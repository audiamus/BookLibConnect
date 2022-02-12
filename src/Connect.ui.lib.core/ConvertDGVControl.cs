using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using core.audiamus.aux;
using core.audiamus.aux.ex;
using core.audiamus.aux.win;
using core.audiamus.aux.win.ex;
using core.audiamus.booksdb;
using abl = core.audiamus.booksdb;
using static core.audiamus.aux.Logging;

namespace core.audiamus.connect.ui {
  public partial class ConvertDGVControl : UserControl {
    private bool _ignoreFlag;
    private bool _snapshotFlag;
    private bool _wasIdle;
    private bool _initConnectionDone;
    private IAudibleApi _audibleApi;
    private IDownloadSettings _downloadSettings;
    private readonly AffineSynchronizationContext _sync;
    private readonly HashSet<abl.Component> _componentsForUpdate = new();
    private readonly List<Conversion> _selectedConversions = new();

    private SortableBindingListSuspensible<ConversionDataSource> DataSourceDownload { get; set; }
    private SortableBindingListSuspensible<object> DataSourceLocal { get; set; }

    private BookLibForm BookLibForm { get; set; }

    public event EventHandler SelectionChanged;
    public event BoolEventHandler IdleChanged;

    public bool PartiallyDisabled {
      get => !panel1.Enabled;
      set {
        panel1.Enabled = !value;
        dataGridView1.ClientAreaEnabled = !value;
        if (BookLibForm is not null)
          BookLibForm.DownloadSelectEnabled = !value;
        if (value)
          snapshotSelection (true);
      }
    }

    public IAudibleApi AudibleApi {
      private get => _audibleApi;
      set {
        _audibleApi = value;
        startWithApi ();
      }
    }

    public IDownloadSettings DownloadSettings {
      private get => _downloadSettings;
      set {
        _downloadSettings = value;
        _downloadSettings.ChangedSettings += settings_ChangedSettings;
      }
    }

    public IExportSettings ExportSettings { private get; set; }

    public bool IsIdle => BookLibForm is null && !(DataSourceDownload?.Any () ?? false);

    public IEnumerable<Conversion> SelectedConversions => _selectedConversions;

    //public new bool Enabled {
    //  get => base.Enabled;
    //  set {
    //    if (BookLibForm is not null)
    //      BookLibForm.DownloadSelectEnabled = value;
    //    base.Enabled = value;
    //    if (!value)
    //      snapshotSelection (true);
    //  }
    //}

    public bool DownloadOnlyMode {
      get => !btnAdd.Visible;
      set {
        btnAdd.Visible = !value;
        if (value) {
          btnDnload.Left = btnAdd.Left;
          btnDnload.Anchor = AnchorStyles.Left | AnchorStyles.Top;
        } else {
          btnDnload.Left = this.Width - btnDnload.Width - 3;
          btnDnload.Anchor = AnchorStyles.Right | AnchorStyles.Top;
        }
      }
    }

    public ConvertDGVControl () {
      InitializeComponent ();

      _sync = new ();

      enable ();
    }

    public void ResetDataSourceItem (Conversion conversion) {
      _sync.Send (resetDataSourceItem, conversion);
    }


    private void resetDataSourceItem (Conversion conversion) {
      int? i = DataSourceDownload.Select ((s, i) => new { s, i }).FirstOrDefault (k => k.s.DataSource == conversion)?.i;
      if (i.HasValue)
        DataSourceDownload.ResetItem (i.Value);
      BookLibForm?.UpdateConversion (conversion);
    }

    private void startWithApi () {
      bool ena = enableDnload ();
      if (ena && DownloadSettings.AutoOpenDownloadDialog)
        btnDnload_Click (this, EventArgs.Empty);
    }

    private void settings_ChangedSettings (object sender, EventArgs e) {
      IEnumerable<Book> previousBooks = getCurrentDownloadBooks ();
      if (previousBooks is null)
        return;

      if (!DownloadSettings.IncludeAdultProducts)
        previousBooks = previousBooks.Where (b => !(b.AdultProduct ?? false));

      resetDataSource (previousBooks);
      timer1.Enabled = true;
    }

    private void updateDownloadDataSource (IEnumerable<Book> books) {
      if (_ignoreFlag)
        return;

      using var rg = new ResourceGuard (x => _ignoreFlag = x);

      if (books is null)
        return;

      if (DataSourceDownload is null) {
        resetDataSource (books);
      } else {
        IEnumerable<Book> previousBooks = getCurrentDownloadBooks ();
        addDownloadBooks (books, previousBooks);
        removeDownloadBooks (books, previousBooks);
      }

      _snapshotFlag = true;
      timer1.Enabled = true;
    }

    private void resetDataSource (IEnumerable<Book> books) {
      var convDS = getDownloadConversions (books);
      fillMetaInfoGaps (convDS);

      DataSourceDownload = new SortableBindingListSuspensible<ConversionDataSource> (convDS) {
        UseBackingProperties = true,
        UseResorting = true
      };
      DataSourceDownload.ListChanged += dataSourceDownload_ListChanged;
      DataSourceDownload.EndSorting += dataSourceDownload_EndSorting;
      this.dataGridView1.DataSource = DataSourceDownload;
    }

    private void dataSourceDownload_ListChanged (object sender, ListChangedEventArgs e) {
      timer1.Enabled = true;
    }

    private void dataSourceDownload_EndSorting (object sender, EventArgs e) {
      timer1.Enabled = true;
    }

    private IEnumerable<Book> getCurrentDownloadBooks () =>
      DataSourceDownload?.Select (c => c.Conversion.ParentBook).Distinct ();

    private void addDownloadBooks (IEnumerable<Book> books, IEnumerable<Book> previousBooks) {
      IEnumerable<Book> booksAdding = books.Except (previousBooks);
      var convDSAdd = getDownloadConversions (booksAdding);
      if (convDSAdd.Any ()) {
        fillMetaInfoGaps (convDSAdd);
        if (convDSAdd.Count () > 1) {
          DataSourceDownload.AddRange (convDSAdd);
        } else {
          DataSourceDownload.Add (convDSAdd.First ());
        }
      }
    }

    private void fillMetaInfoGaps (IEnumerable<ConversionDataSource> conversionsToAdd) {
      using var _ = new LogGuard (3, this);
      var conversions = conversionsToAdd
        .Where (c => c.Conversion.Component is not null &&
                     c.Conversion.Component.RunTimeLengthSeconds is null &&
                     !_componentsForUpdate.Contains (c.Conversion.Component))
        .ToList ();

      if (!conversions.Any ())
        return;

      var components = conversions.Select (c => c.Conversion.Component).ToList ();
      components.ForEach (c => _componentsForUpdate.Add (c));

      Task.Run (async () => await AudibleApi.UpdateMetaInfo (components, onDone));



      void onDone (IEnumerable<abl.Component> comps) => _sync.Post (onDoneSync, comps);

      void onDoneSync (IEnumerable<abl.Component> comps) {
        Log (3, this, () => $"#comps={comps.Count ()}");
        foreach (var comp in comps)
          _componentsForUpdate.Remove (comp);

        if (DataSourceDownload.IsNullOrEmpty ())
          return;

        if (comps.Count () > 1) {
          using var rg = new ResourceGuard (x => DataSourceDownload.Suspended = x);
          update (comps, conversions);
        } else
          update (comps, conversions);

        void update (IEnumerable<abl.Component> comps, List<ConversionDataSource> conversions) {
          foreach (var comp in comps) {
            var convDS = conversions.FirstOrDefault (c => comp.Equals (c.Conversion.Component));
            if (convDS is null)
              continue;
            int idx = DataSourceDownload.IndexOf (convDS);
            if (idx >= 0)
              DataSourceDownload.ResetItem (idx);
          }
        }
      }
    }

    private void removeDownloadBooks (IEnumerable<Book> books, IEnumerable<Book> previousBooks) {
      IEnumerable<Book> booksRemoving = previousBooks.Except (books);
      var convDSRem = getDownloadConversions (booksRemoving);
      removeConversions (convDSRem);
    }

    private void removeConversions (IEnumerable<ConversionDataSource> convDSRem) {
      if (convDSRem.Any ()) {
        if (convDSRem.Count () > 1) {
          DataSourceDownload.RemoveRange (convDSRem);
        } else {
          DataSourceDownload.Remove (convDSRem.First ());
        }
      }
    }

    private IEnumerable<ConversionDataSource> getDownloadConversions (IEnumerable<Book> books) {
      var convDS = new List<ConversionDataSource> ();
      foreach (var book in books) {
        if (DownloadSettings.MultiPartDownload && book.DeliveryType == EDeliveryType.MultiPartBook && book.Components.Any ()) {
          var convDSMultiPart = new List<ConversionDataSource> ();
          foreach (var comp in book.Components)
            convDSMultiPart.Add (new ConversionDataSource (comp.Conversion));
          convDSMultiPart.Sort ((x, y) =>
            Comparer<int>.Default.Compare (x.Conversion.Component.PartNumber, y.Conversion.Component.PartNumber));
          convDS.AddRange (convDSMultiPart);
        } else
          convDS.Add (new ConversionDataSource (book.Conversion));
      }
      return convDS;
    }

    private void dataGridView1_DataBindingComplete (object sender, DataGridViewBindingCompleteEventArgs e) {
      foreach (DataGridViewColumn clm in dataGridView1.Columns) {
        if (clm.ValueType == typeof (Image))
          clm.SortMode = DataGridViewColumnSortMode.Automatic;
      }
      enable ();
    }

    private void enable () {
      enableAdd ();
      enableRem ();
      enableDnload ();
      checkIdle ();
    }

    private void checkIdle () {
      bool isIdle = IsIdle;
      if (isIdle != _wasIdle) {
        _wasIdle = isIdle;
        IdleChanged?.Invoke (this, new BoolEventArgs (isIdle));
      }
    }

    private void enableAdd () {
      btnAdd.Enabled = BookLibForm is null && !(DataSourceDownload?.Any () ?? false);
    }

    private void enableRem () {
      btnRem.Enabled = dataGridView1.SelectedRows.Count > 0;
    }

    private bool enableDnload () {
      bool ena = AudibleApi is not null;
      ena &= BookLibForm is null;
      ena &= !(DataSourceLocal?.Any () ?? false);
      btnDnload.Enabled = ena; //AudibleApi is not null && BookLibForm is null && !(DataSourceLocal?.Any () ?? false);
      return ena;
    }

    private void btnAdd_Click (object sender, EventArgs e) {
      Log (3, this);
      // TODO implement for local
      MsgBox.Show (this, "Adding local files.\r\nNot yet implemented.");
      enable ();
    }

    private void btnRem_Click (object sender, EventArgs e) {
      Log (3, this);
      using (new ResourceGuard (x => _ignoreFlag = x)) {
        if (DataSourceDownload?.Any () ?? false) {
          var selectedConversions = new List<ConversionDataSource> ();
          addSortedByIndex (selectedConversions);
          foreach (DataGridViewRow row in dataGridView1.SelectedRows) {
            int idx = row.Index;
            ConversionDataSource convDS = DataSourceDownload[idx];
            selectedConversions.Add (convDS);
          }

          var booksToBeRemoved = selectedConversions.Select (c => c.Conversion.ParentBook).Distinct ();

          var conversionsToBeRemoved = getDownloadConversions (booksToBeRemoved);

          removeConversions (conversionsToBeRemoved);

          var remainingBooks = getCurrentDownloadBooks ();

          using var rg = new ResourceGuard (x => _ignoreFlag = x);
          BookLibForm?.UpdateDownloads (remainingBooks);
        }

        dataGridView1.ClearSelection ();
        enable ();
      }
      snapshotSelection (false);
    }

    private async void btnDnload_Click (object sender, EventArgs e) {
      Log (3, this);
      if (BookLibForm is not null)
        return;

      btnDnload.Enabled = false;

      Cursor cursor = Cursor.Current;
      Cursor.Current = Cursors.AppStarting;
      var rg = new ResourceGuard (() => Cursor.Current = cursor);

      if (!_initConnectionDone) {
        var s = _downloadSettings;
        Log (3, this, () => $"settings: auto refresh={s.AutoRefresh}, auto update={s.AutoUpdateLibrary}");
        if (!s.AutoRefresh) {
          await AudibleApi.RefreshTokenAsyncFunc ();
        }

        _initConnectionDone = true;
      }

      BookLibForm = new BookLibForm (AudibleApi, DownloadSettings, ExportSettings) { Owner = this.ParentForm };
      BookLibForm.FormClosed += bookLibForm_FormClosed;
      BookLibForm.BookDownloadSelectionChanged += bookLibForm_BookDownloadSelectionChanged;
      BookLibForm.ConversionUpdated += bookLibForm_ConversionUpdated;
      //BookLibForm.DownloadSelectEnabled = dataGridView1.Enabled;
      BookLibForm.SetStartPosition (EFormStartPosition.AtParentTopLeft, this);
      BookLibForm.Show ();

      enable ();
    }

    private void bookLibForm_ConversionUpdated (object sender, ConversionEventArgs args) {
      Log (3, this, () => $"{args.Conversion}");
      if (DataSourceDownload is null)
        return;
      var conv = args.Conversion;
      int idx = DataSourceDownload.IndexOf (conv);
      if (idx >= 0)
        DataSourceDownload.ResetItem (idx);
    }

    private void bookLibForm_BookDownloadSelectionChanged (object sender, BookEventArgs args) {
      Log (3, this, () => $"#books={args.Books.Count ()}");

      updateDownloadDataSource (args.Books);
      dataGridView1.AutoResizeColumns (DataGridViewAutoSizeColumnsMode.DisplayedCells);
      enable ();

      Log (3, this, () => $"#conv={DataSourceDownload.Count}");
    }

    private void bookLibForm_FormClosed (object sender, FormClosedEventArgs e) {
      BookLibForm = null;
      enable ();
    }

    private void dataGridView1_SelectionChanged (object sender, EventArgs e) {
      if (_ignoreFlag)
        return;
      enable ();

      snapshotSelection (false);
    }

    private void snapshotSelection (bool updateSelectedRows) {
      Log (3, this, () => $"upd sel rows={updateSelectedRows}");
      var selectedConversions = new List<Conversion> ();
      addSortedByIndex (selectedConversions);

      var books = selectedConversions.Select (c => c.ParentBook).Distinct ();
      var conversionsDS = getDownloadConversions (books);
      var conversions = conversionsDS.Select (c => c.Conversion);

      if (updateSelectedRows) {
        using (new ResourceGuard (x => _ignoreFlag = x)) {
          dataGridView1.ClearSelection ();
          foreach (var convDS in conversionsDS) {
            int idx = DataSourceDownload.IndexOf (convDS);
            if (idx >= 0)
              dataGridView1.Rows[idx].Selected = true;
          }
        }
      } else {
        _selectedConversions.Clear ();
        _selectedConversions.AddRange (conversions);
        SelectionChanged?.Invoke (this, EventArgs.Empty);
      }

      Log (3, this, () => $"#conv sel={_selectedConversions.Count ()}");
    }

    private void addSortedByIndex (List<Conversion> selectedConversions) =>
      addSortedByIndex (selectedConversions, c => c.Conversion);

    private void addSortedByIndex (List<ConversionDataSource> selectedConversions) =>
      addSortedByIndex (selectedConversions, c => c);

    private void addSortedByIndex<T> (List<T> selected, Func<ConversionDataSource, T> getProp) =>
      dataGridView1.AddSelectedRowsSortedByIndex (selected, getProp, DataSourceDownload);

    private void timer1_Tick (object sender, EventArgs e) {
      timer1.Enabled = false;
      dataGridView1.SelectAll ();
      enable ();

      if (!_snapshotFlag)
        return;

      _snapshotFlag = false;
      snapshotSelection (false);
    }
  }
}
