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

            // Spots imported before this migration only retained the last <Segment>, so multi-segment
            // attachments remain undecodable until those spots are imported again.
            migrationBuilder.Sql(
                """
                UPDATE "Spots"
                SET "NzbMessageIds" = ARRAY["NzbMessageId"]
                WHERE "NzbMessageId" IS NOT NULL AND "NzbMessageId" <> '';
                """
            );

            migrationBuilder.Sql(
                """
                UPDATE "Spots"
                SET "ImageMessageIds" = ARRAY["ImageMessageId"]
                WHERE "ImageMessageId" IS NOT NULL AND "ImageMessageId" <> '';
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

            migrationBuilder.Sql(
                """
                UPDATE "Spots"
                SET "NzbMessageId" = "NzbMessageIds"[1]
                WHERE array_length("NzbMessageIds", 1) > 0;
                """
            );

            migrationBuilder.Sql(
                """
                UPDATE "Spots"
                SET "ImageMessageId" = "ImageMessageIds"[1]
                WHERE array_length("ImageMessageIds", 1) > 0;
                """
            );

            migrationBuilder.DropColumn(name: "ImageMessageIds", table: "Spots");

            migrationBuilder.DropColumn(name: "NzbMessageIds", table: "Spots");
        }
    }
}
