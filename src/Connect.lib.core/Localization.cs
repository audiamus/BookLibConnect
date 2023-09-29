using System.Collections.Generic;
using core.audiamus.common;

namespace core.audiamus.connect {

  interface ILocale {
    ERegion CountryCode { get; }
    string Domain { get; }
    string MarketPlaceId { get; }
  }

  record LocaleTemplate (ERegion CountryCode, string Domain, string MarketPlaceId) : ILocale { }

  static class Locale  {
    static readonly Dictionary<ERegion, LocaleTemplate> LocaleTemplates = new () {
      { ERegion.de, new (ERegion.de, "de", "AN7V1F1VY261K") },
      { ERegion.us, new (ERegion.us, "com", "AF2M0KC94RCEA") },
      { ERegion.uk, new (ERegion.uk, "co.uk", "A2I9A3Q2GNFNGQ") },
      { ERegion.fr, new (ERegion.fr, "fr", "A2728XDNODOQ8T") },
      { ERegion.ca, new (ERegion.ca, "ca", "A2CQZ5RBY40XE") },
      { ERegion.it, new (ERegion.it, "it", "A2N7FU2W2BU2ZC") },
      { ERegion.au, new (ERegion.au, "com.au", "AN7EY7DTAW63G") },
      { ERegion.@in, new (ERegion.@in, "in", "AJO3FBRUE6J4S") },
      { ERegion.jp, new (ERegion.jp, "co.jp", "A1QAP3MOU4173J") },
      { ERegion.es, new (ERegion.es, "es", "ALMIKO4SZCSAR") },
      { ERegion.br, new (ERegion.br, "com.br", "A10J1VAYUDTYRN") },
    };

    public static ILocale FromCountryCode (this ERegion countryCode) {
      bool succ = LocaleTemplates.TryGetValue(countryCode, out var locale);
      if (!succ)
        return null;
      return locale;
    } 
  }
}
