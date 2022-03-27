using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using core.audiamus.aux;
using core.audiamus.booksdb;
using core.audiamus.common;

namespace core.audiamus.connect {
  interface IProfile {
    uint Id { get; }
    ERegion Region { get; }
    IAuthorization Authorization { get; }
    ICustomerInfo CustomerInfo { get; }
    IDeviceInfo DeviceInfo { get; }
    ITokenBearer Token { get; }
    IEnumerable<KeyValuePair<string, string>> Cookies { get; }
    string PrivateKey { get; }
    string AdpToken { get; }
    string StoreAuthentCookie { get; }

    void Refresh (TokenBearer token);
  }

  interface IAuthorization {
    string AuthorizationCode { get;}
    string CodeVerifier { get; }
  }
      
  interface ITokenBearer {
    string RefreshToken { get; }
    string AccessToken { get; }
    DateTime Expiration { get; }
  }

  interface IDeviceInfo {
    string Type { get; }
    string Name { get; }
    string Serial { get; set; }
  }

  interface ICustomerInfo {
    string Name { get; }
    string AccountId { get; }
  }

  public interface IProfileKey {
    uint Id { get; }
    ERegion Region { get; }
    string AccountId { get; }
  }

  public interface IProfileKeyEx : IProfileKey {
    string AccountName { get; }
    string DeviceName{ get; }
  }

  public interface IProfileAliasKey {
    string AccountAlias { get; }
    ERegion Region { get; }
  }

  public interface ICancellation {
    CancellationToken CancellationToken { get; }
  }

  public interface IBookLibrary {
    IEnumerable<Book> GetBooks (ProfileId profileId);
    void GetChapters (IBookCommon item);
    IEnumerable<Chapter> GetChaptersFlattened (IBookCommon item, List<List<ChapterExtract>> accuChapters);
    EConversionState GetPersistentState (Conversion conversion);
    void SavePersistentState (Conversion conversion, EConversionState state);
  }

  public interface IAudibleApi : IProfileAliasKey, IDisposable {
    Func<Task> RefreshTokenAsyncFunc { get; }
    Func<AccountAliasContext, bool> GetAccountAliasFunc { set; }
    Task<adb.json.LibraryResponse> GetLibraryAsync (bool resync);
    Task<string> GetAccountInfoAsync ();
    Task<string> GetUserProfileAsync ();
    Task<bool> GetActivationBytesAsync ();
    Task<adb.json.LicenseResponse> GetDownloadLicenseAsync (
      string asin, EDownloadQuality quality
    );
    Task<bool> DownloadAsync (
      Conversion conversion, Action<Conversion, long> progressAction, CancellationToken cancToken
    );
    Task<bool> DecryptAsync (
      Conversion conversion, Action<Conversion, TimeSpan> progressAction, CancellationToken cancToken
    );
    Task DownloadCoverImagesAsync ();
    Task UpdateMetaInfo (IEnumerable<Component> components, Action<IEnumerable<Component>> onDone);
    Task<bool> GetDownloadLicenseAndSaveAsync (Conversion conversion, EDownloadQuality quality);
    IEnumerable<Book> GetBooks ();
    void SavePersistentState (Conversion conversion, EConversionState state);
    void RestorePersistentState (Conversion conversion);
    EConversionState GetPersistentState (Conversion conversion);
    void CheckUpdateFilesAndState (
      IDownloadSettings downloadSettings, 
      IExportSettings exportSettings, 
      Action<IConversion> callbackRefConversion,
      IInteractionCallback<InteractionMessage<BookLibInteract>, bool?> interactCallback
    );

  }
}