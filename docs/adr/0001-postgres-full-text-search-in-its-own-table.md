# Postgres full text search lives in its own table

`Spots` carried a `GENERATED ALWAYS AS (to_tsvector(...)) STORED` column with a GIN index. Postgres
recomputes a stored generated column on every `UPDATE`, so touching any unrelated column re-TOASTs the
vector and inserts into the GIN posting list of every lexeme in the document, which made routine
updates over ~1.3M spots unaffordable on the NAS-class hardware most installs run on. The search
vector therefore moves to a separate `FtsSpots` table, mirroring the shape SQLite already uses, so an
update to a spot is physically incapable of touching the full text index.

## Considered options

- **Keep the generated column, rely on HOT updates.** Heap-only tuples skip index maintenance only
  when no indexed column changes *and* the new row version fits on the same page. A bulk update fills
  pages and loses that second condition, so the saving is probabilistic where a separate table is
  structural.
- **Replace the column with a GIN index on the `to_tsvector(...)` expression.** Cheaper to migrate,
  but the index still lives on `Spots`, so it has the same probabilistic behaviour.

## Consequences

- `FtsSpots.SearchVector` is a plain `tsvector` column, not a generated one: a generation expression
  may only reference columns of its own row, so generating it would mean duplicating `Title` and
  `Description` into `FtsSpots`. See ADR 0002.
- Search queries join `Spots` to `FtsSpots` instead of reading a column on `Spots`.
