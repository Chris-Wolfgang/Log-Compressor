window.BENCHMARK_DATA = {
  "lastUpdate": 1788449831603,
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
          "id": "61501d2078a993d1be89c260cc5054997d1efaad",
          "message": "Merge pull request #119 from Chris-Wolfgang/dependabot/nuget/dotnet-dependencies-ac8ddf5dbe\n\nBump the dotnet-dependencies group with 9 updates",
          "timestamp": "2026-06-11T20:06:01-04:00",
          "tree_id": "4234d7809ece07b0ff7266e831957fd9db099ada",
          "url": "https://github.com/Chris-Wolfgang/Log-Compressor/commit/61501d2078a993d1be89c260cc5054997d1efaad"
        },
        "date": 1781223739256,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"fastest\")",
            "value": 756153.6419270834,
            "unit": "ns",
            "range": "± 4802.413780348071"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"fastest\")",
            "value": 7700488.432291667,
            "unit": "ns",
            "range": "± 33727.65437251397"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"optimal\")",
            "value": 11483342.052083334,
            "unit": "ns",
            "range": "± 12413.774900817532"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"optimal\")",
            "value": 115122006.66666667,
            "unit": "ns",
            "range": "± 317294.0510191985"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"smallest\")",
            "value": 182797355.55555555,
            "unit": "ns",
            "range": "± 2082316.5078471338"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"smallest\")",
            "value": 2004604397,
            "unit": "ns",
            "range": "± 319374.135585523"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"fastest\")",
            "value": 2439542.8333333335,
            "unit": "ns",
            "range": "± 36029.033356522945"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"fastest\")",
            "value": 24313782.0625,
            "unit": "ns",
            "range": "± 108866.54135685987"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"optimal\")",
            "value": 2969362.00390625,
            "unit": "ns",
            "range": "± 12495.328470678509"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"optimal\")",
            "value": 29262942.083333332,
            "unit": "ns",
            "range": "± 75825.44988566574"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"smallest\")",
            "value": 26103607.833333332,
            "unit": "ns",
            "range": "± 49660.24798982061"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"smallest\")",
            "value": 259909073.16666666,
            "unit": "ns",
            "range": "± 413217.6628631493"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"fastest\")",
            "value": 2755086.6419270835,
            "unit": "ns",
            "range": "± 6783.029597725908"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"fastest\")",
            "value": 27266351.416666668,
            "unit": "ns",
            "range": "± 108769.80247333455"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"optimal\")",
            "value": 3312322.6875,
            "unit": "ns",
            "range": "± 22096.593051578348"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"optimal\")",
            "value": 31964165.416666668,
            "unit": "ns",
            "range": "± 42353.78576452452"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"smallest\")",
            "value": 26294830.041666668,
            "unit": "ns",
            "range": "± 73860.48184495865"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"smallest\")",
            "value": 263425393.83333334,
            "unit": "ns",
            "range": "± 166375.39069400658"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"fastest\")",
            "value": 8368035.65625,
            "unit": "ns",
            "range": "± 183915.77397003252"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"fastest\")",
            "value": 83842805.66666667,
            "unit": "ns",
            "range": "± 811450.4820372807"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"optimal\")",
            "value": 114826950.26666667,
            "unit": "ns",
            "range": "± 216937.7780371403"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"optimal\")",
            "value": 1150561106.6666667,
            "unit": "ns",
            "range": "± 533542.567243077"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"smallest\")",
            "value": 1951858967.3333333,
            "unit": "ns",
            "range": "± 1430906.389521807"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"smallest\")",
            "value": 19732606718,
            "unit": "ns",
            "range": "± 56227616.47366281"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"fastest\")",
            "value": 25159469.03125,
            "unit": "ns",
            "range": "± 104492.57070157304"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"fastest\")",
            "value": 248162128.33333334,
            "unit": "ns",
            "range": "± 161334.21026070666"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"optimal\")",
            "value": 30003494.229166668,
            "unit": "ns",
            "range": "± 19837.40837999512"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"optimal\")",
            "value": 304594220.3333333,
            "unit": "ns",
            "range": "± 2399972.8497712393"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"smallest\")",
            "value": 262298727.83333334,
            "unit": "ns",
            "range": "± 1583968.306303533"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"smallest\")",
            "value": 2602013160.6666665,
            "unit": "ns",
            "range": "± 1029322.6029998241"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"fastest\")",
            "value": 30365865.96875,
            "unit": "ns",
            "range": "± 843860.9812644143"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"fastest\")",
            "value": 289063402,
            "unit": "ns",
            "range": "± 641649.0266682012"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"optimal\")",
            "value": 34681226.333333336,
            "unit": "ns",
            "range": "± 191316.40228502173"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"optimal\")",
            "value": 334421620,
            "unit": "ns",
            "range": "± 529437.5758056846"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"smallest\")",
            "value": 266846578.33333334,
            "unit": "ns",
            "range": "± 90196.98076201517"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"smallest\")",
            "value": 2645198643,
            "unit": "ns",
            "range": "± 2947011.837982162"
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
          "id": "b8135a91f34a8d3718a6e24ffed476bdbda529c2",
          "message": "Merge pull request #123 from Chris-Wolfgang/feature/122-code-based-logging\n\nReplace Serilog.Settings.Configuration with code-based config (trim-safe, keep file config)",
          "timestamp": "2026-06-11T21:58:16-04:00",
          "tree_id": "3398cd843f85c273d1234164d1815828cbc1fb15",
          "url": "https://github.com/Chris-Wolfgang/Log-Compressor/commit/b8135a91f34a8d3718a6e24ffed476bdbda529c2"
        },
        "date": 1781230025572,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"fastest\")",
            "value": 903218.8782552084,
            "unit": "ns",
            "range": "± 12133.509269119178"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"fastest\")",
            "value": 9126065.380208334,
            "unit": "ns",
            "range": "± 60897.54261057999"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"optimal\")",
            "value": 28627459.822916668,
            "unit": "ns",
            "range": "± 29895.65847101155"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"optimal\")",
            "value": 277707866,
            "unit": "ns",
            "range": "± 1067631.9260049318"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"smallest\")",
            "value": 189959767.11111113,
            "unit": "ns",
            "range": "± 125872.06120427419"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"smallest\")",
            "value": 2012228874.3333333,
            "unit": "ns",
            "range": "± 2376943.1991156903"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"fastest\")",
            "value": 2198538.2890625,
            "unit": "ns",
            "range": "± 21029.887231058434"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"fastest\")",
            "value": 22300193.338541668,
            "unit": "ns",
            "range": "± 61067.86135456397"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"optimal\")",
            "value": 2699546.8561197915,
            "unit": "ns",
            "range": "± 37475.833773808416"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"optimal\")",
            "value": 26930709.604166668,
            "unit": "ns",
            "range": "± 75392.02704981485"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"smallest\")",
            "value": 28108575.890625,
            "unit": "ns",
            "range": "± 69992.12840537046"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"smallest\")",
            "value": 282170540,
            "unit": "ns",
            "range": "± 185436.6046651793"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"fastest\")",
            "value": 2618775.9049479165,
            "unit": "ns",
            "range": "± 29717.268625927172"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"fastest\")",
            "value": 26482319.677083332,
            "unit": "ns",
            "range": "± 250076.5634303212"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"optimal\")",
            "value": 3054488.716796875,
            "unit": "ns",
            "range": "± 40067.19547048522"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"optimal\")",
            "value": 30424726.552083332,
            "unit": "ns",
            "range": "± 27613.37200316345"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"smallest\")",
            "value": 28175719.072916668,
            "unit": "ns",
            "range": "± 112929.71942405867"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"smallest\")",
            "value": 281123565.3333333,
            "unit": "ns",
            "range": "± 955433.2170607653"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"fastest\")",
            "value": 12700609.8828125,
            "unit": "ns",
            "range": "± 109386.0363524953"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"fastest\")",
            "value": 125254097.08333333,
            "unit": "ns",
            "range": "± 1144052.9000636656"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"optimal\")",
            "value": 282849514.5,
            "unit": "ns",
            "range": "± 337744.1068457006"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"optimal\")",
            "value": 2788572020.6666665,
            "unit": "ns",
            "range": "± 5498169.78951581"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"smallest\")",
            "value": 2045523076.6666667,
            "unit": "ns",
            "range": "± 932859.2052192729"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"smallest\")",
            "value": 19583981014.333332,
            "unit": "ns",
            "range": "± 37647012.644729"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"fastest\")",
            "value": 26554089.692708332,
            "unit": "ns",
            "range": "± 101556.89436849556"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"fastest\")",
            "value": 263083676,
            "unit": "ns",
            "range": "± 472342.16884664237"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"optimal\")",
            "value": 32925934.979166668,
            "unit": "ns",
            "range": "± 128833.1481708138"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"optimal\")",
            "value": 312919800.3333333,
            "unit": "ns",
            "range": "± 845372.2170358353"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"smallest\")",
            "value": 283318619.1666667,
            "unit": "ns",
            "range": "± 108139.65415185742"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"smallest\")",
            "value": 2817997072.3333335,
            "unit": "ns",
            "range": "± 5999670.212999406"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"fastest\")",
            "value": 34520355.755555555,
            "unit": "ns",
            "range": "± 240569.2112852779"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"fastest\")",
            "value": 339161523.6666667,
            "unit": "ns",
            "range": "± 92739.90571665109"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"optimal\")",
            "value": 38443558.461538464,
            "unit": "ns",
            "range": "± 81357.50318023286"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"optimal\")",
            "value": 382932837,
            "unit": "ns",
            "range": "± 4027177.684517409"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"smallest\")",
            "value": 283381876.1666667,
            "unit": "ns",
            "range": "± 550510.6964188647"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"smallest\")",
            "value": 2840944783.6666665,
            "unit": "ns",
            "range": "± 1716771.9751324966"
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
          "id": "96ae4243396d72c8b56fa4edb0b7d59dd83f85ae",
          "message": "Merge pull request #129 from Chris-Wolfgang/docs/benchmarks-readme\n\ndocs: benchmarks README + regression-tracking instructions (#6)",
          "timestamp": "2026-06-18T18:33:22-04:00",
          "tree_id": "929efa08ab29f914fe9da54d69ffeb8db9c1a567",
          "url": "https://github.com/Chris-Wolfgang/Log-Compressor/commit/96ae4243396d72c8b56fa4edb0b7d59dd83f85ae"
        },
        "date": 1781822508319,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"fastest\")",
            "value": 752507.923828125,
            "unit": "ns",
            "range": "± 675.590293378901"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"fastest\")",
            "value": 7801184.015625,
            "unit": "ns",
            "range": "± 47253.75219574531"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"optimal\")",
            "value": 11540849.296875,
            "unit": "ns",
            "range": "± 35547.899394679676"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"optimal\")",
            "value": 116220480.46666665,
            "unit": "ns",
            "range": "± 470316.5577476252"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"smallest\")",
            "value": 184990581.7777778,
            "unit": "ns",
            "range": "± 23329.975416306628"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"smallest\")",
            "value": 2001827383.6666667,
            "unit": "ns",
            "range": "± 1111924.5354943532"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"fastest\")",
            "value": 2439443.2486979165,
            "unit": "ns",
            "range": "± 7346.753363776701"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"fastest\")",
            "value": 24622531.505208332,
            "unit": "ns",
            "range": "± 24874.674629827394"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"optimal\")",
            "value": 2948441.8697916665,
            "unit": "ns",
            "range": "± 7499.698515488256"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"optimal\")",
            "value": 30220932.5,
            "unit": "ns",
            "range": "± 119091.91937018403"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"smallest\")",
            "value": 26264208.510416668,
            "unit": "ns",
            "range": "± 56210.34458322052"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"smallest\")",
            "value": 260161960.5,
            "unit": "ns",
            "range": "± 209783.26741127376"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"fastest\")",
            "value": 2701761.2955729165,
            "unit": "ns",
            "range": "± 10187.805156651097"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"fastest\")",
            "value": 27645506.59375,
            "unit": "ns",
            "range": "± 76455.51932965843"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"optimal\")",
            "value": 3260440.2200520835,
            "unit": "ns",
            "range": "± 7756.2326314568045"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"optimal\")",
            "value": 32332236.041666668,
            "unit": "ns",
            "range": "± 212254.12890893768"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"smallest\")",
            "value": 26379528.791666668,
            "unit": "ns",
            "range": "± 93761.72113657839"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"smallest\")",
            "value": 262987108.33333334,
            "unit": "ns",
            "range": "± 313569.0329665596"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"fastest\")",
            "value": 8239481.171875,
            "unit": "ns",
            "range": "± 29594.51033045181"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"fastest\")",
            "value": 82504157.23809524,
            "unit": "ns",
            "range": "± 169339.30659209116"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"optimal\")",
            "value": 116258797.8,
            "unit": "ns",
            "range": "± 2102488.1158233043"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"optimal\")",
            "value": 1150833012.3333333,
            "unit": "ns",
            "range": "± 754556.8785773099"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"smallest\")",
            "value": 1939285553.6666667,
            "unit": "ns",
            "range": "± 949946.4038235701"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"smallest\")",
            "value": 19775559928.666668,
            "unit": "ns",
            "range": "± 45629819.21466935"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"fastest\")",
            "value": 26188176.369791668,
            "unit": "ns",
            "range": "± 69251.86323069005"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"fastest\")",
            "value": 260924277,
            "unit": "ns",
            "range": "± 432737.1302670364"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"optimal\")",
            "value": 30100073.395833332,
            "unit": "ns",
            "range": "± 32846.36605459595"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"optimal\")",
            "value": 307854406.8333333,
            "unit": "ns",
            "range": "± 300022.41916744044"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"smallest\")",
            "value": 262474604.83333334,
            "unit": "ns",
            "range": "± 505544.09039972303"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"smallest\")",
            "value": 2613078654.6666665,
            "unit": "ns",
            "range": "± 11499725.74033139"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"fastest\")",
            "value": 29476691.447916668,
            "unit": "ns",
            "range": "± 122178.50789403681"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"fastest\")",
            "value": 288870185.6666667,
            "unit": "ns",
            "range": "± 263384.0353753115"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"optimal\")",
            "value": 34453203.155555554,
            "unit": "ns",
            "range": "± 29798.17046922083"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"optimal\")",
            "value": 345153901,
            "unit": "ns",
            "range": "± 2527764.452008731"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"smallest\")",
            "value": 265210385.16666666,
            "unit": "ns",
            "range": "± 195072.545196866"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"smallest\")",
            "value": 2653241524.3333335,
            "unit": "ns",
            "range": "± 744912.0837906534"
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
          "id": "d0ce72b271244dd1806f331001f93c070e2a8d8c",
          "message": "Merge pull request #132 from Chris-Wolfgang/dependabot/github_actions/github-actions-640176b5ab\n\nbuild(deps): bump actions/checkout from 6 to 7 in the github-actions group",
          "timestamp": "2026-06-19T12:18:01-04:00",
          "tree_id": "dbc9d4961016b1caa88dd7408896587fb36a4e26",
          "url": "https://github.com/Chris-Wolfgang/Log-Compressor/commit/d0ce72b271244dd1806f331001f93c070e2a8d8c"
        },
        "date": 1781886390578,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"fastest\")",
            "value": 801648.2747395834,
            "unit": "ns",
            "range": "± 15038.627864466946"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"fastest\")",
            "value": 8246762.901041667,
            "unit": "ns",
            "range": "± 216173.86503957756"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"optimal\")",
            "value": 13077840.864583334,
            "unit": "ns",
            "range": "± 30033.339633791562"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"optimal\")",
            "value": 131440150.75,
            "unit": "ns",
            "range": "± 1276051.4637872418"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"smallest\")",
            "value": 201505162.66666666,
            "unit": "ns",
            "range": "± 565717.1071281533"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"smallest\")",
            "value": 2239067685,
            "unit": "ns",
            "range": "± 967991.0168689584"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"fastest\")",
            "value": 2512813.2213541665,
            "unit": "ns",
            "range": "± 14821.063623354628"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"fastest\")",
            "value": 25295432.96875,
            "unit": "ns",
            "range": "± 36763.22837449977"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"optimal\")",
            "value": 3108576.9869791665,
            "unit": "ns",
            "range": "± 8936.226099005815"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"optimal\")",
            "value": 30720732.041666668,
            "unit": "ns",
            "range": "± 107148.0599257763"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"smallest\")",
            "value": 17080119.46875,
            "unit": "ns",
            "range": "± 86999.24630913994"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"smallest\")",
            "value": 172363162.11111107,
            "unit": "ns",
            "range": "± 57704.942990984986"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"fastest\")",
            "value": 2809880.87109375,
            "unit": "ns",
            "range": "± 7372.5582476964455"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"fastest\")",
            "value": 28427686.375,
            "unit": "ns",
            "range": "± 186116.4251799021"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"optimal\")",
            "value": 3357039.9244791665,
            "unit": "ns",
            "range": "± 9619.383042933863"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"optimal\")",
            "value": 33705721.26666667,
            "unit": "ns",
            "range": "± 256515.67916997138"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"smallest\")",
            "value": 18829885.630208332,
            "unit": "ns",
            "range": "± 2601288.1992324707"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"smallest\")",
            "value": 173913358,
            "unit": "ns",
            "range": "± 419816.75015535054"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"fastest\")",
            "value": 9518505.580729166,
            "unit": "ns",
            "range": "± 20006.007423835516"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"fastest\")",
            "value": 98572143.94444446,
            "unit": "ns",
            "range": "± 1933026.5607736488"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"optimal\")",
            "value": 130391220,
            "unit": "ns",
            "range": "± 909593.6889686997"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"optimal\")",
            "value": 1301394801.6666667,
            "unit": "ns",
            "range": "± 1260048.7837620943"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"smallest\")",
            "value": 2176397154.3333335,
            "unit": "ns",
            "range": "± 3647313.453252453"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"smallest\")",
            "value": 22060832762.333332,
            "unit": "ns",
            "range": "± 8382111.4930934515"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"fastest\")",
            "value": 26279413.505208332,
            "unit": "ns",
            "range": "± 109826.74914109365"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"fastest\")",
            "value": 256391574.66666666,
            "unit": "ns",
            "range": "± 149637.94914086245"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"optimal\")",
            "value": 31913593.958333332,
            "unit": "ns",
            "range": "± 42181.179162239256"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"optimal\")",
            "value": 312605190.1666667,
            "unit": "ns",
            "range": "± 1933550.2462549515"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"smallest\")",
            "value": 172272918.7777778,
            "unit": "ns",
            "range": "± 109876.40795069822"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"smallest\")",
            "value": 1724547572,
            "unit": "ns",
            "range": "± 4645695.489774163"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"fastest\")",
            "value": 31737012.770833332,
            "unit": "ns",
            "range": "± 119562.0437668101"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"fastest\")",
            "value": 303005067.8333333,
            "unit": "ns",
            "range": "± 240741.3795301575"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"optimal\")",
            "value": 36376353.571428575,
            "unit": "ns",
            "range": "± 4644.1795237672"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"optimal\")",
            "value": 358701260,
            "unit": "ns",
            "range": "± 732382.1666363812"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"smallest\")",
            "value": 176849504,
            "unit": "ns",
            "range": "± 473772.520108895"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"smallest\")",
            "value": 1768544346.6666667,
            "unit": "ns",
            "range": "± 3712775.1601849436"
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
          "id": "dd73c997bc5a2034aa1dca0fe2b4a79b40bf1dcb",
          "message": "Merge pull request #185 from Chris-Wolfgang/vNext\n\nRelease v0.2.0 — zstd/LZ4 formats, hardened verification, supply-chain attestation",
          "timestamp": "2026-08-31T20:05:22-04:00",
          "tree_id": "930a5e0dd97bbbc57ad592e51d9140c35aff0e87",
          "url": "https://github.com/Chris-Wolfgang/Log-Compressor/commit/dd73c997bc5a2034aa1dca0fe2b4a79b40bf1dcb"
        },
        "date": 1788222314345,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"fastest\")",
            "value": 752245.2516276041,
            "unit": "ns",
            "range": "± 1287.854353204611"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"fastest\")",
            "value": 7785971.5625,
            "unit": "ns",
            "range": "± 77100.6058608494"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"optimal\")",
            "value": 11995605.046875,
            "unit": "ns",
            "range": "± 84981.72830028848"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"optimal\")",
            "value": 119084599.33333333,
            "unit": "ns",
            "range": "± 1038301.3720720868"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"smallest\")",
            "value": 182304205.2222222,
            "unit": "ns",
            "range": "± 1054792.297460731"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"smallest\")",
            "value": 2011421290.3333333,
            "unit": "ns",
            "range": "± 603449.0398710842"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"fastest\")",
            "value": 2433461.30859375,
            "unit": "ns",
            "range": "± 8646.651305256446"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"fastest\")",
            "value": 24900290.25,
            "unit": "ns",
            "range": "± 60102.98706730878"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"optimal\")",
            "value": 3019503.6940104165,
            "unit": "ns",
            "range": "± 9309.481799589985"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"optimal\")",
            "value": 30101483.59375,
            "unit": "ns",
            "range": "± 110692.06538354058"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"smallest\")",
            "value": 26297920.291666668,
            "unit": "ns",
            "range": "± 225387.2842645428"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"smallest\")",
            "value": 262243313.83333334,
            "unit": "ns",
            "range": "± 587061.5180205421"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"lz4\", Level: \"fastest\")",
            "value": 1390638.515625,
            "unit": "ns",
            "range": "± 4812.218268822603"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"lz4\", Level: \"fastest\")",
            "value": 9860986.171875,
            "unit": "ns",
            "range": "± 29078.70119944378"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"lz4\", Level: \"optimal\")",
            "value": 121134666.33333333,
            "unit": "ns",
            "range": "± 599390.5970152797"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"lz4\", Level: \"optimal\")",
            "value": 2242674640.6666665,
            "unit": "ns",
            "range": "± 3401476.872120305"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"lz4\", Level: \"smallest\")",
            "value": 294532591,
            "unit": "ns",
            "range": "± 697675.6238147138"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"lz4\", Level: \"smallest\")",
            "value": 2965084509.6666665,
            "unit": "ns",
            "range": "± 19571425.224406943"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"fastest\")",
            "value": 2706138.7513020835,
            "unit": "ns",
            "range": "± 38128.75746998251"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"fastest\")",
            "value": 28385797.489583332,
            "unit": "ns",
            "range": "± 309701.1553887636"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"optimal\")",
            "value": 3297295.875,
            "unit": "ns",
            "range": "± 12749.320756892532"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"optimal\")",
            "value": 32159138.416666668,
            "unit": "ns",
            "range": "± 119530.53030852512"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"smallest\")",
            "value": 26535629.15625,
            "unit": "ns",
            "range": "± 76601.00500038714"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"smallest\")",
            "value": 264570425.5,
            "unit": "ns",
            "range": "± 417008.2460824366"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zstd\", Level: \"fastest\")",
            "value": 1351254.2610677083,
            "unit": "ns",
            "range": "± 19177.91199208668"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zstd\", Level: \"fastest\")",
            "value": 12392934.333333334,
            "unit": "ns",
            "range": "± 1991056.6296341976"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zstd\", Level: \"optimal\")",
            "value": 1812015.3567708333,
            "unit": "ns",
            "range": "± 4373.485230444728"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zstd\", Level: \"optimal\")",
            "value": 18651162.34375,
            "unit": "ns",
            "range": "± 181485.57417607476"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zstd\", Level: \"smallest\")",
            "value": 18776822.427083332,
            "unit": "ns",
            "range": "± 77600.88023236101"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zstd\", Level: \"smallest\")",
            "value": 119191560.53333335,
            "unit": "ns",
            "range": "± 378147.6066300766"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"fastest\")",
            "value": 8638709.802083334,
            "unit": "ns",
            "range": "± 106832.2322900138"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"fastest\")",
            "value": 91464865.94444446,
            "unit": "ns",
            "range": "± 947332.5526713905"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"optimal\")",
            "value": 120088146.66666667,
            "unit": "ns",
            "range": "± 1694472.3453015867"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"optimal\")",
            "value": 1166099038.3333333,
            "unit": "ns",
            "range": "± 10543204.765601886"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"smallest\")",
            "value": 1958816239,
            "unit": "ns",
            "range": "± 21171192.087994974"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"smallest\")",
            "value": 19832881066.666668,
            "unit": "ns",
            "range": "± 42795382.12613766"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"fastest\")",
            "value": 26305671.15625,
            "unit": "ns",
            "range": "± 480303.05415895284"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"fastest\")",
            "value": 258016539,
            "unit": "ns",
            "range": "± 24983.168438971068"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"optimal\")",
            "value": 31529491.791666668,
            "unit": "ns",
            "range": "± 54800.92973151826"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"optimal\")",
            "value": 304512963.6666667,
            "unit": "ns",
            "range": "± 703725.6662173715"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"smallest\")",
            "value": 261499539,
            "unit": "ns",
            "range": "± 403477.52497289114"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"smallest\")",
            "value": 2615086775.3333335,
            "unit": "ns",
            "range": "± 1428286.7741760872"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"lz4\", Level: \"fastest\")",
            "value": 12441474.958333334,
            "unit": "ns",
            "range": "± 49706.39647930726"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"lz4\", Level: \"fastest\")",
            "value": 160032246.41666666,
            "unit": "ns",
            "range": "± 526555.4399999309"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"lz4\", Level: \"optimal\")",
            "value": 2209369840.6666665,
            "unit": "ns",
            "range": "± 1945240.592297604"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"lz4\", Level: \"optimal\")",
            "value": 12225639202.666666,
            "unit": "ns",
            "range": "± 26355005.896040816"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"lz4\", Level: \"smallest\")",
            "value": 5581200192.666667,
            "unit": "ns",
            "range": "± 33706231.578477725"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"lz4\", Level: \"smallest\")",
            "value": 34779580148.333336,
            "unit": "ns",
            "range": "± 65650928.13443871"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"fastest\")",
            "value": 29953466.083333332,
            "unit": "ns",
            "range": "± 115951.5934137931"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"fastest\")",
            "value": 295604313,
            "unit": "ns",
            "range": "± 372522.42226207"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"optimal\")",
            "value": 34491249.13333333,
            "unit": "ns",
            "range": "± 222965.08410697913"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"optimal\")",
            "value": 345238470.3333333,
            "unit": "ns",
            "range": "± 1265414.1069188905"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"smallest\")",
            "value": 265317995.16666666,
            "unit": "ns",
            "range": "± 236092.90413803913"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"smallest\")",
            "value": 2663444338.3333335,
            "unit": "ns",
            "range": "± 6406804.316200108"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zstd\", Level: \"fastest\")",
            "value": 13220145.651041666,
            "unit": "ns",
            "range": "± 285011.59786352736"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zstd\", Level: \"fastest\")",
            "value": 111828793.93333334,
            "unit": "ns",
            "range": "± 648776.548365625"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zstd\", Level: \"optimal\")",
            "value": 19430602.614583332,
            "unit": "ns",
            "range": "± 26440.101133053686"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zstd\", Level: \"optimal\")",
            "value": 228444355.55555555,
            "unit": "ns",
            "range": "± 535974.6906571487"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zstd\", Level: \"smallest\")",
            "value": 112956403.53333335,
            "unit": "ns",
            "range": "± 182697.23382922827"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zstd\", Level: \"smallest\")",
            "value": 1095018108.3333333,
            "unit": "ns",
            "range": "± 1901565.998048538"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressSingleFile(Format: \"brotli\")",
            "value": 755713.9270833334,
            "unit": "ns",
            "range": "± 3992.8489235928323"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressBundle(Format: \"brotli\")",
            "value": 2306003.9212239585,
            "unit": "ns",
            "range": "± 16478.072189877308"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressSingleFile(Format: \"gz\")",
            "value": 2455491.5963541665,
            "unit": "ns",
            "range": "± 6391.014159769699"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressBundle(Format: \"gz\")",
            "value": 7252186.888020833,
            "unit": "ns",
            "range": "± 23059.31072287679"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressSingleFile(Format: \"zip\")",
            "value": 2724789.5885416665,
            "unit": "ns",
            "range": "± 75427.11836799706"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressBundle(Format: \"zip\")",
            "value": 8097148.494791667,
            "unit": "ns",
            "range": "± 3156.8927408806794"
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
          "id": "944981173e93326b41959d7ce5205572194d8847",
          "message": "Merge pull request #199 from Chris-Wolfgang/vNext\n\nRelease v0.3.0 — decompress, naming controls, --on-error",
          "timestamp": "2026-09-01T19:57:01-04:00",
          "tree_id": "e081509412d1e12879b859e958c0e9392b8e9311",
          "url": "https://github.com/Chris-Wolfgang/Log-Compressor/commit/944981173e93326b41959d7ce5205572194d8847"
        },
        "date": 1788308478018,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"fastest\")",
            "value": 769742.9352213541,
            "unit": "ns",
            "range": "± 6250.2741052956335"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"fastest\")",
            "value": 7861685.973958333,
            "unit": "ns",
            "range": "± 16559.628944702057"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"optimal\")",
            "value": 12033947.75,
            "unit": "ns",
            "range": "± 38045.84393035974"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"optimal\")",
            "value": 118417402.86666667,
            "unit": "ns",
            "range": "± 265392.57249699906"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"smallest\")",
            "value": 185501523.11111113,
            "unit": "ns",
            "range": "± 586166.4508988691"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"smallest\")",
            "value": 2032554488,
            "unit": "ns",
            "range": "± 6718276.541854838"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"fastest\")",
            "value": 2426911.4205729165,
            "unit": "ns",
            "range": "± 16576.285941022685"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"fastest\")",
            "value": 25491500.28125,
            "unit": "ns",
            "range": "± 159669.15982112937"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"optimal\")",
            "value": 2959010.8033854165,
            "unit": "ns",
            "range": "± 22991.06752618031"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"optimal\")",
            "value": 30231990.59375,
            "unit": "ns",
            "range": "± 306847.3274457871"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"smallest\")",
            "value": 26381370.46875,
            "unit": "ns",
            "range": "± 85096.07405504693"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"smallest\")",
            "value": 290493304.8333333,
            "unit": "ns",
            "range": "± 2154097.22584412"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"lz4\", Level: \"fastest\")",
            "value": 1408748.72265625,
            "unit": "ns",
            "range": "± 4661.476113482525"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"lz4\", Level: \"fastest\")",
            "value": 9934417.713541666,
            "unit": "ns",
            "range": "± 99005.06491775672"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"lz4\", Level: \"optimal\")",
            "value": 220031686.33333334,
            "unit": "ns",
            "range": "± 196647.08499604417"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"lz4\", Level: \"optimal\")",
            "value": 2244165509,
            "unit": "ns",
            "range": "± 6848430.358548519"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"lz4\", Level: \"smallest\")",
            "value": 300360673,
            "unit": "ns",
            "range": "± 7106110.252606925"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"lz4\", Level: \"smallest\")",
            "value": 5558989683,
            "unit": "ns",
            "range": "± 12444968.972425"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"fastest\")",
            "value": 2713996.6829427085,
            "unit": "ns",
            "range": "± 5470.6667033459"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"fastest\")",
            "value": 28271327.34375,
            "unit": "ns",
            "range": "± 34880.37315153652"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"optimal\")",
            "value": 3289006.2578125,
            "unit": "ns",
            "range": "± 18628.23652060862"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"optimal\")",
            "value": 32843046.520833332,
            "unit": "ns",
            "range": "± 195973.3465938187"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"smallest\")",
            "value": 26790629.197916668,
            "unit": "ns",
            "range": "± 50834.59109977748"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"smallest\")",
            "value": 268196031.83333334,
            "unit": "ns",
            "range": "± 1595953.6314246673"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zstd\", Level: \"fastest\")",
            "value": 1266238.0390625,
            "unit": "ns",
            "range": "± 6692.956705199003"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zstd\", Level: \"fastest\")",
            "value": 10814624.833333334,
            "unit": "ns",
            "range": "± 87202.82481662846"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zstd\", Level: \"optimal\")",
            "value": 1884298.587890625,
            "unit": "ns",
            "range": "± 7534.827644592581"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zstd\", Level: \"optimal\")",
            "value": 22859553.78125,
            "unit": "ns",
            "range": "± 85886.84129714503"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zstd\", Level: \"smallest\")",
            "value": 20180109.239583332,
            "unit": "ns",
            "range": "± 148385.13140826536"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zstd\", Level: \"smallest\")",
            "value": 124663222.66666667,
            "unit": "ns",
            "range": "± 1247543.626497094"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"fastest\")",
            "value": 9432596.807291666,
            "unit": "ns",
            "range": "± 164762.15589939276"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"fastest\")",
            "value": 96139642.05555554,
            "unit": "ns",
            "range": "± 2031336.7599371788"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"optimal\")",
            "value": 119202410.13333333,
            "unit": "ns",
            "range": "± 417623.66766002745"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"optimal\")",
            "value": 1179301390.3333333,
            "unit": "ns",
            "range": "± 4671453.969876438"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"smallest\")",
            "value": 1990287541.6666667,
            "unit": "ns",
            "range": "± 1907633.180664022"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"smallest\")",
            "value": 19920812524.666668,
            "unit": "ns",
            "range": "± 112791057.51013127"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"fastest\")",
            "value": 25391654.005208332,
            "unit": "ns",
            "range": "± 39582.95186349629"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"fastest\")",
            "value": 247046080.55555555,
            "unit": "ns",
            "range": "± 571535.8831239432"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"optimal\")",
            "value": 31250848.229166668,
            "unit": "ns",
            "range": "± 125555.7714585051"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"optimal\")",
            "value": 308240378.5,
            "unit": "ns",
            "range": "± 1012116.0538864849"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"smallest\")",
            "value": 261851929,
            "unit": "ns",
            "range": "± 81954.03530638378"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"smallest\")",
            "value": 2604599532,
            "unit": "ns",
            "range": "± 2367958.0052688858"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"lz4\", Level: \"fastest\")",
            "value": 12028582.651041666,
            "unit": "ns",
            "range": "± 271333.0477550982"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"lz4\", Level: \"fastest\")",
            "value": 115574250.39999999,
            "unit": "ns",
            "range": "± 1003228.053277798"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"lz4\", Level: \"optimal\")",
            "value": 1242811755.6666667,
            "unit": "ns",
            "range": "± 5730774.271270361"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"lz4\", Level: \"optimal\")",
            "value": 22193609842.333332,
            "unit": "ns",
            "range": "± 17397451.59362946"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"lz4\", Level: \"smallest\")",
            "value": 5557587171.666667,
            "unit": "ns",
            "range": "± 3738099.933356027"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"lz4\", Level: \"smallest\")",
            "value": 55677070218.666664,
            "unit": "ns",
            "range": "± 61572312.73189374"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"fastest\")",
            "value": 29432572.302083332,
            "unit": "ns",
            "range": "± 76755.91719641187"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"fastest\")",
            "value": 293102961.8333333,
            "unit": "ns",
            "range": "± 1972067.8307378485"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"optimal\")",
            "value": 35047949.711111106,
            "unit": "ns",
            "range": "± 76209.12119504446"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"optimal\")",
            "value": 349090479.6666667,
            "unit": "ns",
            "range": "± 3810164.2463574107"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"smallest\")",
            "value": 265404712.5,
            "unit": "ns",
            "range": "± 596147.1861006726"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"smallest\")",
            "value": 2662959358,
            "unit": "ns",
            "range": "± 686197.3961281113"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zstd\", Level: \"fastest\")",
            "value": 11033540.171875,
            "unit": "ns",
            "range": "± 118413.68976639501"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zstd\", Level: \"fastest\")",
            "value": 122565561.58333333,
            "unit": "ns",
            "range": "± 842946.9723210771"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zstd\", Level: \"optimal\")",
            "value": 19110022.666666668,
            "unit": "ns",
            "range": "± 55635.62110060717"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zstd\", Level: \"optimal\")",
            "value": 191946750.2222222,
            "unit": "ns",
            "range": "± 861090.3050823549"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zstd\", Level: \"smallest\")",
            "value": 114277815,
            "unit": "ns",
            "range": "± 1144902.4488572138"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zstd\", Level: \"smallest\")",
            "value": 1091787472.6666667,
            "unit": "ns",
            "range": "± 8174663.761495535"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressSingleFile(Format: \"brotli\")",
            "value": 764073.1881510416,
            "unit": "ns",
            "range": "± 1535.2471225220615"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressBundle(Format: \"brotli\")",
            "value": 2346737.6953125,
            "unit": "ns",
            "range": "± 13989.493082832376"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressSingleFile(Format: \"gz\")",
            "value": 2424564.5494791665,
            "unit": "ns",
            "range": "± 15536.464185729159"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressBundle(Format: \"gz\")",
            "value": 7283634.138020833,
            "unit": "ns",
            "range": "± 63309.42130774594"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressSingleFile(Format: \"zip\")",
            "value": 2708840.5638020835,
            "unit": "ns",
            "range": "± 8810.801142752"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressBundle(Format: \"zip\")",
            "value": 8225097.307291667,
            "unit": "ns",
            "range": "± 45545.92462487879"
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
          "id": "b7fbacff463bed747fe469b818faab6acfc4a607",
          "message": "Merge pull request #200 from Chris-Wolfgang/protected/alert-sha-pinning\n\nci: SHA-pin all 46 action refs + fix 5 template-injection findings",
          "timestamp": "2026-09-01T20:42:10-04:00",
          "tree_id": "18ab7ab44253d3c9206203891ffd1aa958a6030d",
          "url": "https://github.com/Chris-Wolfgang/Log-Compressor/commit/b7fbacff463bed747fe469b818faab6acfc4a607"
        },
        "date": 1788310696581,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"fastest\")",
            "value": 710268.2013346354,
            "unit": "ns",
            "range": "± 36594.03359030402"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"fastest\")",
            "value": 7030542.453125,
            "unit": "ns",
            "range": "± 65214.20335858411"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"optimal\")",
            "value": 8096408.981770833,
            "unit": "ns",
            "range": "± 6036.376605103103"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"optimal\")",
            "value": 80538214.61904761,
            "unit": "ns",
            "range": "± 53577.20923256497"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"smallest\")",
            "value": 128907619.83333333,
            "unit": "ns",
            "range": "± 1081587.159774275"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"smallest\")",
            "value": 1338854811.6666667,
            "unit": "ns",
            "range": "± 4775417.301062739"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"fastest\")",
            "value": 1783259.6923828125,
            "unit": "ns",
            "range": "± 1301.731268407868"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"fastest\")",
            "value": 18102744.619791668,
            "unit": "ns",
            "range": "± 80402.99148977734"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"optimal\")",
            "value": 2280217.6634114585,
            "unit": "ns",
            "range": "± 67745.9511317973"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"optimal\")",
            "value": 22496449.546875,
            "unit": "ns",
            "range": "± 66958.92451306464"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"smallest\")",
            "value": 10776539.057291666,
            "unit": "ns",
            "range": "± 580522.7777686383"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"smallest\")",
            "value": 106547658.06666668,
            "unit": "ns",
            "range": "± 3995199.3976962557"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"lz4\", Level: \"fastest\")",
            "value": 1000494.5035807291,
            "unit": "ns",
            "range": "± 32504.6330728525"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"lz4\", Level: \"fastest\")",
            "value": 9803508.802083334,
            "unit": "ns",
            "range": "± 22248.35823588932"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"lz4\", Level: \"optimal\")",
            "value": 101442162.73333333,
            "unit": "ns",
            "range": "± 802414.8180011262"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"lz4\", Level: \"optimal\")",
            "value": 1100429242.3333333,
            "unit": "ns",
            "range": "± 9985393.138159826"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"lz4\", Level: \"smallest\")",
            "value": 271129692.5,
            "unit": "ns",
            "range": "± 340509.6422118616"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"lz4\", Level: \"smallest\")",
            "value": 2784215811,
            "unit": "ns",
            "range": "± 67789856.9002655"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"fastest\")",
            "value": 2125983.900390625,
            "unit": "ns",
            "range": "± 2817.7646189000643"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"fastest\")",
            "value": 21779615.526041668,
            "unit": "ns",
            "range": "± 8305.00104002038"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"optimal\")",
            "value": 2501461.3209635415,
            "unit": "ns",
            "range": "± 5137.353889129335"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"optimal\")",
            "value": 26853847.708333332,
            "unit": "ns",
            "range": "± 151993.90237746708"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"smallest\")",
            "value": 10823996.768229166,
            "unit": "ns",
            "range": "± 15636.255447251508"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"smallest\")",
            "value": 112945109.93333334,
            "unit": "ns",
            "range": "± 4015061.7000118606"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zstd\", Level: \"fastest\")",
            "value": 832530.474609375,
            "unit": "ns",
            "range": "± 399.25609401478226"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zstd\", Level: \"fastest\")",
            "value": 8896811.114583334,
            "unit": "ns",
            "range": "± 217810.95814493185"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zstd\", Level: \"optimal\")",
            "value": 1844901.6569010417,
            "unit": "ns",
            "range": "± 9763.707888617633"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zstd\", Level: \"optimal\")",
            "value": 18786529.885416668,
            "unit": "ns",
            "range": "± 4112.594463240391"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zstd\", Level: \"smallest\")",
            "value": 14297609.708333334,
            "unit": "ns",
            "range": "± 124411.79707134515"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zstd\", Level: \"smallest\")",
            "value": 89387189.27777778,
            "unit": "ns",
            "range": "± 2960714.40518442"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"fastest\")",
            "value": 7586321.15625,
            "unit": "ns",
            "range": "± 431601.38917716476"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"fastest\")",
            "value": 77350731.22222222,
            "unit": "ns",
            "range": "± 607829.0678813531"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"optimal\")",
            "value": 81948443.28571428,
            "unit": "ns",
            "range": "± 215975.47255757992"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"optimal\")",
            "value": 856249951,
            "unit": "ns",
            "range": "± 36909647.62590002"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"smallest\")",
            "value": 2422165971.6666665,
            "unit": "ns",
            "range": "± 31535476.33385756"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"smallest\")",
            "value": 12986630943.666666,
            "unit": "ns",
            "range": "± 136780327.7413688"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"fastest\")",
            "value": 19645894,
            "unit": "ns",
            "range": "± 388944.63111113233"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"fastest\")",
            "value": 194383726.33333334,
            "unit": "ns",
            "range": "± 5109758.800307115"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"optimal\")",
            "value": 23345377.052083332,
            "unit": "ns",
            "range": "± 85724.4005351947"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"optimal\")",
            "value": 231811099.55555558,
            "unit": "ns",
            "range": "± 5581917.979631412"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"smallest\")",
            "value": 105591336.39999999,
            "unit": "ns",
            "range": "± 324552.04773835494"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"smallest\")",
            "value": 1462330962.6666667,
            "unit": "ns",
            "range": "± 1045539.8389933945"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"lz4\", Level: \"fastest\")",
            "value": 10060768.859375,
            "unit": "ns",
            "range": "± 35262.977005937675"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"lz4\", Level: \"fastest\")",
            "value": 101032619.33333333,
            "unit": "ns",
            "range": "± 492405.00017674157"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"lz4\", Level: \"optimal\")",
            "value": 1017266106.3333334,
            "unit": "ns",
            "range": "± 649612.2365537254"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"lz4\", Level: \"optimal\")",
            "value": 10672157964.666666,
            "unit": "ns",
            "range": "± 37278566.87968456"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"lz4\", Level: \"smallest\")",
            "value": 2732046609.6666665,
            "unit": "ns",
            "range": "± 3458071.1746176267"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"lz4\", Level: \"smallest\")",
            "value": 27761547312.666668,
            "unit": "ns",
            "range": "± 121646690.24178813"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"fastest\")",
            "value": 25073222.71875,
            "unit": "ns",
            "range": "± 528588.8201346218"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"fastest\")",
            "value": 247778169.44444445,
            "unit": "ns",
            "range": "± 1440708.172674667"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"optimal\")",
            "value": 28609762.385416668,
            "unit": "ns",
            "range": "± 1006433.3500788432"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"optimal\")",
            "value": 285018170,
            "unit": "ns",
            "range": "± 2202048.95039705"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"smallest\")",
            "value": 118650090.26666665,
            "unit": "ns",
            "range": "± 1922502.4058142984"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"smallest\")",
            "value": 1228621019.6666667,
            "unit": "ns",
            "range": "± 67314501.05882819"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zstd\", Level: \"fastest\")",
            "value": 8871341.932291666,
            "unit": "ns",
            "range": "± 89465.0349313854"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zstd\", Level: \"fastest\")",
            "value": 92999399.72222222,
            "unit": "ns",
            "range": "± 380055.78061524587"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zstd\", Level: \"optimal\")",
            "value": 18619409.197916668,
            "unit": "ns",
            "range": "± 62542.19047721166"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zstd\", Level: \"optimal\")",
            "value": 185592353.77777776,
            "unit": "ns",
            "range": "± 1959665.2964245516"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zstd\", Level: \"smallest\")",
            "value": 88235444.1111111,
            "unit": "ns",
            "range": "± 255315.86760593453"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zstd\", Level: \"smallest\")",
            "value": 771782846,
            "unit": "ns",
            "range": "± 2036923.708373242"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressSingleFile(Format: \"brotli\")",
            "value": 687844.6331380209,
            "unit": "ns",
            "range": "± 1870.91418141665"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressBundle(Format: \"brotli\")",
            "value": 2334226.697265625,
            "unit": "ns",
            "range": "± 53474.17617257955"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressSingleFile(Format: \"gz\")",
            "value": 1835338.884765625,
            "unit": "ns",
            "range": "± 116002.11007459986"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressBundle(Format: \"gz\")",
            "value": 5549246.643229167,
            "unit": "ns",
            "range": "± 168190.73405807588"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressSingleFile(Format: \"zip\")",
            "value": 2184645.1165364585,
            "unit": "ns",
            "range": "± 45796.26855870239"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressBundle(Format: \"zip\")",
            "value": 6856389.201822917,
            "unit": "ns",
            "range": "± 279692.06447758275"
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
          "id": "74ffebea203710fc778cdbcfe9bfa657385048b8",
          "message": "Merge pull request #208 from Chris-Wolfgang/vNext\n\nRelease v0.3.1 — empty-file archives, retry backoff, alert clean-sweep",
          "timestamp": "2026-09-03T11:20:10-04:00",
          "tree_id": "db18e16dc3efc5b08f01a280501c78f0a90bb923",
          "url": "https://github.com/Chris-Wolfgang/Log-Compressor/commit/74ffebea203710fc778cdbcfe9bfa657385048b8"
        },
        "date": 1788449829588,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"fastest\")",
            "value": 704866.2594401041,
            "unit": "ns",
            "range": "± 8366.139633028117"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"fastest\")",
            "value": 7549572.611979167,
            "unit": "ns",
            "range": "± 274176.93393657694"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"optimal\")",
            "value": 8288599.4375,
            "unit": "ns",
            "range": "± 11457.415779698184"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"optimal\")",
            "value": 82068238.47619046,
            "unit": "ns",
            "range": "± 545969.7006159367"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"brotli\", Level: \"smallest\")",
            "value": 134493495,
            "unit": "ns",
            "range": "± 114679.77500522924"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"brotli\", Level: \"smallest\")",
            "value": 1388867242,
            "unit": "ns",
            "range": "± 1452676.1419246204"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"fastest\")",
            "value": 1818524.6236979167,
            "unit": "ns",
            "range": "± 19137.442138406033"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"fastest\")",
            "value": 19093990.552083332,
            "unit": "ns",
            "range": "± 558182.26112145"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"optimal\")",
            "value": 2278163.5501302085,
            "unit": "ns",
            "range": "± 63433.00989447183"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"optimal\")",
            "value": 22540329.46875,
            "unit": "ns",
            "range": "± 67663.20934157178"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"gz\", Level: \"smallest\")",
            "value": 10709247.778645834,
            "unit": "ns",
            "range": "± 59700.142413471694"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"gz\", Level: \"smallest\")",
            "value": 115462227.53333335,
            "unit": "ns",
            "range": "± 3047349.103220992"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"lz4\", Level: \"fastest\")",
            "value": 995284.3444010416,
            "unit": "ns",
            "range": "± 15985.348221191543"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"lz4\", Level: \"fastest\")",
            "value": 9919133.385416666,
            "unit": "ns",
            "range": "± 11718.684089484786"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"lz4\", Level: \"optimal\")",
            "value": 103246857.60000001,
            "unit": "ns",
            "range": "± 336241.72951060056"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"lz4\", Level: \"optimal\")",
            "value": 1055512203.3333334,
            "unit": "ns",
            "range": "± 974498.5619883352"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"lz4\", Level: \"smallest\")",
            "value": 275392189.5,
            "unit": "ns",
            "range": "± 1271430.6778402235"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"lz4\", Level: \"smallest\")",
            "value": 2800249722.3333335,
            "unit": "ns",
            "range": "± 47701717.60624272"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"fastest\")",
            "value": 2258312.5240885415,
            "unit": "ns",
            "range": "± 74325.63037927794"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"fastest\")",
            "value": 22127721.703125,
            "unit": "ns",
            "range": "± 150074.75015777294"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"optimal\")",
            "value": 2595754.2428385415,
            "unit": "ns",
            "range": "± 8205.250908798505"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"optimal\")",
            "value": 26004550.25,
            "unit": "ns",
            "range": "± 109594.19826063605"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zip\", Level: \"smallest\")",
            "value": 11172769.018229166,
            "unit": "ns",
            "range": "± 8897.789669162978"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zip\", Level: \"smallest\")",
            "value": 111674907,
            "unit": "ns",
            "range": "± 798326.176435875"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zstd\", Level: \"fastest\")",
            "value": 846248.5623372396,
            "unit": "ns",
            "range": "± 8353.229973879188"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zstd\", Level: \"fastest\")",
            "value": 9373706.53125,
            "unit": "ns",
            "range": "± 311803.9624667087"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zstd\", Level: \"optimal\")",
            "value": 2033632.9557291667,
            "unit": "ns",
            "range": "± 16953.65219415803"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zstd\", Level: \"optimal\")",
            "value": 18923664.427083332,
            "unit": "ns",
            "range": "± 132965.40750642237"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 10485760, Format: \"zstd\", Level: \"smallest\")",
            "value": 18775525.6875,
            "unit": "ns",
            "range": "± 682183.3595000743"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 10485760, Format: \"zstd\", Level: \"smallest\")",
            "value": 89473503.6111111,
            "unit": "ns",
            "range": "± 238504.0384257618"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"fastest\")",
            "value": 12928332.864583334,
            "unit": "ns",
            "range": "± 533458.234829194"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"fastest\")",
            "value": 123118382.33333333,
            "unit": "ns",
            "range": "± 2040677.1319991988"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"optimal\")",
            "value": 90347065.33333333,
            "unit": "ns",
            "range": "± 4589395.869307755"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"optimal\")",
            "value": 843307046,
            "unit": "ns",
            "range": "± 5709957.881417428"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"brotli\", Level: \"smallest\")",
            "value": 2490231502.6666665,
            "unit": "ns",
            "range": "± 25645346.200885694"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"brotli\", Level: \"smallest\")",
            "value": 13382716082.333334,
            "unit": "ns",
            "range": "± 293118013.13117707"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"fastest\")",
            "value": 23707697.036458332,
            "unit": "ns",
            "range": "± 635640.6722547074"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"fastest\")",
            "value": 231852921.88888887,
            "unit": "ns",
            "range": "± 3764950.3300522356"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"optimal\")",
            "value": 26593373.234375,
            "unit": "ns",
            "range": "± 33318.76148531093"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"optimal\")",
            "value": 269702125.6666667,
            "unit": "ns",
            "range": "± 9878644.356283689"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"gz\", Level: \"smallest\")",
            "value": 111917289.86666667,
            "unit": "ns",
            "range": "± 54028.098721439164"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"gz\", Level: \"smallest\")",
            "value": 1117951861.3333333,
            "unit": "ns",
            "range": "± 10197404.141350403"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"lz4\", Level: \"fastest\")",
            "value": 12583074.078125,
            "unit": "ns",
            "range": "± 321844.1502491507"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"lz4\", Level: \"fastest\")",
            "value": 122026529.41666667,
            "unit": "ns",
            "range": "± 1552607.3290668253"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"lz4\", Level: \"optimal\")",
            "value": 1170350188.6666667,
            "unit": "ns",
            "range": "± 13826596.852591036"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"lz4\", Level: \"optimal\")",
            "value": 10640969690.666666,
            "unit": "ns",
            "range": "± 163779385.72788846"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"lz4\", Level: \"smallest\")",
            "value": 2939907898.6666665,
            "unit": "ns",
            "range": "± 169669997.41085562"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"lz4\", Level: \"smallest\")",
            "value": 28432438854.333332,
            "unit": "ns",
            "range": "± 170845969.09690654"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"fastest\")",
            "value": 25873026.041666668,
            "unit": "ns",
            "range": "± 212026.33687860225"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"fastest\")",
            "value": 285047363.1666667,
            "unit": "ns",
            "range": "± 10423232.828037882"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"optimal\")",
            "value": 32716095.2,
            "unit": "ns",
            "range": "± 1137392.4625974642"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"optimal\")",
            "value": 322936688,
            "unit": "ns",
            "range": "± 1808141.117691592"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zip\", Level: \"smallest\")",
            "value": 121046561.33333333,
            "unit": "ns",
            "range": "± 296068.5564510707"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zip\", Level: \"smallest\")",
            "value": 1210275061,
            "unit": "ns",
            "range": "± 18695908.28953279"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zstd\", Level: \"fastest\")",
            "value": 10207194.036458334,
            "unit": "ns",
            "range": "± 784124.854038718"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zstd\", Level: \"fastest\")",
            "value": 98691293.8888889,
            "unit": "ns",
            "range": "± 4573225.483739651"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zstd\", Level: \"optimal\")",
            "value": 20361993.78125,
            "unit": "ns",
            "range": "± 468723.6687672314"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zstd\", Level: \"optimal\")",
            "value": 211247877.66666666,
            "unit": "ns",
            "range": "± 2415979.7876991853"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressSingleFile(FileSize: 104857600, Format: \"zstd\", Level: \"smallest\")",
            "value": 88920178.8888889,
            "unit": "ns",
            "range": "± 542740.6340053116"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.CompressionBenchmarks.CompressBundle(FileSize: 104857600, Format: \"zstd\", Level: \"smallest\")",
            "value": 780423927.3333334,
            "unit": "ns",
            "range": "± 6187986.093229472"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressSingleFile(Format: \"brotli\")",
            "value": 703870.7864583334,
            "unit": "ns",
            "range": "± 4680.22743773559"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressBundle(Format: \"brotli\")",
            "value": 2131601.66796875,
            "unit": "ns",
            "range": "± 48717.30920645899"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressSingleFile(Format: \"gz\")",
            "value": 1812268.88671875,
            "unit": "ns",
            "range": "± 7265.88872790114"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressBundle(Format: \"gz\")",
            "value": 5565947.223958333,
            "unit": "ns",
            "range": "± 103792.98400748835"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressSingleFile(Format: \"zip\")",
            "value": 2170486.609375,
            "unit": "ns",
            "range": "± 7148.095359758538"
          },
          {
            "name": "Wolfgang.LogCompressor.Benchmarks.PerfSmokeBenchmarks.CompressBundle(Format: \"zip\")",
            "value": 6662367.401041667,
            "unit": "ns",
            "range": "± 172665.28527399202"
          }
        ]
      }
    ]
  }
}