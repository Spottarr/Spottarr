using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spottarr.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentMessageIdCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageMessageIds",
                table: "Spots",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]"
            );

            migrationBuilder.AddColumn<string>(
                name: "NzbMessageIds",
                table: "Spots",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]"
            );

            // Spots imported before this migration only retained the last <Segment>, so multi-segment
            // attachments remain undecodable until those spots are imported again.
            migrationBuilder.Sql(
                """
                UPDATE Spots
                SET NzbMessageIds = json_array(NzbMessageId)
                WHERE NzbMessageId IS NOT NULL AND NzbMessageId <> '';
                """
            );

            migrationBuilder.Sql(
                """
                UPDATE Spots
                SET ImageMessageIds = json_array(ImageMessageId)
                WHERE ImageMessageId IS NOT NULL AND ImageMessageId <> '';
                """
            );

            migrationBuilder.DropColumn(name: "ImageMessageId", table: "Spots");

            migrationBuilder.DropColumn(name: "NzbMessageId", table: "Spots");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageMessageId",
                table: "Spots",
                type: "TEXT",
                maxLength: 128,
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "NzbMessageId",
                table: "Spots",
                type: "TEXT",
                maxLength: 128,
                nullable: true
            );

            migrationBuilder.Sql(
                """
                UPDATE Spots
                SET NzbMessageId = json_extract(NzbMessageIds, '$[0]')
                WHERE json_array_length(NzbMessageIds) > 0;
                """
            );

            migrationBuilder.Sql(
                """
                UPDATE Spots
                SET ImageMessageId = json_extract(ImageMessageIds, '$[0]')
                WHERE json_array_length(ImageMessageIds) > 0;
                """
            );

            migrationBuilder.DropColumn(name: "ImageMessageIds", table: "Spots");

            migrationBuilder.DropColumn(name: "NzbMessageIds", table: "Spots");
        }
    }
}
