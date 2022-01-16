using System.Collections.Generic;
using core.audiamus.common;

namespace core.audiamus.booksdb {
  public interface IPerson {
    string Asin { get; set; }
    string Name { get; set; }
    ICollection<Book> Books { get; }
  }

  public interface IBookCommon : IBookMeta {
    new long? FileSizeBytes { get; set; }
    new int? RunTimeLengthSeconds { get; set; }
    new int? SampleRate { get; set; }
    new int? BitRate { get; set; }

    string LicenseKey { get; set; }
    string LicenseIv { get; set; }
    public ECodec? FileCodec { get; set; }
    ChapterInfo ChapterInfo { get; set; }

    Conversion Conversion { get; }
  }

  public interface IConversion {
    int Id { get; }
    EConversionState State { get; }
    string DownloadFileName { get; }
    string DestDirectory { get; }
  }

}
