using System;
using System.Collections.Generic;
using core.audiamus.common;

namespace core.audiamus.connect {

  public class AccountAliasContext {
    public int LocalId { get; }
    public string CustomerName { get; }
    public IEnumerable<uint> Hashes { get; }
    public string Alias { get; set; }
    
    public AccountAliasContext (int localId, string customerName, IEnumerable<uint> hashes) {
      LocalId = localId;
      CustomerName = customerName;
      Hashes = hashes;
    }
  }

  public class ProfileAliasKey : IEquatable<IProfileAliasKey>, IProfileAliasKey {
    public string AccountAlias { get; set; }
    public ERegion Region { get; set; }

    public ProfileAliasKey () { }
    public ProfileAliasKey (IProfileAliasKey other) : this (other.Region, other.AccountAlias) { }
    public ProfileAliasKey (ERegion region, string accountAlias) {
      Region = region;
      AccountAlias = accountAlias;
    }

    public override string ToString () => $"{AccountAlias}; {Region}";
    public bool Equals (IProfileAliasKey other) {
      return Region == other.Region && string.Equals (AccountAlias, other.AccountAlias);
    }
  }

}
