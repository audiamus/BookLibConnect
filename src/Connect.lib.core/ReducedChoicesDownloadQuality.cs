
using core.audiamus.booksdb;

namespace core.audiamus.connect {
  public enum EDownloadQualityReducedChoices {
    Normal,
    High,
  }

  public static class DownloadQualityExtensions {
    public static EDownloadQuality ToFullChoices (this EDownloadQualityReducedChoices value) {
      return value switch {
        EDownloadQualityReducedChoices.High => EDownloadQuality.High,
        _ => EDownloadQuality.Normal
      };
    }
    public static EDownloadQualityReducedChoices ToReducedChoices (this EDownloadQuality value) {
      return value switch {
        EDownloadQuality.Extreme => EDownloadQualityReducedChoices.High,
        EDownloadQuality.High => EDownloadQualityReducedChoices.High,
        _ => EDownloadQualityReducedChoices.Normal
      };
    }

    public static EDownloadQuality ReduceChoices (this EDownloadQuality value) =>
      value.ToReducedChoices ().ToFullChoices ();
  }
}
