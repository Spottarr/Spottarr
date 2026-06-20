# CLAUDE.md

Guidance for Claude Code (and any other AI agent or human) working in this repository.

If you are adding documentation, prefer extending the files under `docs/` and updating the index below. Use [Mermaid](https://mermaid.js.org/) for any diagrams or charts — GitHub renders them natively. Do not draw diagrams as ASCII art.

## Key conventions

- **Use idiomatic C# with the latest language features.**

## CI

`.github/workflows/build-and-test.yml` (on push/PR to `main`, .NET 10): `dotnet tool restore` → `dotnet restore` → CSharpier check (`dotnet csharpier check .`) → `dotnet build` → CodeQL analyze (public repos only) → `dotnet test` with cobertura coverage. Test results and coverage are posted as sticky PR comments. A separate scheduled `codeql.yml` re-runs CodeQL weekly (Wednesdays, public repos only).

`.github/workflows/docker.yml` (on push to `main` and `v*` tags): builds the `src/Spottarr.Web/Dockerfile` image for `linux/amd64` and `linux/arm64`, pushes each platform by digest, then merges them into a multi-arch manifest published to Docker Hub (`spottarr/spottarr`) and GHCR (`ghcr.io/spottarr/spottarr`). Tag builds set `BUILD_VERSION` from the ref name.

## Notes for AI agents

- **Always run CSharpier** (`dotnet csharpier format .`) after writing C# — CI fails otherwise and `TreatWarningsAsErrors=true` will catch a lot too.
- **Don't pin package versions in `.csproj`** — add or update the `PackageVersion` entry in `Directory.Packages.props`.
- **Don't suppress warnings.** Don't use #pragma or SuppressMessageAttribute. Only use .editorconfig and only for global rules. Always try to fix the issue in code first.
- **Make sure to create individual commits that make sense**.
- **Only build/test when neccesary**, these can be long running operations.
- Prefer running targeted tests instead of the full suite.
- Submit changes as a PR when done.

## Agent skills

### Issue tracker

Issues live in GitHub Issues (`Spottarr/Spottarr`), managed via the `gh` CLI. External PRs are not a triage surface. See `docs/agents/issue-tracker.md`.

### Triage labels

The five canonical triage roles use their default label strings (`needs-triage`, `needs-info`, `ready-for-agent`, `ready-for-human`, `wontfix`). See `docs/agents/triage-labels.md`.

### Domain docs

Single-context: one `CONTEXT.md` + `docs/adr/` at the repo root. See `docs/agents/domain.md`.
