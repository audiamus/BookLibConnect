using System.Linq;

namespace core.audiamus.booksdb.ex {
  public static class EntityExtensions {
    public static EConversionState ApplicableState (this Book book, bool multipart) {
      if (multipart && book.Components.Count > 0) {
        var state = book.Components
          .Select (c => ApplicableState(c.Conversion))
          .Distinct ()
          .Min ();
        return state;
      } else
        return ApplicableState(book.Conversion);
    
    
    }

    public static EConversionState ApplicableState (this Conversion conv) {
      if (conv.State == EConversionState.download && conv.PersistState.HasValue)
        return conv.PersistState.Value;
      else
        return conv.State;
    }

    public static Book GetBook (this IBookCommon common) {
      return common switch {
        Book book => book,
        Component comp => comp.Book,
        Conversion conv => conv.ParentBook,
        _ => null,
      };
    }
  }
}
