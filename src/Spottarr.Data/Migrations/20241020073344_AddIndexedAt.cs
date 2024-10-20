using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spottarr.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "IndexedAt",
                table: "Spots",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IndexedAt",
                table: "Spots");
        }
    }
}
