using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace Spottarr.Data.PostgreSql.Migrations
{
    /// <summary>
    /// Brings every database released since 1.18.1 to the same schema.
    /// </summary>
    /// <remarks>
    /// The releases in between applied different bodies of the migrations before this one, so this is
    /// the only migration that inspects the database it runs against. It moves the search vector out
    /// of the spots table first, so that the table rewrite it then performs carries no tsvector and
    /// rebuilds no GIN index.
    /// </remarks>
    public partial class ConvergePostgresSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Reverted when this migration's transaction ends.
            migrationBuilder.Sql("SET LOCAL maintenance_work_mem = '128MB';");

            migrationBuilder.CreateTable(
                name: "FtsSpots",
                columns: table => new
                {
                    SpotId = table.Column<int>(type: "integer", nullable: false),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FtsSpots", x => x.SpotId);
                    table.ForeignKey(
                        name: "FK_FtsSpots_Spots_SpotId",
                        column: x => x.SpotId,
                        principalTable: "Spots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            // Copies the vectors instead of recomputing them, which would mean running the text
            // search configuration over every stored description again.
            migrationBuilder.Sql(
                """
                INSERT INTO "FtsSpots" ("SpotId", "SearchVector")
                SELECT "Id", "SearchVector" FROM "Spots";
                """
            );

            migrationBuilder
                .CreateIndex(
                    name: "IX_FtsSpots_SearchVector",
                    table: "FtsSpots",
                    column: "SearchVector"
                )
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.DropIndex(name: "IX_Spots_SearchVector", table: "Spots");

            migrationBuilder.DropColumn(name: "SearchVector", table: "Spots");

            // A single ALTER TABLE, so Postgres rewrites the table once. The rewrite is unavoidable
            // because character varying(128)[] is not binary coercible to text[], and it also drops
            // the dead rows left behind by the migrations of 1.19.0 and 1.20.0-beta.1.
            migrationBuilder.Sql(
                """
                DO $$
                DECLARE
                    changes text[] := '{}';
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM pg_attribute
                        WHERE attrelid = '"Spots"'::regclass
                          AND attname = 'NzbMessageId'
                          AND NOT attisdropped
                    ) THEN
                        -- Never filled the collections: the single message id columns are still there.
                        changes := changes || ARRAY[
                            'ALTER COLUMN "NzbMessageIds" TYPE text[] USING (CASE WHEN "NzbMessageId" > '''' THEN ARRAY["NzbMessageId"] ELSE ''{}''::text[] END)',
                            'ALTER COLUMN "ImageMessageIds" TYPE text[] USING (CASE WHEN "ImageMessageId" > '''' THEN ARRAY["ImageMessageId"] ELSE ''{}''::text[] END)'
                        ];
                    ELSE
                        -- Already filled, but as character varying(128)[], which Npgsql cannot write.
                        changes := changes || ARRAY[
                            'ALTER COLUMN "NzbMessageIds" TYPE text[]',
                            'ALTER COLUMN "ImageMessageIds" TYPE text[]'
                        ];
                    END IF;

                    -- An unset ImportedAt marks a spot for a reimport, so existing spots are stamped
                    -- as imported. Spots stamped by 1.20.0-beta.1 keep their value.
                    changes := changes || 'ALTER COLUMN "ImportedAt" TYPE timestamp with time zone USING COALESCE("ImportedAt", "CreatedAt")';

                    EXECUTE 'ALTER TABLE "Spots" ' || array_to_string(changes, ', ');
                END $$;
                """
            );

            migrationBuilder.Sql(
                """
                ALTER TABLE "Spots"
                    DROP COLUMN IF EXISTS "NzbMessageId",
                    DROP COLUMN IF EXISTS "ImageMessageId";
                """
            );
        }

        /// <summary>
        /// Restores the schema, not the data: spots keep the ImportedAt they were stamped with and the
        /// single message id columns are not brought back.
        /// </summary>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "FtsSpots");

            migrationBuilder.AlterColumn<string[]>(
                name: "NzbMessageIds",
                table: "Spots",
                type: "character varying(128)[]",
                nullable: false,
                oldClrType: typeof(string[]),
                oldType: "text[]"
            );

            migrationBuilder.AlterColumn<string[]>(
                name: "ImageMessageIds",
                table: "Spots",
                type: "character varying(128)[]",
                nullable: false,
                oldClrType: typeof(string[]),
                oldType: "text[]"
            );

            migrationBuilder
                .AddColumn<NpgsqlTsVector>(
                    name: "SearchVector",
                    table: "Spots",
                    type: "tsvector",
                    nullable: false
                )
                .Annotation("Npgsql:TsVectorConfig", "dutch")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Title", "Description" });

            migrationBuilder
                .CreateIndex(name: "IX_Spots_SearchVector", table: "Spots", column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }
    }
}
