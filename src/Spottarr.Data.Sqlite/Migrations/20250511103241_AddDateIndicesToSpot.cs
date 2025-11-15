using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spottarr.Data.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class AddDateIndicesToSpot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Spots_ImdbId",
                table: "Spots");

            migrationBuilder.DropIndex(
                name: "IX_Spots_MessageNumber",
                table: "Spots");

            migrationBuilder.DropIndex(
                name: "IX_Spots_TvdbId",
                table: "Spots");

            migrationBuilder.CreateIndex(
                name: "IX_Spots_ImdbId_SpottedAt",
                table: "Spots",
                columns: new[] { "ImdbId", "SpottedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Spots_MessageNumber",
                table: "Spots",
                column: "MessageNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Spots_SpottedAt",
                table: "Spots",
                column: "SpottedAt",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Spots_TvdbId_SpottedAt",
                table: "Spots",
                columns: new[] { "TvdbId", "SpottedAt" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Spots_ImdbId_SpottedAt",
                table: "Spots");

            migrationBuilder.DropIndex(
                name: "IX_Spots_MessageNumber",
                table: "Spots");

            migrationBuilder.DropIndex(
                name: "IX_Spots_SpottedAt",
                table: "Spots");

            migrationBuilder.DropIndex(
                name: "IX_Spots_TvdbId_SpottedAt",
                table: "Spots");

            migrationBuilder.CreateIndex(
                name: "IX_Spots_ImdbId",
                table: "Spots",
                column: "ImdbId");

            migrationBuilder.CreateIndex(
                name: "IX_Spots_MessageNumber",
                table: "Spots",
                column: "MessageNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Spots_TvdbId",
                table: "Spots",
                column: "TvdbId");
        }
    }
}
