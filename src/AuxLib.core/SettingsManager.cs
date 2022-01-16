using System;
using System.Collections.Generic;
using System.IO;

namespace core.audiamus.aux {
  /// <summary>
  /// Simple settings manager for app and user settings serialized as json.
  /// Does not use Microsoft.Extensions.Configuration
  /// </summary>
  public static class SettingsManager {

    class UserConfig {
      public object Settings { get; set; }
      public string File { get; set; }
    }

    private static Dictionary<Type, UserConfig> __userSettingsDict = new  ();
    private static object __appSettings;


    private const string JSON = ".json";

    /// <summary>
    /// The application settings file
    /// </summary>
    public const string APP_SETTINGS_FILE = "appsettings" + JSON;

    /// <summary>
    /// The user settings file
    /// </summary>
    public const string USER_SETTINGS_FILE = "usersettings" + JSON;

    public const string SETTINGS_TEMPLATE_FILE_SUFFIX = ".template" + JSON;

    /// <summary>
    /// Gets the application settings directory.
    /// </summary>
    public static string AppSettingsDirectory => ApplEnv.ApplDirectory;

    /// <summary>
    /// Gets the user settings directory.
    /// </summary>
    public static string UserSettingsDirectory => ApplEnv.SettingsDirectory;


    /// <summary>
    /// Gets the type-safe application settings.
    /// </summary>
    /// <typeparam name="T">Type of the application settings</typeparam>
    /// <param name="optional">Whether app settings file must exist.</param>
    /// <returns>Application settings, or new default instance if optional and not found.</returns>
    public static T GetAppSettings<T> (bool optional = false)
      where T: class, new()
    {
      T settings = __appSettings as T;

      if (settings is null) {
        string path = Path.Combine (AppSettingsDirectory, APP_SETTINGS_FILE);
        bool exists = File.Exists (path);
        if (!optional && !exists)
          throw new InvalidOperationException ($"{path} not found.");
        settings = deserializeJsonFile<T> (path, !optional);
        if (settings is null) {
          //if (!optional)
          //  throw new InvalidOperationException ($"{path}: content does not match.");
          //else
            settings = new T ();

          if (settings is IInitSettings init)
            init.Init ();

        }
        __appSettings = settings;
      }

      return settings;
    }


    /// <summary>
    /// Gets the type-safe user settings for one type. Tries for a preset in the application directory,
    /// if settings can not be found at the designated user settings directory.
    /// </summary>
    /// <typeparam name="T">Type of the user settings.</typeparam>
    /// <param name="renew">Always reads from file and updates existing instance if set to <c>true</c>.</param>
    /// <param name="settingsFile">The settings file. Required for each additional type
    /// if more than one type will be used. Can be file name only without directory.
    /// .json will be added if ncessary.</param>
    /// <returns>
    /// User settings, or new default instance if no settings found.
    /// </returns>
    public static T GetUserSettings<T> (bool renew = false, string settingsFile = null)
    where T : class, IUserSettings, new() =>
      GetUserSettings<T> (settingsFile, renew);


    /// <summary>
    /// Gets the type-safe user settings for one type. Tries for a preset in the application directory,
    /// if settings can not be found at the designated user settings directory.
    /// </summary>
    /// <typeparam name="T">Type of the user settings.</typeparam>
    /// <param name="settingsFile">The settings file. Required for each additional type 
    /// if more than one type will be used. Can be file name only without directory. 
    /// .json will be added if ncessary.</param>
    /// <param name="renew">Always reads from file and updates existing instance if set to <c>true</c>.</param>
    /// <returns>
    /// User settings, or new default instance if if no settings found.
    /// </returns>
    public static T GetUserSettings<T> (string settingsFile, bool renew = false)
      where T : class, IUserSettings, new() {

      T settings = null;

      if (!renew) {
        lock (__userSettingsDict) {
          bool succ = __userSettingsDict.TryGetValue (typeof (T), out var userConfig);
          if (succ)
            settings = userConfig.Settings as T;
        }
      }

      if (settings is null) {
        (string dir, string file) = getUserSettingsPath (settingsFile);
        
        string path = Path.Combine (dir, file);
        settings = deserializeJsonFile<T> (path);
        
        if (settings is null) {
          path = Path.Combine (AppSettingsDirectory, file);
          settings = deserializeJsonFile<T> (path);
        }

        if (settings is null)
          settings = new T ();

        lock (__userSettingsDict) {
          bool succ = __userSettingsDict.TryGetValue (typeof (T), out var userConfig);
          if (succ && userConfig.Settings != settings)
            userConfig.Settings = settings;
          else {
            userConfig = new UserConfig {
              Settings = settings,
              File = settingsFile
            };
            __userSettingsDict[typeof (T)] = userConfig;
          }
        }

        if (settings is IInitSettings init)
          init.Init ();
      }

      return settings;

    }


