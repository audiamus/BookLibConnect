using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using core.audiamus.aux;
using Microsoft.EntityFrameworkCore;
using static core.audiamus.aux.Logging;

namespace core.audiamus.booksdb {

  // PM> Add-Migration InitialCreate -Project BooksDatabase.core -Context BookDbContext
  // PM> Add-Migration -Name <mig name> -Project BooksDatabase.core -Context BookDbContext
  public class BookDbContext : DbContext {
    private const string SUBDIR = "data";
    private const string DBFILE = "audiobooks.db";
    private static readonly string DEFAULT_DIR = Path.Combine (ApplEnv.LocalApplDirectory, SUBDIR);

    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Narrator> Narrators { get; set; }
    public DbSet<Component> Components { get; set; }
    public DbSet<Series> Series { get; set; }
    public DbSet<SeriesBook> SeriesBooks { get; set; }
    public DbSet<Conversion> Conversions { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Ladder> Ladders { get; set; }
    public DbSet<Rung> Rungs { get; set; }
    public DbSet<Codec> Codecs { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<ChapterInfo> ChapterInfos { get; set; }
    public DbSet<Account> Accounts { get; set; }
    internal DbSet<PseudoAsin> PseudoAsins { get; set; }


    public string DbPath { get; }

    public BookDbContext (string dirpath = null, string filename = null) {
      if (dirpath is null)
        dirpath = DEFAULT_DIR;
      if (filename is null)
        filename = DBFILE;
      DbPath = Path.Combine (dirpath, filename);
    }

    public static async Task<bool> StartupAsync (string dirpath = null, string filename = null) {
      using var _ = new LogGuard (3, typeof (BookDbContext), () => $"dir={dirpath}, file={filename}");
      using var dbContext = new BookDbContext (dirpath, filename);

      var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync ();

      if (pendingMigrations.Any ()) {
        Log (3, typeof (BookDbContext), () => "with migrations");
        if (!File.Exists (dbContext.DbPath))
          Directory.CreateDirectory (Path.GetDirectoryName (dbContext.DbPath));

        await dbContext.Database.MigrateAsync ();
      }
      return dbContext.Database.CanConnect ();
    }

    private static readonly Dictionary<Type, EPseudoAsinId> _pseudoAsins
      = new Dictionary<Type, EPseudoAsinId> {
        { typeof (Author), EPseudoAsinId.author },
        { typeof (Narrator), EPseudoAsinId.narrator }
      };

    public string GetNextPseudoAsin<T> () => GetNextPseudoAsin (typeof (T));

    public string GetNextPseudoAsin (Type t) {
      bool succ = _pseudoAsins.TryGetValue (t, out var pseudoAsinId);
      if (!succ)
        return null;
      PseudoAsin ent = PseudoAsins.Find (pseudoAsinId);
      if (ent is null) {
        ent = new PseudoAsin { Id = pseudoAsinId };
        PseudoAsins.Add (ent);
      }
      ent.LatestId++;
      return ent.LatestId.ToString ("D7");
    }

    protected override void OnConfiguring (DbContextOptionsBuilder options)
        => options
            .UseSqlite ($"Data Source={DbPath}");

    protected override void OnModelCreating (ModelBuilder modelBuilder) {
      modelBuilder.Entity<Book> ().HasKey (e => e.Id);
      modelBuilder.Entity<Book> ().HasIndex (e => e.Asin).IsUnique();
      modelBuilder.Entity<Genre> ().HasKey (e => e.Id);
      modelBuilder.Entity<Genre> ().HasIndex (e => e.ExternalId).IsUnique ();
      modelBuilder.Entity<Author> ().HasKey (e => e.Id);
      modelBuilder.Entity<Author> ().HasIndex (e => e.Asin).IsUnique();
      modelBuilder.Entity<Narrator> ().HasKey (e => e.Id);
      modelBuilder.Entity<Narrator> ().HasIndex (e => e.Asin).IsUnique ();
      modelBuilder.Entity<Component> ().HasKey (e => e.Id);
      modelBuilder.Entity<Component> ().HasIndex (e => e.Asin).IsUnique ();
      modelBuilder.Entity<Series> ().HasKey (e => e.Id);
      modelBuilder.Entity<Series> ().HasIndex (e => e.Asin).IsUnique ();
      modelBuilder.Entity<Conversion> ().HasKey (e => e.Id);
      modelBuilder.Entity<SeriesBook> ().HasKey (e => new { e.SeriesId, e.BookId });
      modelBuilder.Entity<Ladder> ().HasKey (e => e.Id);
      modelBuilder.Entity<Rung> ().HasKey (e => new { e.OrderIdx, e.GenreId });
      modelBuilder.Entity<Codec> ().HasKey (e => e.Id);
      modelBuilder.Entity<Chapter> ().HasKey (e => e.Id);
      modelBuilder.Entity<ChapterInfo> ().HasKey (e => e.Id);
      modelBuilder.Entity<Account> ().HasKey (e => e.Id);


      modelBuilder.Entity<Book> ()
        .HasOne (e => e.Conversion)
        .WithOne (e => e.Book)
        .HasForeignKey<Conversion> (e => e.BookId)
        .IsRequired (false)
        .OnDelete (DeleteBehavior.Cascade);

      modelBuilder.Entity<Component> ()
        .HasOne (e => e.Book)
        .WithMany (e => e.Components)
        .HasForeignKey (e => e.BookId)
        .OnDelete (DeleteBehavior.Cascade);

      modelBuilder.Entity<Component> ()
        .HasOne (e => e.Conversion)
        .WithOne (e => e.Component)
        .HasForeignKey<Conversion> (e => e.ComponentId)
        .IsRequired (false)
        .OnDelete (DeleteBehavior.Cascade);

      modelBuilder.Entity<SeriesBook> ()
        .HasOne (e => e.Book)
        .WithMany (e => e.Series)
        .HasForeignKey (e => e.BookId);

      modelBuilder.Entity<SeriesBook> ()
        .HasOne (e => e.Series)
        .WithMany (e => e.Books)
        .HasForeignKey (e => e.SeriesId);

      modelBuilder.Entity<Rung> ()
        .HasOne (e => e.Genre)
        .WithMany ()
        .HasForeignKey (e => e.GenreId);

      modelBuilder.Entity<Narrator> ()
        .HasIndex (e => e.Name)
        .IsUnique (false);

      modelBuilder.Entity<Author> ()
        .HasIndex (e => e.Name)
        .IsUnique (false);

      modelBuilder.Entity<Book> ()
        .HasIndex (e => e.PurchaseDate)
        .IsUnique (false);

      modelBuilder.Entity<Component> ()
        .HasOne (e => e.ChapterInfo)
        .WithOne (e => e.Component)
        .HasForeignKey<ChapterInfo> (e => e.ComponentId)
        .OnDelete (DeleteBehavior.Cascade);
      
      modelBuilder.Entity<Book> ()
        .HasOne (e => e.ChapterInfo)
        .WithOne (e => e.Book)
        .HasForeignKey<ChapterInfo> (e => e.BookId)
        .OnDelete (DeleteBehavior.Cascade);

      modelBuilder.Entity<Chapter> ()
        .HasOne (e => e.ChapterInfo)
        .WithMany (e => e.Chapters)
        .HasForeignKey (e => e.ChapterInfoId)
        .IsRequired (false)
        .OnDelete (DeleteBehavior.Cascade);
      
      modelBuilder.Entity<Chapter> ()
        .HasOne (e => e.ParentChapter)
        .WithMany (e => e.Chapters)
        .HasForeignKey (e => e.ParentChapterId)
        .IsRequired (false)
        .OnDelete (DeleteBehavior.Cascade);

    }
  }

  public class BookDbContextLazyLoad : BookDbContext {

    public BookDbContextLazyLoad (string dirpath = null, string filename = null) : 
      base (dirpath, filename)
    { }

    protected override void OnConfiguring (DbContextOptionsBuilder options) {
      options.UseLazyLoadingProxies ();
      base.OnConfiguring (options);
    }
  }
}
