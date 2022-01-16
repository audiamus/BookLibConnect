using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using core.audiamus.aux.ex;
using core.audiamus.common;
using core.audiamus.connect.ex;
using static core.audiamus.aux.Logging;

namespace core.audiamus.connect {
  class AudibleLogin {

    public const string DEVICE_TYPE = "A2CZJZGLK2JJVM";

    public ERegion Region { get; private set; }
    public bool WithPreAmazonUsername { get; private set; }
    public string CodeVerifierB64 { get; private set; }
    public string CodeChallengeB64 { get; private set; }
    public string Serial { get; private set; }
    public string ClientId { get; private set; }

    public Uri BuildAuthUri (
      ERegion region,
      bool withPreAmazonUsername
    ) {
      Log (3, this, () => $"reg={region}, preAmznAccnt={withPreAmazonUsername}");

      Region = region;
      var locale = region.FromCountryCode ();
      WithPreAmazonUsername = withPreAmazonUsername;

      if (withPreAmazonUsername && !(new[] { ERegion.de, ERegion.uk, ERegion.us }.Contains (locale.CountryCode)))
        throw new ArgumentException ("Login with username is only supported for DE, US and UK marketplaces!");

      Serial = buildDeviceSerial ();
      ClientId = buildClientId (Serial);

      CodeVerifierB64 = createCodeVerifier ();
      CodeChallengeB64 = createSHA256CodeChallenge (CodeVerifierB64);

      string base_url, return_to, assoc_handle, page_id;
      if (withPreAmazonUsername) {
        base_url = $"https://www.audible.{locale.Domain}/ap/signin";
        return_to = $"https://www.audible.{locale.Domain}/ap/maplanding";
        assoc_handle = $"amzn_audible_ios_lap_{locale.CountryCode}";
        page_id = "amzn_audible_ios_privatepool";
      } else {
        base_url = $"https://www.amazon.{locale.Domain}/ap/signin";
        return_to = $"https://www.amazon.{locale.Domain}/ap/maplanding";
        assoc_handle = $"amzn_audible_ios_{locale.CountryCode}";
        page_id = "amzn_audible_ios";
      }

      var oauthParams = new List<KeyValuePair<string, string>> () {
        new ("openid.oa2.response_type", "code"),
        new ("openid.oa2.code_challenge_method", "S256"),
        new ("openid.oa2.code_challenge", CodeChallengeB64),
        new ("openid.return_to", return_to),
        new ("openid.assoc_handle", assoc_handle),
        new ("openid.identity", "http://specs.openid.net/auth/2.0/identifier_select"),
        new ("pageId", page_id),
        new ("accountStatusPolicy", "P1"),
        new ("openid.claimed_id", "http://specs.openid.net/auth/2.0/identifier_select"),
        new ("openid.mode", "checkid_setup"),
        new ("openid.ns.oa2", "http://www.amazon.com/ap/ext/oauth/2"),
        new ("openid.oa2.client_id", $"device:{ClientId}"),
        new ("openid.ns.pape", "http://specs.openid.net/extensions/pape/1.0"),
        new ("marketPlaceId", locale.MarketPlaceId),
        new ("openid.oa2.scope", "device_auth_access"),
        new ("forceMobileLayout", "true"),
        new ("openid.ns", "http://specs.openid.net/auth/2.0"),
        new ("openid.pape.max_auth_age", "0")
      };

      return new Uri ($"{base_url}?{oauthParams.ToQueryString ()}");
    }

    public Profile ParseExternalResponse (Uri uri) {
      var authorization = Authorization.Create (uri);
      if (authorization is null)
        return null;

      authorization.CodeVerifier = CodeVerifierB64;

      return new Profile (Region, authorization, Serial);
    }


    // internal instead of private for testing only
    internal static string buildDeviceSerial () {
      var guid = Guid.NewGuid ().ToString ("N").ToUpper ();
      Log (3, typeof (AudibleLogin), () => guid);
      return guid;
    }

    // internal instead of private for testing only
    internal static string buildClientId (string serial) {
      string serialEx = $"{serial}#{DEVICE_TYPE}";
      byte[] clientId = Encoding.ASCII.GetBytes (serialEx);
      string clientIdHex = clientId.BytesToHexString ();
      Log (3, typeof (AudibleLogin), () => clientIdHex);
      return clientIdHex;
    }


    // internal instead of private for testing only
    internal static string createCodeVerifier () {
      var random = new Random ();
      byte[] tokenBytes = new byte[32];
      random.NextBytes (tokenBytes);
      string codeVerifier = tokenBytes.ToUrlBase64String();
      return codeVerifier;
    }

    // internal instead of private for testing only
    internal static string createSHA256CodeChallenge (string codeVerifier) {
      var sha256 = SHA256.Create ();
      var tokenBytes = codeVerifier.GetBytes ();
      var hash = sha256.ComputeHash (tokenBytes);
      return hash.ToUrlBase64String();
    }



  }
}
