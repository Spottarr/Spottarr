using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spottarr.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImportedAtToSpot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ImportedAt",
                table: "Spots",
                type: "TEXT",
                nullable: true
            );

            // An unset ImportedAt marks a spot for a reimport, so existing spots are stamped as read.
            migrationBuilder.Sql("UPDATE Spots SET ImportedAt = CreatedAt;");

            migrationBuilder.CreateIndex(
                name: "IX_Spots_ImportedAt",
                table: "Spots",
                column: "ImportedAt",
                filter: "\"ImportedAt\" IS NULL"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Spots_IndexedAt",
                table: "Spots",
                column: "IndexedAt",
                filter: "\"IndexedAt\" IS NULL"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Spots_ImportedAt", table: "Spots");

            migrationBuilder.DropIndex(name: "IX_Spots_IndexedAt", table: "Spots");

            migrationBuilder.DropColumn(name: "ImportedAt", table: "Spots");
        }
    }
}
