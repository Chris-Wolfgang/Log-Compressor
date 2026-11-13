# Copilot Coding Agent Instructions

## Repository Summary

**Log-Compressor** is a cross-platform .NET command-line tool (`logc`) for compressing log files. It targets long-running log directories where individual files should be archived (and removed) once they age past a threshold, and supports per-file archives, bundled archives, extraction, and starter configuration files.

- **Repository Type:** Application (CLI tool); nothing is published to NuGet
- **Target Framework:** `net10.0`
- **Primary Language:** C#
- **Distribution:** Self-contained, trimmed single-file executables (`win-x64`, `linux-x64`, `osx-x64`) attached to GitHub Releases
- **Commands:** `compress`, `bundle`, `decompress`, `init compress` / `init bundle`
- **Status:** Released from `main` (see `CHANGELOG.md` for the current version). Base all work on `main`.

## Build and Validation Instructions

### Prerequisites
- .NET SDK 10.0+
- Repository-pinned tools: run `dotnet tool restore` (versions are pinned in `.config/dotnet-tools.json`; do not install them globally)
- PowerShell 7+ for the scripts under `scripts/`

### Build Process

The one-command local gate mirrors CI (build, all tests, coverage gate, DevSkim, gitleaks):

```powershell
pwsh ./scripts/build-pr.ps1
```

Individual steps, if you need them:

1. **Restore and build (Release):**
   ```powershell
   dotnet restore
   dotnet build --no-restore --configuration Release
   ```

2. **Run tests:**
   ```powershell
   dotnet test --configuration Release
   ```

3. **Publish a self-contained, trimmed executable:**
   ```powershell
   dotnet publish src/Wolfgang.LogCompressor -c Release -r linux-x64 --self-contained
   ```
   The output lands in `src/Wolfgang.LogCompressor/bin/Release/net10.0/<rid>/publish/` as `logc` plus `AppSettings.json` (keep them together).

4. **Smoke-test a published binary** (the release gate for trimmed binaries, see ADR-0006):
   ```powershell
   pwsh ./scripts/smoke-published.ps1 -Exe <path to the published logc executable>
   ```

### Critical Build Requirements
- **Code Coverage:** at least 95% line coverage for `src/` and 100% for test assemblies (enforced by `build-pr.ps1` and CI). Do not lower the gate.
- **Changelog fragments:** every PR that changes `src/` adds one fragment under `changelog/unreleased/` (see its `README.md`); `CHANGELOG.md` is assembled at release time.
- **Warnings are errors.** Fix analyzer findings rather than suppressing them.
- **Test framework:** xunit 2.9.3 with NSubstitute; property-based tests use CsCheck.
- **Benchmarks:** BenchmarkDotNet under `benchmarks/`.

## Project Layout and Architecture

### Directory Structure
```
root/
├── src/
│   └── Wolfgang.LogCompressor/                 # CLI entry point + compression engine
├── tests/
│   ├── Wolfgang.LogCompressor.Tests.Unit/        # unit tests (xunit, NSubstitute)
│   ├── Wolfgang.LogCompressor.Tests.Integration/ # real file system, all OSes
│   └── Wolfgang.LogCompressor.Tests.Fuzz/        # property-based tests (CsCheck)
├── benchmarks/                                 # BenchmarkDotNet projects
├── changelog/unreleased/                       # changelog fragments
├── docfx_project/                              # DocFX documentation source
├── docs/                                       # hand-written docs and ADRs (docs/adr/)
├── scripts/                                    # build-pr.ps1, smoke-published.ps1, changelog.ps1, ...
└── .github/                                    # workflows, issue/PR templates, Dependabot
```

### Key Configuration Files
- **`.editorconfig`** — Code style rules and analyzer severities.
- **`Directory.Build.props` / `Directory.Build.targets`** — Shared MSBuild properties and analyzer references.
- **`BannedSymbols.txt`** — Banned APIs (e.g. `Task.Wait`, `Thread.Sleep`, sync I/O — use async equivalents).
- **`.config/dotnet-tools.json`** — Pinned local tools.
- **`stryker-config.json`** / **`docs/stryker-baseline.json`** — Mutation-testing configuration and score floor.

These, the workflows and the CI scripts are **protected files**: `protected-files.yaml` fails a PR that mixes them with other changes. Put changes to them in their own PR.

### GitHub Integration
- **Workflows** (`.github/workflows/`): `pr.yaml` (PR Checks v3, gated stages), `protected-files.yaml`, `release.yaml` (publishes the per-RID binaries, attestations and SBOM on a published release), `docfx.yaml` / `build-all-versions.yaml` (docs), `codeql.yaml`, `semgrep.yaml`, `scorecard.yaml`, `stryker.yaml`, `fuzz.yaml`, `shadow.yaml`, `reproducible-build.yaml`, `cross-platform-differential.yaml`, `sourcelink.yaml`, `sbom.yaml`, `license-audit.yaml`, `actions-audit.yaml`, `benchmarks.yaml` / `pr-benchmarks.yaml`, `security-alerts.yml`.
- **Issue/PR Templates:** Bug report, feature request and maintenance task forms (all YAML); structured PR template.
- **CODEOWNERS:** `.github/CODEOWNERS` assigns `@Chris-Wolfgang` as the owner of everything.
- **Dependabot:** NuGet, GitHub Actions and the pinned Python requirements.

### Continuous Integration Pipeline (`.github/workflows/pr.yaml`)
Runs on `pull_request` to `main` (and on push to `main`), so a PR's own copy of the workflow is what runs:

1. **Secrets scan** (gitleaks), **Detect .NET Projects**, **Changelog Fragment Check**, **ReSharper InspectCode**.
2. **Stage 1 — Linux:** tests with the coverage gate.
3. **Stage 2 — Windows** (gated by Stage 1).
4. **Stage 3 — macOS** (gated by Stage 2).
5. **Security Scan** (DevSkim) and **Published binary smoke** (`scripts/smoke-published.ps1` against the trimmed binary for each RID).

Tests run on the frameworks the test projects target (`net10.0`).

### Branch Protection
The ruleset on `main` requires the PR checks to pass and conversations to be resolved, and forbids force-push and deletion.

## Agent Guidelines

### Trust These Instructions First
This file describes Log-Compressor's actual structure and conventions. **Only search for additional information if these instructions are incomplete or appear incorrect.** When in doubt, the in-tree `README.md` on `main` is the project's source of truth.

### When Working on This Project
1. **Code style:** Follow `.editorconfig`. File-scoped namespaces, Allman braces, `var` for obvious types, async-first APIs.
2. **Adding dependencies:** Use `dotnet add package`. Avoid pinning `Microsoft.Bcl.*` independently — they cascade. New dependencies must be trim-safe (ADR-0006) and must not add a source generator (the cross-OS reproducible build relies on there being none).
3. **Banned APIs:** Don't introduce calls flagged by `BannedSymbols.txt`.
4. **Testing:** Add tests to the matching project under `tests/`. Coverage must stay at the gate.
5. **Changelog:** Add a fragment under `changelog/unreleased/` for any `src/` change.
6. **Security:** If DevSkim, CodeQL or Semgrep flag new findings, fix them (do not use inline suppressions).

### Validation Steps Before Submitting Changes
1. `pwsh ./scripts/build-pr.ps1` passes.
2. For changes that affect the published binary, publish and run `scripts/smoke-published.ps1`.
3. Ensure all GitHub Actions checks pass.
