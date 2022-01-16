using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using core.audiamus.aux.ex;
using core.audiamus.aux.propgrid;
using core.audiamus.booksdb;
using core.audiamus.common;
using R = core.audiamus.connect.ui.Properties.Resources;

namespace core.audiamus.connect.ui {
  public abstract class DataSourceBase<T> : BasePropertyGridAdapter<T> 
    where T: IBookMeta
  {
    protected static readonly Dictionary<EConversionState, Image> __icons =
      new Dictionary<EConversionState, Image> {
        { EConversionState.unknown, R.ImgQuestion },
        { EConversionState.remote, R.ImgGlobe },
        { EConversionState.download, R.ImgGlobeDown },
        { EConversionState.license_granted, R.ImgKey },
        { EConversionState.license_denied, R.ImgKeyDenied },
        { EConversionState.downloading, R.ImgDown },
        { EConversionState.download_error, R.ImgDownBroken },
        { EConversionState.local_locked, R.ImgLocked },
        { EConversionState.unlocking, R.ImgUnlocking },
        { EConversionState.unlocking_failed, R.ImgUnlockingFailed },
        { EConversionState.local_unlocked, R.ImgUnlocked },
        { EConversionState.exported, R.ImgCheckMauve },
        { EConversionState.converting, R.ImgRight },
        { EConversionState.converted, R.ImgCheck },
        { EConversionState.converted_unknown, R.ImgCheckGrey },
        { EConversionState.conversion_error, R.ImgRightFailed },
      };

    protected DataSourceBase (T datasource) : base (datasource) {
    }

    protected static Image imagePlus (Image image) {
      Bitmap bitmap = new Bitmap (image);
      using var gr = Graphics.FromImage (bitmap);
      gr.DrawImage (R.ImgPlus, 0, 0);
      return bitmap;
    }

    [GlobalizedProperty]
    public virtual string Title => DataSource.Title;

    [GlobalizedProperty]
    public virtual string Author => DataSource.Author;

    [GlobalizedProperty]
    public virtual string Duration =>
      DurationBacking?.ToStringHMS ();

    [Browsable (false)]
    public TimeSpan? DurationBacking => DataSource.RunTimeLengthSeconds.HasValue ? 
      TimeSpan.FromSeconds (DataSource.RunTimeLengthSeconds.Value) : null;
    
    [Browsable (false)]
    public virtual long? FileSizeBacking => DataSource.FileSizeBytes;

    [GlobalizedProperty]
    public virtual string Narrator => DataSource.Narrator;

    [GlobalizedProperty]
    public virtual DateTime? PurchaseDate => DataSource.PurchaseDate?.Date;

    protected string fileSize (long ? size) => size is null ? null : $"{size.Value / (1024 * 1024)} MB";
  }
}
