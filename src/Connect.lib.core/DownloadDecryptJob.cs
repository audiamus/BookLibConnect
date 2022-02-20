using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using core.audiamus.aux;
using core.audiamus.aux.ex;
using core.audiamus.booksdb;
using core.audiamus.booksdb.ex;
using core.audiamus.util;
using static core.audiamus.aux.Logging;
using R = core.audiamus.connect.Properties.Resources;

namespace core.audiamus.connect {
  public class DownloadDecryptJob<T> : IDisposable where T : ICancellation {
    private static int MaxDecrypts => 1; // Environment.ProcessorCount / 2;

    private readonly ConcurrentDictionary<(Conversion, int), ThreadProgressBase<ProgressMessage>> _threadProgress = new ();
    private readonly ConcurrentBag<Task> _runningTasks = new ();
    private readonly ConcurrentBag<Book> _booksForConversion = new ();
    private readonly Semaphore _throttlingSemaphore = new (MaxDecrypts, MaxDecrypts);

    private IAudibleApi AudibleApi { get; }
    private IDownloadSettings Settings { get; }
    private Action<Conversion> OnNewStateCallback { get; }

    public DownloadDecryptJob (
      IAudibleApi api, 
      IDownloadSettings settings, 
      Action<Conversion> onNewStateCallback
    ) {
      AudibleApi = api;
      Settings = settings;
      OnNewStateCallback = onNewStateCallback;
    }

    public void Dispose () => _throttlingSemaphore?.Dispose ();

    public async Task DownloadDecryptAndConvertAsync (
      IEnumerable<Conversion> selectedConversions,
      IProgress<ProgressMessage> progress,
      T context,
      ConvertDelegate<T> convertAction
    ) {
      using var lg = new LogGuard (3, this, () => $"#conv={selectedConversions.Count ()}");

      using var rg = new ResourceGuard (() => {
        _runningTasks.Clear ();
        _threadProgress.Clear ();
        _booksForConversion.Clear ();
      });


      progress.Report (new (selectedConversions.Count (), null, null, null));
      var convs = selectedConversions.ToList ();
      foreach (var conv in convs) {
        if (context.CancellationToken.IsCancellationRequested)
          return;
        progress.Report (new (null, 1, null, null));
        await getLicenseAndDownloadAsync (conv, progress, context, convertAction);
      }

      while (_runningTasks.Any (t => !t.IsCompleted)) {
        await Task.WhenAll (_runningTasks.ToArray ());
      }

    }

    private async Task getLicenseAndDownloadAsync (
      Conversion conversion,
      IProgress<ProgressMessage> progress,
      T context,
      ConvertDelegate<T> convertAction
    ) {
      const int TP_KEY = 1;
      using var lg = new LogGuard (3, this, () => conversion.ToString ());

      using var tp = new ThreadProgressPerMille (pm => progress.Report (pm));
      _threadProgress.TryAdd ((conversion, TP_KEY), tp);

      bool succ = true;

      // Do we need to download?
      var savedState = AudibleApi.GetPersistentState (conversion);
      // the locked file may already exist
      bool hasLockedFile = File.Exists ((conversion.DownloadFileName + R.EncryptedFileExt).AsUncIfLong());
      // the unlocked file may already exist
      bool hasUnlockedFile = File.Exists ((conversion.DownloadFileName + R.DecryptedFileExt).AsUncIfLong());

      // download if neither file exists or state too low
      bool doDownload = savedState < EConversionState.local_locked || !hasLockedFile;
      bool doDecrypt = savedState < EConversionState.local_unlocked || !hasUnlockedFile;

      var previousQuality = conversion.ParentBook.ApplicableDownloadQuality (Settings.MultiPartDownload);
      var quality = Settings.DownloadQuality;
      bool higherQual = quality > previousQuality;
      if (higherQual)
        Log (3, this, () => $"{conversion}; desired higher quality: {quality}");
      doDownload |= higherQual;
      doDecrypt |= higherQual;

      if (doDownload && doDecrypt) {

        conversion.DownloadFileName = Settings.DownloadDirectory;

        var licTask = AudibleApi.GetDownloadLicenseAndSaveAsync (conversion, quality);
        OnNewStateCallback (conversion);
        succ = await licTask;
        OnNewStateCallback (conversion);

        if (!succ) {
          AudibleApi.SavePersistentState (conversion, EConversionState.license_denied);
          return;
        }

        var dnldTask = AudibleApi.DownloadAsync (conversion, onProgressSize, context.CancellationToken);
        OnNewStateCallback (conversion);
        succ = await dnldTask;
        OnNewStateCallback (conversion);

        if (!succ)
          return;
      } else {
        AudibleApi.RestorePersistentState (conversion);
        OnNewStateCallback (conversion);
      }

      if (succ) {
        Log (3, this, () => $"{conversion}; submit for decryption.");
        var decryptTask = Task.Run (() => decryptAsync (conversion, progress, context, convertAction));
        _runningTasks.Add (decryptTask);
      }


      void onProgressSize (Conversion conversion, long progPos) {
        if (_threadProgress.TryGetValue ((conversion, TP_KEY), out var tp)) {
          double filesize = conversion.BookCommon.FileSizeBytes ?? 0;
          double val = progPos / filesize;
          tp.Report (val);
        }
      }

    }

