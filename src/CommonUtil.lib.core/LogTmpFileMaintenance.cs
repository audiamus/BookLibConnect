using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.audiamus.aux;
using core.audiamus.aux.ex;

namespace core.audiamus.util {
  public class LogTmpFileMaintenance {
    record DirectoryStatistics (int NumFiles, long TotalSize, DateTime Timestamp);
    //record DirectoryFilesAndStatistics (List<FileInfo> Files, DirectoryStatistics Statistics);

    const int MAX_NUM_FILES_PER_DIR = 500;
    const long MAX_SIZE_PER_DIR = 100_000_000; // 100 MB 
    const int MAX_AGE_DAYS_PER_DIR = 365; 
    
    private bool _inProgress;
    private DateTime Today { get; set; }
    private DateTime Timestamp { get; set; }

    private static LogTmpFileMaintenance __instance;

    public static LogTmpFileMaintenance Instance { 
      get {
        if (__instance is null)
          __instance = new LogTmpFileMaintenance ();
        return __instance;
      } 
    }

    private LogTmpFileMaintenance () { }

    public async Task CleanupAsync () => await Task.Run (() => Cleanup ());

    public void Cleanup () {
      if (_inProgress)
        return;

      using var rg = new ResourceGuard (x => _inProgress = x);

      TimeSpan days = TimeSpan.FromDays (MAX_AGE_DAYS_PER_DIR);
      DateTime now = DateTime.Now;
      Timestamp = now - days;
      Today = now.Date;

      var tmp = gather (ApplEnv.TempDirectory);
      var log = gather (ApplEnv.LogDirectory);

      var tmp2 = cleanup (tmp.files, tmp.stats);
      var log2 = cleanup (log.files, log.stats);

      var tmp3 = cleanup (tmp.files, log2?.Timestamp ?? default);
      var log3 = cleanup (log.files, tmp2?.Timestamp ?? default);

      int numFiles = (tmp.stats?.NumFiles ?? 0) + (log.stats?.NumFiles ?? 0);
      long totalSize = (tmp.stats?.TotalSize ?? 0) + (log.stats?.TotalSize ?? 0);

      int removedFiles = tmp2?.NumFiles ?? 0 + log2?.NumFiles ?? 0 + tmp3?.NumFiles ?? 0 + log3?.NumFiles ?? 0;
      long removedSize = (tmp2?.TotalSize ?? 0 + log2?.TotalSize ?? 0 + tmp3?.TotalSize ?? 0 + log3?.TotalSize ?? 0);

      Logging.Log (2, this, () => $"before/after/removed: #files={numFiles}/{numFiles - removedFiles}/{removedFiles} " +
        $"size={totalSize / 1024}/{(totalSize - removedSize) / 1024}/{removedSize / 1024} kB");
    }

    private DirectoryStatistics cleanup (List<FileInfo> fileInfos, DirectoryStatistics stats) => cleanup (fileInfos, stats, null);
    
    private DirectoryStatistics cleanup (List<FileInfo> fileInfos, DateTime enforceByDate) => cleanup (fileInfos, null, enforceByDate);

    private DirectoryStatistics cleanup (List<FileInfo> fileInfos, DirectoryStatistics stats, DateTime? enforceByDate) {
      if (fileInfos is null)
        return null;

      bool exceeds = enforceByDate.HasValue || exceedsThresholds (stats);

      if (!exceeds)
        return default;

      if (enforceByDate.HasValue && enforceByDate.Value < fileInfos.Last().LastWriteTime)
        return default;

      if (!enforceByDate.HasValue && stats is null)
        return default;

      FileInfo [] files = fileInfos.ToArray ();

      int numFiles = 0;
      long totalSize = 0;
      DateTime oldest = default;

      for (int i = files.Length - 1; i >= 0; i--) {
        var fi = files[i];

        if (fi.LastWriteTime.Date == Today)
          break;

        try {
          File.Delete (fi.FullName);

          numFiles++;
          totalSize += fi.Length;
          oldest = i > 1 ? files[i - 1].LastWriteTime : default;
          
          fileInfos.RemoveAt (i);

          bool done;
          if (enforceByDate.HasValue)
            done = oldest > enforceByDate.Value;
          else
            done = !exceedsThresholds (new DirectoryStatistics (stats.NumFiles - numFiles, stats.TotalSize - totalSize, oldest));
          if (done)
            break;
        } catch (Exception exc) {
          Logging.Log (1, this, () => exc.Summary ());
        }
      }

      return new DirectoryStatistics (numFiles, totalSize, oldest);
    }

    private (List<FileInfo> files, DirectoryStatistics stats) gather (string dir) {
      if (!Directory.Exists (dir))
        return default;

      var di = new DirectoryInfo (dir);
      var fis = di.GetFiles ().OrderByDescending(fi => fi.LastWriteTime).ToList();
      
      long totalSize = 0;
      DateTime oldest = default;

      fis.ForEach (fi => {
        totalSize += fi.Length;
        if (oldest == default || oldest > fi.LastWriteTime)
          oldest = fi.LastWriteTime;
      });

      return (fis, new DirectoryStatistics (fis.Count, totalSize, oldest));
    }

    bool exceedsThresholds (DirectoryStatistics stats) {
      if (stats is null)
        return false;

      bool exceed = stats.NumFiles > MAX_NUM_FILES_PER_DIR ||
        stats.TotalSize > MAX_SIZE_PER_DIR ||
        stats.Timestamp < Timestamp;

      return exceed;
    }
  }
}
