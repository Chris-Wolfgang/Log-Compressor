using System.IO.Compression;
using System.Text;
using BenchmarkDotNet.Attributes;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor.Benchmarks;

/// <summary>
/// Benchmarks for comparing compression strategies across file sizes, formats, and compression levels.
/// </summary>
[MemoryDiagnoser]
public class CompressionBenchmarks
{
    private byte[] _testData = [];
    private ICompressionStrategy _strategy = null!;
    private readonly CompressionStrategyFactory _factory = new();



    /// <summary>
    /// Gets or sets the simulated file size in bytes.
    /// </summary>
    [Params(10_485_760, 104_857_600)]
    public int FileSize { get; set; }



    /// <summary>
    /// Gets or sets the compression format name to benchmark.
    /// </summary>
    [Params("zip", "gz", "brotli")]
    public string Format { get; set; } = "zip";



    /// <summary>
    /// Gets or sets the compression level to benchmark.
    /// </summary>
    [Params("fastest", "optimal", "smallest")]
    public string Level { get; set; } = "smallest";



    /// <summary>
    /// Generates repeating log-like text data and initializes the compression strategy.
    /// </summary>
    [GlobalSetup]
    public void Setup()
    {
        var format = Format switch
        {
            "zip" => CompressionFormat.Zip,
            "gz" => CompressionFormat.Gz,
            "brotli" => CompressionFormat.Brotli,
            _ => CompressionFormat.Zip
        };

        var level = Level switch
        {
            "fastest" => CompressionLevel.Fastest,
            "optimal" => CompressionLevel.Optimal,
            "smallest" => CompressionLevel.SmallestSize,
            _ => CompressionLevel.SmallestSize
        };

        _strategy = _factory.Create(format, level);

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
    /// Benchmarks compressing 10 files into a bundle.
    /// </summary>
    [Benchmark]
    public async Task CompressBundle()
    {
        var inputs = new List<(Stream Stream, string EntryName)>(10);

        for (var i = 0; i < 10; i++)
        {
            inputs.Add((new MemoryStream(_testData), $"file{i}.log"));
        }

        using var output = new MemoryStream();
        await _strategy.CompressFilesAsync(inputs, output);

        foreach (var (stream, _) in inputs)
        {
            await stream.DisposeAsync();
        }
    }
}
