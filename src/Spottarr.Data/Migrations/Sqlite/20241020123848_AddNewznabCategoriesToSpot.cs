using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spottarr.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewznabCategoriesToSpot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewznabCategories",
                table: "Spots",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewznabCategories",
                table: "Spots");
        }
    }
}
