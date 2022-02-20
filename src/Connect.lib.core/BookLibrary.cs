using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using core.audiamus.aux;
using core.audiamus.aux.ex;
using core.audiamus.booksdb;
using core.audiamus.booksdb.ex;
using core.audiamus.connect.ex;
using Microsoft.EntityFrameworkCore;
using R = core.audiamus.connect.Properties.Resources;
using static core.audiamus.aux.Logging;

namespace core.audiamus.connect {
  class BookLibrary : IBookLibrary {
    const int PAGE_SIZE = 200;

    public readonly string _dbDir = null;
    public readonly string IMG_DIR = Path.Combine (ApplEnv.LocalApplDirectory, "img");

    public readonly Dictionary<ProfileId, IEnumerable<Book>> _bookCache = 
      new Dictionary<ProfileId, IEnumerable<Book>> ();

    public BookLibrary (string dbDir = null) => _dbDir = dbDir;

    public async Task<DateTime> SinceLatestPurchaseDateAsync (ProfileId profileId, bool resync) {
      return await Task.Run (() => sinceLatestPurchaseDate (profileId, resync));
    }

    public async Task AddRemBooksAsync (List<adb.json.Product> libProducts, ProfileId profileId, bool resync) {
      using var _ = new LogGuard (3, this, () => $"#items={libProducts.Count}");
      await Task.Run (() => addRemBooks (libProducts, profileId, resync));
      await Task.Run (() => cleanupDuplicateAuthors ());
    }

    public async Task AddCoverImagesAsync (Func<string, Task<byte[]>> downloadFunc) {
      using var _ = new LogGuard (3, this);

      Directory.CreateDirectory (IMG_DIR);

      using var dbContext = new BookDbContextLazyLoad (_dbDir);
      var files = Directory.GetFiles (IMG_DIR);

      var books = dbContext.Books
        .ToList ()
        .Where (c => c.CoverImageFile is null || !files.Contains (c.CoverImageFile))
        .ToList ();

      Log (3, this, () => $"#img={books.Count}");
      foreach (Book book in books) {
        Log (3, this, () => book.ToString ());
        string url = book.CoverImageUrl;
        if (url is null)
          continue;
        byte[] img = await downloadFunc (url);
        if (img is null)
          continue;

        string ext = img.FindImageFormat ();
        if (ext is null)
          continue;
        string filename = $"{book.Asin}{ext}";
        string path = Path.Combine (IMG_DIR, filename);
        try {
          await File.WriteAllBytesAsync (path, img);

          book.CoverImageFile = path;
        } catch (Exception) { }
      }

      dbContext.SaveChanges ();
    }

    public IEnumerable<Book> GetBooks (ProfileId profileId) {
      using var _ = new LogGuard (3, this, () => profileId.ToString ());

      lock (_bookCache) {
        bool succ = _bookCache.TryGetValue (profileId, out var cached);
        if (succ) {
          Log (3, this, () => $"from cache, #books={cached.Count ()}");
          return cached;
        }
      }

      using var dbContext = new BookDbContext (_dbDir);
      //using var rg = new ResourceGuard (x => dbContext.ChangeTracker.LazyLoadingEnabled = !x);

      IEnumerable<Book> books = dbContext.Books
        .Include (b => b.Conversion)
        .Include (b => b.Components)
        .ThenInclude (c => c.Conversion)
        .Include (b => b.Authors)
        .Include (b => b.Narrators)
        .Include (b => b.Series)
        .ThenInclude (s => s.Series)
        .Include (b => b.Ladders)
        .ThenInclude (l => l.Rungs)
        .ThenInclude (r => r.Genre)
        .Include (b => b.Genres)
        .Include (b => b.Codecs)
        .ToList ();

      var booksByProfile = books
        .Where (b => b.Conversion.AccountId == profileId.AccountId && b.Conversion.Region == profileId.Region)
        .ToList ();

      lock (_bookCache)
        _bookCache[profileId] = booksByProfile;

      Log (3, this, () => $"from DB, #books={booksByProfile.Count ()}");

      return booksByProfile;
    }

    public IEnumerable<AccountAlias> GetAccountAliases () {
      using var _ = new LogGuard (3, this);
      using var dbContext = new BookDbContextLazyLoad (_dbDir);
      var accounts = dbContext.Accounts.ToList ();
      var contexts = accounts
        .Select (a => new AccountAlias (a.AudibleId, a.Alias))
        .ToList ();
      Log (3, this, () => $"#contexts={contexts.Count}");
      return contexts;
    }

