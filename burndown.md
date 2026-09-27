# Stryker + release-blocker burndown — Log-Compressor

Started 2026-09-27 against `origin/main` @ `5f79f56` (release v0.4.1). No open PRs and no `vNext`,
so the stack is rooted on **main**.

## Definitions

- **Mutation score**: `dotnet stryker` with the repo's `stryker-config.json` (unit test project
  only, `ignore-methods` for `Log*`/`WriteLine*`/`ConfigureAwait`), as run by `stryker.yaml`.
  Ratchet: `docs/stryker-baseline.json` (`floor` = `thresholds.break`, moves up only in a PR that
  measures). Local scoped runs: `dotnet stryker --config-file <copy with project/test paths> --concurrency 4 -m "<glob>"`
  from `tests/Wolfgang.LogCompressor.Tests.Unit`.
- **Release blockers**: what the repo gates a release on —
  1. `dotnet build Wolfgang.LogCompressor.slnx -c Release` warnings + errors
     (`TreatWarningsAsErrors` is on for Release in `Directory.Build.props`), counted with
     `grep -cE " (warning|error) "` on the build log;
  2. InspectCode error-severity findings (the `ReSharper InspectCode` gate job in `pr.yaml`);
  3. open code-scanning alerts: `gh api repos/Chris-Wolfgang/Log-Compressor/code-scanning/alerts?state=open --jq length`.

## Baseline (2026-09-27, `5f79f56`)

- Mutation: **92.52 %** — 570 killed / 43 survived / 11 timeout / 4 no-coverage
  (CI: `stryker.yaml` run 36299378795 on main). Already above the 85 % target and the 91 floor.
- Blockers: **0** — Release build 0 warnings / 0 errors; InspectCode gate green on main
  (run 36276338257); 0 open code-scanning alerts.
- Survivors by file: ArchiveVerifier 14, compression strategies (leaveOpen / in-loop
  ThrowIfCancellationRequested) 9, DecompressService 7, Decompress command 3, Bundle/Compress
  commands 3, SerilogConfigurator 2, BundleService 2, and one each in SharedOptions,
  FileSinkOptions, CompressionOptions, CompressionStrategyFactory, FileFilterService, FileNamingService.

## Running table

| PR | Cluster | Mutation before → after | Blockers before → after | Status |
|----|---------|-------------------------|-------------------------|--------|
| (this PR) | Option defaults, Serilog enrichment, decompress default report path, zip entry CRC (real bug) | 92.52 → see PR (scoped files: 6 survivors killed) | 0 → 0 | open |
