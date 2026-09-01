# ADR-0001: Distribute self-contained per-RID binaries, not a NuGet package

- **Status:** Accepted
- **Date:** 2026-08-29 (decision originally made during initial development; recorded retroactively)

## Context

`logc` is an end-user CLI tool whose primary use case is unattended scheduled
jobs on servers. The sibling Wolfgang.* repos are class libraries and publish
to NuGet; the shared `release.yaml` pipeline is built around that path. A CLI
could also ship as a `dotnet tool` NuGet package, but that requires the .NET
SDK/runtime on every target machine — precisely what a server cron/Task
Scheduler box often does not have.

## Decision

We ship `logc` as **self-contained single-file executables, one per runtime
identifier (win-x64, linux-x64, osx-x64), attached to the GitHub Release**.
The csproj sets `<IsPackable>false</IsPackable>`, which makes the shared
`release.yaml` NuGet path (pack → smoke-test → publish) no-op gracefully via
its `has-packages` gate, while the `publish-binaries` job cross-compiles all
three RIDs from a single Linux runner.

## Consequences

- Target machines need no .NET installation; a copied binary just runs.
- Fleet-wide maintenance issues written for NuGet libraries do not all apply
  here: ApiCompat/ABI diffs, SourceLink consumer step-into verification,
  NuGet Trusted Publishing (OIDC), and package signing have no NuGet package
  to act on. Supply-chain work (SBOM, provenance, reproducible builds) applies
  to the release binaries instead.
- Each release carries ~3 platform archives instead of one package; download
  size per asset is larger (the runtime is bundled).
- There is no ABI/public-API compatibility surface — compatibility promises
  attach to the CLI contract (flags, exit codes, output formats) instead. See
  the migration-guide template in `docs/migrations/`.