    public AccountAliasContext GetAccountId (IProfile profile) {
      using var _ = new LogGuard (3, this);
      using var dbContext = new BookDbContextLazyLoad (_dbDir);

      string accountId = profile.CustomerInfo.AccountId;
      var account = dbContext.Accounts.FirstOrDefault (a => a.AudibleId == accountId);
      if (account is null) {
        List<uint> hashes = getAliasHashes ();
        account = new Account {
          AudibleId = accountId
        };
        dbContext.Accounts.Add (account);
        dbContext.SaveChanges ();
        return new AccountAliasContext (account.Id, profile.CustomerInfo.Name, hashes);
      } else {
        if (account.Alias.IsNullOrWhiteSpace ())
          return new AccountAliasContext (account.Id, profile.CustomerInfo.Name, getAliasHashes ());
        else
          return new AccountAliasContext (account.Id, null, null) {
            Alias = account.Alias
          };
      }

      List<uint> getAliasHashes () {
        return dbContext.Accounts
          .ToList ()
          .Where (a => !a.Alias.IsNullOrWhiteSpace ())
          .Select (a => a.Alias.Checksum32 ())
          .ToList ();
      }
    }


    public void SetAccountAlias (AccountAliasContext ctxt) {
      using var _ = new LogGuard (3, this);
      if (ctxt.Alias.IsNullOrWhiteSpace ())
        return;
      using var dbContext = new BookDbContextLazyLoad (_dbDir);
      var account = dbContext.Accounts.FirstOrDefault (a => a.Id == ctxt.LocalId);
      if (account is null)
        return;
      account.Alias = ctxt.Alias;
      dbContext.SaveChanges ();
    }

    public void SavePersistentState (Conversion conversion, EConversionState state) {
      using var _ = new LogGuard (4, this);
      using var dbContext = new BookDbContext (_dbDir);
      var conv = dbContext.Conversions.FirstOrDefault (c => conversion.Id == c.Id);
      if (conv is null)
        return;
      updateState (conv, state, conversion);
      dbContext.SaveChanges ();
    }

    public void RestorePersistentState (Conversion conversion) {
      using var _ = new LogGuard (4, this);
      using var dbContext = new BookDbContext (_dbDir);
      Conversion saved = dbContext.Conversions.FirstOrDefault (c => c.Id == conversion.Id);
      if (saved is not null)
        conversion.State = saved.State;
    }

    public EConversionState GetPersistentState (Conversion conversion) {
      using var _ = new LogGuard (4, this);
      using var dbContext = new BookDbContext (_dbDir);
      Conversion saved = dbContext.Conversions.FirstOrDefault (c => c.Id == conversion.Id);
      return saved?.State ?? EConversionState.unknown;
    }

    public void UpdateComponentProduct (IEnumerable<ProductComponentPair> componentPairs) {
      using var _ = new LogGuard (3, this);
      lock (this) {
        using var dbContext = new BookDbContext (_dbDir);
        foreach (var (item, comp) in componentPairs) {
          dbContext.Components.Attach (comp);
          comp.RunTimeLengthSeconds = item.runtime_length_min * 60;
          comp.Title = item.title;
        }
        dbContext.SaveChanges ();
      }
    }

    public void GetChapters (IBookCommon item) {
      if (item.ChapterInfo?.Chapters?.Count > 0)
        return;
      
      using var _ = new LogGuard (3, this, () => item.ToString ());

      try {
        using var dbContext = new BookDbContext (_dbDir);
        if (item is Book book) {
          dbContext.Books.Attach (book);
          dbContext.Entry (book).Reference (b => b.ChapterInfo).Load ();
          dbContext.Entry (book.ChapterInfo).Collection (ci => ci.Chapters).Load ();
        } else if (item is Component comp) {
          dbContext.Components.Attach (comp);
          dbContext.Entry (comp).Reference (c => c.ChapterInfo).Load ();
          dbContext.Entry (comp.ChapterInfo).Collection (ci => ci.Chapters).Load ();
        }

        getChapters (dbContext, item.ChapterInfo.Chapters);

        sortChapters (item.ChapterInfo.Chapters);

      } catch (Exception exc) {
        Log (1, this, () =>
          $"{item}, throwing{Environment.NewLine}" +
          $"{exc.Summary ()})");
        throw;
      }

    }

    public IEnumerable<Chapter> GetChaptersFlattened (IBookCommon item) {
      GetChapters (item);

      var flattened = new List<Chapter> ();

      getChaptersFlattened (item.ChapterInfo?.Chapters, flattened);

      return flattened;
    }


