# Log-Compressor

A cross-platform .NET CLI (`logc`) for compressing log files. Built for **unattended scheduled jobs** on servers — point it at a log directory, optionally filter by age, and it produces archives and removes the originals.

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/)
[![GitHub](https://img.shields.io/badge/GitHub-Repository-181717?logo=github)](https://github.com/Chris-Wolfgang/Log-Compressor)

---

## ✨ Features

- **Two compression modes:**
  - `logc compress <path>` — produces **one archive per source file** (each log gets its own `.zip` / `.tar.gz` / `.tar.br`)
  - `logc bundle <path>` — produces **one archive containing many files** (good for end-of-month rollups)
- **Three formats:** ZIP (default), GZip (`.tar.gz`), Brotli (`.tar.br`)
- **Filtering:**
  - `-r|--recurse` — descend into subdirectories
  - `--older-than <days>` — only files modified N+ calendar days ago
  - `--min-datetime` / `--max-datetime` — explicit date-range window (mutually exclusive with `--older-than`)
- **Safe by default** — original is deleted **only after** the archive is written successfully
- **Self-contained single-file executable** — no .NET runtime required on the target server
- **Response-file support** — pass arguments via a config file for repeatable scheduled jobs
- **Structured logging** via Serilog (console + file sinks); records what was compressed, skipped, errors, before/after sizes

---

## 🚀 Quick Start

> **Status:** v0.1.0 in active development on the [`initial-dev`](https://github.com/Chris-Wolfgang/Log-Compressor/tree/initial-dev) branch. Not yet released. Build from source for now.

### Build from source

```bash
git clone https://github.com/Chris-Wolfgang/Log-Compressor.git
cd Log-Compressor
git checkout initial-dev
dotnet restore
dotnet build --configuration Release
dotnet test
```

### Compress a single file

```bash
logc compress /var/log/myapp/app.log
# Produces: /var/log/myapp/app-2026-05-04_10-15-30.zip
# Then deletes: /var/log/myapp/app.log
```

### Compress every old log in a directory (each to its own archive)

```bash
logc compress /var/log/myapp --recurse --older-than 7
```

### Bundle a month's logs into a single archive

```bash
logc bundle /var/log/myapp --recurse --min-datetime 2026-04-01 --max-datetime 2026-04-30 --format gz
# Produces: /var/log/myapp/myapp-2026-04-01 to 2026-04-30.tar.gz
```

### Output filename conventions

| Mode | Pattern |
|------|---------|
| `compress` | `{filename}-{lastModified yyyy-MM-dd_HH-mm-ss}.{ext}` |
| `bundle`   | `{foldername}-{minModified} to {maxModified}.{ext}` |

---

## 🔧 CLI Reference

### Shared flags (apply to both `compress` and `bundle`)

| Flag | Purpose | Default |
|------|---------|---------|
| `-r`, `--recurse` | Recurse subdirectories | `false` |
| `-o`, `--output <dir>` | Output directory | source directory |
| `--older-than <days>` | Only files modified N+ days ago | (no filter) |
| `--min-datetime <dt>` | Inclusive lower bound on last-modified date | (no filter) |
| `--max-datetime <dt>` | Inclusive upper bound on last-modified date | (no filter) |
| `-f`, `--format <fmt>` | `zip` \| `gz` \| `brotli` | `zip` |

`--older-than` is mutually exclusive with `--min-datetime` / `--max-datetime`. DateTime values are parsed using the local culture.

### Response files

McMaster CommandLineUtils response-file support is enabled. Pass `@path/to/args.txt` and the file's lines become arguments — useful for scheduled tasks where the command-line is awkward to express.

```text
# example.rsp
compress
/var/log/myapp
--recurse
--older-than
7
--format
gz
--output
/archive/myapp
```

```bash
logc @example.rsp
```

---

## 🧩 Architecture

Three layers, all `internal` and exposed to the test project via `InternalsVisibleTo`:

| Layer | Examples |
|-------|----------|
| **Command** | `SharedOptions`, `Compress`, `Bundle` (McMaster CommandLineUtils) |
| **Service** | `CompressService`, `BundleService`, `FileFilterService`, `FileNamingService`, compression strategies |
| **Abstraction** | `IFileSystem`, `ICompressionStrategy`, `IFileFilter`, `IFileNamer` |

Compression strategies (`ZipStrategy`, `GZipStrategy`, `BrotliStrategy`) are dispatched via `CompressionStrategyFactory`. GZip and Brotli bundles use `System.Formats.Tar.TarWriter` to wrap many files into a single tar before compressing — `.tar.gz` and `.tar.br` respectively.

---

## 🎯 Target Framework & Distribution

- **TFM:** `net10.0`
- **Distribution:** self-contained single-file executable
- **Runtimes:** `win-x64`, `linux-x64`, `osx-x64`

> The `src/Wolfgang.LogCompressor` project lives on the [`initial-dev`](https://github.com/Chris-Wolfgang/Log-Compressor/tree/initial-dev) branch until v0.1.0 lands on `main`. Check it out first (see [Build from source](#build-from-source) above).

```bash
dotnet publish src/Wolfgang.LogCompressor -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true
```

The resulting binary in `bin/Release/net10.0/linux-x64/publish/` is a single file — copy it to your server and run.

---

## 🧪 Quality

| Metric | Value |
|--------|-------|
| Unit tests (xunit 2.9.3, NSubstitute) | 163 |
| Code coverage | 95.9% (line) |
| Benchmarks | BenchmarkDotNet (compression-format throughput) |

---

## 📚 Documentation

- **GitHub Repository:** [https://github.com/Chris-Wolfgang/Log-Compressor](https://github.com/Chris-Wolfgang/Log-Compressor)
- **Contributing Guide:** [CONTRIBUTING.md](CONTRIBUTING.md)
- **Code of Conduct:** [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md)
- **Security Policy:** [SECURITY.md](SECURITY.md)

---

## 🛣️ Roadmap

Items deferred from v0.1.0:

- `decompress` sub-command
- Compressed-timestamp naming mode (`{name}-{now}` instead of `{name}-{lastModified}`)
- Custom name prefix (`--name`)
- Glob-pattern input (`*.log`, `app-*.log`)
- Mid-batch error-handling strategies (currently: skip & continue for `compress`, fail-fast for `bundle`)

---

## 🤝 Contributing

Contributions welcome. The architecture is built around interfaces (`ICompressionStrategy`, `IFileSystem`, etc.) so adding a new compression format or filter is a single-file change plus tests. See [CONTRIBUTING.md](CONTRIBUTING.md) for code style and PR guidelines.

---

## 📄 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.
