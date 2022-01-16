using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using core.audiamus.aux;
using core.audiamus.aux.ex;
using core.audiamus.common;
using core.audiamus.connect.ex;

namespace core.audiamus.connect {

  class Authorization : IAuthorization {
    public string AuthorizationCode { get; set; }
    public string CodeVerifier { get; set; }

    public static Authorization Create (Uri uri) {
      const string AUTH_KEY = "openid.oa2.authorization_code";

      if (!uri.IsAbsoluteUri)
        return null;

      var query = uri.Query;
      if (query is null)
        return null;

      NameValueCollection paras = HttpUtility.ParseQueryString (query);

      string auth = paras[AUTH_KEY];

      if (auth is null)
        return null;

      return new Authorization { AuthorizationCode = auth };
    }
  }
    
  class TokenBearer : ITokenBearer {

    public string RefreshToken { get; set; }
    public string AccessToken { get; set; }
    public DateTime Expiration { get; set; }

    public TokenBearer () { }
    public TokenBearer (string accToken, DateTime expiration) {
      AccessToken = accToken;
      Expiration = expiration;
    }

    public TokenBearer (string accToken, string refrToken, DateTime expiration) : 
      this (accToken, expiration)     
    {
      RefreshToken = refrToken;
    }
    
    public static TokenBearer Create (Uri uri) {
      const string TOKEN_KEY = "openid.oa2.access_token";
      const string EXPIR_KEY = "openid.pape.auth_time";


      if (!uri.IsAbsoluteUri) {
        return null;
      }

      var query = uri.Query;
      if (query is null)
        return null;

      NameValueCollection paras = HttpUtility.ParseQueryString (query);

      string token = paras[TOKEN_KEY];
      string expir = paras[EXPIR_KEY];

      if (token is null || expir is null)
        return null;

      DateTime.TryParse (expir, out DateTime expirTime);
      expirTime += TimeSpan.FromHours (1);
      expirTime = expirTime.ToUniversalTime ();

      var acctoken = new TokenBearer (token, expirTime);
      if (!Validate (acctoken))
        return null;

      return acctoken;

    }

    public static bool Validate (TokenBearer token) {
      const string ACC_TOKEN_STUB = "Atna|";
      const string REFR_TOKEN_STUB = "Atnr|";
      if (token is null)
        return false;

      if (token.AccessToken is null || !token.AccessToken.StartsWith (ACC_TOKEN_STUB))
        return false;
      
      if (token.RefreshToken is not null && !token.RefreshToken.StartsWith (REFR_TOKEN_STUB))
        return false;

      var utcnow = DateTime.UtcNow;
      return token.Expiration > utcnow;
    }

  }

  class DeviceInfo : IDeviceInfo {
    public string Type { get; set; }
    public string Name { get; set; }
    public string Serial { get; set; }
  }

  class CustomerInfo : ICustomerInfo {
    public string Name { get; set; }
    public string AccountId { get; set; }
  }

  class Profile : IProfile {
    public uint Id { get; set; }
    public ERegion Region { get; set; }
    public Authorization Authorization { get; set; }
    public TokenBearer Token { get; set; }
    public DeviceInfo DeviceInfo { get; set; }
    public CustomerInfo CustomerInfo { get; set; }
    public IEnumerable<KeyValuePair<string, string>> Cookies { get; set; }

    public string PrivateKey { get; set; }
    public string AdpToken { get; set; }
    public string StoreAuthentCookie { get; set; }

    IAuthorization IProfile.Authorization => Authorization;
    ITokenBearer IProfile.Token => Token;
    IDeviceInfo IProfile.DeviceInfo => DeviceInfo;
    ICustomerInfo IProfile.CustomerInfo => CustomerInfo;

    public Profile () { }

    public Profile (ERegion region, TokenBearer token, IEnumerable<KeyValuePair<string, string>> cookies, string serial) {
      Region = region;
      Token = token;
      Cookies = cookies;
      DeviceInfo = new DeviceInfo {
        Serial = serial
      };
    }

    public Profile (ERegion region, Authorization authorization, string serial) {
      Region = region;
      Authorization = authorization;
      DeviceInfo = new DeviceInfo {
        Serial = serial
      };
    }

