using System;
using System.ComponentModel;

namespace core.audiamus.aux.win {
  public interface ISortingEvents {
    event EventHandler BeginSorting;
    event EventHandler EndSorting;
  }

  public interface ISortableBindingList : IBindingList, ISortingEvents {
  }
}
