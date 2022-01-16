using System;
using System.ComponentModel;
using System.Linq;
using core.audiamus.aux;
using core.audiamus.aux.ex;
using core.audiamus.aux.propgrid;
using core.audiamus.booksdb;

namespace core.audiamus.connect.ui {
  public class BookPGA : BookDataSourceBase {
    public BookPGA (Book book, IDownloadSettings settings) : base (book, settings) {
      SetReadonly (EReadonly.all);
    }


    [PropertyOrder (1)]
    [GlobalizedProperty]
    [TypeConverter (typeof (EnumConverterRM<EConversionState>))]
    public EConversionState State => getState();
    
    [PropertyOrder (2)]
    public override string Title => base.Title;

    [PropertyOrder (3)]
    public override string Author => base.DataSource.MultiAuthors;

    [PropertyOrder (4)]
    public override string Duration => base.Duration;

    [PropertyOrder (5)]
    [GlobalizedProperty]
    [TypeConverter (typeof (BooleanYesNoConverter))]
    public bool? Unabridged => Book.Unabridged;

    [PropertyOrder (6)]
    [GlobalizedProperty]
    public string Language => Book.Language;
    
    [PropertyOrder (7)]
    [GlobalizedProperty]
    public DateTime? ReleaseDate => Book.ReleaseDate;

    [PropertyOrder (8)]
    public override string Narrator => base.DataSource.MultiNarrators;

    [PropertyOrder (9)]
    [GlobalizedProperty]
    public string Publisher => Book.PublisherName;
   
    [PropertyOrder (10)]
    [GlobalizedProperty]
    //[Editor (typeof (MultilineStringEditor), typeof (UITypeEditor))]
    public string Series => Book.Series.Select (l => l.ToString()).Combine(true);
    
    [PropertyOrder (11)]
    [GlobalizedProperty]
    [TypeConverter (typeof (BooleanYesNoConverter))]
    public bool? AdultProduct => Book.AdultProduct;

    [PropertyOrder (12)]
    [GlobalizedProperty]
    //[Editor (typeof (MultilineStringEditor), typeof (UITypeEditor))]
    public string Genres => Book.Ladders.Select (l => l.ToString()).Combine(true);

    [PropertyOrder (13)]
    [GlobalizedProperty]
    public string AverageRating => Book.AverageRating is null ? null : Book.AverageRating.Value.ToString("f1");

    [PropertyOrder (14)]
    [GlobalizedProperty]
    [TypeConverter (typeof (EnumConverterRM<EDeliveryType>))]
    public EDeliveryType? DeliveryType => Book.DeliveryType;
    
    [PropertyOrder (15)]
    [GlobalizedProperty]
    public int? Parts => Book.Components.Count > 0 ? Book.Components.Count : null;

    [PropertyOrder (16)]
    [GlobalizedProperty]
    public string FileSize => fileSize (fileSizeBytes ());

    [PropertyOrder (17)]
    [GlobalizedProperty]
    public string SampleRate => SampleRateBacking.HasValue ? $"{SampleRateBacking.Value} Hz" : null;

    [PropertyOrder (18)]
    [GlobalizedProperty]
    public string BitRate => BitRateBacking.HasValue ? $"{BitRateBacking.Value} kb/s" : null;

    [PropertyOrder (19)]
    public override DateTime? PurchaseDate => base.PurchaseDate;

    public void OnUpdatedSettings (bool showAdultProduct) {
      PropertyCommands[nameof (AdultProduct)].Visible = showAdultProduct;
    }

    private long? fileSizeBytes () {
      if (Book.FileSizeBytes.HasValue)
        return Book.FileSizeBytes;
      else {
        long? sum = Book.Components?.Select (c => c.FileSizeBytes).Sum ();
        if (!sum.HasValue || sum.Value == 0)
          return null;
        return sum;
      }
    }
  }
}
