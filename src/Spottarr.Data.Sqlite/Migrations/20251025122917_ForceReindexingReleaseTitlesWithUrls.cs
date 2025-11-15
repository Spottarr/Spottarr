using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spottarr.Data.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class ForceReindexingReleaseTitlesWithUrls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Spots SET IndexedAt = NULL WHERE ReleaseTitle LIKE 'www.%'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
