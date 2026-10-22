# Introduction

Welcome to Wolfgang.LogCompressor!

## Overview

A cross-platform .NET CLI (`logc`) for compressing log files, built for unattended scheduled jobs on servers. Point it at a log directory, optionally filter by age or date range, and it produces archives and removes the originals.

## Key Features

- **Four commands:** `logc compress` (one archive per source file), `logc bundle` (many files into a single archive), `logc decompress` (extract logc archives) and `logc init` (starter configuration files)
- **Five formats:** ZIP (default), GZip, Brotli, Zstandard and LZ4 (`.gz` / `.br` / `.zst` / `.lz4` for single files; `.tar.*` bundles, `.zip` for ZIP)
- **Filtering:** recurse subdirectories, `--older-than <days>`, or an explicit `--min-datetime` / `--max-datetime` range
- **Safe by default:** the original is deleted only after its archive is written successfully
- **Self-contained:** ships as a single-file executable — no .NET runtime needed on the target server
- **Automation-friendly:** response-file support and structured Serilog logging (console + file) for scheduled jobs

## Getting Help

If you need help with Wolfgang.LogCompressor, please:

- Check the [Getting Started](getting-started.md) guide
- Review the [API Reference](../api/index.md)
- Visit the [GitHub repository](https://github.com/Chris-Wolfgang/Log-Compressor)
- Open an issue on [GitHub Issues](https://github.com/Chris-Wolfgang/Log-Compressor/issues)
