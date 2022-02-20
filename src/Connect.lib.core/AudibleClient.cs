using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.audiamus.aux;
using core.audiamus.aux.ex;
using core.audiamus.common;
using core.audiamus.connect.ex;
using core.audiamus.sysmgmt;
using static core.audiamus.aux.Logging;

namespace core.audiamus.connect {
  public class AudibleClient {

    private IAudibleApi _audibleApi;
    private AudibleLogin AudibleLogin { get; }
    private Authorize Authorize { get; }
    private BookLibrary BookLibrary { get; }

    private ProfileBundle Profile { get; set; }
    private IAuthorizeSettings AuthorizeSettings { get; }

    public IProfileKey ProfileKey => Profile?.Key;
    public IProfileAliasKey ProfileAliasKey => Profile?.AliasKey;

    public ConfigSettings ConfigSettings { get; }
    public IBookLibrary BookLibraryExcerpt => BookLibrary;

    public Action WeakConfigEncryptionCallback { set => Authorize.WeakConfigEncryptionCallback = value; }

    public IAudibleApi Api {
      get {
        if (_audibleApi is null) {
          if (Profile is null)
            return null;

          _audibleApi = new AudibleApi (
            Profile.Profile, 
            Authorize.HttpClientAmazon, 
            BookLibrary,
            Authorize.RefreshTokenAsync);
        }
        return _audibleApi;
      }
    }

    internal AudibleApi FullApi => _audibleApi as AudibleApi;

    public AudibleClient (ConfigSettings configSettings, IAuthorizeSettings authSettings, string dbDir = null) {
      Log (3, this);

      ConfigSettings = configSettings;
      if (ConfigSettings is not null)
        ConfigSettings.ChangedSettings += settings_ChangedSettings;
      AuthorizeSettings = authSettings;
      BookLibrary = new BookLibrary (dbDir);
      Authorize = new Authorize(getConfigurationToken, authSettings);
      AudibleLogin = new AudibleLogin ();
    }


    public async Task<RegisterResult> ConfigFromExternalLoginAsync (
      ERegion region,
      bool withPreAmazonUsername,
      Callbacks callbacks
    ) {
      Log (3, this, () => $"reg={region}, preAmznAccnt={withPreAmazonUsername}");
      Uri uri = ConfigBuildNewLoginUri (region, withPreAmazonUsername);

      if (callbacks.ExternalLoginCallback is null)
        return null;

      Uri responseUri = callbacks.ExternalLoginCallback (uri);
      
      var result = await ConfigParseExternalLoginResponseAsync (responseUri, callbacks);
      
      return result;
    }

    public Uri ConfigBuildNewLoginUri (
      ERegion region,
      bool withPreAmazonUsername
    ) {
      Log (3, this, () => $"reg={region}, preAmznAccnt={withPreAmazonUsername}");
      disposeProfileAndApi ();

      return AudibleLogin.BuildAuthUri (region, withPreAmazonUsername);
    }

    public async Task<RegisterResult> ConfigParseExternalLoginResponseAsync (
      Uri uri,
      Callbacks callbacks
    ) {
      using var _ = new LogGuard (3, this);

      var profile = AudibleLogin.ParseExternalResponse(uri);

      if (profile is null) {
        Log (1, this, () => "response parsing failed.");
        return new RegisterResult(EAuthorizeResult.authorizationFailed, null, null);
      }

      var (succ, prevProfile) = await Authorize.RegisterAsync (profile);
      if (!succ)
        return new RegisterResult (EAuthorizeResult.registrationFailed, null, null);

      EAuthorizeResult result = EAuthorizeResult.succ;

      if (profile.Matches (prevProfile))
        Profile = null;

      // TODO modify/test
      //bool deregister = prevProfile is not null &&
      //  (callbacks.DeregisterDeviceConfirmCallback?.Invoke (prevProfile.CreateKeyEx ()) ?? true);

      //if (deregister) {
      //  succ = await Authorize.DeregisterAsync (prevProfile);
      //  if (!succ)
      //    result = EAuthorizeResult.deregistrationFailed;
      //}
      bool deregister = prevProfile is not null;
      if (deregister)
        result = EAuthorizeResult.deregistrationFailed;
      
      return new (result, profile.CreateKeyEx(), prevProfile?.DeviceInfo?.Name);
    }

    public async Task<IProfileAliasKey> ConfigFromFileAsync (
      IProfileAliasKey aliasKey,
      Func<AccountAliasContext, bool> getAccountAliasFunc
    ) {
      Log (3, typeof (AudibleClient), () => aliasKey?.ToString());
      disposeProfileAndApi ();
      var resultKey = await fromFileAsync (aliasKey, getAccountAliasFunc);
      return resultKey;
    }

