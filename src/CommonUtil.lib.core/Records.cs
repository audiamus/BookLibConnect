using System;

namespace core.audiamus.util {

  public record ProgressMessage (
    int? ItemCount,
    int? IncItem,
    int? IncStepsPerCent,
    int? IncStepsPerMille
  );

  public interface IPackageInfo {
    string AppName { get; }
    Version Version { get; }
    bool Preview { get; }
    bool DefaultApp { get; }
    string Desc { get; }
  }

  public record UpdateInteractionMessage (EUpdateInteract Kind, IPackageInfo pckInfo);

  public record PackageInfo {
    public string Url { get; init; }
    public string AppName { get; init; }
    public string Version { get; init; }
    public bool Preview { get; init; }
    public string Desc { get; init; }
    public string Md5 { get; init; }
  }

  public record PackageInfoLocal : PackageInfo, IPackageInfo {
    public new Version Version { get; init; }
    public string SetupFile { get; init; }
    public bool DefaultApp { get; init; }

    public PackageInfoLocal () { }
    public PackageInfoLocal (PackageInfo pi) {
      Url = pi.Url;
      AppName = pi.AppName;
      Version = tryParse (pi.Version); 
      Preview = pi.Preview;
      Desc = pi.Desc;
      Md5 = pi.Md5;

      static Version tryParse (string s) {
        bool succ = Version.TryParse (s, out Version version);
        return succ ? version : null;
      }
    }
  }

      
}


