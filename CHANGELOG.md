# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed

- `--on-error retry:N` now backs off between attempts (200 ms x attempt, capped at 2 s) instead of retrying immediately — transient conditions like a writer rotating the file rarely clear within microseconds.

## [0.3.0] - 2026-09-01

### Added

- **Naming controls** (#188, #189): `--timestamp <modified|compressed>` selects whether archive names embed the source's last-modified time (default, stable across re-runs) or the compression time; `--name <prefix>` replaces the source file/folder name in generated archive names. Two sources resolving to the same archive name in one run are uniquified (`-2`, `-3`, ...) — this also fixes a pre-existing edge where recursed same-named files with identical modification times could silently overwrite each other's archives.
- **`--on-error <skip|fail|retry:N>`** (#190) on `compress`, `bundle` and `decompress`: `skip` (default) logs a failing item and continues; `fail` stops at the first failure and exits 11; `retry:N` (1–100) re-attempts a failing item N times before skipping it. For `bundle`, `fail` aborts the whole archive instead of shipping one that silently omits an unreadable input.
- New exit code **3 — completed with skips**: a run that finishes but skipped one or more failed items now exits 3 instead of 11, so schedulers can distinguish a degraded run from a broken one. **Migration note:** scripts that treated exit 11 as "some files failed" should also (or instead) check for exit 3; exit 11 now means the run itself failed (`--on-error fail` or a fatal error).
- **`decompress` sub-command** (#187) — extracts logc archives back out: every format (`zip`/`gz`/`brotli`/`zstd`/`lz4`), single archives and tar bundles, with `--output`, `--recurse`, `--include`/`--exclude`, `--force`, `--keep-archives`, `--no-lock` and `--report`. Safety mirrors compress's verify-then-delete: entries are confined to the destination (zip-slip protected), collisions fail the archive unless `--force`, and an archive is deleted only after every entry extracted successfully. An explicitly named file with an unknown extension falls back to magic-byte sniffing (directory scans select recognized archive extensions only; brotli excepted — the format has no signature).

### Changed

- CI: weekly cross-platform differential run (Linux/macOS/Windows × x64/ARM64 output-equivalence checks, #78) and nightly shadow testing against sampled consumer workloads (#69). No consumer-visible behavior change.

## [0.2.0] - 2026-08-31

### Added

- **Zstandard (`zstd`) and LZ4 (`lz4`) compression formats** (#3, #4) — singles as `.zst`/`.lz4`, bundles as `.tar.zst`/`.tar.lz4`, selectable via `-f zstd` / `-f lz4`.
- Compression comparison chart in the README with measured ratio, compression and decompression throughput for all five formats (#5); regenerate with `dotnet run -c Release --project benchmarks/Wolfgang.LogCompressor.Benchmarks -- --ratio`.
- Supply-chain artifacts attached to every release (#77, #86, #93): SLSA build-provenance attestation for each binary archive (verify with `gh attestation verify`), a CycloneDX SBOM (`logc.bom.json`), a reproducible-build manifest (`reproducible-build-manifest.json`), and generated `THIRD-PARTY-NOTICES.md`.
- Documentation: Architecture Decision Records under `docs/adr/` (#88), a major-version migration-guide template (#87), `docs/REPRODUCIBLE-BUILD.md` (#93), and a "Release path & compromise scope" appendix in `SECURITY.md` (#89).

### Fixed

- **Archive verification now detects truncated archives.** Modern .NET's `GZipStream`/`BrotliStream` silently return partial data on truncated input, so a torn write could previously pass verification — and verify-then-delete would remove the original. Gzip archives are now validated against their CRC-32/length trailer; brotli, zstd and LZ4 are checked against the source file's size. Found by the new property-fuzz suite.
- **`ProcessLock` race conditions on Linux** (#172): the stale-lock takeover and release paths each had a window where two instances could hold the lock simultaneously; acquisition is now a single atomic open. The `.logc.lock` file is also excluded from compression, so a run can no longer compress-and-delete its own live lock.
- **Archive names are now locale-invariant** (#83): on hosts with a non-Gregorian default calendar (e.g. `ar-SA`), embedded timestamps previously used that calendar (Hijri dates in archive names). Note: this changes generated archive names on such systems.
- Exit-code and retention-directory edge cases pinned by new tests; retention's age cutoff is exclusive by contract (a file exactly N days old is kept).

### Changed

- Test and CI hardening across the board: property-based fuzzing (CsCheck, weekly deep sweep), mutation testing enabled with a gated score floor (Stryker, floor 83%), per-PR benchmark deltas with an allocation-regression gate, OSSF Scorecard, workflow security linting (zizmor + actionlint), a transitive-dependency license audit, reproducible-build verification, and 100% line coverage across all assemblies (test code included).

## [0.1.0] - 2026-06-11

First release of `logc`, a cross-platform .NET CLI for compressing log files.

### Added

- `compress` command — produces one archive per source file.
- `bundle` command — produces a single archive containing many files.
- Three compression formats — ZIP (default), GZip (`.tar.gz`), and Brotli (`.tar.br`) — each with a selectable compression level (`fastest` / `optimal` / `smallest`).
- Filtering: `-r|--recurse`, `--older-than <days>`, and an explicit `--min-datetime` / `--max-datetime` range (mutually exclusive with `--older-than`).
- Archive integrity verification before the original is deleted — a source file is never deleted unless its archive was written and verified (`--no-verify` to skip).
- `bundle` skips unreadable files (logging a warning) and returns a non-success exit code instead of aborting; a file that was not archived is never deleted.
- Output-collision safety — an existing output archive is never overwritten, and already-compressed archives are excluded from directory enumeration so a repeated run does not re-compress its own output.
- Single-instance directory lock to prevent concurrent runs (`--no-lock` to disable).
- Archive retention via `--delete-archives-older-than <days>`.
- `init` command to generate starter response/configuration files.
- Optional JSON/CSV run report (`--report`).
- Response-file support (`@args.txt`) for repeatable scheduled jobs.
- Structured logging via Serilog (console + file sinks).
- Self-contained, per-platform release archives for `win-x64`, `linux-x64`, and `osx-x64` (each bundles the single-file `logc` executable plus its `AppSettings.json`); no .NET runtime required on the target. Distributed via GitHub Releases, not NuGet.

[Unreleased]: https://github.com/Chris-Wolfgang/Log-Compressor/compare/v0.3.0...HEAD
[0.3.0]: https://github.com/Chris-Wolfgang/Log-Compressor/compare/v0.2.0...v0.3.0
[0.2.0]: https://github.com/Chris-Wolfgang/Log-Compressor/compare/v0.1.0...v0.2.0
[0.1.0]: https://github.com/Chris-Wolfgang/Log-Compressor/releases/tag/v0.1.0
