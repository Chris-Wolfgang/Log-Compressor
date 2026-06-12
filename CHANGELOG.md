# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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

[Unreleased]: https://github.com/Chris-Wolfgang/Log-Compressor/compare/v0.1.0...HEAD
[0.1.0]: https://github.com/Chris-Wolfgang/Log-Compressor/releases/tag/v0.1.0