    public AudioQuality UpdateLicenseAndChapters (
      adb.json.ContentLicense license, 
      Conversion conversion, 
      EDownloadQuality downloadQuality
    ) {
      using var _ = new LogGuard (3, this, () => conversion.ToString ());
      try {
        using var dbContext = new BookDbContext (_dbDir);
        dbContext.Conversions.Attach (conversion);

        conversion.DownloadUrl = license.content_metadata.content_url.offline_url;

        var product = conversion.BookCommon;

        if (product is Component comp)
          dbContext.Components.Attach (comp);
        else if (product is Book book)
          dbContext.Books.Attach (book);

        var voucher = license.voucher;

        // Key and IV
        product.LicenseKey = voucher?.key;
        product.LicenseIv = voucher?.iv;

        var aq = setDownloadFilenameAndCodec (license, conversion, downloadQuality);

        // file size
        product.FileSizeBytes = license.content_metadata?.content_reference?.content_size_in_bytes;

        // duration
        int? runtime = license.content_metadata?.chapter_info?.runtime_length_sec;
        if (runtime.HasValue)
          product.RunTimeLengthSeconds = runtime;

        // chapters
        addChapters (dbContext, license, conversion);

        updateState (conversion, EConversionState.license_granted);

        dbContext.SaveChanges ();
        return aq;
      } catch (Exception exc) {
        Log (1, this, () =>
          $"{conversion}, throwing{Environment.NewLine}" +
          $"{exc.Summary ()})");
        throw;
      }

    }

    private static bool __checkUpdateAnswered;
    private static bool? __checkUpdateAnswer;

    public void CheckUpdateFilesAndState (
      ProfileId profileId,
      IDownloadSettings downloadSettings,
      IExportSettings exportSettings,
      Action<IConversion> callbackRefConversion,
      IInteractionCallback<InteractionMessage<BookLibInteract>, bool?> interactCallback
    ) {

      using var lg = new LogGuard (3, this);
      using var dbContext = new BookDbContextLazyLoad (_dbDir);

      var collectedCallbacks = new List<IConversion> ();

      var conversions = dbContext.Conversions
        .ToList ();

      conversions = conversions
        .Where (c => c.AccountId == profileId.AccountId && c.Region == profileId.Region)
        .ToList ();

      var dnlddir = downloadSettings.DownloadDirectory;
      foreach (var conv in conversions) {
        var _ = conv.State switch {
          EConversionState.local_locked => checkLocalLocked (conv, callback, dnlddir),
          EConversionState.local_unlocked => checkLocalUnlocked (conv, callback, dnlddir),
          EConversionState.exported => checkExported (conv, callback, dnlddir, exportSettings?.ExportDirectory),
          EConversionState.converted => checkConverted (conv, callback, dnlddir),
          _ => false
        };
        checkRemoved (conv, callback);
      }

      if (collectedCallbacks.Any ()) {
        if (!__checkUpdateAnswered && interactCallback is not null) {
          __checkUpdateAnswer = interactCallback.Interact (
            new InteractionMessage<BookLibInteract> (
              ECallbackType.question3,
              null,
              new (EBookLibInteract.checkFile)));
          __checkUpdateAnswered = true;
        }

        Log (3, this, () => $"Interact response={__checkUpdateAnswer}");

        if (!__checkUpdateAnswer.HasValue)
          return;

        collectedCallbacks.ForEach (c => callbackRefConversion (c));

        if (__checkUpdateAnswer.HasValue && __checkUpdateAnswer.Value)
          dbContext.SaveChanges ();
      }

      void callback (IConversion conv) {
        collectedCallbacks.Add (conv);
      }
    }

    private void checkRemoved (Conversion conv, Action<IConversion> callback) {
      var book = conv.Book;
      if (book?.Deleted is null)
        return;
      bool removed = book.Deleted.Value;
      if (removed) {
        if (conv.State > EConversionState.unknown && conv.State < EConversionState.local_locked) {
          updateState (conv, EConversionState.unknown);
          callback (conv);
          Log (3, this, () => $"removed: {conv}");
        }
        foreach (var comp in book.Components) {
          var cconv = comp.Conversion;
          if (cconv.State > EConversionState.unknown && cconv.State < EConversionState.local_locked) {
            updateState (cconv, EConversionState.unknown);
            callback (cconv);
            Log (3, this, () => $"removed: {cconv}");
          }
        }
      } else {
        if (conv.State == EConversionState.unknown) {
          updateState (conv, EConversionState.remote);
          callback (conv);
          Log (3, this, () => $"re-added: {conv}");
        }
        foreach (var comp in book.Components) {
          var cconv = comp.Conversion;
          if (cconv.State == EConversionState.unknown) {
            updateState (cconv, EConversionState.remote);
            callback (cconv);
            Log (3, this, () => $"re-added: {cconv}");
          }
        }
      } 
    }

