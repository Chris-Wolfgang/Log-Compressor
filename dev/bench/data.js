window.BENCHMARK_DATA = {
  "lastUpdate": 1781822509206,
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
      }
    ]
  }
}