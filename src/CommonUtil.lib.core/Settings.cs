using System;
using System.Collections.Generic;

namespace core.audiamus.util {
  public interface IUpdateSettings {
    EOnlineUpdate OnlineUpdate { get; }
    HashSet<Version> UpdateNotes { get; }
  }

  public class UpdateSettings : IUpdateSettings {
    public EOnlineUpdate OnlineUpdate { get; set; } = EOnlineUpdate.promptForDownload;
    public HashSet<Version> UpdateNotes { get; set; } = [];
  }
}