    private bool checkLocalLocked (Conversion conv, Action<IConversion> callback, string downloadDirectory) =>
      checkFile (conv, R.EncryptedFileExt, callback, downloadDirectory,
        EConversionState.remote, ECheckFile.deleteIfMissing | ECheckFile.relocatable);


    private bool checkLocalUnlocked (Conversion conv, Action<IConversion> callback, string downloadDirectory) {
      return checkLocal (conv, callback, downloadDirectory);
    }

    private bool checkLocal (
      Conversion conv,
      Action<IConversion> callback,
      string downloadDirectory,
      EConversionState? transientfallback = null
    ) {
      bool succ = checkFile (conv, R.DecryptedFileExt, callback, downloadDirectory,
        EConversionState.local_locked, ECheckFile.relocatable, transientfallback);
      if (!succ)
        succ = checkFile (conv, R.EncryptedFileExt, callback, downloadDirectory,
          EConversionState.remote, ECheckFile.deleteIfMissing | ECheckFile.relocatable, transientfallback);
      return succ;
    }

    private bool checkExported (
      Conversion conv, Action<IConversion> callback,
      string downloadDirectory, string exportDirectory
    ) {
      bool succ = checkFile (conv, R.ExportedFileExt, callback, exportDirectory,
        EConversionState.local_unlocked, ECheckFile.none, EConversionState.converted_unknown);
      if (!succ)
        succ = checkLocal (conv, callback, downloadDirectory, EConversionState.converted_unknown);
      return succ;
    }

    static readonly IEnumerable<string> __extensions = new string[] { ".m3u", ".mp3", ".m4a", ".m4b" };

    private bool checkConverted (Conversion conv, Action<IConversion> callback, string downloadDirectory) {
      bool succ = checkConvertedFiles (conv, callback);
      if (!succ)
        succ = checkLocal (conv, callback, downloadDirectory, EConversionState.converted_unknown);
      return succ;
    }

    private bool checkConvertedFiles (Conversion conv, Action<IConversion> callback) {
      string dir = conv.DestDirectory.AsUncIfLong ();
      bool exists = false;
      if (Directory.Exists (dir)) {
        string[] files = Directory.GetFiles (dir);
        exists = files
          .Select (f => Path.GetExtension (f).ToLower ())
          .Where (e => __extensions.Contains (e))
          .Any ();
      }

      if (exists)
        return true;
      else {
        Log (3, this, () => $"not found: \"{Path.GetFileNameWithoutExtension (conv.DownloadFileName)}\"");
        conv.State = EConversionState.converted_unknown;
        callback?.Invoke (conv);
        return false;
      }
    }

    private bool checkFile (
      Conversion conv,
      string ext,
      Action<IConversion> callback,
      string downloadDirectory,
      EConversionState fallback,
      ECheckFile flags,
      EConversionState? transientfallback = null
    ) {

      if (flags.HasFlag (ECheckFile.relocatable)) {
        if (downloadDirectory is null)
          return false;
        string path = (conv.DownloadFileName + ext).AsUncIfLong ();
        if (File.Exists (path))
          return true;

        if (conv.DownloadFileName is not null) {
          string filename = Path.GetFileNameWithoutExtension (conv.DownloadFileName);
          string pathStub = Path.Combine (downloadDirectory, filename);
          path = (pathStub + ext).AsUncIfLong ();

          if (File.Exists (path)) {
            conv.DownloadFileName = pathStub;
            return true;
          }
        }
      } else {
        string filename = Path.GetFileNameWithoutExtension (conv.DownloadFileName);
        string pathStub = Path.Combine (downloadDirectory, filename);
        string path = (pathStub + ext).AsUncIfLong ();
        if (File.Exists (path))
          return true;
      }

      Log (3, this, () => $"not found \"{ext}\": \"{Path.GetFileNameWithoutExtension(conv.DownloadFileName)}\"");

      if (flags.HasFlag (ECheckFile.deleteIfMissing))
        conv.DownloadFileName = null;

      if (transientfallback.HasValue) {
        var tmp = conv.Copy ();
        tmp.State = transientfallback.Value;
        callback?.Invoke (tmp);
      }

      updateState (conv, fallback);
      if (!transientfallback.HasValue)
        callback?.Invoke (conv);

      return false;
    }

    private static void updateState (Conversion conversion, EConversionState state, Conversion original = null) {
      conversion.State = state;
      conversion.LastUpdate = DateTime.UtcNow;
      if (original is not null) {
        original.State = conversion.State;
        original.LastUpdate = conversion.LastUpdate;
        original.PersistState = conversion.State;
      }
    }

