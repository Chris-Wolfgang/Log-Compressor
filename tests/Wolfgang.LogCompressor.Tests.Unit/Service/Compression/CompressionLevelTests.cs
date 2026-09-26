using System.IO.Compression;
using System.Text;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor.Tests.Unit.Service.Compression;

public sealed class CompressionLevelTests
{
    // Realistic, compressible log text: repetitive structure with varying numbers,
    // large enough that the fastest and smallest levels of every codec diverge.
    private static readonly byte[] LogText = Encoding.UTF8.GetBytes
    (
        string.Concat
        (
            Enumerable
                .Range(0, 2000)
                .Select(i => $"2026-09-26 12:{i % 60:D2}:{i * 7 % 60:D2} INFO request {i * 31 % 997} served in {i * 13 % 250} ms\n")
        )
    );



    [Theory]
    [InlineData("zip")]
    [InlineData("gz")]
    [InlineData("br")]
    [InlineData("zst")]
    [InlineData("lz4")]
    public async Task CompressFileAsync_when_smallestSizeLevel_expected_smallerThanFastest(string format)
    {
        var fastest = await CompressAsync(Create(format, CompressionLevel.Fastest));
        var smallest = await CompressAsync(Create(format, CompressionLevel.SmallestSize));

        // A strategy that ignored its constructor's level would produce the
        // same size for both.
        Assert.True
        (
            smallest < fastest,
            $"{format}: SmallestSize produced {smallest} bytes, Fastest {fastest}"
        );
    }



    private static ICompressionStrategy Create(string format, CompressionLevel level) => format switch
    {
        "zip" => new ZipCompressionStrategy(level),
        "gz" => new GZipCompressionStrategy(level),
        "br" => new BrotliCompressionStrategy(level),
        "zst" => new ZstdCompressionStrategy(level),
        _ => new Lz4CompressionStrategy(level)
    };



    private static async Task<long> CompressAsync(ICompressionStrategy strategy)
    {
        using var input = new MemoryStream(LogText);
        using var output = new MemoryStream();

        await strategy.CompressFileAsync(input, output, "app.log");

        return output.Length;
    }
}
