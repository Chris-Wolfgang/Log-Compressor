# Getting Started

This guide will help you quickly get up and running with Wolfgang.LogCompressor — the `logc` command-line tool for compressing log files.

## Prerequisites

- A terminal on Windows, Linux, or macOS
- .NET 10.0 SDK or later only if you build from source

## Installation

Wolfgang.LogCompressor ships as a self-contained single-file executable (`logc`) — no .NET runtime is required on the target machine. Each release attaches one archive per platform:

| Platform | Download |
|----------|----------|
| Windows (x64) | [logc-win-x64.zip](https://github.com/Chris-Wolfgang/Log-Compressor/releases/latest/download/logc-win-x64.zip) |
| Linux (x64) | [logc-linux-x64.tar.gz](https://github.com/Chris-Wolfgang/Log-Compressor/releases/latest/download/logc-linux-x64.tar.gz) |
| macOS (Intel, x64) | [logc-osx-x64.tar.gz](https://github.com/Chris-Wolfgang/Log-Compressor/releases/latest/download/logc-osx-x64.tar.gz) |

Each archive contains `logc` and its `AppSettings.json`; keep the two together in one folder.

```bash
curl -L -o logc-linux-x64.tar.gz \
  https://github.com/Chris-Wolfgang/Log-Compressor/releases/latest/download/logc-linux-x64.tar.gz
mkdir -p ~/logc && tar -xzf logc-linux-x64.tar.gz -C ~/logc
chmod +x ~/logc/logc
```

To build from source instead:

```bash
git clone https://github.com/Chris-Wolfgang/Log-Compressor.git
cd Log-Compressor
dotnet publish src/Wolfgang.LogCompressor -c Release -r linux-x64 --self-contained
```

Replace `linux-x64` with `win-x64` or `osx-x64` for your platform. The output (`logc` plus `AppSettings.json`) is written to `src/Wolfgang.LogCompressor/bin/Release/net10.0/<rid>/publish/` — copy both to your server.

## Quick Start

Compress a single log file (the original is deleted only after the archive is written successfully):

```bash
logc compress /var/log/myapp/app.log
```

Compress every log older than 7 days in a directory, each to its own archive:

```bash
logc compress /var/log/myapp --recurse --older-than 7
```

Bundle a date range of logs into a single archive:

```bash
logc bundle /var/log/myapp --min-datetime 2026-04-01 --max-datetime 2026-04-30 --format gz
```

## Next Steps

- Read the [Introduction](introduction.md) to learn more about what Wolfgang.LogCompressor does
- Browse the [API Reference](../api/index.md) for the internal types
- See the full CLI reference and response-file usage in the [README](https://github.com/Chris-Wolfgang/Log-Compressor#-cli-reference)

## Additional Resources

- [GitHub Repository](https://github.com/Chris-Wolfgang/Log-Compressor)
- [Contributing Guidelines](https://github.com/Chris-Wolfgang/Log-Compressor/blob/main/CONTRIBUTING.md)
- [Report an Issue](https://github.com/Chris-Wolfgang/Log-Compressor/issues)
