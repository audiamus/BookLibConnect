using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using core.audiamus.aux.ex;

namespace core.audiamus.aux.win {

  /// <summary>
  /// http://www.timvw.be/2008/08/02/presenting-the-sortablebindinglistt-take-two/
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class SortableBindingList<T> : BindingList<T>, ISortableBindingList {

    const string BACKING_SUFFIX = "Backing";

    private readonly Dictionary<Type, PropertyComparer<T>> _comparers;
    private Dictionary<string, PropertyDescriptor> _backingProperties;
    private bool _isSorted;
    private bool _resortInProgress;

    private ListSortDirection _listSortDirection;
    private PropertyDescriptor _propertyDescriptor;

    public event EventHandler BeginSorting;
    public event EventHandler EndSorting;

    /// <summary>
    /// Gets or sets a value indicating whether to use backing properties for sorting.
    /// <para>A displayed property value may have been formatted to non-sortable text. (Should be readonly)</para>
    /// <para>A backing property can hold the original and sortable value. (Should be non-browsable.)</para>
    /// <para>The backing property name must be the same as the original property name
    /// with "Backing" as suffix.</para>
    /// </summary>
    /// <example>
    /// <code>
    /// [Browsable(false)]
    /// public DateTime MyPropertyBacking {get; set;}
    ///
    /// public string MyProperty { get { return customFormat(MyPropertyBacking); } }
    /// </code>
    /// </example>
    public bool UseBackingProperties { get; set; }
    
    public bool UseResorting { get; set; }

    public SortableBindingList ()
      : base (new List<T> ()) {
      this._comparers = new Dictionary<Type, PropertyComparer<T>> ();
    }

    public SortableBindingList (IEnumerable<T> enumeration)
      : base (new List<T> (enumeration)) {
      this._comparers = new Dictionary<Type, PropertyComparer<T>> ();
    }

    protected override bool SupportsSortingCore {
      get {
        return true;
      }
    }

    protected override bool IsSortedCore {
      get {
        return this._isSorted;
      }
    }

    protected override PropertyDescriptor SortPropertyCore {
      get {
        return this._propertyDescriptor;
      }
    }

    protected override ListSortDirection SortDirectionCore {
      get {
        return this._listSortDirection;
      }
    }

    protected override bool SupportsSearchingCore {
      get {
        return true;
      }
    }

    protected override void ApplySortCore (PropertyDescriptor property, ListSortDirection direction) {
      if (property == null)
        return;

      var appliedProperty = property;
      if (UseBackingProperties)
        appliedProperty = getBackingProperty (property);

      BeginSorting?.Invoke (this, EventArgs.Empty);

      List<T> itemsList = (List<T>)this.Items;

      Type propertyType = appliedProperty.PropertyType;
      PropertyComparer<T> comparer;
      if (!this._comparers.TryGetValue (propertyType, out comparer)) {
        comparer = new PropertyComparer<T> (appliedProperty, direction);
        this._comparers.Add (propertyType, comparer);
      }

      comparer.SetPropertyAndDirection (appliedProperty, direction);
      //itemsList.Sort (comparer);
      itemsList.StableSort (comparer);

      this._propertyDescriptor = property;
      this._listSortDirection = direction;
      this._isSorted = true;

      this.OnListChanged (new ListChangedEventArgs (ListChangedType.Reset, -1));
      
      EndSorting?.Invoke (this, EventArgs.Empty);

    }

    private PropertyDescriptor getBackingProperty (PropertyDescriptor propDesc) {
      if (!UseBackingProperties)
        return propDesc;
      
      string propName = propDesc.Name;
      if (!(_backingProperties is null)) {
        bool succ = _backingProperties.TryGetValue (propName, out var backingPropDesc);
        if (succ) {
          if (backingPropDesc is null)
            return propDesc;
          else
            return backingPropDesc;
        }
      }

      if (_backingProperties is null)
        _backingProperties = new Dictionary<string, PropertyDescriptor> ();

      string propNameBacking = propName + BACKING_SUFFIX;
      
      //Get properties for type
      PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties (typeof(T));

      //Get property descriptor for backing property, can be null
      PropertyDescriptor pd = pdc[propNameBacking];

      _backingProperties[propName] = pd;
      if (pd is null)
        return propDesc;
      else
        return pd;
    }

    protected override void OnListChanged (System.ComponentModel.ListChangedEventArgs e) {
      try {
        base.OnListChanged (e);
      } catch (Exception exc) {
        Logging.Log (1, this, () => exc.Summary ());
        return; 
      }
      if (e.ListChangedType == ListChangedType.Reset) {
        return;
      }

      resort ();
    }

    protected void resort () {
      if (!UseResorting || _resortInProgress || SortPropertyCore is null)
        return;
      using (new ResourceGuard (flag => _resortInProgress = flag)) {
        ApplySortCore (SortPropertyCore, SortDirectionCore);
      }
    }

    protected override void RemoveSortCore () {
      this._isSorted = false;
      this._propertyDescriptor = base.SortPropertyCore;
      this._listSortDirection = base.SortDirectionCore;

      this.OnListChanged (new ListChangedEventArgs (ListChangedType.Reset, -1));
    }

    protected override int FindCore (PropertyDescriptor property, object key) {
      int count = this.Count;
      for (int i = 0; i < count; ++i) {
        T element = this[i];
        if (property.GetValue (element).Equals (key)) {
          return i;
        }
      }

      return -1;
    }
  }

  public class PropertyComparer<T> : IComparer<T> {
    private readonly IComparer comparer;
    private PropertyDescriptor propertyDescriptor;
    private int reverse;

    public PropertyComparer (PropertyDescriptor property, ListSortDirection direction) {
      this.propertyDescriptor = property;
      Type comparerForPropertyType = typeof (Comparer<>).MakeGenericType (property.PropertyType);
      this.comparer = (IComparer)comparerForPropertyType.InvokeMember 
        ("Default", BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public, null, null, null);
      this.SetListSortDirection (direction);
    }

    #region IComparer<T> Members

    public int Compare (T x, T y) {
      return this.reverse * this.comparer.Compare (this.propertyDescriptor.GetValue (x), this.propertyDescriptor.GetValue (y));
    }

    #endregion

    private void SetPropertyDescriptor (PropertyDescriptor descriptor) {
      this.propertyDescriptor = descriptor;
    }

    private void SetListSortDirection (ListSortDirection direction) {
      this.reverse = direction == ListSortDirection.Ascending ? 1 : -1;
    }

    public void SetPropertyAndDirection (PropertyDescriptor descriptor, ListSortDirection direction) {
      this.SetPropertyDescriptor (descriptor);
      this.SetListSortDirection (direction);
    }
  }


}