    public void Update (
      TokenBearer token,
      IEnumerable<KeyValuePair<string, string>> cookies,
      DeviceInfo device,
      CustomerInfo customer,
      string privateKey,
      string adpToken,
      string storeAuthentCookie
    ) {
      Token = token;
      Cookies = cookies;
      DeviceInfo = device;
      CustomerInfo = customer;
      PrivateKey = privateKey;
      AdpToken = adpToken;
      StoreAuthentCookie = storeAuthentCookie;

      // TODO validate inputs
    }
        
    public void Refresh (TokenBearer token) {
      Token.AccessToken = token.AccessToken;
      Token.Expiration = token.Expiration;
    }
  }

  class Configuration {
    private static readonly string CONFIG_DIR = Path.Combine (ApplEnv.LocalApplDirectory, "config");

    class SerializableConfig { 
      public List<Profile> Profiles { get; set; }
      public string Secure { get; set; }
    }

    private List<Profile> _profiles;
    public IReadOnlyList<Profile> Profiles => _profiles;

    public bool IsEncrypted { get; private set; }

    public IProfile AddOrReplace (Profile profile) {
      if (_profiles is null)
        _profiles = new List<Profile> ();

      // uniqueness constraint is customer account id and region
      // this may create zombies unless old profile device is deregistered

      uint nextId = 0;
      if (_profiles.Any())
        nextId = _profiles.Select (p => p.Id).Max () + 1;
      profile.Id = nextId;

      var existing =
        Profiles.FirstOrDefault (d => d.Matches (profile));

      if (existing is null) {
        _profiles.Add (profile);
        return null;
      }


      int i = _profiles.IndexOf (existing);
      IProfile prevProfile = _profiles[i];
      _profiles[i] = profile;
      return prevProfile;

    }

    public IProfile Remove (IProfileKey key) {
      if (Profiles is null)
        return null;

      var existing =
        Profiles.FirstOrDefault (d => d.Matches (key));

      if (existing is null)
        return null;

      bool succ = _profiles.Remove (existing);

      return existing;
    }

    public Profile Get (IProfileKey key) => 
      Profiles?.FirstOrDefault (p => p.Matches (key));

    public IEnumerable<Profile> GetSorted () {
      // by customer and region
      return Profiles?.OrderBy (p => p.CustomerInfo.AccountId).ThenBy (p => p.Region).ToList ();
    }

    public async Task ReadAsync (string token) {
      var config = await FileExtensions.ReadJsonFileAsync<SerializableConfig> (CONFIG_DIR, this.GetType().Name);
      if (config is null)
        return;
      bool encrypted = !token.IsNullOrWhiteSpace ();
      Logging.Log (3, this, () => $"{(encrypted ? "decrypt" : string.Empty)}");
      if (!token.IsNullOrWhiteSpace ())
        decrypt (config, token);
      IsEncrypted = config.Profiles is null && !config.Secure.IsNullOrWhiteSpace ();
      _profiles = config.Profiles;
    }

    public async Task WriteAsync (string token) {
      var config = new SerializableConfig { Profiles = _profiles };
      bool encrypted = !token.IsNullOrWhiteSpace ();
      Logging.Log (3, this, () => $"{(encrypted ? "encrypt" : string.Empty)}");
      if (encrypted)
        encrypt (config, token);

      Directory.CreateDirectory (CONFIG_DIR);
      await config.WriteJsonFileAsync (CONFIG_DIR, this.GetType().Name);
    }

    private static void encrypt (SerializableConfig configuration, string token) {
      if (configuration.Profiles is null)
        return;

      string json = JsonSerializer.Serialize (configuration.Profiles);
      byte[] encrypted = SymmetricEncryptor.EncryptString (json, token);
      string base64 = Convert.ToBase64String (encrypted);
      configuration.Secure = base64;
      configuration.Profiles = null;
    }
  
    private static void decrypt (SerializableConfig configuration, string token) {
      if (configuration.Secure is null || configuration.Secure.IsNullOrWhiteSpace())
        return;
      try {
        byte[] encrypted = Convert.FromBase64String (configuration.Secure);
        string json = SymmetricEncryptor.DecryptToString (encrypted, token);
        var profiles = JsonSerializer.Deserialize<List<Profile>> (json);
        configuration.Profiles = profiles;
        configuration.Secure = null;
      } catch (Exception) { }
    }
  }
}