    private static AudioQuality setDownloadFilenameAndCodec (
      adb.json.ContentLicense license, 
      Conversion conversion,
      EDownloadQuality downloadQuality
    ) {
      var product = conversion.BookCommon;

      product.DownloadQuality = downloadQuality;

      // download destination
      string dir = conversion.DownloadFileName;

      var sb = new StringBuilder ();

      // title plus asin plus codec.aaxc.m4b 
      string title = product.Title.Prune ();
      title = title.Substring (0, Math.Min (20, title.Length));
      sb.Append (title);

      string asin = product.Asin;
      sb.Append ($"_{asin}_LC");

      AudioQuality aq = null;
      string format = license.content_metadata?.content_reference?.content_format?.ToLower ();
      bool succ = Enum.TryParse<ECodec> (format, out ECodec codec);
      if (succ) {
        product.FileCodec = codec;
        aq = codec.ToQuality ();
        if (aq is not null) {
          product.BitRate = aq.BitRate;
          product.SampleRate = aq.SampleRate;
          if (aq.BitRate.HasValue)
            sb.Append ($"_{aq.BitRate.Value}");
          if (aq.SampleRate.HasValue)
            sb.Append ($"_{aq.SampleRate.Value}");
        }
      }

      string filename = sb.ToString ();// + ".aaxc.m4b";
      string path = Path.Combine (dir, filename);
      conversion.DownloadFileName = path;
      return aq;
    }

    // internal instead of private for testing only
    internal static void addChapters (BookDbContext dbContext, adb.json.ContentLicense license, Conversion conversion) {
      var source = license?.content_metadata?.chapter_info;
      if (source is null)
        return;

      var product = conversion.BookCommon;

      ChapterInfo chapterInfo = new ChapterInfo ();
      dbContext.ChapterInfos.Add (chapterInfo);
      if (product is Book book) {
        dbContext.Entry (book).Reference (b => b.ChapterInfo).Load ();
        if (book.ChapterInfo is not null)
          dbContext.Remove (book.ChapterInfo);
        book.ChapterInfo = chapterInfo;
      } else if (product is Component comp) {
        dbContext.Entry (comp).Reference (b => b.ChapterInfo).Load ();
        if (comp.ChapterInfo is not null)
          dbContext.Remove (comp.ChapterInfo);
        comp.ChapterInfo = chapterInfo;
      }

      chapterInfo.BrandIntroDurationMs = source.brandIntroDurationMs;
      chapterInfo.BrandOutroDurationMs = source.brandOutroDurationMs;
      chapterInfo.IsAccurate = source.is_accurate;
      chapterInfo.RuntimeLengthMs = source.runtime_length_ms;

      if (source.chapters.IsNullOrEmpty ())
        return;

      foreach (var ch in source.chapters) {
        Chapter chapter = new Chapter ();
        dbContext.Chapters.Add (chapter);
        chapterInfo.Chapters.Add (chapter);

        setChapter (ch, chapter);

        if (!ch.chapters.IsNullOrEmpty ())
          addChapters (dbContext, ch, chapter);
      }
    }

    private static void setChapter (adb.json.Chapter src, Chapter chapter) {
      chapter.LengthMs = src.length_ms;
      chapter.StartOffsetMs = src.start_offset_ms;
      chapter.Title = src.title;
    }

    private static void addChapters (BookDbContext dbContext, adb.json.Chapter source, Chapter parent) {
      foreach (var ch in source.chapters) {
        Chapter chapter = new Chapter ();
        dbContext.Chapters.Add (chapter);

        parent.Chapters.Add (chapter);

        setChapter (ch, chapter);

        if (!ch.chapters.IsNullOrEmpty ())
          addChapters (dbContext, ch, chapter);
      }
    }

    private void addRemBooks (List<adb.json.Product> libProducts, ProfileId profileId, bool resync) {
      lock (_bookCache)
        _bookCache.Remove (profileId);

      using var dbContext = new BookDbContextLazyLoad (_dbDir);

      var bcl = new BookCompositeLists (
        dbContext.Books.Select (b => b.Asin).ToList (),
        dbContext.Conversions.ToList (),
        dbContext.Components.ToList (),
        dbContext.Series.ToList (),
        dbContext.SeriesBooks.ToList (),
        dbContext.Authors.ToList (),
        dbContext.Narrators.ToList (),
        dbContext.Genres.ToList (),
        dbContext.Ladders.ToList (),
        dbContext.Rungs.ToList (),
        dbContext.Codecs.ToList ()
      );


      int page = 0;
      int remaining = libProducts.Count;
      while (remaining > 0) {
        int count = Math.Min (remaining, PAGE_SIZE);
        int start = page * PAGE_SIZE;
        page++;
        remaining -= count;
        var subrange = libProducts.GetRange (start, count);
        addPageBooks (dbContext, bcl, subrange, profileId, resync);
      }

      if (resync)
        removeBooks (dbContext, bcl, libProducts, profileId);
    }


