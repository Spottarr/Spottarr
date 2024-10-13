using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spottarr.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingSubCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationSpot_Types",
                table: "Spots",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GameSpot_Formats",
                table: "Spots",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GameSpot_Types",
                table: "Spots",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationSpot_Types",
                table: "Spots");

            migrationBuilder.DropColumn(
                name: "GameSpot_Formats",
                table: "Spots");

            migrationBuilder.DropColumn(
                name: "GameSpot_Types",
                table: "Spots");
        }
    }
}
