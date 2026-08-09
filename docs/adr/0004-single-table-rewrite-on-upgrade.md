# The Postgres upgrade pays for exactly one table rewrite

Converting `NzbMessageIds`/`ImageMessageIds` from `character varying(128)[]` to `text[]` cannot avoid a
table rewrite, because that cast is not binary coercible. Rather than adding a second full pass to
backfill `ImportedAt`, both are folded into a single `ALTER TABLE` — the type changes plus
`ALTER COLUMN "ImportedAt" TYPE timestamptz USING COALESCE("ImportedAt", "CreatedAt")` — which
Postgres executes as one rewrite pass that also compacts the dead tuples left behind by the failed
1.19.0 and 1.20.0-beta.1 migrations and physically reclaims the dropped search vector column.

The rewrite runs only after the search vector has moved to `FtsSpots`, so it carries no tsvector and
rebuilds no GIN index.

## Consequences

- The upgrade needs free disk roughly equal to the current size of `Spots`, and takes an
  `ACCESS EXCLUSIVE` lock for its duration.
- It is all or nothing: interrupting it rolls back and the next start redoes it. This is preferred
  over a resumable batched copy, which would write a new row version per spot and leave the table
  more bloated than it started.
- The array columns are `text[]` because Npgsql binds `IList<string>` parameters as `text[]` whatever
  the column type says, so `.ElementType(e => e.HasMaxLength(...))` must not be used on them.
