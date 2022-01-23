using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using core.audiamus.aux;
using core.audiamus.aux.ex;
using core.audiamus.booksdb;
using core.audiamus.booksdb.ex;
using core.audiamus.connect.ex;
using core.audiamus.util;
using R = core.audiamus.connect.Properties.Resources;
using static core.audiamus.aux.Logging;

namespace core.audiamus.connect {
  public class AaxExporter {
    const string JSON = ".json";
    const string CONTENT_METADATA = "content_metadata_";
    const string SERIES_TITLES = "series_titles_";

    private static readonly object __lockable = new object ();

    private IExportSettings ExportSettings { get; }
    private IMultiPartSettings MultipartSettings { get; }
    public IBookLibrary BookLibrary { private get; set; }

    public AaxExporter (IExportSettings exportSettings, IMultiPartSettings multipartSettings) {
      ExportSettings = exportSettings;
      MultipartSettings = multipartSettings;
    }

    public void Export (Book book, SimpleConversionContext context, Action<Conversion> onNewStateCallback) {
      using var _ = new LogGuard (3, this, () => book.ToString ());
      if (book.Components.Count == 0 || !MultipartSettings.MultiPartDownload)
        exportSinglePart (book, context, onNewStateCallback);
      else
        exportMultiPart (book, context, onNewStateCallback);
    }

    private void exportSinglePart (
      IBookCommon book, 
      SimpleConversionContext context, 
      Action<Conversion> onNewStateCallback, 
      bool skipSeries = false
    ) {
      Log (3, this, () => book.ToString ());

      book.Conversion.State = EConversionState.converting;
      onNewStateCallback?.Invoke (book.Conversion);

      bool succ = copyFile (book, context);
      if (!succ) {
        book.Conversion.State = EConversionState.conversion_error;
        onNewStateCallback?.Invoke (book.Conversion);
        return;
      }
      exportChapters (book);

      exportProduct (book);

      if (!skipSeries)
        exportSeries (book);

      BookLibrary.SavePersistentState (book.Conversion, EConversionState.exported);
      onNewStateCallback?.Invoke (book.Conversion);
    }


    private void exportMultiPart (
      Book book, 
      SimpleConversionContext context, 
      Action<Conversion> onNewStateCallback
    ) {
      Log (3, this, () => book.ToString ());

      bool skipSeries = false;
      foreach (var comp in book.Components) {
        exportSinglePart (comp, context, onNewStateCallback, skipSeries);
        skipSeries = true;
      }
    }

    private bool copyFile (IBookCommon book, SimpleConversionContext context) {
      Log (3, this, () => book.ToString ());
      Conversion conv = book.Conversion;
      string sourcefile = (conv.DownloadFileName + R.DecryptedFileExt).AsUncIfLong();
      if (!File.Exists (sourcefile))
        return false;

      string filename = Path.GetFileNameWithoutExtension (conv.DownloadFileName);
      string destfile = Path.Combine (ExportSettings.ExportDirectory, filename + R.ExportedFileExt).AsUncIfLong();

      lock (__lockable) {
        bool succ = FileEx.Copy (sourcefile, destfile, true,
          pm => context.Progress?.Report (pm),
          () => context.CancellationToken.IsCancellationRequested
        );
        return succ;
      }
    }

