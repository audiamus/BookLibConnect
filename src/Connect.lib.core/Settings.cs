using System;
using core.audiamus.aux.diagn;
using core.audiamus.util;

namespace core.audiamus.connect {
  public interface IConfigSettings {
    bool EncryptConfiguration { get; }
  }

  public interface IMultiPartSettings {
    bool MultiPartDownload { get; }
  }

  public interface IAuthorizeSettings {
    bool AutoRefresh { get; }
  }

  public interface IDownloadSettings : IMultiPartSettings, IAuthorizeSettings {
    event EventHandler ChangedSettings; 

    bool AutoUpdateLibrary { get; }
    bool AutoOpenDownloadDialog { get; }
    bool IncludeAdultProducts { get; }
    bool KeepEncryptedFiles { get; }
    string DownloadDirectory { get; }
    EInitialSorting InitialSorting { get; }
  }

  public interface IExportSettings {
    bool? ExportToAax { get; }
    [ToString (typeof (ToStringConverterPath))]
    string ExportDirectory { get; }
  }

  public abstract class SettingsBase {
    public event EventHandler ChangedSettings;
    public void OnChange () => ChangedSettings?.Invoke (this, EventArgs.Empty);
  }

  public class ConfigSettings : SettingsBase, IConfigSettings {
    private bool _encryptConfiguration = true;

    public bool EncryptConfiguration {
      get => _encryptConfiguration;
      set {
        _encryptConfiguration = value;
        OnChange ();
      } 
    }
  }

  public class DownloadSettings : SettingsBase, IDownloadSettings {

    public bool AutoRefresh { get; set; } = true;
    public bool AutoUpdateLibrary { get; set; } = true;
    public bool AutoOpenDownloadDialog { get; set; }
    public bool IncludeAdultProducts { get; set; }
    public bool MultiPartDownload { get; set; }
    public bool KeepEncryptedFiles { get; set; }
    public string DownloadDirectory { get; set; }
    public EInitialSorting InitialSorting { get; set; }
    public ProfileAliasKey Profile { get; set; }

  }

  public class ExportSettings : SettingsBase, IExportSettings {
    private bool? _exportToAax;
    private string _exportDirectory;

    public bool? ExportToAax {
      get => _exportToAax;
      set {
        _exportToAax = value;
        OnChange ();
      }
    }

    public string ExportDirectory {
      get => _exportDirectory;
      set {
        _exportDirectory = value;
        OnChange ();
      }
    }
  }
}
