using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using core.audiamus.aux;
using core.audiamus.aux.win;
using core.audiamus.booksdb;
using core.audiamus.booksdb.ex;
using R = core.audiamus.connect.ui.Properties.Resources;
using static core.audiamus.aux.Logging;
using core.audiamus.aux.ex;

namespace core.audiamus.connect.ui {

  public partial class BookLibDGVControl : UserControl {

    private SortableBindingList<BookDataSource> DataSource { get; set; }
    private List<BookDataSource> CurrentlySelectedBooks { get; } = new List<BookDataSource> ();

    private bool _ignoreFlag;
    private IEnumerable<Book> _allBooks;
    private IEnumerable<Conversion> _allConversions;
    private IEnumerable<Book> SelectedBooks => CurrentlySelectedBooks.Select (b => b.Book).ToList ();
    private List<Book> SelectedBooksForDownload { get; } = new List<Book> ();
    private bool _isSortingAftermath;
    private BookDataSource _firstDisplayedBook;
    private BookDataSource _firstDisplayedSelectedBook;
    private int _firstDisplayedSelectedBookOffset;
    private Color? _defaultCellBackColor;
    private IDownloadSettings _settings;

    private static readonly Color __downloadSelectCellBackColor = Color.LightCyan;

    public event BookSelectionChangedEventHandler BookSelectionChanged;
    public event BookSelectionChangedEventHandler BookDownloadSelectionChanged;
    public event ConversionUpdatedEventHandler ConversionUpdated;
    public event EventHandler Close;
    public event EventHandler Resync;

    public IEnumerable<Book> Books { set => setDataSource (value); }

    public IDownloadSettings Settings {
      private get => _settings;
      set {
        _settings = value;
        _settings.ChangedSettings += settings_ChangedSettings;
      }
    }

    public bool DownloadSelectEnabled {
      get => panelDownloadSelect.Enabled;
      set => panelDownloadSelect.Enabled = value;
    }

    public BookLibDGVControl () {
      InitializeComponent ();
    }

    private void settings_ChangedSettings (object sender, EventArgs e) {
      resetDataSource (_allBooks);
      onUpdatedSettings ();
    }

    public void UpdateDownloads (IEnumerable<Book> books) {
      using var lg = new LogGuard (1, this, () => $"#books={books.Count ()}");
      using var rg = new ResourceGuard (x => _ignoreFlag = x);

      var booksToBeRemoved = SelectedBooksForDownload.Except (books).ToList ();

      revertDownloadSelection (booksToBeRemoved, true);
    }

    public void UpdateConversion (Conversion conversion) {
      var book = conversion.ParentBook;
      resetDataSourceItem (book);
    }

    public void UpdateConversionStateFromOther (IConversion other) {
      var conv = _allConversions.FirstOrDefault (c => c.Id == other.Id);
      if (conv is null)
        return;
      conv.State = other.State;
      UpdateConversion (conv);
      ConversionUpdated?.Invoke (this, new ConversionEventArgs (conv));
    }

    private void resetDataSource (IEnumerable<Book> allBooks) {
      if (allBooks is null)
        return;
      IEnumerable<Book> books = allBooks.ToList ();

      if (!Settings.IncludeAdultProducts)
        books = books.Where (b => !(b.AdultProduct ?? false));

      Log (3, this, () => $"#books={allBooks.Count ()} (filtered)");

      IEnumerable<Book> initiallySortedBooks = Settings.InitialSorting switch {
        EInitialSorting.state_date => books
            .OrderBy (b => b.ApplicableState (Settings.MultiPartDownload))
            .ThenByDescending (b => b.PurchaseDate),
        EInitialSorting.date => books
            .OrderByDescending (b => b.PurchaseDate),
        EInitialSorting.author_title => books
            .OrderBy (b => b.Author)
            .ThenBy (b => b.Title),
        EInitialSorting.author_date => books
            .OrderBy (b => b.Author)
            .ThenByDescending (b => b.PurchaseDate),
        EInitialSorting.title_author => books
            .OrderBy (b => b.Title)
            .ThenBy (b => b.Author),
        _ => books.OrderBy (b => b.ApplicableState (Settings.MultiPartDownload))
      };

      books = initiallySortedBooks.ToList ();
      var booksDS = new List<BookDataSource> ();
      foreach (var book in books)
        booksDS.Add (new BookDataSource (book, Settings));

      DataSource = new SortableBindingList<BookDataSource> (booksDS) {
        UseBackingProperties = true
      };

      this.dataGridView1.DataSource = DataSource;
    }

