using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spottarr.Data.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAttachmentMessageIdCollections : Migration
    {
        private const int BatchSize = 50000;

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Spots imported before this migration only retained the last <Segment>, so multi-segment
            // attachments remain undecodable until those spots are imported again.
            Backfill(
                migrationBuilder,
                """
                UPDATE "Spots"
                SET "NzbMessageIds" = CASE
                        WHEN "NzbMessageId" > '' AND "NzbMessageIds" = '{}' THEN ARRAY["NzbMessageId"]
                        ELSE "NzbMessageIds"
                    END,
                    "ImageMessageIds" = CASE
                        WHEN "ImageMessageId" > '' AND "ImageMessageIds" = '{}' THEN ARRAY["ImageMessageId"]
                        ELSE "ImageMessageIds"
                    END
                WHERE "Id" >= lo AND "Id" < lo + batch_size
                  AND (("NzbMessageId" > '' AND "NzbMessageIds" = '{}')
                    OR ("ImageMessageId" > '' AND "ImageMessageIds" = '{}'));
                """
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            Backfill(
                migrationBuilder,
                """
                UPDATE "Spots"
                SET "NzbMessageId" = "NzbMessageIds"[1],
                    "ImageMessageId" = "ImageMessageIds"[1],
                    "NzbMessageIds" = '{}',
                    "ImageMessageIds" = '{}'
                WHERE "Id" >= lo AND "Id" < lo + batch_size
                  AND (cardinality("NzbMessageIds") > 0 OR cardinality("ImageMessageIds") > 0);
                """
            );
        }

        /// <summary>
        /// Runs <paramref name="statement"/> in committed batches outside the migration transaction.
        /// A full table scan rewrites every row, which recomputes the stored tsvector column and its
        /// GIN index entries, so a single statement is too slow to complete within a command timeout.
        /// Each statement needs its own command because Npgsql wraps multi-statement batches in an
        /// implicit transaction, which forbids the COMMIT inside the procedure.
        /// </summary>
        private static void Backfill(MigrationBuilder migrationBuilder, string statement)
        {
            migrationBuilder.Sql(
                "SET statement_timeout = 0; SET lock_timeout = '30s'; SET synchronous_commit = off;",
                suppressTransaction: true
            );

            migrationBuilder.Sql(
                $"""
                CREATE PROCEDURE pg_temp.spottarr_backfill_message_ids() LANGUAGE plpgsql AS $proc$
                DECLARE
                    batch_size constant int := {BatchSize};
                    lo int := 0;
                    hi int;
                BEGIN
                    SELECT COALESCE(MAX("Id"), -1) INTO hi FROM "Spots";

                    WHILE lo <= hi LOOP
                        {statement}
                        COMMIT;
                        lo := lo + batch_size;
                    END LOOP;
                END $proc$;
                """,
                suppressTransaction: true
            );

            migrationBuilder.Sql(
                "CALL pg_temp.spottarr_backfill_message_ids();",
                suppressTransaction: true
            );

            migrationBuilder.Sql(
                "DROP PROCEDURE pg_temp.spottarr_backfill_message_ids();",
                suppressTransaction: true
            );

            migrationBuilder.Sql(
                "RESET statement_timeout; RESET lock_timeout; RESET synchronous_commit;",
                suppressTransaction: true
            );
        }
    }
}