    /// <summary>
    /// Saves the specified user settings to the designated user settings directory.
    /// Writes to type-specific file, specified when reading the settings.
    /// </summary>
    /// <typeparam name="T">Type of the user settings</typeparam>
    /// <param name="settings">The user settings.</param>
    public static bool Save<T> (this T settings)
      where T : IUserSettings 
    {
      
      string settingsFile;

      // use actual arg type, not the generic type which may be an interface.
      Type type = settings.GetType ();

      lock (__userSettingsDict) {
        bool succ = __userSettingsDict.TryGetValue (type, out var userConfig);
        if (!succ)
          return false;
        if (!ReferenceEquals (userConfig.Settings,settings))
          userConfig.Settings = settings;
        settingsFile = userConfig.File;
      }

      (string dir, string file) = getUserSettingsPath (settingsFile);
      Directory.CreateDirectory (dir);
      var filename = Path.Combine (dir, file);
      try {
        settings.ToJsonFile (filename);
        return true;
      } catch (IOException) {
        return false;
      }
    }

    /// <summary>
    /// <para>Writes the specified settings to a JSON file, as a template.</para>
    /// <para>Will write to specified path if filename is not <c>null</c>. 
    /// Otherwise will use type name of specified settings object as filename and
    /// add .template.json to it. Will try AppSettingsDirectory first. If this fails 
    /// (write protected), will use UserSettingsDirectory.</para>
    /// </summary>
    /// <typeparam name="T">Type of settings</typeparam>
    /// <param name="settings">The settings object to serialize</param>
    /// <param name="filename">Optional custom filename.</param>
    /// <returns><c>true</c> on success.</returns>
    public static bool SaveAsTemplate<T> (this T settings, string filename = null) {
      if (!string.IsNullOrWhiteSpace (filename)) {
        try {
          var dir = Path.GetDirectoryName (filename);
          if (!string.IsNullOrWhiteSpace (dir))
            Directory.CreateDirectory (dir);
          settings.ToJsonFile (filename);
          return true;
        } catch (IOException) {
          return false;
        }
      } else {
        string name = settings.GetType ().Name;
        filename = name + SETTINGS_TEMPLATE_FILE_SUFFIX;
        string path = Path.Combine (AppSettingsDirectory, filename);
        try {
          settings.ToJsonFile (path);
          return true;
        } catch (IOException) {
          Directory.CreateDirectory (UserSettingsDirectory);
          path = Path.Combine (UserSettingsDirectory, filename);
        }
        try {
          settings.ToJsonFile (path);
          return true;
        } catch (IOException) {
        }
      }
      return false;
    }



    private static (string dir, string path) getUserSettingsPath (string settingsFile) {
      if (string.IsNullOrWhiteSpace (settingsFile))
        return (UserSettingsDirectory, USER_SETTINGS_FILE);
      string dir = Path.GetDirectoryName (settingsFile);
      if (string.IsNullOrWhiteSpace (dir))
        dir = UserSettingsDirectory;

      string file = Path.GetFileName (settingsFile);
      if (!string.IsNullOrWhiteSpace (file)) {
        string ext = Path.GetExtension (file).ToLower ();
        if (ext != JSON)
          file += JSON;
      }
      return (dir, file);
    }

    private static T deserializeJsonFile<T> (string path, bool doThrow = false) where T : class, new() {
      try {
        return JsonSerialization.FromJsonFile<T> (path);
      } catch (Exception exc) {
        if (doThrow)
          throw new InvalidOperationException (path, exc); 
        return null;
      }
    }

  }
}
