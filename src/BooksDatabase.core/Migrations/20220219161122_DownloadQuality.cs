using Microsoft.EntityFrameworkCore.Migrations;

namespace core.audiamus.booksdb.Migrations
{
    public partial class DownloadQuality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DownloadQuality",
                table: "Components",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DownloadQuality",
                table: "Books",
                type: "INTEGER",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DownloadQuality",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "DownloadQuality",
                table: "Books");
        }
    }
}
