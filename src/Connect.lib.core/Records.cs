using System;
using System.Collections.Generic;
using System.Threading;

using core.audiamus.aux.ex;
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

  public record ProfileKey (uint Id, ERegion Region, string AccountId) : IProfileKey {
    public override string ToString () => 
      $"{GetType().Name} {nameof (Id)}={Id}, {nameof (Region)}={Region}, {nameof (AccountId)}=#<{AccountId.Checksum32 ()}>";
  }
  public record ProfileKeyEx (uint Id, ERegion Region, string AccountName, string AccountId, string DeviceName) :  
    ProfileKey (Id, Region, AccountId), IProfileKeyEx {
    public override string ToString () => 
      $"{base.ToString()}, {nameof (AccountName)}=#<{AccountName.Checksum32}>, {nameof (DeviceName)}=#<{DeviceName.Checksum32 ()}>";
  }

  public record ProfileId (int AccountId, ERegion Region);

  public record AccountAlias (string AccountId, string Alias); 
  
  public record SimpleConversionContext (
    IProgress<ProgressMessage> Progress, 
    CancellationToken CancellationToken
  );

  public record BookLibInteract (EBookLibInteract Kind);

  public record ChapterExtract (string Title, int Length);

  record ProductComponentPair (adb.json.Product Product, Component Component);

  record ProfileBundle (IProfile Profile, IProfileKey Key, IProfileAliasKey AliasKey);

  record BookCompositeLists (
    List<string> BookAsins,  
    List<Conversion> Conversions,  
    List<Component> Components,  
    List<Series> Series,  
    List<SeriesBook> SeriesBooks,  
    List<Author> Authors,  
    List<Narrator> Narrators,  
    List<Genre> Genres,  
    List<Ladder> Ladders,  
    List<Rung> Rungs,  
    List<Codec> Codecs  
  );

  record ConfigurationTokenResult (string Token, bool Weak);

}


