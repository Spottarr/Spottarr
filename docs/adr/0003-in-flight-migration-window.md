# Migrations released since 1.18.1 are rewritten as an in-flight window

The Postgres migrations added after 1.18.1 could not complete on large databases, and one of them was
already rewritten in place, leaving `main` unable to upgrade a database that had successfully applied
the 1.19.0 version. Because EF applies pending migrations in migration-id order, a correction added
today would run *after* the migrations that break the database, so the broken ones are rewritten in
place instead, followed by a single guarded convergence migration for databases that already applied
an earlier variant.

## The rule this relies on

A released migration may be rewritten only when, for a database that has *not* applied it, the new body
converges to the same schema the old body produced — and no later migration may assume which variant
ran. `main` violated this: a follow-up migration referenced columns the released body had already
dropped.

## Consequences

- Databases upgraded with the `:main` development tag are out of scope; a partially applied,
  unrecorded batched backfill cannot be detected reliably.
- The convergence migration uses conditional DDL, which is deliberately confined to that one migration.
