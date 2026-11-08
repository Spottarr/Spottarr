# A term is a run of alphanumerics, everywhere

Searching for `Gooische Vrouwen` did not find `Gooische.Vrouwen.S08E02.DUTCH.1080p.WEB.h264-TRIPEL` on
Postgres, because the default text search parser folds a dotted release title into a single `host`
token that no keyword can match. One rule now governs all three tokenizers: a term is a run of
alphanumeric characters. SQLite's `unicode61` already works that way, the Postgres vector expression
is made to obey it by replacing non-alphanumeric runs with spaces before `to_tsvector`, and
`QueryExclusionParser` emits a term per run instead of per whitespace-delimited word.

## Considered options

- **A custom text search parser.** The only way to make Postgres itself split the token, but it is a C
  extension, which rules it out for the managed and NAS-class installs Spottarr targets.
- **Dropping the `host`/`url`/`email` mappings from the text search configuration.** Unmapped tokens
  are discarded, not re-split, so the title would become unsearchable instead of mis-searchable.
- **Replacing only `.` and `/`.** Enough for the reported titles and ~25% cheaper, but it leaves the
  index rule different from what the query parser can emit, which is the drift this ADR exists to
  prevent.
- **Normalizing in .NET and storing the result.** `Title` is the verbatim Usenet subject that
  `ReleaseTitleParser` parses dots out of and that the Newznab response ships to clients, so it cannot
  be normalized in place, and ADR 0002 keeps the vector out of .NET anyway.

## Consequences

- Compound tokens Postgres would otherwise keep (`h264-tripel`, `www.example.com`,
  `user@example.com`) are no longer indexed. Nothing is lost: the query parser cannot produce them.
- `_` is a word character for the query parser but not for `[:alnum:]`, so `Show_Name` is one term and
  two lexemes. `to_tsquery` turns it into the phrase query `'show' <-> 'nam'`, which matches because
  normalization leaves those lexemes adjacent. Changing either side independently breaks this.
- Existing Postgres databases are corrected with `POST /api/spots/reindex`, not with a migration, per
  ADR 0002. The fix is therefore opt-in per install: a database that is never reindexed keeps the
  broken vectors.
- Reversing the rule means reindexing every spot again, on every install.
