using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using core.audiamus.aux;
using core.audiamus.aux.ex;
using static core.audiamus.aux.Logging;
using InteractMessage = core.audiamus.aux.InteractionMessage<core.audiamus.util.UpdateInteractionMessage>;

namespace core.audiamus.util {
  public class OnlineUpdate {

    const string SETUP_REF_URL = "https://raw.githubusercontent.com/audiamus/{0}/master/res/Setup.json";
    //const string RGX_VERSION = @"-(\d+(?:\.\d+){1,3})-Setup";

    private static readonly Regex __rgxMd5 = new Regex (@"^(?:MD5:\s)?([A-Fa-f\d]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    //private readonly Regex _rgxVersion;
    private readonly string _setupRefUrl;
    private readonly string _defaultAppName;
    private readonly bool _dbg;

    //private string _downloadUri;
    //private string _setupFile;
    //private string _md5;
    //private Version _version = new Version ();

    private static HttpClient HttpClient { get; } = new HttpClient ();
    private static string DownloadDir { get; }

    private IUpdateSettings Settings { get; }

    IEnumerable<PackageInfoLocal> _packageInfos;
    PackageInfoLocal _defaultPackageInfo;

    static OnlineUpdate () {
      string downloads = Environment.ExpandEnvironmentVariables (@"%USERPROFILE%\Downloads");
      if (string.IsNullOrWhiteSpace (downloads))
        downloads = ApplEnv.TempDirectory;
      DownloadDir = downloads;
    }

    public OnlineUpdate(IUpdateSettings settings, string defaultAppName, string setupRefUrl, bool dbg) {
      Settings = settings;
      _dbg = dbg;
      _defaultAppName = defaultAppName;
      if (setupRefUrl is null)
        _setupRefUrl = string.Format (SETUP_REF_URL, defaultAppName);
      else
        _setupRefUrl = setupRefUrl;

      //string sRgxVers = "/" + defaultAppName + RGX_VERSION;
      //_rgxVersion = new Regex (sRgxVers, RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }

    public async Task UpdateAsync (
      IInteractionCallback<InteractMessage, bool?> interactCallback,
      Action finalCallback,
      Func<bool> busyCallback
  ) {
      if (Settings.OnlineUpdate == EOnlineUpdate.no)
        return;

      await getSetupRefAsync ();

      var pi = _defaultPackageInfo;
      if (pi is null) {
        Log (1, this, () => "no package info");
        return;
      }

      Log (3, this, () => $"server={pi.Version}");
    
      // do we have a new version?
      bool newVersion = pi.Version > ApplEnv.AssemblyVersion;
      if (_dbg)
        newVersion = true;
      if (!newVersion)
        return;
          
      // do we have it downloaded already?
      bool exists = await checkDownloadAsync (pi);
      Log (3, this, () => $"download exists={exists}");

      if (!exists) {
        if (Settings.OnlineUpdate == EOnlineUpdate.promptForDownload) {
          var interactmsg = 
            new InteractMessage (ECallbackType.question, null, new (EUpdateInteract.newVersAvail, pi));

          bool? result1 = interactCallback.Interact (interactmsg);
          if (!(result1.Value))
            return;
        }

        // yes: download,  verify md5
        await downloadSetupAsync (pi);
      }

      bool isBusy = busyCallback ();
      if (isBusy) {
        Log (3, this, () => "is already busy, cancel.");
        return;
      }

      bool cont = install (pi, interactCallback, EUpdateInteract.installNow);
      if (!cont) {
        Log (3, this, () => "do not install now, cancel.");
        return;
      }

      if (pi.DefaultApp)
        finalCallback?.Invoke ();
    }

    public async Task<bool> InstallAsync (
      IInteractionCallback<InteractMessage, bool?> interactCallback, 
      Action finalCallback
    ) {
      if (Settings.OnlineUpdate == EOnlineUpdate.no)
        return false;

      await getSetupRefAsync ();

      var pi = _defaultPackageInfo;
      if (pi is null) {
        Log (1, this, () => "no package info");
        return false;
      }

      // do we have a new version?
      bool newVersion = pi.Version > ApplEnv.AssemblyVersion;
      if (_dbg)
        newVersion = true;
      if (!newVersion)
        return false;

      // do we have it downloaded already?
      bool exists = await checkDownloadAsync (pi);
      if (!exists)
        return false;

      install (pi, interactCallback, EUpdateInteract.installLater);

      if (pi.DefaultApp)
        finalCallback?.Invoke ();

      return true;
    }

    private bool install (
      PackageInfoLocal pi, 
      IInteractionCallback<InteractMessage, bool?> interactCallback, 
      EUpdateInteract prompt
    ) {
      var interactmsg =
        new InteractMessage (ECallbackType.question, null, new (prompt, pi));
      bool? result2 = interactCallback.Interact (interactmsg);
      if (!(result2 ?? true))
        return false;

      // launch installer
      try {
        Log (3, this, () => "launch.");
        Process.Start (pi.SetupFile);
      } catch (Exception exc) {
        Log (1, this, () => $"{exc.Summary ()}");
      }
      return true;
    }

    private async Task getSetupRefAsync () {
      string result = null;

      try {
        using (HttpResponseMessage response = await HttpClient.GetAsync (_setupRefUrl)) {
          response.EnsureSuccessStatusCode ();
          using (HttpContent content = response.Content) {
            result = await content.ReadAsStringAsync ();
          }
        }
        if (string.IsNullOrWhiteSpace (result))
            return;

        var packageInfos = JsonSerialization.FromJsonString<PackageInfo[]> (result);

        _packageInfos = packageInfos.Select (pi => {
          var pil = new PackageInfoLocal (pi);

          string file = Path.GetFileName (pi.Url);
          string setupFile = Path.Combine (DownloadDir, file);
          pil = pil with { SetupFile = setupFile };

          return pil;
        }).
        ToList ();

        var defaultPackageInfo = _packageInfos
          .FirstOrDefault (pi => string.Equals (pi.AppName, _defaultAppName, StringComparison.InvariantCultureIgnoreCase));

        _defaultPackageInfo = defaultPackageInfo with { DefaultApp = true };

      } catch (Exception exc) {
        Log (1, this, () => $"{exc.Summary ()}{Environment.NewLine}  \"{result}\"");
      }
    }

    private async Task<bool> checkDownloadAsync (PackageInfoLocal pi) {
      if (!File.Exists (pi.SetupFile))
        return false;

      string md5 = await computeMd5ForFileAsync (pi.SetupFile);
      bool succ = string.Equals (pi.Md5, md5, StringComparison.InvariantCultureIgnoreCase);
      Log (3, this, () => $"succ={succ}, file={md5}, server={pi.Md5}");
      return succ;
    }

    private async Task<string> computeMd5ForFileAsync (string filePath) {
      return await Task.Run (() => computeMd5HashForFile (filePath));
    }

    private string computeMd5HashForFile (string filePath) {
      using (var md5 = MD5.Create ()) {
        using (var stream = File.OpenRead (filePath)) {
          var hash = md5.ComputeHash (stream);
          return BitConverter.ToString (hash).Replace ("-", "").ToLowerInvariant ();
        }
      }
    }


    private async Task<bool> downloadSetupAsync (PackageInfoLocal pi) {
      Log (3, this);
      await downloadAsync (pi.Url, pi.SetupFile);
      return await checkDownloadAsync (pi);
    }


    private async Task downloadAsync (string requestUri, string filename) {
      Log (3, this, $"\"{requestUri}\"");
      try {
        // for .Net framework
        //ServicePointManager.Expect100Continue = true;
        //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        using (var fileStream = File.OpenWrite (filename)) {
          using (var networkStream = await HttpClient.GetStreamAsync (requestUri)) {
            Log (3, this, $"copy to \"{filename.SubstitUser()}\"");
            await networkStream.CopyToAsync (fileStream);
            Log (3, this, "flush");
            await fileStream.FlushAsync ();
          }
        }
        Log (1, this, () => "complete");
      } catch (Exception exc) {
        Log (1, this, () => $"{exc.Summary ()}");
      }
    }

  }
}
