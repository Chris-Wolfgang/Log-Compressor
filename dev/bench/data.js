window.BENCHMARK_DATA = {
  "lastUpdate": 1781197437260,
  "repoUrl": "https://github.com/Chris-Wolfgang/Log-Compressor",
  "entries": {
    "BenchmarkDotNet": [
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "c6ed0cb9d5c186eca79611cbb0d7ac574c374528",
          "message": "Merge pull request #101 from Chris-Wolfgang/initial-dev\n\ninitial-dev → protected/vnext (features + canonical non-protected baseline)",
          "timestamp": "2026-06-11T12:55:29-04:00",
          "tree_id": "d813ea722c3a556b3b48ee6b5900210cfddd5a05",
          "url": "https://github.com/Chris-Wolfgang/Log-Compressor/commit/c6ed0cb9d5c186eca79611cbb0d7ac574c374528"
        },
        "date": 1781197436966,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"fastest\")",
            "value": 792089.6803385416,
            "unit": "ns",
            "range": "± 3273.522784395509"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"fastest\")",
            "value": 8040107.830729167,
            "unit": "ns",
            "range": "± 27005.108281526223"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"optimal\")",
            "value": 12945113.375,
            "unit": "ns",
            "range": "± 8252.63910693107"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"optimal\")",
            "value": 130038359.5,
            "unit": "ns",
            "range": "± 28981.781606079705"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"smallest\")",
            "value": 207678197,
            "unit": "ns",
            "range": "± 520202.19198253955"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"smallest\")",
            "value": 2242378294.3333335,
            "unit": "ns",
            "range": "± 4243573.443115406"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"fastest\")",
            "value": 2508120.9375,
            "unit": "ns",
            "range": "± 8735.397711337013"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"fastest\")",
            "value": 25293834.84375,
            "unit": "ns",
            "range": "± 58866.56629513805"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"optimal\")",
            "value": 3046448.3216145835,
            "unit": "ns",
            "range": "± 11216.122691067965"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"optimal\")",
            "value": 30822655.833333332,
            "unit": "ns",
            "range": "± 101174.3619448697"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"smallest\")",
            "value": 17090927.916666668,
            "unit": "ns",
            "range": "± 85385.88815593628"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"smallest\")",
            "value": 170579766.22222224,
            "unit": "ns",
            "range": "± 90052.8762211583"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"fastest\")",
            "value": 2799077.6953125,
            "unit": "ns",
            "range": "± 11951.560176153187"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"fastest\")",
            "value": 28491668.53125,
            "unit": "ns",
            "range": "± 398977.8661845158"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"optimal\")",
            "value": 3350723.9830729165,
            "unit": "ns",
            "range": "± 9708.106683310194"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"optimal\")",
            "value": 33477821.399999995,
            "unit": "ns",
            "range": "± 39482.17145446878"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"smallest\")",
            "value": 17475238.541666668,
            "unit": "ns",
            "range": "± 83365.38952989441"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"smallest\")",
            "value": 174540745.44444442,
            "unit": "ns",
            "range": "± 219537.48986406255"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"fastest\")",
            "value": 9275198.869791666,
            "unit": "ns",
            "range": "± 40769.79980961038"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"fastest\")",
            "value": 92950800.5,
            "unit": "ns",
            "range": "± 336399.271375177"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"optimal\")",
            "value": 129634641.41666667,
            "unit": "ns",
            "range": "± 283825.98975322157"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"optimal\")",
            "value": 1301070017.6666667,
            "unit": "ns",
            "range": "± 2596354.099744935"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"smallest\")",
            "value": 2173568502.6666665,
            "unit": "ns",
            "range": "± 757866.2115982829"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"smallest\")",
            "value": 22002866296.666668,
            "unit": "ns",
            "range": "± 24629616.88055032"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"fastest\")",
            "value": 26429078.104166668,
            "unit": "ns",
            "range": "± 60124.98465058221"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"fastest\")",
            "value": 254760909.5,
            "unit": "ns",
            "range": "± 285352.9740939281"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"optimal\")",
            "value": 31883938.208333332,
            "unit": "ns",
            "range": "± 112626.9272116314"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"optimal\")",
            "value": 311683820,
            "unit": "ns",
            "range": "± 73097.1977968376"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"smallest\")",
            "value": 173102063.66666666,
            "unit": "ns",
            "range": "± 214706.57363817806"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"smallest\")",
            "value": 1719633685,
            "unit": "ns",
            "range": "± 1621081.3305682722"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"fastest\")",
            "value": 32262743.604166668,
            "unit": "ns",
            "range": "± 61074.165667984074"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"fastest\")",
            "value": 300702235.5,
            "unit": "ns",
            "range": "± 710193.4557081965"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"optimal\")",
            "value": 35960607.69047619,
            "unit": "ns",
            "range": "± 14710.38844441174"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"optimal\")",
            "value": 353640526,
            "unit": "ns",
            "range": "± 291483.0514729802"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"smallest\")",
            "value": 175592630.88888893,
            "unit": "ns",
            "range": "± 48653.31058889533"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"smallest\")",
            "value": 1806448484.6666667,
            "unit": "ns",
            "range": "± 2387196.0768278614"
          }
        ]
      }
    ]
  }
}