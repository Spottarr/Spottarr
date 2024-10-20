using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spottarr.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFullTextSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
             See: https://www.bricelam.net/2020/08/08/sqlite-fts-and-efcore.html
            migrationBuilder.CreateTable(
                name: "FtsSpots",
                columns: table => new
                {
                    RowId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    FtsSpot = table.Column<string>(type: "TEXT", nullable: true),
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "FtsSpots");
        }
    }
}
