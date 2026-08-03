using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spottarr.Data.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class DropAttachmentMessageIdSingle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ImageMessageId", table: "Spots");

            migrationBuilder.DropColumn(name: "NzbMessageId", table: "Spots");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageMessageId",
                table: "Spots",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "NzbMessageId",
                table: "Spots",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true
            );
        }
    }
}
