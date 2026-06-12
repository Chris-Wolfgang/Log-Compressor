window.BENCHMARK_DATA = {
  "lastUpdate": 1781223229013,
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
      },
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
          "id": "51be20e08dcf674cdb29abc43d3acf84f8fbc835",
          "message": "Merge pull request #120 from Chris-Wolfgang/ci/release-publish-binaries\n\nDistribute logc as per-RID Release binaries; disable NuGet publish",
          "timestamp": "2026-06-11T20:05:21-04:00",
          "tree_id": "491f7f2350368d6b23407a9cc92b30223c23b35c",
          "url": "https://github.com/Chris-Wolfgang/Log-Compressor/commit/51be20e08dcf674cdb29abc43d3acf84f8fbc835"
        },
        "date": 1781223228650,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"fastest\")",
            "value": 793221.9088541666,
            "unit": "ns",
            "range": "± 2634.3280554042626"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"fastest\")",
            "value": 8111228.671875,
            "unit": "ns",
            "range": "± 32340.88305745208"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"optimal\")",
            "value": 12976273.989583334,
            "unit": "ns",
            "range": "± 24678.405797402174"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"optimal\")",
            "value": 130243414.66666667,
            "unit": "ns",
            "range": "± 386801.1794884367"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"smallest\")",
            "value": 206737487.55555555,
            "unit": "ns",
            "range": "± 91760.26632803395"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"smallest\")",
            "value": 2239713786.6666665,
            "unit": "ns",
            "range": "± 298157.49801796593"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"fastest\")",
            "value": 2524543.26171875,
            "unit": "ns",
            "range": "± 47518.35936601251"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"fastest\")",
            "value": 25436687.15625,
            "unit": "ns",
            "range": "± 191976.12138042197"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"optimal\")",
            "value": 3076488.9479166665,
            "unit": "ns",
            "range": "± 48500.48604466517"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"optimal\")",
            "value": 31084661.8125,
            "unit": "ns",
            "range": "± 407655.2173020122"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"smallest\")",
            "value": 21581946.557291668,
            "unit": "ns",
            "range": "± 119740.15966988626"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"smallest\")",
            "value": 173263758,
            "unit": "ns",
            "range": "± 237332.143697393"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"fastest\")",
            "value": 2811993.2135416665,
            "unit": "ns",
            "range": "± 15879.412483750024"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"fastest\")",
            "value": 28241233.822916668,
            "unit": "ns",
            "range": "± 54449.23176392819"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"optimal\")",
            "value": 3416955.2265625,
            "unit": "ns",
            "range": "± 16498.687958333256"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"optimal\")",
            "value": 33641699.199999996,
            "unit": "ns",
            "range": "± 66601.66899113213"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"smallest\")",
            "value": 17622739.729166668,
            "unit": "ns",
            "range": "± 83878.94384467637"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"smallest\")",
            "value": 219548981.7777778,
            "unit": "ns",
            "range": "± 690136.7589644191"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"fastest\")",
            "value": 10064083.265625,
            "unit": "ns",
            "range": "± 40135.27615160235"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"fastest\")",
            "value": 102162431.26666667,
            "unit": "ns",
            "range": "± 190221.28007941743"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"optimal\")",
            "value": 130249304.41666667,
            "unit": "ns",
            "range": "± 74811.03739854056"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"optimal\")",
            "value": 1306975197.6666667,
            "unit": "ns",
            "range": "± 2274027.557536261"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"smallest\")",
            "value": 2185000486,
            "unit": "ns",
            "range": "± 10424122.3290573"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"smallest\")",
            "value": 22130242116.666668,
            "unit": "ns",
            "range": "± 25816307.207742345"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"fastest\")",
            "value": 26766254.411458332,
            "unit": "ns",
            "range": "± 144846.5530283131"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"fastest\")",
            "value": 262921198.66666666,
            "unit": "ns",
            "range": "± 3091047.5157976113"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"optimal\")",
            "value": 32311524.770833332,
            "unit": "ns",
            "range": "± 47468.813375424004"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"optimal\")",
            "value": 315979339.8333333,
            "unit": "ns",
            "range": "± 209943.63747583146"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"smallest\")",
            "value": 172431341.11111113,
            "unit": "ns",
            "range": "± 95781.84941737546"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"smallest\")",
            "value": 1728730859.3333333,
            "unit": "ns",
            "range": "± 2308374.8210770134"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"fastest\")",
            "value": 32235890.604166668,
            "unit": "ns",
            "range": "± 153965.49002437727"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"fastest\")",
            "value": 310863954.6666667,
            "unit": "ns",
            "range": "± 878999.2154475073"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"optimal\")",
            "value": 37067703.5952381,
            "unit": "ns",
            "range": "± 179693.22117514114"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"optimal\")",
            "value": 360849056.3333333,
            "unit": "ns",
            "range": "± 366432.3325122025"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"smallest\")",
            "value": 176670197.2222222,
            "unit": "ns",
            "range": "± 26437.60669604565"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"smallest\")",
            "value": 1764381295.6666667,
            "unit": "ns",
            "range": "± 7731348.711224021"
          }
        ]
      }
    ]
  }
}