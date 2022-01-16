using System.Collections.Generic;
using System.Linq;
using core.audiamus.aux.ex;

namespace core.audiamus.aux.win {
  /// <summary>
  /// A ResortableBindingList that can be paused.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class SortableBindingListSuspensible<T> : SortableBindingList<T> {
    #region private fields

    private bool _suspended = false;
    private bool _dirty = false;
    List<T> _backorderListAdd = new List<T> ();
    List<T> _backorderListRem = new List<T> ();

    #endregion
    #region properties

    public bool Suspended {
      get => _suspended;
      set {
        if (_suspended == value)
          return;
        if (!value) {
          _dirty |= _backorderListAdd.Any () || _backorderListRem.Any ();
          foreach (T item in _backorderListAdd)
            base.Add (item);
          foreach (T item in _backorderListRem)
            base.Remove (item);
          _backorderListAdd.Clear ();
          _backorderListRem.Clear ();
        }

        _suspended = value;
        if (!value && _dirty) {
          _dirty = false;
          ResetBindings ();
          resort ();
        }
      }
    }

    public IEnumerable<T> AllItems {
      get {
        if (Suspended) {
          List<T> items = new List<T> ();
          items.AddRange (this);
          items.AddRange (_backorderListAdd);
          return items.ToList();
        } else
          return this;
      }
    }
    #endregion
    #region ctors
    public SortableBindingListSuspensible () : base (new List<T> ()) {
    }

    public SortableBindingListSuspensible (IEnumerable<T> enumeration) : base (new List<T> (enumeration)) {
    }
    #endregion ctors
    #region replacing methods (non-virtual)

    public new void Clear () {
      if (Suspended) {
        _backorderListAdd.Clear ();
        _backorderListRem.Clear ();
        return;
      }
      base.Clear ();
    }

    public new void Add (T item) {
      if (Suspended) {
        _backorderListAdd.Add (item);
        return;
      }
      base.Add (item);
    }

    public new void Remove (T item) {
      if (Suspended) {
        bool succ = _backorderListAdd.Remove (item);
        if (!succ)
          _backorderListRem.Add (item);
        return;
      }
      base.Remove (item);
    }

    public void AddRange (IEnumerable<T> range) {
      if (range is null)
        return;
      bool wasPaused = Suspended;
      using (new ResourceGuard (f => Suspended = f || wasPaused))
        range.ForEach (o => Add (o));
    }

    public void RemoveRange (IEnumerable<T> range) {
      if (range is null)
        return;
      bool wasPaused = Suspended;
      using (new ResourceGuard (f => Suspended = f || wasPaused))
        range.ForEach (o => Remove (o));
    }


    #endregion
    #region overrides

    protected override void OnListChanged (System.ComponentModel.ListChangedEventArgs e) {
      if (Suspended) {
        _dirty = true;
        return;
      }
      base.OnListChanged (e);
    }
    #endregion

  }
}
