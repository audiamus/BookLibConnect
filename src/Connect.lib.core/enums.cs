using System;

namespace core.audiamus.connect {

  public enum EInitialSorting {
    state_date,
    date,
    author_title,
    author_date,
    title_author
  }

  public enum EDownloadQuality {
    Extreme,
    High,
    Normal,
    Low
  }

  [Flags]
  enum ECheckFile {
    none = 0,
    deleteIfMissing = 1,
    relocatable = 2
  }

  public enum EAuthorizeResult {
    none,
    invalidUrl,
    authorizationFailed,
    registrationFailed,
    removeFailed,
    succ,
    deregistrationFailed,
    removeProfileFailed,
  }
}
