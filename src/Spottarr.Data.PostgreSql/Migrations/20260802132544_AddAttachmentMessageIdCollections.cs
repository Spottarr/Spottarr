using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spottarr.Data.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentMessageIdCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "ImageMessageIds",
                table: "Spots",
                type: "character varying(128)[]",
                nullable: false,
                defaultValue: new string[0]
            );

            migrationBuilder.AddColumn<string[]>(
                name: "NzbMessageIds",
                table: "Spots",
                type: "character varying(128)[]",
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
