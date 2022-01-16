using System.ComponentModel;
using System.Linq;
using core.audiamus.booksdb;
using core.audiamus.booksdb.ex;

namespace core.audiamus.connect.ui {
  public abstract class BookDataSourceBase : DataSourceBase<Book> {

    [Browsable (false)]
    public Book Book => base.DataSource;

    public BookDataSourceBase (Book book, IDownloadSettings settings) : base (book) {
      Settings = settings;
    }

    //[GlobalizedProperty]
    //public virtual string SampleRate => SampleRateBacking.HasValue ? $"{SampleRateBacking.Value} Hz" : null;

    [Browsable (false)]
    public int? SampleRateBacking => Book.Codecs.MaxQuality()?.SampleRate;

    //[GlobalizedProperty]
    //public virtual string BitRate => BitRateBacking.HasValue ? $"{BitRateBacking.Value} kb/s" : null;

    [Browsable (false)]
    public int? BitRateBacking => Book.Codecs.MaxQuality()?.BitRate;


    public override string ToString () => $"\"{Book.Authors.FirstOrDefault()?.Name} - {Book.Title}\"";

    protected IDownloadSettings Settings { get; }

    protected bool MultiState {
      get {
        if (!Settings.MultiPartDownload || Book.Components.Count == 0)
          return false;
        return Book.Components
          .Select (c => c.Conversion.State)
          .Distinct ()
          .Count () > 1;
      }
    }

    protected EConversionState getState () {
      if (Settings.MultiPartDownload && Book.Components.Count > 0) {
        var state = Book.Components
          .Select (c => c.Conversion.State)
          .Distinct ()
          .Min ();
        return state;
      } else
        return Book.Conversion.State;
    }


  }
}

