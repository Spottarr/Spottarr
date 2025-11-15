#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Spottarr.Data.Migrations.Sqlite
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
                    Title = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Spotter = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Bytes = table.Column<long>(type: "INTEGER", nullable: false),
                    MessageId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    MessageNumber = table.Column<long>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageTypes = table.Column<string>(type: "TEXT", nullable: false),
                    ImageFormats = table.Column<string>(type: "TEXT", nullable: false),
                    ImageSources = table.Column<string>(type: "TEXT", nullable: false),
                    ImageLanguages = table.Column<string>(type: "TEXT", nullable: false),
                    ImageGenres = table.Column<string>(type: "TEXT", nullable: false),
                    AudioTypes = table.Column<string>(type: "TEXT", nullable: false),
                    AudioFormats = table.Column<string>(type: "TEXT", nullable: false),
                    AudioSources = table.Column<string>(type: "TEXT", nullable: false),
                    AudioBitrates = table.Column<string>(type: "TEXT", nullable: false),
                    AudioGenres = table.Column<string>(type: "TEXT", nullable: false),
                    GamePlatforms = table.Column<string>(type: "TEXT", nullable: false),
                    GameFormats = table.Column<string>(type: "TEXT", nullable: false),
                    GameGenres = table.Column<string>(type: "TEXT", nullable: false),
                    GameTypes = table.Column<string>(type: "TEXT", nullable: false),
                    ApplicationPlatforms = table.Column<string>(type: "TEXT", nullable: false),
                    ApplicationGenres = table.Column<string>(type: "TEXT", nullable: false),
                    ApplicationTypes = table.Column<string>(type: "TEXT", nullable: false),
                    Years = table.Column<string>(type: "TEXT", nullable: false),
                    Seasons = table.Column<string>(type: "TEXT", nullable: false),
                    Episodes = table.Column<string>(type: "TEXT", nullable: false),
                    SpottedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IndexedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Spots", x => x.Id); });

            // See: https://www.bricelam.net/2020/08/08/sqlite-fts-and-efcore.html
            /*
            migrationBuilder.CreateTable(
                name: "FtsSpots",
                columns: table => new
                {
                    RowId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    FtsSpots = table.Column<string>(type: "TEXT", nullable: true),
                    Rank = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FtsSpots", x => x.RowId);
                    table.ForeignKey(
                        name: "FK_FtsSpots_Spots_RowId",
                        column: x => x.RowId,
                        principalTable: "Spots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            */
            migrationBuilder.Sql("CREATE VIRTUAL TABLE FtsSpots USING fts5(Title, Description)");

            migrationBuilder.CreateIndex(
                name: "IX_Spots_MessageId",
                table: "Spots",
                column: "MessageId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FtsSpots");

            migrationBuilder.DropTable(
                name: "Spots");
        }
    }
}