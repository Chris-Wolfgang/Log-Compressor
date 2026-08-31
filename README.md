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
- **Five formats:** ZIP (default), GZip (`.tar.gz`), Brotli (`.tar.br`), Zstandard (`.tar.zst`), LZ4 (`.tar.lz4`)
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

> **Status:** v0.1.0 — first release. Download a prebuilt archive from [Releases](https://github.com/Chris-Wolfgang/Log-Compressor/releases), or build from source below.

### Download & run (no .NET runtime required)

Each release attaches one self-contained archive per platform. Download the one for your OS (these links always resolve to the **latest** release):

| Platform | Download |
|----------|----------|
| Windows (x64) | **[logc-win-x64.zip](https://github.com/Chris-Wolfgang/Log-Compressor/releases/latest/download/logc-win-x64.zip)** |
| Linux (x64) | **[logc-linux-x64.tar.gz](https://github.com/Chris-Wolfgang/Log-Compressor/releases/latest/download/logc-linux-x64.tar.gz)** |
| macOS (Intel, x64) | **[logc-osx-x64.tar.gz](https://github.com/Chris-Wolfgang/Log-Compressor/releases/latest/download/logc-osx-x64.tar.gz)** |

Or browse every release on the [Releases page](https://github.com/Chris-Wolfgang/Log-Compressor/releases).

Each archive contains the `logc` executable **and** its `AppSettings.json` — keep the two together in the same folder (`logc` reads `AppSettings.json` from beside itself at startup).

**Linux / macOS**

```bash
# Download + extract (Linux x64 shown; swap in osx-x64 for macOS):
curl -L -o logc-linux-x64.tar.gz \
  https://github.com/Chris-Wolfgang/Log-Compressor/releases/latest/download/logc-linux-x64.tar.gz
mkdir -p ~/logc && tar -xzf logc-linux-x64.tar.gz -C ~/logc
cd ~/logc
chmod +x logc            # first run only
./logc compress /var/log/myapp --recurse --older-than 7
```

> macOS may quarantine a downloaded binary. If you see *"cannot be opened because the developer cannot be verified,"* run `xattr -d com.apple.quarantine ./logc` once.

**Windows (PowerShell)**

```powershell
# Download + extract:
Invoke-WebRequest -Uri https://github.com/Chris-Wolfgang/Log-Compressor/releases/latest/download/logc-win-x64.zip -OutFile logc-win-x64.zip
Expand-Archive logc-win-x64.zip -DestinationPath C:\Tools\logc
cd C:\Tools\logc
.\logc.exe compress C:\Logs\myapp --recurse --older-than 7
```

Put the folder on your `PATH` to call `logc` from anywhere.

### Verify the build

Every release archive carries a keyless SLSA provenance attestation binding it
to this repo, commit and workflow run:

```bash
gh attestation verify logc-win-x64.zip --repo Chris-Wolfgang/Log-Compressor
```

Each release also attaches an SBOM (`logc.bom.json`) and a
`reproducible-build-manifest.json` with the deterministic-build hash —
[docs/REPRODUCIBLE-BUILD.md](docs/REPRODUCIBLE-BUILD.md) shows how to rebuild
from source and confirm the release matches it byte for byte.

### Build from source

```bash
git clone https://github.com/Chris-Wolfgang/Log-Compressor.git
cd Log-Compressor
git checkout v0.1.0
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
| `-f`, `--format <fmt>` | `zip` \| `gz` \| `brotli` \| `zstd` \| `lz4` | `zip` |
| `--include <glob>` | Only process files matching this glob (repeatable) | (no filter) |
| `--exclude <glob>` | Skip files matching this glob (repeatable; applied after `--include`) | (no filter) |

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

## 📊 Choosing a format

Measured on synthetic realistic log text (varied timestamps, ids, paths — deterministically seeded), single-file compression, Windows x64 / .NET 10, 2026-08-31. Regenerate on your own hardware with:

```bash
dotnet run -c Release --project benchmarks/Wolfgang.LogCompressor.Benchmarks -- --ratio
```

| Format | Ratio* | Compress | Decompress | Best for |
|--------|--------|----------|------------|----------|
| **ZIP** (default) | 14.2% | 77 MB/s | 895 MB/s | Universal compatibility — every OS opens it |
| **GZip** | 14.2% | 79 MB/s | 885 MB/s | Linux/Unix tooling (`zcat`, `gunzip`, pipelines) |
| **Brotli** | 14.1%† | 83 MB/s | 621 MB/s | Best ratio when CPU time is cheap |
| **Zstd** | 15.9% | 179 MB/s | 1.07 GB/s | Ratio/speed balance at scale |
| **LZ4** | 18.4% | 414 MB/s‡ | 1.49 GB/s | Maximum throughput; ratio is the trade |

\* compressed size as % of original, 100 MB corpus, `optimal` level (logc's CLI default is `smallest`).
† brotli at `smallest` reaches 10.6% but drops to ~0.5 MB/s — use it when archive size matters far more than job duration.
‡ LZ4's headline speed is at `fastest` (its `optimal`/`smallest` levels trade most of that speed for only ~6 points of ratio).

<details>
<summary>Full measurements (5 formats × 3 levels × 2 sizes)</summary>

| Format | Level | File Size | Compressed | Ratio | Compress (MB/s) | Decompress (MB/s) |
|--------|-------|-----------|------------|-------|-----------------|-------------------|
| ZIP    | Fastest |   10.0 MB |     2.2 MB | 22.3% |           297.2 |             680.0 |
| ZIP    | Optimal |   10.0 MB |     1.4 MB | 14.2% |            68.3 |             793.4 |
| ZIP    | Smallest |   10.0 MB |     1.3 MB | 13.4% |            35.2 |             793.8 |
| GZip   | Fastest |   10.0 MB |     2.2 MB | 22.3% |           298.9 |             678.4 |
| GZip   | Optimal |   10.0 MB |     1.4 MB | 14.2% |            73.5 |             806.8 |
| GZip   | Smallest |   10.0 MB |     1.3 MB | 13.4% |            38.5 |             798.9 |
| Brotli | Fastest |   10.0 MB |     1.6 MB | 16.5% |           312.2 |             369.0 |
| Brotli | Optimal |   10.0 MB |     1.4 MB | 14.1% |            79.6 |             567.8 |
| Brotli | Smallest |   10.0 MB |     1.1 MB | 10.6% |             0.5 |             547.8 |
| Zstd   | Fastest |   10.0 MB |     1.5 MB | 15.1% |           122.9 |             500.9 |
| Zstd   | Optimal |   10.0 MB |     1.6 MB | 15.9% |            76.5 |             559.9 |
| Zstd   | Smallest |   10.0 MB |     1.1 MB | 10.8% |             1.5 |             434.2 |
| LZ4    | Fastest |   10.0 MB |     2.5 MB | 24.5% |           165.7 |             535.8 |
| LZ4    | Optimal |   10.0 MB |     1.8 MB | 18.4% |            16.3 |             595.9 |
| LZ4    | Smallest |   10.0 MB |     1.8 MB | 18.1% |             8.3 |            1047.2 |
| ZIP    | Fastest |  100.0 MB |    22.3 MB | 22.3% |           224.8 |             640.2 |
| ZIP    | Optimal |  100.0 MB |    14.2 MB | 14.2% |            76.8 |             895.4 |
| ZIP    | Smallest |  100.0 MB |    13.4 MB | 13.4% |            42.1 |             928.7 |
| GZip   | Fastest |  100.0 MB |    22.3 MB | 22.3% |           365.9 |             814.6 |
| GZip   | Optimal |  100.0 MB |    14.2 MB | 14.2% |            78.6 |             884.6 |
| GZip   | Smallest |  100.0 MB |    13.4 MB | 13.4% |            39.1 |             871.7 |
| Brotli | Fastest |  100.0 MB |    16.5 MB | 16.5% |           301.2 |             450.4 |
| Brotli | Optimal |  100.0 MB |    14.1 MB | 14.1% |            82.9 |             621.4 |
| Brotli | Smallest |  100.0 MB |    10.6 MB | 10.6% |             0.5 |             597.0 |
| Zstd   | Fastest |  100.0 MB |    15.1 MB | 15.1% |           321.3 |             877.0 |
| Zstd   | Optimal |  100.0 MB |    15.9 MB | 15.9% |           178.8 |            1068.6 |
| Zstd   | Smallest |  100.0 MB |    10.7 MB | 10.7% |             1.6 |            1382.0 |
| LZ4    | Fastest |  100.0 MB |    24.5 MB | 24.5% |           413.9 |            1493.1 |
| LZ4    | Optimal |  100.0 MB |    18.4 MB | 18.4% |            28.4 |            1094.6 |
| LZ4    | Smallest |  100.0 MB |    18.1 MB | 18.1% |             8.2 |            1562.7 |

</details>

Numbers are single-run wall-clock on one machine — treat relative differences as guidance, not absolutes. Highly repetitive logs compress far better than shown; already-compressed or binary content far worse.

---

## 🧩 Architecture

Three layers, all `internal` and exposed to the test project via `InternalsVisibleTo`:

| Layer | Examples |
|-------|----------|
| **Command** | `SharedOptions`, `Compress`, `Bundle` (McMaster CommandLineUtils) |
| **Service** | `CompressService`, `BundleService`, `FileFilterService`, `FileNamingService`, compression strategies |
| **Abstraction** | `IFileSystem`, `ICompressionStrategy`, `IFileFilter`, `IFileNamer` |

Compression strategies (`ZipCompressionStrategy`, `GZipCompressionStrategy`, `BrotliCompressionStrategy`, `ZstdCompressionStrategy`, `Lz4CompressionStrategy`) are dispatched via `CompressionStrategyFactory`. The non-ZIP formats use `System.Formats.Tar.TarWriter` to wrap many files into a single tar before compressing — `.tar.gz`, `.tar.br`, `.tar.zst`, and `.tar.lz4` respectively. Zstandard uses [ZstdSharp.Port](https://www.nuget.org/packages/ZstdSharp.Port/) and LZ4 uses [K4os.Compression.LZ4](https://www.nuget.org/packages/K4os.Compression.LZ4.Streams/) (both pure-managed, cross-platform).

---

## 🎯 Target Framework & Distribution

- **TFM:** `net10.0`
- **Distribution:** self-contained, per-platform archive attached to each GitHub Release (no .NET runtime required on the target)
- **Runtimes:** `win-x64`, `linux-x64`, `osx-x64`
- **Not on NuGet:** `logc` is an application, not a library, so it is not published as a NuGet package.

Releases are produced by the `publish-binaries` job in [`.github/workflows/release.yaml`](.github/workflows/release.yaml), which cross-compiles all three runtimes and uploads one archive each (`.zip` for Windows, `.tar.gz` for Linux/macOS).

Each archive contains two files:

| File | Purpose |
|------|---------|
| `logc` (`logc.exe` on Windows) | the self-contained single-file executable |
| `AppSettings.json` | runtime configuration (logging sinks/levels) — loaded from beside the executable at startup; required |

To build the same artifact locally for one runtime:

```bash
dotnet publish src/Wolfgang.LogCompressor -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true
```

The output lands in `bin/Release/net10.0/linux-x64/publish/` as `logc` alongside `AppSettings.json` — copy **both** to your server (keep them together) and run `./logc`.

---

## 🧪 Quality

| Metric | Value |
|--------|-------|
| Unit tests (xunit 2.9.3, NSubstitute) | 183 |
| Integration tests (real file system, all OSes) | 14 |
| Code coverage | 100% line, 95% branch; 90% per-module gate in CI |
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
- Mid-batch error-handling strategies (currently: skip & continue for `compress`, fail-fast for `bundle`)

---

## 🤝 Contributing

Contributions welcome. The architecture is built around interfaces (`ICompressionStrategy`, `IFileSystem`, etc.) so adding a new compression format or filter is a single-file change plus tests. See [CONTRIBUTING.md](CONTRIBUTING.md) for code style and PR guidelines.

---

## 📄 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.
