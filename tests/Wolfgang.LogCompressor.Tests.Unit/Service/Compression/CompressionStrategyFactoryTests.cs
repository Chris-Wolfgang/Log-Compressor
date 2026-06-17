using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor.Tests.Unit.Service.Compression;

public sealed class CompressionStrategyFactoryTests
{
    private readonly CompressionStrategyFactory _sut = new();



    [Fact]
    public void Create_when_zip_expected_ZipCompressionStrategy()
    {
        var result = _sut.Create(CompressionFormat.Zip);

        Assert.IsType<ZipCompressionStrategy>(result);
    }



    [Fact]
    public void Create_when_gz_expected_GZipCompressionStrategy()
    {
        var result = _sut.Create(CompressionFormat.Gz);

        Assert.IsType<GZipCompressionStrategy>(result);
    }



    [Fact]
    public void Create_when_brotli_expected_BrotliCompressionStrategy()
    {
        var result = _sut.Create(CompressionFormat.Brotli);

        Assert.IsType<BrotliCompressionStrategy>(result);
    }



    [Fact]
    public void Create_when_zstd_expected_ZstdCompressionStrategy()
    {
        var result = _sut.Create(CompressionFormat.Zstd);

        Assert.IsType<ZstdCompressionStrategy>(result);
    }



    [Fact]
    public void Create_when_lz4_expected_Lz4CompressionStrategy()
    {
        var result = _sut.Create(CompressionFormat.Lz4);

        Assert.IsType<Lz4CompressionStrategy>(result);
    }



    [Fact]
    public void Create_when_invalidFormat_expected_throwsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.Create((CompressionFormat)99));
    }
}
