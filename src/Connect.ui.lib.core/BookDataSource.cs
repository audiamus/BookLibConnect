using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using core.audiamus.aux;
using core.audiamus.aux.propgrid;
using core.audiamus.booksdb;
using R = core.audiamus.connect.ui.Properties.Resources;

namespace core.audiamus.connect.ui {
  public class BookDataSource : BookDataSourceBase {
    public BookDataSource (Book book, IDownloadSettings settings) : base (book, settings) { }

    [GlobalizedProperty]
    public Image State {
      get {
        bool succ = __icons.TryGetValue (StateBacking, out var img);
        if (succ) {
          if (MultiState)
            img = imagePlus (img);
          return img;
        } else
          return R.ImgBlank;
      }
    }

    [Browsable (false)]
    public EConversionState StateBacking => getState ();

    public override DateTime? PurchaseDate => base.PurchaseDate;

    public override string Title => base.Title;

    public override string Author => base.Author;

    public override string Duration => base.Duration;

    [GlobalizedProperty]
    public int? Year {
      get {
        int? year = Book.ReleaseDate?.Date.Year;
        if (year.HasValue && year.Value == 2200)
          return null;
        return year;
      }
    }

    public override string Narrator => base.Narrator;

    [GlobalizedProperty]
    public string Series => Book.Series.FirstOrDefault ()?.ToString ();

    [GlobalizedProperty]
    [TypeConverter (typeof (BooleanYesNoConverter))]
    public bool? Adult => Book.AdultProduct;

    [GlobalizedProperty]
    public string Genre => Book.Ladders.FirstOrDefault ()?.ToString ();

    [GlobalizedProperty]
    public int? Parts => Book.Components.Count > 0 ? Book.Components.Count : null;

  }
}

