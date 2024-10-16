using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spottarr.Data.Migrations
{
    /// <inheritdoc />
    public partial class ResolveTphColumnNameClashes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Types",
                table: "Spots",
                newName: "ImageTypes");

            migrationBuilder.RenameColumn(
                name: "Sources",
                table: "Spots",
                newName: "ImageSources");

            migrationBuilder.RenameColumn(
                name: "Platforms",
                table: "Spots",
                newName: "ImageLanguages");

            migrationBuilder.RenameColumn(
                name: "Languages",
                table: "Spots",
                newName: "ImageGenres");

            migrationBuilder.RenameColumn(
                name: "Genres",
                table: "Spots",
                newName: "ImageFormats");

            migrationBuilder.RenameColumn(
                name: "GameSpot_Types",
                table: "Spots",
                newName: "GameTypes");

            migrationBuilder.RenameColumn(
                name: "GameSpot_Genres",
                table: "Spots",
                newName: "GamePlatforms");

            migrationBuilder.RenameColumn(
                name: "GameSpot_Formats",
                table: "Spots",
                newName: "GameGenres");

            migrationBuilder.RenameColumn(
                name: "Formats",
                table: "Spots",
                newName: "GameFormats");

            migrationBuilder.RenameColumn(
                name: "Bitrates",
                table: "Spots",
                newName: "AudioTypes");

            migrationBuilder.RenameColumn(
                name: "AudioSpot_Types",
                table: "Spots",
                newName: "AudioSources");

            migrationBuilder.RenameColumn(
                name: "AudioSpot_Sources",
                table: "Spots",
                newName: "AudioGenres");

            migrationBuilder.RenameColumn(
                name: "AudioSpot_Genres",
                table: "Spots",
                newName: "AudioFormats");

            migrationBuilder.RenameColumn(
                name: "AudioSpot_Formats",
                table: "Spots",
                newName: "AudioBitrates");

            migrationBuilder.RenameColumn(
                name: "ApplicationSpot_Types",
                table: "Spots",
                newName: "ApplicationTypes");

            migrationBuilder.RenameColumn(
                name: "ApplicationSpot_Platforms",
                table: "Spots",
                newName: "ApplicationPlatforms");

            migrationBuilder.RenameColumn(
                name: "ApplicationSpot_Genres",
                table: "Spots",
                newName: "ApplicationGenres");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageTypes",
                table: "Spots",
                newName: "Types");

            migrationBuilder.RenameColumn(
                name: "ImageSources",
                table: "Spots",
                newName: "Sources");

            migrationBuilder.RenameColumn(
                name: "ImageLanguages",
                table: "Spots",
                newName: "Platforms");

            migrationBuilder.RenameColumn(
                name: "ImageGenres",
                table: "Spots",
                newName: "Languages");

            migrationBuilder.RenameColumn(
                name: "ImageFormats",
                table: "Spots",
                newName: "Genres");

            migrationBuilder.RenameColumn(
                name: "GameTypes",
                table: "Spots",
                newName: "GameSpot_Types");

            migrationBuilder.RenameColumn(
                name: "GamePlatforms",
                table: "Spots",
                newName: "GameSpot_Genres");

            migrationBuilder.RenameColumn(
                name: "GameGenres",
                table: "Spots",
                newName: "GameSpot_Formats");

            migrationBuilder.RenameColumn(
                name: "GameFormats",
                table: "Spots",
                newName: "Formats");

            migrationBuilder.RenameColumn(
                name: "AudioTypes",
                table: "Spots",
                newName: "Bitrates");

            migrationBuilder.RenameColumn(
                name: "AudioSources",
                table: "Spots",
                newName: "AudioSpot_Types");

            migrationBuilder.RenameColumn(
                name: "AudioGenres",
                table: "Spots",
                newName: "AudioSpot_Sources");

            migrationBuilder.RenameColumn(
                name: "AudioFormats",
                table: "Spots",
                newName: "AudioSpot_Genres");

            migrationBuilder.RenameColumn(
                name: "AudioBitrates",
                table: "Spots",
                newName: "AudioSpot_Formats");

            migrationBuilder.RenameColumn(
                name: "ApplicationTypes",
                table: "Spots",
                newName: "ApplicationSpot_Types");

            migrationBuilder.RenameColumn(
                name: "ApplicationPlatforms",
                table: "Spots",
                newName: "ApplicationSpot_Platforms");

            migrationBuilder.RenameColumn(
                name: "ApplicationGenres",
                table: "Spots",
                newName: "ApplicationSpot_Genres");
        }
    }
}