    private void setDataSource (IEnumerable<Book> allBooks) {
      if (allBooks is null)
        return;

      // HACK currently exclude podcasts
      allBooks = allBooks
        .Where (b => b.DeliveryType == EDeliveryType.SinglePartBook || b.DeliveryType == EDeliveryType.MultiPartBook)
        .ToList ();

      Log (3, this, () => $"#books={allBooks.Count ()} (deliv type filtered)");

      _allBooks = allBooks;
      resetDataSource (allBooks);

      var allConversions = _allBooks.Select (b => b.Conversion).ToList ();
      var allCompConversions = _allBooks.SelectMany (b => b.Components).Select (c => c.Conversion).ToList ();
      allConversions.AddRange (allCompConversions);
      allConversions.Sort ((x, y) => x.Id.CompareTo (y.Id));
      _allConversions = allConversions;

    }

    private void onUpdatedSettings () {
      var clm = dataGridView1.Columns[nameof (BookDataSource.Adult)];
      if (clm is null)
        return;
      clm.Visible = Settings.IncludeAdultProducts;
    }

    private void resetDataSourceItem (Book book) {
      int? i = DataSource.Select ((s, i) => new { s, i }).FirstOrDefault (k => k.s.DataSource == book)?.i;
      if (i.HasValue)
        DataSource.ResetItem (i.Value);
    }


    private void dataGridView1_BeginSorting (object sender, EventArgs e) {
      int firstDisplayedRowIdx = dataGridView1.FirstDisplayedScrollingRowIndex;
      _firstDisplayedBook = DataSource[firstDisplayedRowIdx];

      _firstDisplayedSelectedBookOffset = 0;
      _firstDisplayedSelectedBook = null;
      if (dataGridView1.SelectedRows.Count > 0) {
        // is the 1st selected book visible?
        int firstSelBookRowIdx = dataGridView1.SelectedRows[0].Index;
        int offs = firstSelBookRowIdx - firstDisplayedRowIdx;
        int nVisRows = dataGridView1.Rows.GetRowCount (DataGridViewElementStates.Displayed);
        if (offs <= nVisRows) {
          _firstDisplayedSelectedBook = DataSource[firstSelBookRowIdx];
          _firstDisplayedSelectedBookOffset = offs;
        }
      }

    }

    private void dataGridView1_EndSorting (object sender, EventArgs e) {
      dataGridView1.ClearSelection ();

      if (CurrentlySelectedBooks.Any ()) {
        var selectedRows = new List<int> ();
        foreach (var book in CurrentlySelectedBooks) {
          int idx = DataSource.IndexOf (book);
          selectedRows.Add (idx);
        }
        selectedRows.Sort ();
        foreach (var rowIdx in selectedRows)
          dataGridView1.Rows[rowIdx].Selected = true;
      } else
        _isSortingAftermath = true;

      if (SelectedBooksForDownload.Any ()) {
        foreach (var book in SelectedBooksForDownload) {
          int idx = DataSource.IndexOf (book);
          dataGridView1.Rows[idx].DefaultCellStyle.BackColor = Color.Azure;
        }
      }

    }

    private void dataGridView1_SortingCompleteToSetVerticalPosition (object sender, EventArgs e) {
      int idx;
      if (_firstDisplayedSelectedBook is null) {
        idx = DataSource.IndexOf (_firstDisplayedBook);
      } else {
        int selidx = DataSource.IndexOf (_firstDisplayedSelectedBook) - _firstDisplayedSelectedBookOffset;
        idx = Math.Max (0, selidx);
      }
      idx = Math.Min (idx, DataSource.Count - 1);
      dataGridView1.FirstDisplayedScrollingRowIndex = idx;
    }


    private void dataGridView1_DataBindingComplete (object sender, DataGridViewBindingCompleteEventArgs e) {
      onUpdatedSettings ();
    }


    private void dataGridView1_SelectionChanged (object sender, EventArgs e) {
      if (_isSortingAftermath) {
        _isSortingAftermath = false;
        dataGridView1.ClearSelection ();
        return;
      }

      renewCurrentylSelectBooksSorted ();

      enableDownloadButtons ();

      BookSelectionChanged?.Invoke (this, new BookEventArgs (SelectedBooks));
    }

