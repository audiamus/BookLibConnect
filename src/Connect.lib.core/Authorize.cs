using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using core.audiamus.aux;
using core.audiamus.aux.ex;
using core.audiamus.connect.ex;
using static core.audiamus.aux.Logging;

namespace core.audiamus.connect {

  class Authorize {
    const string HTTP_AUTHORITY = @"https://api.amazon.";
    const string HTTP_PATH_REGISTER = @"/auth/register";
    const string HTTP_PATH_DEREGISTER = @"/auth/deregister";
    const string HTTP_PATH_TOKEN = @"/auth/token";

    const string IOS_VERSION = "15.0.0";
    const string APP_VERSION = "3.56.2";
    const string SOFTWARE_VERSION = "35602678";
    const string APP_NAME = "Audible";

    private IAuthorizeSettings Settings { get; }

    private Uri BaseUri => HttpClientAmazon?.BaseAddress;

    private Configuration Configuration { get; set; }

    private ConfigTokenDelegate GetTokenFunc { get; }

    public HttpClientEx HttpClientAmazon { get; private set; }

    public Action WeakConfigEncryptionCallback { private get; set; }

    public Authorize (ConfigTokenDelegate getTokenFunc, IAuthorizeSettings settings) {
      Log (3, this);
      GetTokenFunc = getTokenFunc;
      Settings = settings;
    }

    internal IProfile GetProfile (IProfileKey key) => Configuration?.Get (key);

    public async Task<(bool, IProfile)> RegisterAsync (Profile profile) {
      using var _ = new LogGuard (3, this);

      ensureHttpClient (profile);

      try {
        var request = buildRegisterRequest (profile);

        await request.LogAsync (4, this, HttpClientAmazon.DefaultRequestHeaders, HttpClientAmazon.CookieContainer, BaseUri);

        var response = await HttpClientAmazon.SendAsync (request);
        await response.LogAsync (4, this, HttpClientAmazon.CookieContainer, BaseUri);

        response.EnsureSuccessStatusCode ();
        string content = await response.Content.ReadAsStringAsync ();

        return await addProfileToConfig (profile, content);
      } catch (Exception exc) {
        Log (1, this, () => exc.Summary ());
        return (false, null);
      }
    }

    // internal instead of private for testing only
    internal async Task<(bool, IProfile)> addProfileToConfig (Profile profile, string content) {
      bool succ = updateProfile (profile, content);
      if (!succ)
        return (false, null);

      await readConfigurationAsync ();

      IProfile prevProfile = Configuration.AddOrReplace (profile);

      await WriteConfigurationAsync ();

      return (true, prevProfile);
    }

    public async Task<bool> DeregisterAsync (IProfile profile) {
      ensureHttpClient (profile);
      try {
        await refreshTokenAsync (profile);

        var request = buildDeregisterRequest (profile);

        await request.LogAsync (4, this, HttpClientAmazon.DefaultRequestHeaders, HttpClientAmazon.CookieContainer, BaseUri);

        var response = await HttpClientAmazon.SendAsync (request);
        await response.LogAsync (4, this, HttpClientAmazon.CookieContainer, BaseUri);

        response.EnsureSuccessStatusCode ();
        string content = await response.Content.ReadAsStringAsync ();

        return true;
      } catch (Exception exc) {
        Log (1, this, () => exc.Summary ());
        return false;
      }
    }

    public async Task<EAuthorizeResult> RemoveProfileAsync (IProfileKey key) {
      Log (3, this, () => key.ToString());

      IProfile profile = Configuration.Remove (key);
      if (profile is null)
        return EAuthorizeResult.removeProfileFailed;
      await WriteConfigurationAsync ();

      // TODO modify/test
      //bool succ = await DeregisterAsync (profile);
      //EAuthorizeResult result = succ ? EAuthorizeResult.succ : EAuthorizeResult.deregistrationFailed;
      //return result;
      return EAuthorizeResult.succ;
    }

    internal async Task WriteConfigurationAsync () {
      Log (3, this);

      if (Configuration is null)
        return;
      (string token, bool weak) = GetTokenFunc?.Invoke ();

      bool existed = Configuration.Existed;
      await Configuration.WriteAsync (token);

      if (!existed && weak)
        WeakConfigEncryptionCallback?.Invoke ();

    }

