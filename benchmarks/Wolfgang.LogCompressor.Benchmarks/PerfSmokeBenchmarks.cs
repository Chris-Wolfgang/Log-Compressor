using System.Text;
using BenchmarkDotNet.Attributes;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor.Benchmarks;

/// <summary>
/// The curated fast subset used by the per-PR benchmark-delta workflow
/// (pr-benchmarks.yaml, #92): 10 MB of log-like data, every format, fastest
/// level only, so HEAD and merge-base both finish in minutes. The full matrix
/// (100 MB, optimal/smallest levels) stays in <see cref="CompressionBenchmarks"/>
/// for the weekly gh-pages chart. Allocation numbers from these cases are the
/// per-PR hard gate; wall-clock is advisory (shared-runner noise).
/// </summary>
[MemoryDiagnoser]
public class PerfSmokeBenchmarks
{
    private const int FileSize = 10_485_760;

    private byte[] _testData = [];
    private ICompressionStrategy _strategy = null!;
    private readonly CompressionStrategyFactory _factory = new();



    /// <summary>
    /// Gets or sets the compression format name to benchmark.
    /// </summary>
    [Params("zip", "gz", "brotli")]
    public string Format { get; set; } = "zip";



    /// <summary>
    /// Generates repeating log-like text data and initializes the compression strategy.
    /// </summary>
    [GlobalSetup]
    public void Setup()
    {
        var format = Format switch
        {
            "gz" => CompressionFormat.Gz,
            "brotli" => CompressionFormat.Brotli,
            _ => CompressionFormat.Zip
        };

        _strategy = _factory.Create(format, System.IO.Compression.CompressionLevel.Fastest);

        var line = "2026-03-15 23:00:15.123 [INF] Processing request id=abc123 method=GET path=/api/data duration=42ms\n";
        var sb = new StringBuilder(FileSize);
        while (sb.Length < FileSize)
        {
            sb.Append(line);
        }

        _testData = Encoding.UTF8.GetBytes(sb.ToString(0, FileSize));
    }



    /// <summary>
    /// Benchmarks compressing a single file.
    /// </summary>
    [Benchmark]
    public async Task CompressSingleFile()
    {
        using var input = new MemoryStream(_testData);
        using var output = new MemoryStream();
        await _strategy.CompressFileAsync(input, output, "benchmark.log");
    }



    /// <summary>
    /// Benchmarks compressing three files into a bundle.
    /// </summary>
    [Benchmark]
    public async Task CompressBundle()
    {
        var inputs = new List<(Stream Stream, string EntryName)>(3);

        for (var i = 0; i < 3; i++)
        {
            inputs.Add((new MemoryStream(_testData), $"file{i}.log"));
        }

        using var output = new MemoryStream();
        await _strategy.CompressFilesAsync(inputs, output);
    }
}
