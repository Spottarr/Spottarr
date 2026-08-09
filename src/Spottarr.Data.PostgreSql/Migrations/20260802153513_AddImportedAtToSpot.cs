using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spottarr.Data.PostgreSql.Migrations
{
    /// <summary>
    /// Adds the reimport timestamp. Existing spots are stamped by
    /// <see cref="ConvergePostgresSchema"/> so that the value is written by the table rewrite that
    /// migration has to perform anyway, instead of by an update of every row.
    /// </summary>
    /// <remarks>
    /// The released 1.20.0-beta.1 body of this migration stamped the spots itself. Databases that
    /// applied it keep those values; the convergence migration only fills the ones still unset.
    /// </remarks>
    public partial class AddImportedAtToSpot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ImportedAt",
                table: "Spots",
                type: "timestamp with time zone",
                nullable: true
            );

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
