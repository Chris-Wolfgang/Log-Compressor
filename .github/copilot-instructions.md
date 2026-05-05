# Copilot Coding Agent Instructions

## Repository Summary

**Log-Compressor** is a cross-platform .NET command-line tool (`logc`) for compressing log files. It targets long-running log directories where individual files should be archived (and removed) once they age past a threshold, and supports both per-file and bundled archives.

- **Repository Type:** Application (CLI tool)
- **Target Framework:** `net10.0`
- **Primary Language:** C#
- **Distribution:** Self-contained single-file executable (`win-x64`, `linux-x64`, `osx-x64`)
- **Status:** v0.1.0 in active development. The source under `src/Wolfgang.LogCompressor/` currently lives on the [`initial-dev`](https://github.com/Chris-Wolfgang/Log-Compressor/tree/initial-dev) branch and has not yet merged to `main`. Until then, agents should base most code work on `initial-dev`.

## Build and Validation Instructions

### Prerequisites
- .NET SDK 10.0+ (CI matrix tests against .NET 5.0–10.0 and .NET Framework 4.6.2–4.8.1, but the project itself targets `net10.0`)
- ReportGenerator tool (`dotnet tool install -g dotnet-reportgenerator-globaltool`)
- DevSkim CLI (`dotnet tool install --global Microsoft.CST.DevSkim.CLI`)

### Build Process

> Run from `initial-dev` (or any branch where `src/Wolfgang.LogCompressor/` is present).

1. **Restore dependencies:**
   ```powershell
   dotnet restore
   ```

2. **Build (Release):**
   ```powershell
   dotnet build --no-restore --configuration Release
   ```

3. **Run tests with coverage:**
   ```powershell
   dotnet test --configuration Release --collect:"XPlat Code Coverage" --results-directory ./TestResults
   ```

4. **Generate coverage report:**
   ```powershell
   reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"CoverageReport" -reporttypes:"Html;TextSummary;MarkdownSummaryGithub;CsvSummary"
   ```

5. **Publish single-file executable:**
   ```powershell
   dotnet publish src/Wolfgang.LogCompressor -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true
   ```
   The output binary in `bin/Release/net10.0/<rid>/publish/` is a single self-contained file.

6. **Security scanning (optional locally; runs in CI):**
   ```powershell
   devskim analyze --source-code . -f text --output-file devskim-results.txt -E
   ```

### Critical Build Requirements
- **Code Coverage:** Minimum 90% line coverage. Current run is ~96.8%.
- **Security Scanning:** DevSkim must pass with no errors of `error`/`critical`/`high` severity.
- **Build Configuration:** Always use Release configuration for CI.
- **Test framework:** xunit 2.9.3 with NSubstitute for mocking. ~75+ unit tests today.
- **Benchmarks:** BenchmarkDotNet under `benchmarks/` (run manually; not gated by CI).

### Common Issues and Workarounds
- **Coverage Threshold Failures:** Below 90% blocks the merge by design. Add tests rather than lowering the gate.
- **DevSkim findings:** Review `devskim-results.txt`; suppress only with justification.
- **net10.0 SDK availability:** CI installs the required SDK via `actions/setup-dotnet`. Locally you must have it installed.

## Project Layout and Architecture

### Directory Structure
```
root/
├── src/
│   └── Wolfgang.LogCompressor/    # CLI entry point + compression engine
├── tests/                          # Unit tests (xunit 2.9.3, NSubstitute)
├── benchmarks/                     # BenchmarkDotNet projects (compression-format throughput)
├── docfx_project/                  # DocFX documentation source
├── docs/                           # Generated documentation output
└── .github/                        # GitHub configuration
```

### Key Configuration Files
- **`.editorconfig`** — Code style rules (file-scoped namespaces, var preferences, analyzer severity).
- **`.gitignore`** — Comprehensive .NET gitignore.
- **`Directory.Build.props`** — Shared MSBuild properties (analyzer references, common settings).
- **`BannedSymbols.txt`** — Banned APIs (e.g. `Task.Wait`, `Thread.Sleep`, sync I/O — use async equivalents).
- **`CONTRIBUTING.md`** — Contribution guidelines.
- **`CODE_OF_CONDUCT.md`** — Standard Contributor Covenant v2.0.

### GitHub Integration
- **Workflows:**
  - `.github/workflows/pr.yaml` — multi-stage PR validation (Linux → Windows → macOS, Stage-gated).
  - `.github/workflows/release.yaml` — release/publish pipeline.
  - `.github/workflows/docfx.yaml` — documentation build/deploy.
- **Issue/PR Templates:** Bug reports (YAML) and feature requests (Markdown); structured PR template.
- **CODEOWNERS:** `@Chris-Wolfgang`.
- **Dependabot:** Configured for NuGet packages.

### Continuous Integration Pipeline (`.github/workflows/pr.yaml`)
The workflow runs on `pull_request_target` to `main`:

1. **Stage 1 — Linux:** Tests on .NET 5.0–10.0 with 90% coverage gate.
2. **Stage 2 — Windows (gated by Stage 1):** Tests on .NET 5.0–10.0 plus .NET Framework 4.6.2–4.8.1.
3. **Stage 3 — macOS (gated by Stage 2):** Tests on .NET 6.0–10.0 (ARM64 compatible only).
4. **Security:** gitleaks (secrets) and DevSkim (static analysis) run in parallel.

`pull_request_target` means the workflow YAML always runs from `main`, so changes to `pr.yaml` in a PR are not validated by that PR's own run. Validate workflow changes by re-running an existing PR after the workflow change merges.

### Branch Protection
Configured by `scripts/Setup-BranchRuleset.ps1`. The current ruleset on `main` requires:
- All PR-Checks status checks to pass.
- Conversation resolution before merging.
- No force-push, no deletion.

## Agent Guidelines

### Trust These Instructions First
This file describes Log-Compressor's actual structure and conventions. **Only search for additional information if these instructions are incomplete or appear incorrect.** When in doubt, the README on `chore/finalize-readme` (or, after merge, on `main`) is the canonical project description.

### When Working on This Project
1. **Code style:** Follow `.editorconfig`. Use file-scoped namespaces, Allman braces, `var` for obvious types, async-first APIs.
2. **Adding dependencies:** Use `dotnet add package`. Avoid pinning `System.Linq.Async`/`Microsoft.Bcl.*` independently — they cascade.
3. **Banned APIs:** Don't introduce calls flagged by `BannedSymbols.txt` (sync I/O, `Task.Wait`, `Thread.Sleep`, `BinaryFormatter`, `WebClient`, …).
4. **Testing:** Add tests in `tests/` matching `*Tests.csproj` naming. Coverage must stay ≥ 90%.
5. **Benchmarks:** Add new benchmarks under `benchmarks/` if you change a hot path; do not change the existing benchmark harness without a clear reason.
6. **Security:** If DevSkim flags new findings, address or justify before merging.

### Validation Steps Before Submitting Changes
1. `dotnet restore && dotnet build --configuration Release`
2. `dotnet test --configuration Release --collect:"XPlat Code Coverage"`
3. Verify coverage stays ≥ 90% (regenerate the report locally if a module slips).
4. Run DevSkim if you touched code paths that handle untrusted input.
5. Ensure all GitHub Actions checks pass.
