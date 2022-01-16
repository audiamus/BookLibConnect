using System;
using System.ComponentModel;
using System.Drawing;
using core.audiamus.aux.ex;
using core.audiamus.aux.propgrid;
using core.audiamus.booksdb;
using core.audiamus.booksdb.ex;
using core.audiamus.common;
using R = core.audiamus.connect.ui.Properties.Resources;

namespace core.audiamus.connect.ui {
  class ConversionDataSource : DataSourceBase<Conversion>, IEquatable<ConversionDataSource> {

    [Browsable (false)]
    public Conversion Conversion => base.DataSource;


    public ConversionDataSource (Conversion conv) : base (conv) { }

    [GlobalizedProperty]
    public Image State {
      get {
        bool succ = __icons.TryGetValue (StateBacking, out var img);
        if (succ)
          return img;
        else
          return R.ImgBlank;
      }
    }

    [Browsable (false)]
    public EConversionState StateBacking => Conversion.State;


    public override string Title => title ();

    public override string Author => base.Author;

    [GlobalizedProperty]
    public string FileSize => fileSize (FileSizeBacking);

    public override string Duration => base.Duration;

    [GlobalizedProperty]
    public int? Year {
      get {
        int? year = Conversion.ReleaseDate?.Date.Year;
        if (year.HasValue && year.Value == 2200)
          return null;
        return year;
      }
    }

    public override string Narrator => Conversion.Narrator;

    [Browsable (false)]
    public int? SampleRateBacking => rateBacking (a => a?.SampleRate);

    [Browsable (false)]
    public int? BitRateBacking => rateBacking (a => a?.BitRate);


    [GlobalizedProperty]
    public string SampleRate => SampleRateBacking.HasValue ? $"{SampleRateBacking.Value} Hz" : null;

    [GlobalizedProperty]
    public string BitRate => BitRateBacking.HasValue ? $"{BitRateBacking.Value} kb/s" : null;

    [GlobalizedProperty]
    public override DateTime? PurchaseDate => base.PurchaseDate;

    public bool Equals (ConversionDataSource other) => Conversion.Equals(other.Conversion);
    public override bool Equals (object obj) => Equals (obj as ConversionDataSource);
    public override int GetHashCode () => Conversion.GetHashCode ();
      

    private string title () {
      if (!base.Title.IsNullOrWhiteSpace ())
        return base.Title;
      var comp = DataSource.Component;
      if (comp is null)
        return null;
      return $"{comp.Book.Title} {R.Part} {comp.PartNumber}";
    }

    private int? rateBacking (Func<IAudioQuality, int?> getProp) {
      int? rate = getProp (Conversion);

      if (rate.HasValue)
        return rate;
      if (Conversion.Book is null)
        return getRate (Conversion.Component.Book, getProp);
      else 
        return getRate (Conversion.Book, getProp);      
    }

    private static int? getRate(Book book, Func<IAudioQuality, int?> getProp) {
      int? rate = getProp (book);  
      if (rate is null)
          return getProp(book.Codecs.MaxQuality());
        else 
          return rate;

    }
  }
}
