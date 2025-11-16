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
                name: "data_protection_keys",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    friendly_name = table.Column<string>(type: "text", nullable: true),
                    xml = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_data_protection_keys", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "spots",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    release_title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    tag = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    url = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    filename = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    newsgroup = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    spotter = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    bytes = table.Column<long>(type: "bigint", nullable: false),
                    message_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    nzb_message_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    image_message_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    message_number = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    image_types = table.Column<int[]>(type: "integer[]", nullable: false),
                    image_formats = table.Column<int[]>(type: "integer[]", nullable: false),
                    image_sources = table.Column<int[]>(type: "integer[]", nullable: false),
                    image_languages = table.Column<int[]>(type: "integer[]", nullable: false),
                    image_genres = table.Column<int[]>(type: "integer[]", nullable: false),
                    audio_types = table.Column<int[]>(type: "integer[]", nullable: false),
                    audio_formats = table.Column<int[]>(type: "integer[]", nullable: false),
                    audio_sources = table.Column<int[]>(type: "integer[]", nullable: false),
                    audio_bitrates = table.Column<int[]>(type: "integer[]", nullable: false),
                    audio_genres = table.Column<int[]>(type: "integer[]", nullable: false),
                    game_platforms = table.Column<int[]>(type: "integer[]", nullable: false),
                    game_formats = table.Column<int[]>(type: "integer[]", nullable: false),
                    game_genres = table.Column<int[]>(type: "integer[]", nullable: false),
                    game_types = table.Column<int[]>(type: "integer[]", nullable: false),
                    application_platforms = table.Column<int[]>(type: "integer[]", nullable: false),
                    application_genres = table.Column<int[]>(type: "integer[]", nullable: false),
                    application_types = table.Column<int[]>(type: "integer[]", nullable: false),
                    newznab_categories = table.Column<int[]>(type: "integer[]", nullable: false),
                    years = table.Column<int[]>(type: "integer[]", nullable: false),
                    seasons = table.Column<int[]>(type: "integer[]", nullable: false),
                    episodes = table.Column<int[]>(type: "integer[]", nullable: false),
                    imdb_id = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    tvdb_id = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    spotted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    indexed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_spots", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "fts_spots",
                columns: table => new
                {
                    spot_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    search_vector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "dutch")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "title", "description" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fts_spots", x => x.spot_id);
                    table.ForeignKey(
                        name: "fk_fts_spots_spots_spot_id",
                        column: x => x.spot_id,
                        principalTable: "spots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_fts_spots_search_vector",
                table: "fts_spots",
                column: "search_vector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_spots_imdb_id_spotted_at",
                table: "spots",
                columns: new[] { "imdb_id", "spotted_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "ix_spots_message_id",
                table: "spots",
                column: "message_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_spots_message_number",
                table: "spots",
                column: "message_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_spots_spotted_at",
                table: "spots",
                column: "spotted_at",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "ix_spots_tvdb_id_spotted_at",
                table: "spots",
                columns: new[] { "tvdb_id", "spotted_at" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "data_protection_keys");

            migrationBuilder.DropTable(
                name: "fts_spots");

            migrationBuilder.DropTable(
                name: "spots");
        }
    }
}