    private void ensureHttpClient (IProfile profile) {
      string domain = Locale.FromCountryCode (profile.Region).Domain; 
      Uri baseUri = new Uri (HTTP_AUTHORITY + domain);

      if (HttpClientAmazon is not null) { 
        if (BaseUri == baseUri)
          return;
        else
          HttpClientAmazon.Dispose ();
      }
        
      HttpClientAmazon = HttpClientEx.Create (baseUri);
    }

    public async Task RefreshTokenAsync (IProfile profile) =>
      await RefreshTokenAsync (profile, false);

    internal async Task RefreshTokenAsync (IProfile profile, bool late) {
      using var _ = new LogGuard (3, this, () => $"auto={Settings?.AutoRefresh}, late={late}");
      ensureHttpClient (profile);

      await readConfigurationAsync ();

      if ((Settings?.AutoRefresh ?? true) != late) {
        if (profile is Profile prof1 && (Configuration.Profiles?.Contains (prof1) ?? false))
          await refreshTokenAsync (prof1);
        else {
          Profile prof2 = Configuration.Profiles?.FirstOrDefault (d => d.Matches (profile));
          if (prof2 is not null)
            await refreshTokenAsync (prof2);
        }
        await WriteConfigurationAsync ();
      }
    }

    private async Task refreshTokenAsync (IProfile profile) {
      if (profile is null)
        return;

      using var _ = new LogGuard (3, this);

      try {
        if (DateTime.UtcNow < profile.Token.Expiration - TimeSpan.FromMinutes (5))
          return;

        Log (3, this, () => "from server");

        HttpRequestMessage request = buildRefreshRequest (profile);

        await request.LogAsync (4, this, HttpClientAmazon.DefaultRequestHeaders, HttpClientAmazon.CookieContainer, BaseUri);

        var response = await HttpClientAmazon.SendAsync (request);
        await response.LogAsync (4, this, HttpClientAmazon.CookieContainer, BaseUri);

        response.EnsureSuccessStatusCode ();
        string content = await response.Content.ReadAsStringAsync ();

        refreshToken (profile, content);
      } catch (Exception exc) {
        Log (1, this, () => exc.Summary ());
      }
    }

    private HttpRequestMessage buildRefreshRequest (IProfile profile) {

      var content = new Dictionary<string, string> {
        ["app_name"] = APP_NAME,
        ["app_version"] = APP_VERSION,
        ["source_token"] = profile.Token.RefreshToken,
        ["requested_token_type"] = "access_token",
        ["source_token_type"] = "refresh_token"
      };

      Uri uri = new Uri (HTTP_PATH_TOKEN, UriKind.Relative);

      var request = new HttpRequestMessage {
        Method = HttpMethod.Post,
        RequestUri = uri,
        Content = new FormUrlEncodedContent (content)
      };
      request.Headers.Add ("x-amzn-identity-auth-domain", BaseUri.Host);
      request.Headers.Add ("Accept", "application/json");
      return request;
    }

    private void refreshToken (IProfile profile, string json) {
      try {
        var jsonDoc = JsonDocument.Parse (json);
        var elRoot = jsonDoc.RootElement;

        string accessToken = elRoot.GetProperty ("access_token").GetString ();
        int expires = elRoot.GetProperty ("expires_in").GetInt32();
        DateTime expiration = DateTime.UtcNow.AddSeconds (expires);

        var bearer = new TokenBearer (
          elRoot.GetProperty ("access_token").GetString (),
          DateTime.UtcNow.AddSeconds (expires)
        );

        profile.Refresh (bearer);

      } catch (Exception exc) {
        Log (1, this, () => exc.Summary ());
      }

    }

    public async Task<IEnumerable<IProfile>> GetRegisteredProfilesAsync () {
      if (Configuration is null)
        await readConfigurationAsync ();
      return Configuration.GetSorted ();
    }

    private async Task readConfigurationAsync () {
      using var _ = new LogGuard (3, this);

      if (Configuration is not null)
        return;
      Configuration = new Configuration ();
      await readConfigAsync (false);
      if (Configuration.IsEncrypted)
        await readConfigAsync (true);

      async Task readConfigAsync (bool enforce) {
        var cfgToken = GetTokenFunc?.Invoke (enforce);
        await Configuration.ReadAsync (cfgToken.Token);
      }
    }

    private HttpRequestMessage buildRegisterRequest (IProfile profile) {
      ILocale locale = profile.Region.FromCountryCode ();
      Uri uri = new Uri (HTTP_PATH_REGISTER, UriKind.Relative);
      string jsonBody = buildRegisterBody (profile, locale);
      HttpContent content = new StringContent (jsonBody, Encoding.UTF8, "application/json");
      var request = new HttpRequestMessage {
        Method = HttpMethod.Post,
        RequestUri = uri,
        Content = content
      };
      request.Headers.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));

