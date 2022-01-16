using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace core.audiamus.booksdb.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Alias = table.Column<string>(type: "TEXT", nullable: true),
                    AudibleId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Asin = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Asin = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Subtitle = table.Column<string>(type: "TEXT", nullable: true),
                    PublisherName = table.Column<string>(type: "TEXT", nullable: true),
                    PublisherSummary = table.Column<string>(type: "TEXT", nullable: true),
                    MerchandisingSummary = table.Column<string>(type: "TEXT", nullable: true),
                    AverageRating = table.Column<float>(type: "REAL", nullable: true),
                    RunTimeLengthSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    FileSizeBytes = table.Column<long>(type: "INTEGER", nullable: true),
                    SampleRate = table.Column<int>(type: "INTEGER", nullable: true),
                    BitRate = table.Column<int>(type: "INTEGER", nullable: true),
                    FileCodec = table.Column<int>(type: "INTEGER", nullable: true),
                    DeliveryType = table.Column<int>(type: "INTEGER", nullable: true),
                    Unabridged = table.Column<bool>(type: "INTEGER", nullable: true),
                    AdultProduct = table.Column<bool>(type: "INTEGER", nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReleaseDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Language = table.Column<string>(type: "TEXT", nullable: true),
                    CoverImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    CoverImageFile = table.Column<string>(type: "TEXT", nullable: true),
                    Sku = table.Column<string>(type: "TEXT", nullable: true),
                    SkuLite = table.Column<string>(type: "TEXT", nullable: true),
                    LicenseKey = table.Column<string>(type: "TEXT", nullable: true),
                    LicenseIv = table.Column<string>(type: "TEXT", nullable: true),
                    Removed = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Codecs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Codecs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExternalId = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ladders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ladders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Narrators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Asin = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Narrators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PseudoAsins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    LatestId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PseudoAsins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Series",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Asin = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Sku = table.Column<string>(type: "TEXT", nullable: true),
                    SkuLite = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthorBook",
                columns: table => new
                {
                    AuthorsId = table.Column<int>(type: "INTEGER", nullable: false),
                    BooksId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorBook", x => new { x.AuthorsId, x.BooksId });
                    table.ForeignKey(
                        name: "FK_AuthorBook_Authors_AuthorsId",
                        column: x => x.AuthorsId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthorBook_Books_BooksId",
                        column: x => x.BooksId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Components",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Asin = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    PartNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    RunTimeLengthSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    FileSizeBytes = table.Column<long>(type: "INTEGER", nullable: true),
                    SampleRate = table.Column<int>(type: "INTEGER", nullable: true),
                    BitRate = table.Column<int>(type: "INTEGER", nullable: true),
                    FileCodec = table.Column<int>(type: "INTEGER", nullable: true),
                    Sku = table.Column<string>(type: "TEXT", nullable: true),
                    SkuLite = table.Column<string>(type: "TEXT", nullable: true),
                    LicenseKey = table.Column<string>(type: "TEXT", nullable: true),
                    LicenseIv = table.Column<string>(type: "TEXT", nullable: true),
                    BookId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Components", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Components_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookCodec",
                columns: table => new
                {
                    BooksId = table.Column<int>(type: "INTEGER", nullable: false),
                    CodecsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookCodec", x => new { x.BooksId, x.CodecsId });
                    table.ForeignKey(
                        name: "FK_BookCodec_Books_BooksId",
                        column: x => x.BooksId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookCodec_Codecs_CodecsId",
                        column: x => x.CodecsId,
                        principalTable: "Codecs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookGenre",
                columns: table => new
                {
                    BooksId = table.Column<int>(type: "INTEGER", nullable: false),
                    GenresId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookGenre", x => new { x.BooksId, x.GenresId });
                    table.ForeignKey(
                        name: "FK_BookGenre_Books_BooksId",
                        column: x => x.BooksId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookGenre_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rungs",
                columns: table => new
                {
                    OrderIdx = table.Column<int>(type: "INTEGER", nullable: false),
                    GenreId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rungs", x => new { x.OrderIdx, x.GenreId });
                    table.ForeignKey(
                        name: "FK_Rungs_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookLadder",
                columns: table => new
                {
                    BooksId = table.Column<int>(type: "INTEGER", nullable: false),
                    LaddersId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookLadder", x => new { x.BooksId, x.LaddersId });
                    table.ForeignKey(
                        name: "FK_BookLadder_Books_BooksId",
                        column: x => x.BooksId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookLadder_Ladders_LaddersId",
                        column: x => x.LaddersId,
                        principalTable: "Ladders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookNarrator",
                columns: table => new
                {
                    BooksId = table.Column<int>(type: "INTEGER", nullable: false),
                    NarratorsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookNarrator", x => new { x.BooksId, x.NarratorsId });
                    table.ForeignKey(
                        name: "FK_BookNarrator_Books_BooksId",
                        column: x => x.BooksId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookNarrator_Narrators_NarratorsId",
                        column: x => x.NarratorsId,
                        principalTable: "Narrators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeriesBooks",
                columns: table => new
                {
                    SeriesId = table.Column<int>(type: "INTEGER", nullable: false),
                    BookId = table.Column<int>(type: "INTEGER", nullable: false),
                    BookNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    SubNumber = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriesBooks", x => new { x.SeriesId, x.BookId });
                    table.ForeignKey(
                        name: "FK_SeriesBooks_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeriesBooks_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChapterInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BrandIntroDurationMs = table.Column<int>(type: "INTEGER", nullable: false),
                    BrandOutroDurationMs = table.Column<int>(type: "INTEGER", nullable: false),
                    RuntimeLengthMs = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAccurate = table.Column<bool>(type: "INTEGER", nullable: true),
                    BookId = table.Column<int>(type: "INTEGER", nullable: true),
                    ComponentId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChapterInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChapterInfos_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChapterInfos_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Conversions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DownloadFileName = table.Column<string>(type: "TEXT", nullable: true),
                    DestDirectory = table.Column<string>(type: "TEXT", nullable: true),
                    ConvMode = table.Column<int>(type: "INTEGER", nullable: true),
                    ConvFormat = table.Column<int>(type: "INTEGER", nullable: true),
                    Mp4AAudio = table.Column<int>(type: "INTEGER", nullable: true),
                    AveTrackLengthMinutes = table.Column<int>(type: "INTEGER", nullable: true),
                    NamedChapters = table.Column<bool>(type: "INTEGER", nullable: true),
                    ChapterMarkAdjusting = table.Column<bool>(type: "INTEGER", nullable: true),
                    PreferEmbChapMarks = table.Column<bool>(type: "INTEGER", nullable: true),
                    VariableBitRate = table.Column<bool>(type: "INTEGER", nullable: true),
                    ReducedBitRate = table.Column<int>(type: "INTEGER", nullable: true),
                    ShortChapDurSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    VeryShortChapDurSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    AccountId = table.Column<int>(type: "INTEGER", nullable: false),
                    Region = table.Column<int>(type: "INTEGER", nullable: false),
                    BookId = table.Column<int>(type: "INTEGER", nullable: true),
                    ComponentId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversions_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conversions_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LadderRung",
                columns: table => new
                {
                    LaddersId = table.Column<int>(type: "INTEGER", nullable: false),
                    RungsOrderIdx = table.Column<int>(type: "INTEGER", nullable: false),
                    RungsGenreId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LadderRung", x => new { x.LaddersId, x.RungsOrderIdx, x.RungsGenreId });
                    table.ForeignKey(
                        name: "FK_LadderRung_Ladders_LaddersId",
                        column: x => x.LaddersId,
                        principalTable: "Ladders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LadderRung_Rungs_RungsOrderIdx_RungsGenreId",
                        columns: x => new { x.RungsOrderIdx, x.RungsGenreId },
                        principalTable: "Rungs",
                        principalColumns: new[] { "OrderIdx", "GenreId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chapters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LengthMs = table.Column<int>(type: "INTEGER", nullable: false),
                    StartOffsetMs = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    ChapterInfoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chapters_ChapterInfos_ChapterInfoId",
                        column: x => x.ChapterInfoId,
                        principalTable: "ChapterInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorBook_BooksId",
                table: "AuthorBook",
                column: "BooksId");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_Asin",
                table: "Authors",
                column: "Asin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Authors_Name",
                table: "Authors",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_BookCodec_CodecsId",
                table: "BookCodec",
                column: "CodecsId");

            migrationBuilder.CreateIndex(
                name: "IX_BookGenre_GenresId",
                table: "BookGenre",
                column: "GenresId");

            migrationBuilder.CreateIndex(
                name: "IX_BookLadder_LaddersId",
                table: "BookLadder",
                column: "LaddersId");

            migrationBuilder.CreateIndex(
                name: "IX_BookNarrator_NarratorsId",
                table: "BookNarrator",
                column: "NarratorsId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Asin",
                table: "Books",
                column: "Asin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_PurchaseDate",
                table: "Books",
                column: "PurchaseDate");

            migrationBuilder.CreateIndex(
                name: "IX_ChapterInfos_BookId",
                table: "ChapterInfos",
                column: "BookId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChapterInfos_ComponentId",
                table: "ChapterInfos",
                column: "ComponentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_ChapterInfoId",
                table: "Chapters",
                column: "ChapterInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_Asin",
                table: "Components",
                column: "Asin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Components_BookId",
                table: "Components",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversions_BookId",
                table: "Conversions",
                column: "BookId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversions_ComponentId",
                table: "Conversions",
                column: "ComponentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genres_ExternalId",
                table: "Genres",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LadderRung_RungsOrderIdx_RungsGenreId",
                table: "LadderRung",
                columns: new[] { "RungsOrderIdx", "RungsGenreId" });

            migrationBuilder.CreateIndex(
                name: "IX_Narrators_Asin",
                table: "Narrators",
                column: "Asin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Narrators_Name",
                table: "Narrators",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Rungs_GenreId",
                table: "Rungs",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Series_Asin",
                table: "Series",
                column: "Asin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeriesBooks_BookId",
                table: "SeriesBooks",
                column: "BookId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "AuthorBook");

            migrationBuilder.DropTable(
                name: "BookCodec");

            migrationBuilder.DropTable(
                name: "BookGenre");

            migrationBuilder.DropTable(
                name: "BookLadder");

            migrationBuilder.DropTable(
                name: "BookNarrator");

            migrationBuilder.DropTable(
                name: "Chapters");

            migrationBuilder.DropTable(
                name: "Conversions");

            migrationBuilder.DropTable(
                name: "LadderRung");

            migrationBuilder.DropTable(
                name: "PseudoAsins");

            migrationBuilder.DropTable(
                name: "SeriesBooks");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Codecs");

            migrationBuilder.DropTable(
                name: "Narrators");

            migrationBuilder.DropTable(
                name: "ChapterInfos");

            migrationBuilder.DropTable(
                name: "Ladders");

            migrationBuilder.DropTable(
                name: "Rungs");

            migrationBuilder.DropTable(
                name: "Series");

            migrationBuilder.DropTable(
                name: "Components");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
