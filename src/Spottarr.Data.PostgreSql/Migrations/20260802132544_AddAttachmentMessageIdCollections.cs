using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spottarr.Data.PostgreSql.Migrations
{
    /// <summary>
    /// Adds the message id collections. Adding a column with a constant default only touches the
    /// catalog, so this stays instant on a large table. Filling the collections from the single
    /// message id columns is left to <see cref="ConvergePostgresSchema"/>, which rewrites the table
    /// once for every schema change that needs one.
    /// </summary>
    /// <remarks>
    /// The released 1.19.0 body of this migration also filled and dropped the single message id
    /// columns. Databases that applied it never run this body again; they are converged later.
    /// </remarks>
    public partial class AddAttachmentMessageIdCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "ImageMessageIds",
                table: "Spots",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]
            );

            migrationBuilder.AddColumn<string[]>(
                name: "NzbMessageIds",
                table: "Spots",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ImageMessageIds", table: "Spots");

            migrationBuilder.DropColumn(name: "NzbMessageIds", table: "Spots");
        }
    }
}
