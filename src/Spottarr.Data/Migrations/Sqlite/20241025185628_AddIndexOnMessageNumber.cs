#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Spottarr.Data.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class AddIndexOnMessageNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Spots_MessageNumber",
                table: "Spots",
                column: "MessageNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Spots_MessageNumber",
                table: "Spots");
        }
    }
}