    private DateTime sinceLatestPurchaseDate (ProfileId profileId, bool resync) {
      DateTime dt = new DateTime (1970, 1, 1);
      if (resync)
        return dt;

      using var dbContext = new BookDbContextLazyLoad (_dbDir);

      var latest = dbContext.Books
          .Where (b => b.PurchaseDate.HasValue && 
            b.Conversion.AccountId == profileId.AccountId &&
            b.Conversion.Region == profileId.Region)
          .Select (b => b.PurchaseDate.Value)
          .OrderBy (b => b)
          .LastOrDefault ();
      if (latest != default)
        dt = latest + TimeSpan.FromMilliseconds (1);

      return dt;
    }

    private void cleanupDuplicateAuthors () {
      using var dbContext = new BookDbContextLazyLoad (_dbDir);

      var authors = dbContext.Authors;

      var duplicates = authors
        .ToList ()
        .GroupBy (x => x.Name)
        .Where (g => g.Count () > 1)
        .ToList ();

      const int PseudoKeyLength = 7;
      foreach (var d in duplicates) {
        var asinAuthor = d.FirstOrDefault (d => d.Asin.Length > PseudoKeyLength);
        if (asinAuthor is null)
          continue;
        foreach (var author in d) {
          if (author == asinAuthor)
            continue;

          foreach (var book in author.Books) {
            book.Authors.Remove (author);
            book.Authors.Add (asinAuthor);
          }
          authors.Remove (author);

        }
      }

      dbContext.SaveChanges ();
    }

    private void addPageBooks (BookDbContextLazyLoad dbContext, BookCompositeLists bcl, IEnumerable<adb.json.Product> products, ProfileId profileId, bool resync) {
      try {
        using var _ = new LogGuard (3, this, () => $"#items={products.Count ()}");

        foreach (var product in products) {
          try {

            if (readded (bcl, product, resync))
              continue;

            Book book = addBook (dbContext, product);

            addComponents (book, bcl.Components, product.relationships);

            addConversions (book, bcl.Conversions, profileId);

            addSeries (book, bcl.Series, bcl.SeriesBooks, product.relationships);

            addPersons (dbContext, book, bcl.Authors, product.authors, b => b.Authors);
            addPersons (dbContext, book, bcl.Narrators, product.narrators, b => b.Narrators);

            addGenres (book, bcl.Genres, bcl.Ladders, bcl.Rungs, product.category_ladders);

            addCodecs (book, bcl.Codecs, product.available_codecs);
          } catch (Exception exc) {
            Log (1, this, () =>
              $"asin={product.asin}, \"{product.title}\", throwing{Environment.NewLine}" +
              $"{exc.Summary ()})");
            throw;
          }
        }

        dbContext.SaveChanges ();
      } catch (Exception exc) {
        Log (1, this, () => exc.Summary ());
        throw;
      }
    }

    private static bool readded (BookCompositeLists bcl, adb.json.Product product, bool resync) {
      if (bcl.BookAsins.Contains (product.asin)) {
        if (!resync)
          return true;

        var bk = bcl.Conversions
          .FirstOrDefault (conv => string.Equals (conv.Book?.Asin, product.asin))?.Book;
        if (!(bk?.Deleted ?? false))
          return true;

        bk.Deleted = false;
        if (bk.Conversion.State < EConversionState.local_locked)
          updateState (bk.Conversion, EConversionState.remote);

        foreach (var comp in bk.Components) {
          if (comp.Conversion.State < EConversionState.local_locked)
            updateState (comp.Conversion, EConversionState.remote);
        }

        return true;
      } else
        return false;
    }

    private static Book addBook (BookDbContextLazyLoad dbContext, adb.json.Product product) {
      Book book = new Book {
        Asin = product.asin,
        Title = product.title,
        Subtitle = product.subtitle,
        PublisherName = product.publisher_name,
        PublisherSummary = product.publisher_summary,
        MerchandisingSummary = product.merchandising_summary,
        AverageRating = product.rating?.overall_distribution?.average_rating,
        RunTimeLengthSeconds = product.runtime_length_min.HasValue ? product.runtime_length_min.Value * 60 : null,
        AdultProduct = product.is_adult_product,
        PurchaseDate = product.purchase_date,
        ReleaseDate = product.release_date ?? product.issue_date,
        Language = product.language,
        CoverImageUrl = product.product_images?._500,
        Sku = product.sku,
        SkuLite = product.sku_lite
      };

      bool succ = Enum.TryParse<EDeliveryType> (product.content_delivery_type, out var deltype);
      if (succ)
        book.DeliveryType = deltype;

      if (!product.format_type.IsNullOrEmpty ())
        book.Unabridged = product.format_type == "unabridged";

      dbContext.Books.Add (book);
      return book;
    }

