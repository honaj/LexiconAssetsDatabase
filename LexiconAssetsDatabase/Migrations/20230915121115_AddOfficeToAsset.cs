using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LexiconAssetsDatabase.Migrations
{
    /// <inheritdoc />
    public partial class AddOfficeToAsset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Office",
                table: "Assets",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Office",
                table: "Assets");
        }
    }
}
