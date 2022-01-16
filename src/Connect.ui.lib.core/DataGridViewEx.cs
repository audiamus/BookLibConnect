using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using core.audiamus.aux;
using core.audiamus.aux.win;

namespace core.audiamus.connect.ui {
  class DataGridViewEx : DataGridView, ISortingEvents {

    private readonly Timer _timer1 = new Timer ();
    private readonly Timer _timer2 = new Timer ();
    private bool _ignoreFlag;
    private bool _isSorting;
    private bool _sortEndedVertPosSet;
    private int? _sortColumnRelX;

    public event EventHandler BeginSorting;
    public event EventHandler EndSorting;
    public event EventHandler SortingCompleteToSetVerticalPosition;

    public new ISortableBindingList DataSource {
      get => base.DataSource as ISortableBindingList;
      set {
        base.DataSource = value;
        value.BeginSorting += dataSource_BeginSorting;
        value.EndSorting += dataSource_EndSorting;
      }
    }

    public DataGridViewEx () {
      _timer1.Interval = 100;
      _timer1.Tick += timer1_Tick;
      _timer2.Interval = 10;
      _timer2.Tick += timer2_Tick;
    }

    public void AddSelectedRowsSortedByIndex<T, S> (List<T> selected, Func<S, T> getProp, IReadOnlyList<S> dataSource) {
      var selectedRows = new DataGridViewRow[this.SelectedRows.Count];
      this.SelectedRows.CopyTo (selectedRows, 0);
      Array.Sort (selectedRows, (x, y) => x.Index.CompareTo (y.Index));
      foreach (DataGridViewRow row in selectedRows) {
        int idx = row.Index;
        S item = dataSource[idx];
        selected.Add (getProp (item));
      }
    }

    protected override void OnDataBindingComplete (DataGridViewBindingCompleteEventArgs e) {
      foreach (DataGridViewColumn clm in base.Columns) {
        if (clm.ValueType == typeof (Image))
          clm.SortMode = DataGridViewColumnSortMode.Automatic;
      }
      base.OnDataBindingComplete (e);
    }

    protected override void OnScroll (ScrollEventArgs e) {
      if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll) {
        if (_ignoreFlag)
          return;
        _sortColumnRelX = null;
        return;
      }
      _timer1.Enabled = false;
      _timer1.Enabled = true;

      base.OnScroll (e);
    }

    protected override void OnSelectionChanged (EventArgs e) {
      if (_isSorting)
        return;


      base.OnSelectionChanged (e);
    }

    private void timer1_Tick (object sender, EventArgs e) {
      _timer1.Enabled = false;
      base.AutoResizeColumns (DataGridViewAutoSizeColumnsMode.DisplayedCells);
      setHorizontalPos ();
    }

    private void timer2_Tick (object sender, EventArgs e) {
      if (_sortEndedVertPosSet) {
        _timer2.Enabled = false;
        _timer2.Interval = 10;

        setHorizontalPos ();
      } else {
        _timer2.Interval = 50;
      
        SortingCompleteToSetVerticalPosition?.Invoke (sender, e);

        _sortEndedVertPosSet = true;
      }

    }

    private void dataSource_BeginSorting (object sender, EventArgs e) {
      _isSorting = true;
      _sortEndedVertPosSet = false;

      var column = base.SortedColumn;
      var clmRect = base.GetColumnDisplayRectangle (column.Index, true);
      _sortColumnRelX = clmRect.X;

      BeginSorting?.Invoke (sender, e);
    }

    private void dataSource_EndSorting (object sender, EventArgs e) {
      EndSorting?.Invoke (sender, e);

      _timer2.Enabled = true;
      _isSorting = false;

    }

    private void setHorizontalPos () {
      if (!_sortColumnRelX.HasValue)
        return;
      using var rg = new ResourceGuard (x => _ignoreFlag = x);
      // abs position of sort column
      int aggrclmwid = absSortColumnX ();

      // offset of hor scrollbar
      int offs = aggrclmwid - _sortColumnRelX.Value;
      offs = Math.Max (0, offs);
      base.HorizontalScrollingOffset = offs;
    }


    private int absSortColumnX () {
      if (base.SortedColumn is null)
        return 0;
      int aggrclmwid = 0;
      for (int i = 0; i < base.SortedColumn.Index; i++)
        aggrclmwid += base.Columns[i].Width;
      return aggrclmwid;
    }

  }
}