    // internal instead of private for testing only
    internal string exportChapters (IBookCommon book) {
      if (book.ChapterInfo is null)
        BookLibrary?.GetChapters (book);

      if (book.ChapterInfo is null)
        return null;

      Log (3, this, () => book.ToString ());

      var chapterInfo = book.ChapterInfo;


      var cr = new adb.json.ContentReference {
        asin = book.Asin,
        content_size_in_bytes = book.FileSizeBytes ?? 0,
        sku = book.Sku
      };

      var ci = new adb.json.ChapterInfo ();
      var metadata = new adb.json.ContentMetadata {
        chapter_info = ci,
        content_reference = cr
      };
      var container = new adb.json.MetadataContainer {
        content_metadata = metadata
      };

      ci.brandIntroDurationMs = chapterInfo.BrandIntroDurationMs;
      ci.brandOutroDurationMs = chapterInfo.BrandOutroDurationMs;
      ci.is_accurate = chapterInfo.IsAccurate ?? false;
      ci.runtime_length_ms = chapterInfo.RuntimeLengthMs;
      ci.runtime_length_sec = chapterInfo.RuntimeLengthMs / 1000;

      var flattenedChapters = BookLibrary?.GetChaptersFlattened (book);

      if (!flattenedChapters.IsNullOrEmpty()) {
        var chapters = new List<adb.json.Chapter> ();
        foreach (var chapter in flattenedChapters) {
          var ch = new adb.json.Chapter {
            length_ms = chapter.LengthMs,
            start_offset_ms = chapter.StartOffsetMs,
            start_offset_sec = chapter.StartOffsetMs / 1000,
            title = chapter.Title
          };
          chapters.Add (ch);
        }
        ci.chapters = chapters.ToArray ();
      }

      string json = container.Serialize ();
      json = json.CompactJson ();

      string filename = CONTENT_METADATA + chapterInfo.BookMeta.Asin + JSON;
      string outpath = Path.Combine (ExportSettings.ExportDirectory, filename).AsUncIfLong();

      File.WriteAllText (outpath, json);

      return outpath;
    }

    private void exportProduct (IBookCommon book) {
      Log (3, this, () => book.ToString ());
      var product = makeProduct (book);

      var container = new adb.json.ProductResponse {
        product = product
      };

      string json = container.Serialize ();
      json = json.CompactJson ();

      string filename = book.Asin + JSON;
      string outpath = Path.Combine (ExportSettings.ExportDirectory, filename).AsUncIfLong();

      File.WriteAllText (outpath, json);

    }

    private void exportSeries (IBookCommon prod) {
      Book book = prod.GetBook ();
      if (book.Series.IsNullOrEmpty ())
        return;
      Log (3, this, () => book.ToString ());

      foreach (var serbook in book.Series) {
        var series = serbook.Series;       
        string asin = series.Asin;

        var products = new List<adb.json.Product> ();

        // sort by sort/num+sub/sequence
        IOrderedEnumerable<SeriesBook> sbks;
        if (!series.Books.Where (b => b.Sort is null).Any())
          sbks = series.Books.OrderBy (b => b.Sort);
        else if (!series.Books.Where (b => b.BookNumber == 0).Any())
          sbks = series.Books.OrderBy (b => b.BookNumber).ThenBy (b => b.SubNumber);
        else
          sbks = series.Books.OrderBy (b => b.Sequence);

        foreach (var sbk in sbks) {
          var p = makeProduct (sbk.Book);
          products.Add (p);
        }

        var container = new adb.json.SimsBySeriesResponse {
          similar_products = products.ToArray ()
        };

        string json = container.Serialize ();
        json = json.CompactJson ();

        string filename = SERIES_TITLES + asin + JSON;
        string outpath = Path.Combine (ExportSettings.ExportDirectory, filename).AsUncIfLong();

        File.WriteAllText (outpath, json);
      }
    }

    private adb.json.Product makeProduct (IBookCommon prod) {

      Book book = prod.GetBook ();
      Log (3, this, () => book.ToString ());

      // has_children;is_adult_product;is_listenable
      // asin
      // authors:name
      // title
      // series:title,sequence
      // sku; sku_lite

      var product = new adb.json.Product {
        asin = prod.Asin,
        title = prod.Title,
        sku = prod.Sku,
        sku_lite = prod.SkuLite,
        is_listenable = true,
        runtime_length_min = prod.RunTimeLengthSeconds / 60,
        has_children = prod is Book && book.Components.Count > 0,
        is_adult_product = book.AdultProduct ?? false
      };

      if (!book.Authors.IsNullOrEmpty ()) {
        var authors = new List<adb.json.Author> ();
        foreach (var author in book.Authors) {
          var a = new adb.json.Author {
            asin = author.Asin,
            name = author.Name
          };
          authors.Add (a);
        }
        product.authors = authors.ToArray ();
      }

      if (!book.Series.IsNullOrEmpty()) {
        var series = new List<adb.json.Series> ();
        foreach (var serbook in book.Series) {
          var s = new adb.json.Series {
            asin = serbook.Series.Asin,
            title = serbook.Series.Title,
            sequence = serbook.SeqString
          };
          series.Add (s);
        }
        product.series = series.ToArray ();
      }

      return product;
    }

  }
}
