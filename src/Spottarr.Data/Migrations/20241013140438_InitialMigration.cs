using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spottarr.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Spots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Spotter = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Bytes = table.Column<long>(type: "INTEGER", nullable: false),
                    MessageId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    MessageNumber = table.Column<long>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    SpottedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ApplicationSpot_Platforms = table.Column<string>(type: "TEXT", nullable: true),
                    ApplicationSpot_Genres = table.Column<string>(type: "TEXT", nullable: true),
                    AudioSpot_Types = table.Column<string>(type: "TEXT", nullable: true),
                    AudioSpot_Formats = table.Column<string>(type: "TEXT", nullable: true),
                    AudioSpot_Sources = table.Column<string>(type: "TEXT", nullable: true),
                    Bitrates = table.Column<string>(type: "TEXT", nullable: true),
                    AudioSpot_Genres = table.Column<string>(type: "TEXT", nullable: true),
                    GameSpot_Genres = table.Column<string>(type: "TEXT", nullable: true),
                    Platforms = table.Column<string>(type: "TEXT", nullable: true),
                    Types = table.Column<string>(type: "TEXT", nullable: true),
                    Formats = table.Column<string>(type: "TEXT", nullable: true),
                    Sources = table.Column<string>(type: "TEXT", nullable: true),
                    Languages = table.Column<string>(type: "TEXT", nullable: true),
                    Genres = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spots", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Spots");
        }
    }
}
