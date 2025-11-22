using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

#nullable disable

namespace Spottarr.Data.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FriendlyName = table.Column<string>(type: "text", nullable: true),
                    Xml = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ReleaseTitle = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "character varying(32768)", maxLength: 32768, nullable: true),
                    Tag = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Url = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Filename = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Newsgroup = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Spotter = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Bytes = table.Column<long>(type: "bigint", nullable: false),
                    MessageId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    NzbMessageId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ImageMessageId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    MessageNumber = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    ImageTypes = table.Column<int[]>(type: "integer[]", nullable: false),
                    ImageFormats = table.Column<int[]>(type: "integer[]", nullable: false),
                    ImageSources = table.Column<int[]>(type: "integer[]", nullable: false),
                    ImageLanguages = table.Column<int[]>(type: "integer[]", nullable: false),
                    ImageGenres = table.Column<int[]>(type: "integer[]", nullable: false),
                    AudioTypes = table.Column<int[]>(type: "integer[]", nullable: false),
                    AudioFormats = table.Column<int[]>(type: "integer[]", nullable: false),
                    AudioSources = table.Column<int[]>(type: "integer[]", nullable: false),
                    AudioBitrates = table.Column<int[]>(type: "integer[]", nullable: false),
                    AudioGenres = table.Column<int[]>(type: "integer[]", nullable: false),
                    GamePlatforms = table.Column<int[]>(type: "integer[]", nullable: false),
                    GameFormats = table.Column<int[]>(type: "integer[]", nullable: false),
                    GameGenres = table.Column<int[]>(type: "integer[]", nullable: false),
                    GameTypes = table.Column<int[]>(type: "integer[]", nullable: false),
                    ApplicationPlatforms = table.Column<int[]>(type: "integer[]", nullable: false),
                    ApplicationGenres = table.Column<int[]>(type: "integer[]", nullable: false),
                    ApplicationTypes = table.Column<int[]>(type: "integer[]", nullable: false),
                    NewznabCategories = table.Column<int[]>(type: "integer[]", nullable: false),
                    Years = table.Column<int[]>(type: "integer[]", nullable: false),
                    Seasons = table.Column<int[]>(type: "integer[]", nullable: false),
                    Episodes = table.Column<int[]>(type: "integer[]", nullable: false),
                    ImdbId = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    TvdbId = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    SpottedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IndexedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "dutch")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "Title", "Description" }),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spots", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Spots_ImdbId_SpottedAt",
                table: "Spots",
                columns: new[] { "ImdbId", "SpottedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Spots_MessageId",
                table: "Spots",
                column: "MessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Spots_MessageNumber",
                table: "Spots",
                column: "MessageNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Spots_SearchVector",
                table: "Spots",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

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
            migrationBuilder.DropTable(
                name: "DataProtectionKeys");

            migrationBuilder.DropTable(
                name: "Spots");
        }
    }
}
