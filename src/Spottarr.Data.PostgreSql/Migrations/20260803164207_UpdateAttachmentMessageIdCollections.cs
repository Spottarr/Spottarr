using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spottarr.Data.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAttachmentMessageIdCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
