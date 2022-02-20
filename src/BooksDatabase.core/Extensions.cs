using System.Linq;

namespace core.audiamus.booksdb.ex {
  public static class EntityExtensions {
    public static EConversionState ApplicableState (this Book book, bool multipart) {
      if (multipart && book.Components.Count > 0) {
        var state = book.Components
          .Select (c => c.Conversion.ApplicableState())
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

    public static EDownloadQuality ApplicableDownloadQuality (this Book book, bool multipart) {
      if (multipart && book.Components.Count > 0) {
        var dnldqual = book.Components
          .Select (c => c.ApplicableDownloadQuality())
          .Distinct ()
          .Min ();
        return dnldqual;
      } else
        return book.ApplicableDownloadQuality();
    }

    public static EDownloadQuality ApplicableDownloadQuality (this IBookCommon book) => 
      book.DownloadQuality ?? EDownloadQuality.Extreme;

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