    public IEnumerable<AccountAlias> GetAccountAliases () => 
      BookLibrary.GetAccountAliases ();

    public async Task<string> GetProfileAliasAsync (
      IProfileKey key, Func<AccountAliasContext, bool> getAccountAliasFunc
    ) {
      var profiles = await Authorize.GetRegisteredProfilesAsync ();
      if (profiles is null)
        return null;

      var profile = profiles.FirstOrDefault (p => p.Region == key.Region && string.Equals (p.CustomerInfo.AccountId, key.AccountId));
      if (profile is null)
        return null;
      string alias = profile.GetAccountAlias (BookLibrary, getAccountAliasFunc);
      return alias;
    }

    public async Task<IEnumerable<IProfileKeyEx>> GetProfilesAsync () {
      Log (3, this);
      var profiles = await Authorize.GetRegisteredProfilesAsync ();
      if (profiles is null)
        return null;

      var profileKeys = profiles
        .Select (p => p.CreateKeyEx ())
        .ToList ();
      return profileKeys;
    }

    public async Task<EAuthorizeResult> RemoveProfileAsync (IProfileKey key) {
      Log (3, this, () => key.ToString());
      var result = await Authorize.RemoveProfileAsync (key);
      if (result >= EAuthorizeResult.succ)
        setProfile (null, null);
      return result;
    }

    public async Task<bool?> ChangeProfileAsync (IProfileKey key) {
      Log (3, this, () => key.ToString());
      //Key may be the same but profile could still be different, check Id instead
      if (Profile.MatchesId (key))
        return false;

      Log (3, this, () => Authorize?.GetProfile (key)?.CreateAliasKey(BookLibrary, null)?.ToString());
      disposeProfileAndApi ();
      
      var profiles = await Authorize.GetRegisteredProfilesAsync ();
      if (profiles is null)
        return null;

      var profile = profiles.FirstOrDefault (p => p.Matches(key));
      if (profile is null)
        return null;

      setProfile (profile, null);

      await Authorize.RefreshTokenAsync (profile);

      return true;
    }

    private IProfileAliasKey setProfile (IProfile profile, Func<AccountAliasContext, bool> getAccountAliasFunc) {
      if (profile is null) {
        Profile = null;
        return null;
      }

      var key = profile.CreateKey ();
      var aliasKey = profile.CreateAliasKey (BookLibrary, getAccountAliasFunc);
      Profile = new ProfileBundle (profile, key, aliasKey);
      return aliasKey;
    } 

    private void disposeProfileAndApi () {
      Profile = null;
      _audibleApi?.Dispose ();
      _audibleApi = null;
    }

    private async void settings_ChangedSettings (object sender, EventArgs e) => 
      await Authorize.WriteConfigurationAsync ();

    private ConfigurationTokenResult getConfigurationToken (bool enforce) {
      if (!(ConfigSettings?.EncryptConfiguration ?? false) && !enforce)
        return default;
      bool weak = false;
      var sb = new StringBuilder ();
      
      string uid = ApplEnv.UserName.Rot13 ();
      sb.Append (uid);
      
      string cid = HardwareId.GetCpuId ();
      if (cid.IsNullOrWhiteSpace ())
        weak = true;
      else
        sb.Append (cid);
      
      string mbId = HardwareId.GetMotherboardId ();
      if (mbId.IsNullOrWhiteSpace ())
        mbId = MotherboardInfo.PNPDeviceID;
      if (mbId.IsNullOrWhiteSpace ())
        weak = true;
      else
        sb.Append (mbId);
      
      return new (sb.ToString (), weak);
    }

    private async Task<IProfileAliasKey> fromFileAsync (
      IProfileAliasKey aliasKey,
      Func<AccountAliasContext, bool> getAccountAliasFunc
    ) {
      var accountAlisases = GetAccountAliases ();
      var profiles = await Authorize.GetRegisteredProfilesAsync ();
      if (profiles is null)
        return null;

      if (aliasKey is not null) {
        profiles = profiles.Where (p => p.Region == aliasKey.Region); 
        if (!aliasKey.AccountAlias.IsNullOrWhiteSpace ()) {
          string accountId = accountAlisases.FirstOrDefault (aa => aa.Alias == aliasKey.AccountAlias)?.AccountId;
          if (accountId is not null)
            profiles = profiles.Where (p => string.Equals(p.CustomerInfo.AccountId, accountId));
        }
      }
      
      if (!profiles.Any ())
        return null;

      var profile = profiles.First ();

      await Authorize.RefreshTokenAsync (profile);

      var resultKey = setProfile (profile, getAccountAliasFunc);
      return resultKey;
    }


  }
}