    private async Task decryptAsync (
      Conversion conversion,
      IProgress<ProgressMessage> progress,
      T context,
      ConvertDelegate<T> convertAction
    ) {
      const int TP_KEY = 2;
      using var lg = new LogGuard (3, this, () => conversion.ToString ());

      using var tp = new ThreadProgressPerCent (pm => progress.Report (pm));
      _threadProgress.TryAdd ((conversion, TP_KEY), tp);

      bool succ = true;

      // Do we need to decrypt?
      var savedState = AudibleApi.GetPersistentState (conversion);
      // the unlocked file may already exist
      bool hasUnlockedFile = File.Exists (conversion.DownloadFileName + R.DecryptedFileExt);
      // decrypt if file does not exist or state too low
      bool doDecrypt = savedState < EConversionState.local_unlocked || !hasUnlockedFile;

      if (doDecrypt) {
        _throttlingSemaphore.WaitOne ();
        Log (3, this, () => $"{conversion}; clear to run");
        using (new ResourceGuard (() => _throttlingSemaphore.Release ())) {
          int runLengthSecs = conversion.BookCommon.RunTimeLengthSeconds ?? 0;
          TimeSpan length = TimeSpan.FromSeconds (runLengthSecs);

          var decrTask = AudibleApi.DecryptAsync (conversion, onProgressTime, context.CancellationToken);
          OnNewStateCallback (conversion);
          succ = await decrTask;
          OnNewStateCallback (conversion);
        }

        try {
          if (succ && !Settings.KeepEncryptedFiles)
            File.Delete (conversion.DownloadFileName + R.EncryptedFileExt);
        } catch (Exception) {
        }
      } else {
        AudibleApi.RestorePersistentState (conversion);
        OnNewStateCallback (conversion);
      }

      if (succ && convertAction is not null) {
        Book book = conversion.ParentBook;
        if (book.ApplicableState (Settings.MultiPartDownload) >= EConversionState.local_unlocked) {
          bool filesExist = true;
          if (Settings.MultiPartDownload && !book.Components.IsNullOrEmpty ()) {
            foreach (var comp in book.Components) {
              hasUnlockedFile = File.Exists ((comp.Conversion.DownloadFileName + R.DecryptedFileExt).AsUncIfLong());
              filesExist &= hasUnlockedFile;
              if (!filesExist)
                break;
            }
          }
          if (filesExist && !_booksForConversion.Contains (book)) {
            _booksForConversion.Add (book);
            Log (3, this, () => $"{conversion}; submit for conversion.");
            var convertTask = Task.Run (() => convertAction (book, context, OnNewStateCallback));
            _runningTasks.Add (convertTask);
          }
        }
      }

      void onProgressTime (Conversion conversion, TimeSpan progPos) {
        if (_threadProgress.TryGetValue ((conversion, TP_KEY), out var tp)) {
          double runLengthSecs = conversion.BookCommon.RunTimeLengthSeconds ?? 0;
          double val = progPos.TotalSeconds / runLengthSecs;
          tp.Report (val);
        }
      }

    }

  }
}
