using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spottarr.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFtdFieldsAndExternalIdsToSpot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Filename",
                table: "Spots",
                type: "TEXT",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImdbId",
                table: "Spots",
                type: "TEXT",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Newsgroup",
                table: "Spots",
                type: "TEXT",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "Spots",
                type: "TEXT",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TvdbId",
                table: "Spots",
                type: "TEXT",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Spots",
                type: "TEXT",
                maxLength: 512,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Spots_ImdbId",
                table: "Spots",
                column: "ImdbId");

            migrationBuilder.CreateIndex(
                name: "IX_Spots_TvdbId",
                table: "Spots",
                column: "TvdbId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Spots_ImdbId",
                table: "Spots");

            migrationBuilder.DropIndex(
                name: "IX_Spots_TvdbId",
                table: "Spots");

            migrationBuilder.DropColumn(
                name: "Filename",
                table: "Spots");

            migrationBuilder.DropColumn(
                name: "ImdbId",
                table: "Spots");

            migrationBuilder.DropColumn(
                name: "Newsgroup",
                table: "Spots");

            migrationBuilder.DropColumn(
                name: "Tag",
                table: "Spots");

            migrationBuilder.DropColumn(
                name: "TvdbId",
                table: "Spots");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Spots");
        }
    }
}