    private void renewCurrentylSelectBooksSorted () {
      CurrentlySelectedBooks.Clear ();
      dataGridView1.AddSelectedRowsSortedByIndex (CurrentlySelectedBooks, book => book, DataSource, selectable);

      bool selectable (int idx) {
        try {
          var item = DataSource[idx];
          var state = item.Book.ApplicableState (Settings.MultiPartDownload);
          return state > EConversionState.unknown;
        } catch (Exception exc) {
          Log (1, this, exc.Summary ());
        }
        return true;
      }
    }

    private void enableDownloadButtons () {
      var selectedBooks = SelectedBooks;
      var addBooks = selectedBooks.Except (SelectedBooksForDownload);
      btnAddSel.Enabled = addBooks.Any ();

      var remBooks = selectedBooks.Intersect (SelectedBooksForDownload);
      btnRemSel.Enabled = remBooks.Any ();

      btnRemAll.Enabled = SelectedBooksForDownload.Any ();
    }


    private void btnOk_Click (object sender, EventArgs e) {
      Close?.Invoke (this, EventArgs.Empty);
    }

    private void btnAddSel_Click (object sender, EventArgs e) {
      var addBooks = SelectedBooks
        .Except (SelectedBooksForDownload)
        .ToList ();

      SelectedBooksForDownload.AddRange (addBooks);

      foreach (var book in addBooks) {
        //book.Conversion.PrevState = book.ApplicableState (Settings);
        //book.Conversion.State = EConversionState.download;
        setStateForDownload (book);
        int idx = DataSource.IndexOf (book);
        if (!_defaultCellBackColor.HasValue)
          _defaultCellBackColor = dataGridView1.Rows[idx].DefaultCellStyle.BackColor;
        dataGridView1.Rows[idx].DefaultCellStyle.BackColor = __downloadSelectCellBackColor;
        DataSource.ResetItem (idx);
      }

      afterDownloadSelectionChanged ();
    }

    private void btnRemSel_Click (object sender, EventArgs e) {
      var remBooks = SelectedBooks
        .Intersect (SelectedBooksForDownload)
        .ToList ();

      revertDownloadSelection (remBooks, true);
    }


    private void btnRemAll_Click (object sender, EventArgs e) {
      var remBooks = SelectedBooksForDownload.ToList ();
      SelectedBooksForDownload.Clear ();
      revertDownloadSelection (remBooks);
    }

    private void revertDownloadSelection (IEnumerable<Book> remBooks, bool remove = false) {
      foreach (var book in remBooks) {
        if (remove)
          SelectedBooksForDownload.Remove (book);

        //book.Conversion.State = book.Conversion.PrevState ?? EConversionState.remote;
        resetStateForDownload (book);

        int idx = DataSource.IndexOf (book);
        if (idx < 0)
          continue;
        dataGridView1.Rows[idx].DefaultCellStyle.BackColor = _defaultCellBackColor ?? SystemColors.Window;
        DataSource.ResetItem (idx);
      }

      afterDownloadSelectionChanged ();
    }

    private void afterDownloadSelectionChanged () {
      int nBooks = SelectedBooksForDownload.Count;

      if (nBooks > 0) {
        int nParts = SelectedBooksForDownload.Select (b => b.Components.Count == 0 ? 1 : b.Components.Count).Sum ();
        string info = $"{nBooks} {(nBooks == 1 ? R.Book : R.Books)}" +
          $" / {nParts} {(nParts == 1 ? R.Part : R.Parts)}";

        lblDnloadList.Text = info;
        Log (3, this, () => $"download selected: #books={nBooks}, #parts={nParts}");
      } else {
        lblDnloadList.Text = null;
        Log (3, this, () => "download selected: none");
      }
      enableDownloadButtons ();

      if (_ignoreFlag)
        return;

      BookDownloadSelectionChanged?.Invoke (this, new BookEventArgs (SelectedBooksForDownload));
    }

    private void setStateForDownload (Book book) {
      book.Conversion.PersistState = book.Conversion.State;
      book.Conversion.State = EConversionState.download;
      foreach (var comp in book.Components) {
        comp.Conversion.PersistState = comp.Conversion.State;
        comp.Conversion.State = EConversionState.download;
      }
    }

    private void resetStateForDownload (Book book) {
      book.Conversion.State = book.Conversion.PersistState ?? EConversionState.remote;
      foreach (var comp in book.Components) {
        comp.Conversion.State = comp.Conversion.PersistState ?? EConversionState.remote;
      }

    }

    private void btnResync_Click (object sender, EventArgs e) {
      Resync?.Invoke (this, EventArgs.Empty);
    }
  }
}
