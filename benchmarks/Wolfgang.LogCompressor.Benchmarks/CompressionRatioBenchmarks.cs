using System.Diagnostics;
using System.Formats.Tar;
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
        ("Brotli", CompressionFormat.Brotli),
        ("Zstd", CompressionFormat.Zstd),
        ("LZ4", CompressionFormat.Lz4)
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

        // Warm up every strategy's compress and decompress paths so the first
        // measured row doesn't pay the JIT/tiering cost.
        var warmupData = GenerateTestData(1_048_576);
        foreach (var (_, format) in Formats)
        {
            var strategy = factory.Create(format, CompressionLevel.Fastest);
            using var input = new MemoryStream(warmupData);
            using var output = new MemoryStream();
            await strategy.CompressFileAsync(input, output, "warmup.log");
            Decompress(format, output.ToArray());
        }

        Console.WriteLine("## Compression Comparison");
        Console.WriteLine();
        Console.WriteLine("| Format | Level | File Size | Compressed | Ratio | Compress (MB/s) | Decompress (MB/s) |");
        Console.WriteLine("|--------|-------|-----------|------------|-------|-----------------|-------------------|");

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

                    var swDecompress = Stopwatch.StartNew();
                    Decompress(format, output.ToArray());
                    swDecompress.Stop();
                    var decompressMbPerSec = fileSize / 1_048_576.0 / swDecompress.Elapsed.TotalSeconds;

                    Console.WriteLine
                    (
                        $"| {formatName,-6} | {levelName,-7} | {fileSizeLabel,9} | {FormatSize(compressedSize),10} | {ratio,4:F1}% | {speedMbPerSec,15:F1} | {decompressMbPerSec,17:F1} |"
                    );
                }
            }
        }

        Console.WriteLine();
        Console.WriteLine($"*Measured on {System.Runtime.InteropServices.RuntimeInformation.OSDescription}, .NET {Environment.Version}, {DateTimeOffset.Now:yyyy-MM-dd}. Single-file compression of synthetic realistic log text (varied timestamps/ids/paths, deterministically seeded); decompression via the same libraries' read paths.*");
    }



    private static void Decompress(CompressionFormat format, byte[] archive)
    {
        using var input = new MemoryStream(archive);
        Stream reader = format switch
        {
            CompressionFormat.Zip => new ZipArchive(input, ZipArchiveMode.Read).Entries[0].Open(),
            CompressionFormat.Gz => new GZipStream(input, CompressionMode.Decompress),
            CompressionFormat.Brotli => new BrotliStream(input, CompressionMode.Decompress),
            CompressionFormat.Zstd => new ZstdSharp.DecompressionStream(input),
            CompressionFormat.Lz4 => K4os.Compression.LZ4.Streams.LZ4Stream.Decode(input),
            _ => throw new ArgumentOutOfRangeException(nameof(format))
        };

        using (reader)
        {
            reader.CopyTo(Stream.Null);
        }
    }



    private static byte[] GenerateTestData(int size)
    {
        // Realistic varied log text, deterministically seeded. A single
        // repeated line is pathologically compressible (brotli shrinks 100 MB
        // of it to ~250 bytes) and produces ratio/speed numbers that mislead
        // rather than guide format choice.
        var random = new Random(42);
        string[] levels = ["INF", "INF", "INF", "INF", "DBG", "DBG", "WRN", "ERR"];
        string[] methods = ["GET", "POST", "PUT", "DELETE"];
        string[] paths =
        [
            "/api/data", "/api/users", "/api/orders/items", "/health",
            "/api/reports/daily", "/api/inventory", "/auth/token", "/api/search"
        ];

        var timestamp = new DateTime(2026, 3, 15, 23, 0, 15, 123);
        var sb = new StringBuilder(size + 256);

        while (sb.Length < size)
        {
            timestamp = timestamp.AddMilliseconds(random.Next(1, 2500));
            var level = levels[random.Next(levels.Length)];
            sb.Append(timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture))
              .Append(" [").Append(level).Append("] Processing request id=")
              .Append(random.Next(int.MaxValue).ToString("x8", System.Globalization.CultureInfo.InvariantCulture))
              .Append(" method=").Append(methods[random.Next(methods.Length)])
              .Append(" path=").Append(paths[random.Next(paths.Length)])
              .Append(" status=").Append(random.Next(10) == 0 ? 500 : 200)
              .Append(" duration=").Append(random.Next(1, 1200)).Append("ms\n");

            if (string.Equals(level, "ERR", StringComparison.Ordinal))
            {
                sb.Append("   at Wolfgang.Sample.Api.Handler.Process(Request request) in /src/Handler.cs:line ")
                  .Append(random.Next(20, 400)).Append('\n');
            }
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
