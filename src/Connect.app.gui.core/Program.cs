using System;
using System.Globalization;
using System.Windows.Forms;
using core.audiamus.aux;

namespace core.audiamus.connect.app.gui {
  static class Program {
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main (string[] args) {
      Application.SetHighDpiMode (HighDpiMode.PerMonitorV2);
      Application.EnableVisualStyles ();
      Application.SetCompatibleTextRenderingDefault (false);

      CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

      var appSettings = SettingsManager.GetAppSettings<AppSettings> ();
      if (appSettings?.LogLevel is not null)
        Logging.Level = (int)appSettings.LogLevel.Value;
      else {
        ArgParser argParser = new ArgParser (args);
        uint? level = argParser.FindUIntArg ("Log");
        if (level.HasValue)
          Logging.Level = (int)level.Value;
        else
          Logging.Level = 3;
      }
      Logging.InstantFlush = true;

      Application.Run (new MainForm ());
    }

  }
}