    private static void addComponents (Book book, ICollection<Component> components, IEnumerable<adb.json.Relationship> itmRelations) {
      var relations = itmRelations?
        .Where (r => r.relationship_to_product == "child" && r.relationship_type == "component")
        .ToList ();

      if (relations.IsNullOrEmpty ())
        return;

      foreach (var rel in relations) {
        int.TryParse (rel.sort, out int partNum);
        var component = new Component {
          Asin = rel.asin,
          Title = rel.title,
          Sku = rel.sku,
          SkuLite = rel.sku_lite,
          PartNumber = partNum
        };

        components.Add (component);
        book.Components.Add (component);
      }
    }

    const string REGEX_SERIES = @"(\d+)(\.(\d+))?";
    static readonly Regex _regexSeries = new Regex (REGEX_SERIES, RegexOptions.Compiled);

    private static void addSeries (Book book, ICollection<Series> series, ICollection<SeriesBook> seriesBooks, IEnumerable<adb.json.Relationship> itmRelations) {
      if (itmRelations is null)
        return;

      var itmSeries = itmRelations.Where (r => r.relationship_to_product == "parent" && r.relationship_type == "series").ToList ();

      foreach (var itmSerie in itmSeries) {
        var serie = series.FirstOrDefault (s => s.Asin == itmSerie.asin);
        if (serie is null) {
          serie = new Series {
            Asin = itmSerie.asin,
            Title = itmSerie.title,
            Sku = itmSerie.sku,
            SkuLite = itmSerie.sku_lite
          };
          series.Add (serie);
        }

        var seriesBook = new SeriesBook {
          Book = book,
          Series = serie,
          Sequence = itmSerie.sequence
        };

        bool succ = int.TryParse (itmSerie.sort, out int sort);
        if (succ)
          seriesBook.Sort = sort;

        Match match = _regexSeries.Match (itmSerie.sequence);
        if (match.Success) {

          int n = match.Groups.Count;
          if (n >= 2) {

            string major = match.Groups[1].Value;
            succ = int.TryParse (major, out int num);
            if (succ) {
              seriesBook.BookNumber = num;

              if (n >= 3) {
                string minor = match.Groups[3].Value;
                succ = int.TryParse (minor, out int subnum);
                if (succ)
                  seriesBook.SubNumber = int.Parse (minor);
              }
            }
          }
        }

        seriesBooks.Add (seriesBook);
        book.Series.Add (seriesBook);
      }
    }

    private static void addPersons<TPerson> (
      BookDbContextLazyLoad dbContext,
      Book book,
      ICollection<TPerson> persons,
      IEnumerable<adb.json.IPerson> itmPersons,
      Func<Book, ICollection<TPerson>> getBookPersons
    )
      where TPerson : class, IPerson, new() {
      if (itmPersons is null)
        return;

      foreach (var itmPerson in itmPersons) {
        TPerson person = null;
        if (itmPerson.asin is null) {
          person = persons.FirstOrDefault (a => a.Name == itmPerson.name);
          if (person is null)
            itmPerson.asin = dbContext.GetNextPseudoAsin (typeof (TPerson));
        }
        if (person is null)
          person = persons.FirstOrDefault (a => a.Asin == itmPerson.asin);

        if (person is null) {
          person = new TPerson {
            Asin = itmPerson.asin,
            Name = itmPerson.name
          };

          persons.Add (person);

        }
        person.Books.Add (book);
        getBookPersons (book).Add (person);
      }
    }


