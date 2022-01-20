using Microsoft.EntityFrameworkCore.Migrations;

namespace core.audiamus.booksdb.Migrations
{
    public partial class SeriesSeq : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sequence",
                table: "SeriesBooks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sort",
                table: "SeriesBooks",
                type: "INTEGER",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "SeriesBooks");

            migrationBuilder.DropColumn(
                name: "Sort",
                table: "SeriesBooks");
        }
    }
}
