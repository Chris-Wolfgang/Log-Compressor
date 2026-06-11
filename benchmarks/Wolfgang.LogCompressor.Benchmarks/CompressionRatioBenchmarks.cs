using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor.Benchmarks;

/// <summary>
/// Measures compressed output size, compression ratio, and throughput for each format and level.
/// Outputs a markdown table suitable for the README.
/// </summary>
public static class CompressionRatioBenchmarks
{
    private static readonly (string Name, CompressionFormat Format)[] Formats =
    [
        ("ZIP", CompressionFormat.Zip),
        ("GZip", CompressionFormat.Gz),
        ("Brotli", CompressionFormat.Brotli)
    ];

    private static readonly (string Name, CompressionLevel Level)[] Levels =
    [
        ("Fastest", CompressionLevel.Fastest),
        ("Optimal", CompressionLevel.Optimal),
        ("Smallest", CompressionLevel.SmallestSize)
    ];

    private static readonly int[] FileSizes = [10_485_760, 104_857_600];



    /// <summary>
    /// Runs the compression ratio measurements and outputs a markdown table.
    /// </summary>
    public static async Task RunAsync()
    {
        var factory = new CompressionStrategyFactory();

        Console.WriteLine("## Compression Comparison");
        Console.WriteLine();
        Console.WriteLine("| Format | Level | File Size | Compressed | Ratio | Speed (MB/s) |");
        Console.WriteLine("|--------|-------|-----------|------------|-------|-------------|");

        foreach (var fileSize in FileSizes)
        {
            var testData = GenerateTestData(fileSize);
            var fileSizeLabel = FormatSize(fileSize);

            foreach (var (formatName, format) in Formats)
            {
                foreach (var (levelName, level) in Levels)
                {
                    var strategy = factory.Create(format, level);

                    using var input = new MemoryStream(testData);
                    using var output = new MemoryStream();

                    var sw = Stopwatch.StartNew();
                    await strategy.CompressFileAsync(input, output, "benchmark.log");
                    sw.Stop();

                    var compressedSize = output.Length;
                    var ratio = (double)compressedSize / fileSize * 100;
                    var speedMbPerSec = fileSize / 1_048_576.0 / sw.Elapsed.TotalSeconds;

                    Console.WriteLine
                    (
                        $"| {formatName,-6} | {levelName,-7} | {fileSizeLabel,9} | {FormatSize(compressedSize),10} | {ratio,4:F1}% | {speedMbPerSec,11:F1} |"
                    );
                }
            }
        }

        Console.WriteLine();
        Console.WriteLine($"*Measured on {Environment.MachineName}, .NET {Environment.Version}, {DateTimeOffset.Now:yyyy-MM-dd}*");
    }



    private static byte[] GenerateTestData(int size)
    {
        var line = "2026-03-15 23:00:15.123 [INF] Processing request id=abc123 method=GET path=/api/data duration=42ms\n";
        var sb = new StringBuilder(size);
        while (sb.Length < size)
        {
            sb.Append(line);
        }

        return Encoding.UTF8.GetBytes(sb.ToString(0, size));
    }



    private static string FormatSize(long bytes)
    {
        return bytes switch
        {
            >= 1_048_576 => $"{bytes / 1_048_576.0:F1} MB",
            >= 1024 => $"{bytes / 1024.0:F1} KB",
            _ => $"{bytes} B"
        };
    }
}
