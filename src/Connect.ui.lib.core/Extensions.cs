using System.ComponentModel;
using System.Linq;
using core.audiamus.booksdb;

namespace core.audiamus.connect.ui {
  public static class Extensions {

    public static int IndexOf (this BindingList<BookDataSource> bindingList, Book dataItem) {
      int idx = bindingList.IndexOf (bindingList.FirstOrDefault (k => ReferenceEquals (k.DataSource, dataItem)));
      return idx;
    }

    internal static int IndexOf (this BindingList<ConversionDataSource> bindingList, Conversion dataItem) {
      int idx = bindingList.IndexOf (bindingList.FirstOrDefault (k => ReferenceEquals (k.DataSource, dataItem)));
      return idx;
    }

  }
}
