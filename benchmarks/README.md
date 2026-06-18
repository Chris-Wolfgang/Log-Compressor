# Benchmarks

[BenchmarkDotNet](https://benchmarkdotnet.org/) micro-benchmarks for `logc`'s
compression paths, plus the stored baselines used for regression tracking.

## Projects & modes

`Wolfgang.LogCompressor.Benchmarks` has two entry points (see `Program.cs`):

| Mode | Command | Purpose |
|------|---------|---------|
| BenchmarkDotNet (default) | `dotnet run -c Release -- --filter "*"` | Timed compress/bundle micro-benchmarks across format × level × file-size. |
| Ratio table | `dotnet run -c Release -- --ratio` | Prints a Markdown comparison table (ratio + MB/s per format/level/size) — the source for the README comparison chart. |

Both cover all five formats: `zip`, `gz`, `brotli`, `zstd`, `lz4`.

## Running locally

```bash
cd benchmarks/Wolfgang.LogCompressor.Benchmarks

# Full timed suite (slow — full BDN run):
dotnet run -c Release -- --filter "*"

# Fast smoke run (matches CI's job):
dotnet run -c Release -- --filter "*" --job short --memory --exporters json

# Comparison/ratio table for the README:
dotnet run -c Release -- --ratio
```

Results are written under `BenchmarkDotNet.Artifacts/results/`.

## Baselines (`benchmarks/baselines/`)

Known-good results are committed under [`baselines/`](baselines/) so changes can be
compared against a fixed reference. Each file is the BenchmarkDotNet JSON export
from a run, named by date (e.g. `baseline-2026-04-13.json`).

### Updating a baseline

Only refresh a baseline deliberately (e.g. after an intentional perf change), on a
quiet machine for stable numbers:

```bash
cd benchmarks/Wolfgang.LogCompressor.Benchmarks
dotnet run -c Release -- --filter "*" --exporters json
# Copy the produced *-report-full.json into the baselines folder, dated:
cp BenchmarkDotNet.Artifacts/results/*-report-full.json ../baselines/baseline-$(date +%F).json
```

Commit the new baseline in its own PR with a note on what changed and why.

## Regression tracking

Regressions are tracked **automatically**: the
[`benchmarks.yaml`](../.github/workflows/benchmarks.yaml) workflow runs the suite on
every push to `main` and publishes results via
[`github-action-benchmark`](https://github.com/benchmark-action/github-action-benchmark)
to the `gh-pages` branch under `dev/bench/`. The trend chart is published at:

<https://chris-wolfgang.github.io/Log-Compressor/dev/bench/>

`github-action-benchmark` keeps the full history there and flags commits that
regress beyond its alert threshold.

### Manual comparison

To compare a local run against a stored baseline, run the suite with the JSON
exporter and diff the `Mean`/`Ratio` columns against the baseline file, or feed both
JSON reports to a tool such as BenchmarkDotNet's `ResultComparer`. The gh-pages trend
above is the canonical, always-current view.
