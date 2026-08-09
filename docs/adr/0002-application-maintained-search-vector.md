# The Postgres search vector is maintained by the application

`FtsSpots.SearchVector` is a plain `tsvector` column that the import, reimport and reindex paths write,
rather than a `GENERATED ALWAYS` column. Generating it would require `FtsSpots` to hold its own copies
of `Title` and `Description`, because a Postgres generation expression can only reference columns of
its own row, which would double the stored text and force the upgrade to re-run Dutch stemming over
~1.3M documents instead of copying the vectors that already exist.

## Consequences

- The vector cannot be built in .NET (that would mean reimplementing the text search configuration), so
  `UpsertFtsSpotsAsync` computes it with `to_tsvector` in SQL for the Postgres provider.
- The vector can drift from `Title`/`Description`. This is the same exposure SQLite already has, and
  `SpotReindexService` is the repair path for it.