      return request;
    }

    private HttpRequestMessage buildDeregisterRequest (IProfile profile) {
      Uri uri = new Uri (HTTP_PATH_DEREGISTER, UriKind.Relative);

      var content = new Dictionary<string, string> {
        ["deregister_all_existing_accounts"] = "false",
      };

      var request = new HttpRequestMessage {
        Method = HttpMethod.Post,
        RequestUri = uri,
        Content = new FormUrlEncodedContent (content)
      };
      request.Headers.Add ("Authorization", $"Bearer {profile.Token.AccessToken}");
      request.Headers.Add ("Accept", "application/json");

      return request;
    }

    private string buildRegisterBody (IProfile profile, ILocale locale) {
      string json = $@"{{ 
        ""requested_token_type"":
            [""bearer"", ""mac_dms"", ""website_cookies"",
             ""store_authentication_cookie""],
        ""cookies"": {{
          ""website_cookies"": [],
          ""domain"": "".amazon.{locale.Domain}""
        }},
        ""registration_data"": {{
          ""domain"": ""Device"",
          ""app_version"": ""{APP_VERSION}"",
          ""device_serial"": ""{profile.DeviceInfo.Serial}"",
          ""device_type"": ""{AudibleLogin.DEVICE_TYPE}"",
          ""device_name"": 
              ""%FIRST_NAME%%FIRST_NAME_POSSESSIVE_STRING%%DUPE_STRATEGY_1ST%Audible for iPhone"",
          ""os_version"": ""{IOS_VERSION}"",
          ""software_version"": ""{SOFTWARE_VERSION}"",
          ""device_model"": ""iPhone"",
          ""app_name"": ""{APP_NAME}""
          }},
        ""auth_data"": {{
          ""client_id"": ""{AudibleLogin.buildClientId(profile.DeviceInfo.Serial)}"",
          ""authorization_code"": ""{profile.Authorization.AuthorizationCode}"",
          ""code_verifier"": ""{profile.Authorization.CodeVerifier}"",
          ""code_algorithm"": ""SHA-256"",
          ""client_domain"": ""DeviceLegacy""
        }},
        ""requested_extensions"": [""device_info"", ""customer_info""]
      }}";

      if (Logging.Level >= 4) {
        string file = json.WriteTempTextFile ();
        Log (4, this, () => $"buildRegisterBody: {file}");
      } 

      json = json.CompactJson ();

      if (!json.ValidateJson ())
        throw new InvalidOperationException ("invalid json");

      return json;
    }


    // internal instead of private for testing only
    internal bool updateProfile (Profile profile, string json) {
      try {
        var root = adb.json.RegistrationResponse.Deserialize (json);

        var response = root.response;
        var success = response.success;
        var extensions = success.extensions;
        var device_info = extensions.device_info;

        var deviceInfo = new DeviceInfo {
          Name = device_info.device_name,
          Type = device_info.device_type,
          Serial = device_info.device_serial_number
        };

        var customer_info = extensions.customer_info;

        var customerInfo = new CustomerInfo {
          Name = customer_info.name,
          AccountId = customer_info.user_id
        };

        var tokens = success.tokens;
        var website_cookies = tokens.website_cookies;

        var cookies = new List<KeyValuePair<string, string>> ();
        foreach (var cookie in website_cookies) {
          cookies.Add (new KeyValuePair<string, string> (
            cookie.Name,
            cookie.Value.Replace ("\"", "")
          ));
        }

        var store_authentication_cookie = tokens.store_authentication_cookie;
        string storeAuthentCookie = store_authentication_cookie.cookie;

        var mac_dms = tokens.mac_dms;
        string devicePrivateKey = mac_dms.device_private_key;
        string adpToken = mac_dms.adp_token;

        var bearer = tokens.bearer;
        int.TryParse (bearer.expires_in, out var expires);

        var tokenBearer = new TokenBearer (
          bearer.access_token,
          bearer.refresh_token,
          DateTime.UtcNow.AddSeconds (expires)
        );

        profile.Update (
          tokenBearer, 
          cookies, 
          deviceInfo, 
          customerInfo, 
          devicePrivateKey, 
          adpToken, 
          storeAuthentCookie);

      } catch (Exception exc) {
        Log (1, this, () => exc.Summary ());
        return false;
      }
      return true;
    }

  }
}