    private static void addGenres (Book book, ICollection<Genre> genres, ICollection<Ladder> ladders, ICollection<Rung> rungs, IEnumerable<adb.json.Category> itmCategories) {
      if (itmCategories is null)
        return;

      var categories = itmCategories.Where (c => c.root == "Genres").ToList ();

      foreach (var category in categories) {
        var ladder = new Ladder ();

        for (int i = 0; i < category.ladder.Length; i++) {
          var itmLadder = category.ladder[i];
          int idx = i + 1;
          bool succ = long.TryParse (itmLadder.id, out long id);
          if (!succ)
            continue;

          var genre = genres.FirstOrDefault (g => g.ExternalId == id);
          if (genre is null) {
            genre = new Genre {
              ExternalId = id,
              Name = itmLadder.name
            };
            genres.Add (genre);
          }

          book.Genres.Add (genre);

          var rung = rungs.FirstOrDefault (r => r.OrderIdx == idx && r.Genre == genre);
          if (rung is null) {
            rung = new Rung {
              OrderIdx = idx,
              Genre = genre
            };
            rungs.Add (rung);
          }

          ladder.Rungs.Add (rung);

        }

        var existingLadder = ladders.FirstOrDefault (l => equals (l, ladder));

        if (existingLadder is null)
          ladders.Add (ladder);
        else
          ladder = existingLadder;

        book.Ladders.Add (ladder);

      }

      // local function
      static bool equals (Ladder oldLadder, Ladder newLadder) {
        if (newLadder.Rungs.Count != oldLadder.Rungs.Count)
          return false;
        var rungs = oldLadder.Rungs.OrderBy (r => r.OrderIdx);
        var iter1 = newLadder.Rungs.GetEnumerator ();
        var iter2 = rungs.GetEnumerator ();
        while (iter1.MoveNext ()) {
          iter2.MoveNext ();
          var r1 = iter1.Current;
          var r2 = iter2.Current;
          if (r1.Genre != r2.Genre)
            return false;
        }
        return true;
      }
    }


    private static void addCodecs (Book book, ICollection<Codec> codecList, IEnumerable<adb.json.Codec> itmCodecs) {
      if (itmCodecs is null)
        return;

      foreach (var itmCodec in itmCodecs) {
        bool succ = Enum.TryParse<ECodec> (itmCodec.name, out var codecName);
        if (!succ)
          continue;

        var codec = codecList.FirstOrDefault (c => c.Name == codecName);
        if (codec is null) {
          codec = new Codec {
            Name = codecName
          };

          codecList.Add (codec);
        }

        book.Codecs.Add (codec);
      }
    }

    private static void addConversions (Book book, ICollection<Conversion> conversions, ProfileId profileId) {
      // default
      {
        var conversion = new Conversion {
          AccountId = profileId.AccountId,
          Region = profileId.Region
        };
        updateState (conversion, EConversionState.remote);
        book.Conversion = conversion;
        conversions.Add (conversion);
      }

      // components
      foreach (var component in book.Components) {
        if (component.Conversion is not null)
          continue;

        var conversion = new Conversion {
          State = EConversionState.remote,
          AccountId = profileId.AccountId,
          Region = profileId.Region
        };
        component.Conversion = conversion;
        conversions.Add (conversion);
      }

    }

    private void removeBooks (
      BookDbContextLazyLoad dbContext, 
      BookCompositeLists bcl, 
      IEnumerable<adb.json.Product> products, 
      ProfileId profileId
    ) {
      try {
        using var _ = new LogGuard (3, this, () => $"#items={products.Count ()}");

        var currentAsins = products.Select ( p=> p.asin).ToList ();
        var removeAsins = bcl.BookAsins.Except (currentAsins).ToList ();
        if (!removeAsins.Any ())
          return;

        Log (3, this, () => $"#remove={removeAsins.Count}");

        foreach (string asin in removeAsins) {

          var book = bcl.Conversions.FirstOrDefault (conv => string.Equals (conv.Book?.Asin, asin))?.Book;
          if (book is null)
            continue;

          book.Deleted = true;
          if (book.Conversion.State < EConversionState.local_locked)
            updateState (book.Conversion, EConversionState.unknown);
          foreach (var comp in book.Components) {
            if (comp.Conversion.State < EConversionState.local_locked)
              updateState (comp.Conversion, EConversionState.unknown);
          }
        }

        dbContext.SaveChanges ();
      } catch (Exception exc) {
        Log (1, this, () => exc.Summary ());
        throw;
      }

    }

    private void getChaptersFlattened (IEnumerable<Chapter> source, List<Chapter> dest) {
      if (source.IsNullOrEmpty ())
        return;

      foreach (var ch in source) {
        dest.Add (new Chapter(ch));
        getChaptersFlattened (ch.Chapters, dest);
      }
    }

    private void getChapters (BookDbContext dbContext, ICollection<Chapter> chapters) {
      if (chapters.IsNullOrEmpty ())
        return;
      foreach (var ch in chapters) {
        dbContext.Entry (ch).Collection (ci => ci.Chapters).Load ();
        getChapters (dbContext, ch.Chapters);
      }
    }

    private void sortChapters (ICollection<Chapter> chapters) {
      if (chapters.IsNullOrEmpty ())
        return;
      if (chapters is List<Chapter> list)
        list.Sort ((x, y) => x.StartOffsetMs.CompareTo (y.StartOffsetMs));

      chapters.ForEach (ch => sortChapters (ch.Chapters));
    }



  }

}
