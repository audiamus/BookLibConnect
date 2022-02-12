using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using core.audiamus.aux.ex;
using core.audiamus.common;
using core.audiamus.booksdb.ex;

namespace core.audiamus.booksdb {
  public class Book : IBookMeta, IBookCommon {
    public int Id { get; internal set; }
    public string Asin { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string PublisherName { get; set; }
    public string PublisherSummary { get; set; }
    public string MerchandisingSummary { get; set; }
    public float? AverageRating { get; set; }
    public int? RunTimeLengthSeconds { get; set; }
    public long? FileSizeBytes { get; set; }
    public int? SampleRate { get; set; }
    public int? BitRate { get; set; }
    public ECodec? FileCodec { get; set; }
    public EDeliveryType? DeliveryType { get; set; }
    public bool? Unabridged { get; set; }
    public bool? AdultProduct { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string Language { get; set; }
    public string CoverImageUrl { get; set; }
    public string CoverImageFile { get; set; }
    public string Sku { get; set; }
    public string SkuLite { get; set; }
    public string LicenseKey { get; set; }
    public string LicenseIv { get; set; }
    public bool? Deleted { get; set; }

    [NotMapped]
    public string Author => Authors?.Select(a => a.Name).FirstEtAl ();
    [NotMapped]
    public string MultiAuthors => Authors?.Select(a => a.Name).Combine ();
    [NotMapped]
    public string Narrator => Narrators?.Select(a => a.Name).FirstEtAl ();
    [NotMapped]
    public string MultiNarrators => Narrators?.Select(a => a.Name).Combine ();

    public virtual ChapterInfo ChapterInfo { get; set; }
    public virtual Conversion Conversion { get; set; }
    public virtual ICollection<Author> Authors { get; } = new List<Author> ();
    public virtual ICollection<Narrator> Narrators { get; } = new List<Narrator> ();
    public virtual ICollection<Component> Components { get; } = new List<Component> ();
    public virtual ICollection<SeriesBook> Series { get; } = new List<SeriesBook> ();
    public virtual ICollection<Genre> Genres { get; } = new List<Genre> ();
    public virtual ICollection<Ladder> Ladders { get; } = new List<Ladder> ();
    public virtual ICollection<Codec> Codecs { get; } = new List<Codec> ();

    public override string ToString () => $"asin={Asin}, \"{Title}\"";
  }


  public class Author : IPerson {
    public int Id { get; internal set; }
    public string Asin { get; set; }
    public string Name { get; set; }

    public virtual ICollection<Book> Books { get; } = new List<Book> ();

    public override string ToString () => $"asin={Asin}, \"{Name}\"";
  }

  public class Narrator : IPerson {
    public int Id { get; internal set; }
    public string Asin { get; set; }
    public string Name { get; set; }

    public virtual ICollection<Book> Books { get; } = new List<Book> ();
    
    public override string ToString () => $"asin={Asin}, \"{Name}\"";
  }

  public class Component : IBookCommon {
    public int Id { get; internal set; }
    public string Asin { get; set; }
    public string Title { get; set; }
    public int PartNumber { get; set; }

    public int? RunTimeLengthSeconds { get; set; }
    public long? FileSizeBytes { get; set; }
    public int? SampleRate { get; set; }
    public int? BitRate { get; set; }

    public ECodec? FileCodec { get; set; }

    public string Sku { get; set; }
    public string SkuLite { get; set; }

    public string LicenseKey { get; set; }
    public string LicenseIv { get; set; }

    internal int BookId { get; set; }

    public virtual Conversion Conversion { get; set; }
    public virtual Book Book { get; set; }
    public virtual ChapterInfo ChapterInfo { get; set; }

    [NotMapped]
    public string Author => Meta?.Author;

    [NotMapped]
    string IBookMeta.MultiAuthors => Meta?.MultiAuthors;

    [NotMapped]
    public string Narrator => Meta?.Narrator;

    [NotMapped]
    string IBookMeta.MultiNarrators => Meta?.MultiNarrators;

    [NotMapped]
    public DateTime? ReleaseDate => Meta?.ReleaseDate;

    [NotMapped]
    public DateTime? PurchaseDate => Meta?.PurchaseDate;

    private IBookMeta Meta => Book;

    public override string ToString () => $"asin={Asin}, \"{Book.Title}, part {PartNumber}\"";
  }

  public class Series {
    public int Id { get; internal set; }
    public string Asin { get; set; }
    public string Title { get; set; }
    public string Sku { get; set; }
    public string SkuLite { get; set; }

    public virtual ICollection<SeriesBook> Books { get; } = new List<SeriesBook> ();
    
    public override string ToString () => $"asin={Asin}, \"{Title}\"";
  }

  public class SeriesBook {
    internal int SeriesId { get; set; }
    internal int BookId { get; set; }
    public int BookNumber { get; set; }
    public int? SubNumber { get; set; }
    public string Sequence { get; set; }
    public int? Sort { get; set; }

    public virtual Series Series { get; set; }
    public virtual Book Book { get; set; }

    [NotMapped]
    public string SeqString {
      get {
        if (Sequence is null)
          return $"{BookNumber}{(SubNumber.HasValue ? $".{SubNumber.Value}" : string.Empty)}";
        else
          return Sequence;
      }
    }

    public override string ToString () => 
      $"{Series.Title} [{SeqString}]";
  }


  public class Conversion : IConversion, IBookMeta {
    public int Id { get; internal set; }
    public EConversionState State { get; set; }
    public DateTime LastUpdate { get; set; }
    [NotMapped]
    public EConversionState? PersistState { get; set; }
    public string DownloadFileName { get; set; }
    [NotMapped]
    public string DownloadUrl { get; set; }
    public string DestDirectory { get; set; }
    public int? ConvMode { get; set; } // enum?
    public int? ConvFormat { get; set; } // enum?
    public int? Mp4AAudio { get; set; } // enum?
    public int? AveTrackLengthMinutes { get; set; }
    public bool? NamedChapters { get; set; }
    public bool? ChapterMarkAdjusting { get; set; } 
    public bool? PreferEmbChapMarks { get; set; } 
    public bool? VariableBitRate { get; set; }
    public int? ReducedBitRate { get; set; }
    public int? ShortChapDurSeconds { get; set; }
    public int? VeryShortChapDurSeconds { get; set; }
    public int AccountId { get; set; }
    public ERegion Region { get; set; }

    public int? BookId { get; internal set; }
    public int? ComponentId { get; internal set; }
    public virtual Book Book { get; set; }
    public virtual Component Component { get; set; }

    [NotMapped]
    public string Asin => BookMeta?.Asin;

    [NotMapped]
    public string Title => BookMeta?.Title;

    [NotMapped]
    public string Author => BookMeta?.Author;

    [NotMapped]
    string IBookMeta.MultiAuthors => BookMeta?.MultiAuthors;

    [NotMapped]
    public string Narrator => BookMeta?.Narrator;

    [NotMapped]
    string IBookMeta.MultiNarrators => BookMeta?.MultiNarrators;

    [NotMapped]
    public long? FileSizeBytes => BookMeta?.FileSizeBytes;

    [NotMapped]
    public int? RunTimeLengthSeconds => BookMeta?.RunTimeLengthSeconds;

    [NotMapped]
    public int? SampleRate => BookMeta?.SampleRate;

    [NotMapped]
    public int? BitRate => BookMeta?.BitRate;

    [NotMapped]
    public string Sku => BookMeta?.Sku;

    [NotMapped]
    public string SkuLite => BookMeta?.SkuLite;

    [NotMapped]
    public DateTime? ReleaseDate => BookMeta?.ReleaseDate;

    [NotMapped]
    public DateTime? PurchaseDate => BookMeta?.PurchaseDate;

    [NotMapped]
    public Book ParentBook => Book ?? Component?.Book; 

    [NotMapped]
    public IBookMeta BookMeta => BookCommon;

    [NotMapped]
    public IBookCommon BookCommon => Book is null ? Component : Book;

    public Conversion () { }
    public Conversion (int id) => Id = id;

    public override string ToString () {
      var sb = new StringBuilder ();
      sb.Append ($"Id={Id}, ");
      if (Book is null) {
        if (Component.Title is null)
          sb.Append ($"\"{ParentBook.Title} Part {Component.PartNumber}\"");
        else
          sb.Append ($"\"{Component.Title}\"");
      } else {
        sb.Append ($"\"{Book.Title}\"");
      }
      sb.Append ($": {this.ApplicableState()}");

      string s = sb.ToString ();
      return s;
    }
  }


  public class Genre {
    internal int Id { get; set; }
    public long ExternalId { get; set; }
    public string Name { get; set; }

    public virtual ICollection<Book> Books { get; } = new List<Book> ();

    public override string ToString () => $"{ExternalId}: \"{Name}\"";
  }

  public class Ladder {
    internal int Id { get; set; }

    public virtual ICollection<Rung> Rungs { get; } = new List<Rung> ();
    public virtual ICollection<Book> Books { get; } = new List<Book> ();

    public override string ToString () => 
      Rungs.Select (r => r.Genre.Name).Combine (" - ");
  }

  public class Rung {
    public int OrderIdx { get; set; }
    public int GenreId { get; set; }  

    public virtual Genre Genre { get; set; }
    public virtual ICollection<Ladder> Ladders { get; } = new List<Ladder> ();
    
    public override string ToString () => $"{OrderIdx}: {Genre?.Name}";
  }

  public class Codec {
    internal int Id { get; set; }
    public ECodec Name { get; set; }

    public virtual ICollection<Book> Books { get; } = new List<Book> ();

    public override string ToString () => $"{Id}: {Name}";
  }

  public class ChapterInfo {
    internal int Id { get; set; }

    public int BrandIntroDurationMs { get; set; }
    public int BrandOutroDurationMs { get; set; }
    public int RuntimeLengthMs { get; set; }
    public bool? IsAccurate { get; set; }

    internal int? BookId { get; set; }
    internal int? ComponentId { get; set; }

    public virtual Book Book { get; set; }
    public virtual Component Component { get; set; }
    public virtual ICollection<Chapter> Chapters { get; } = new List<Chapter> ();

    [NotMapped]
    public IBookMeta BookMeta => Book is null ? Component : Book;

    public override string ToString () => 
      $"{Component.Title}: #chapters={Chapters.Count}, accurate={IsAccurate}";
  }

  public class Chapter {
    internal int Id { get; set; }

    public int LengthMs { get; set; }
    public int StartOffsetMs { get; set; }
    public string Title { get; set; }

    internal int? ChapterInfoId { get; set; }
    public virtual ChapterInfo ChapterInfo { get; set; }
    
    public virtual ICollection<Chapter> Chapters { get; } = new List<Chapter> ();
    
    internal int? ParentChapterId { get; set; }
    public virtual Chapter ParentChapter { get; set; }

    public Chapter () { }
    
    public Chapter (Chapter other) {
      LengthMs = other.LengthMs;
      StartOffsetMs = other.StartOffsetMs;
      Title = other.Title;
    }

    public override string ToString () => 
      $"{Title}: offs={TimeSpan.FromMilliseconds(StartOffsetMs)}, len={TimeSpan.FromMilliseconds(LengthMs)}, #children={Chapters?.Count}";
  }

  public class Account {
    public int Id { get; internal set; }
    public string Alias { get; set; }
    public string AudibleId { get; set; }
  }

  internal class PseudoAsin {
    public EPseudoAsinId Id { get; set; }
    public int LatestId { get; set; }
    public override string ToString () => $"{Id}: {LatestId:d7}";
  }

}
