using System;
using System.Threading;
using core.audiamus.booksdb;
using core.audiamus.common;
using core.audiamus.util;

namespace core.audiamus.connect {
  public record Callbacks {
    public Func<byte[], string> CaptchaCallback { get; init; }
    public Action ApprovalCallback { get; init; }
    public Func<string> MfaCallback { get; init; }
    public Func<string> CvfCallback { get; init; }
    
    public Func<Uri, Uri> ExternalLoginCallback { get; init; }
    public Func<IProfileKeyEx, bool> DeregisterDeviceConfirmCallback { get; init; }
    public Func<AccountAliasContext, bool> GetAccountAliasFunc { get; init; }
  };

  public record Credentials (string Username, string Password);

  public record CredentialsUrl (Credentials Credentials, string BaseUriString) :
    Credentials (Credentials.Username, Credentials.Password);

  public record RegisterResult (
    EAuthorizeResult Result, 
    IProfileKeyEx NewProfileKey, 
    string PrevDeviceName
  );

  public record ProfileKey (uint Id, ERegion Region, string AccountId) : IProfileKey;
  public record ProfileKeyEx (uint Id, ERegion Region, string AccountName, string AccountId, string DeviceName) :  
    ProfileKey (Id, Region, AccountId), IProfileKeyEx;

  public record ProfileId (int AccountId, ERegion Region);

  public record AccountAlias (string AccountId, string Alias); 
  
  public record SimpleConversionContext (
    IProgress<ProgressMessage> Progress, 
    CancellationToken CancellationToken
  );

  record ProductComponentPair (adb.json.Product Product, Component Component);

  record ProfileBundle (IProfile Profile, IProfileKey Key, IProfileAliasKey AliasKey);

}


