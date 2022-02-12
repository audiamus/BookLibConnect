using Microsoft.EntityFrameworkCore.Migrations;

namespace core.audiamus.booksdb.Migrations
{
    public partial class NullableDeleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Removed",
                table: "Books");

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Books",
                type: "INTEGER",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Books");

            migrationBuilder.AddColumn<bool>(
                name: "Removed",
                table: "Books",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
