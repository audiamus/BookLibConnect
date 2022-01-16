namespace core.audiamus.booksdb {
  public enum ECodec {
    format4,
    mp4_22_32,
    mp4_22_64,
    mp4_44_64,
    mp4_44_128,
    aax,
    aax_22_32,
    aax_22_64,
    aax_44_64,
    aax_44_128,
  }

  public enum EDeliveryType {
    SinglePartBook,
    MultiPartBook,
    AudioPart,
    BookSeries,
    Periodical
  }

  public enum EConversionState {
    unknown,          // question mark
    remote,           // globe
    download,         // globe with down arrow
    license_granted,  // key
    license_denied,   // strikethru key
    downloading,      // down arrow
    download_error,   // strikethru down arrow
    local_locked,     // lock closed
    unlocking,        // key over lock
    unlocking_failed, // strikethru key over lock
    local_unlocked,   // lock open
    exported,         // checkmark mauve
    converting,       // right arrow
    converted,        // checkmark green
    converted_unknown,// checkmark gray
    conversion_error  // red cross
  }

  public enum ELicenseStatusCode {
    Unknown,
    Granted
  }

  internal enum EPseudoAsinId {
    none,
    author,
    narrator
  }
}
