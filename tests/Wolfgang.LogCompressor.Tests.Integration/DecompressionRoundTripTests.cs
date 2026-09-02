using Microsoft.Extensions.Logging.Abstractions;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor.Tests.Integration;

/// <summary>
/// Full-fidelity round trips over the real file system (#187): compress with
/// the real services, decompress with <see cref="DecompressService"/>, and
/// the extracted bytes must equal the originals for every format — the
/// end-to-end proof behind decompress's delete-archive-after-success default.
/// </summary>
public sealed class DecompressionRoundTripTests : IDisposable
{
    private readonly TempDirectory _tempDir = new();
    private readonly FileSystemWrapper _fileSystem = new();
    private readonly FileFilterService _fileFilter = new();
    private readonly DecompressService _decompress;



    public DecompressionRoundTripTests()
    {
        _decompress = new DecompressService(_fileSystem, _fileFilter, NullLogger<DecompressService>.Instance);
    }



    private CompressService NewCompressService()
    {
        return new CompressService
        (
            _fileSystem,
            _fileFilter,
            new FileNamingService(),
            new ArchiveVerifier(NullLogger<ArchiveVerifier>.Instance),
            new CompressionStrategyFactory(),
            NullLogger<CompressService>.Instance
        );
    }



    private BundleService NewBundleService()
    {
        return new BundleService
        (
            _fileSystem,
            _fileFilter,
            new FileNamingService(),
            new ArchiveVerifier(NullLogger<ArchiveVerifier>.Instance),
            new CompressionStrategyFactory(),
            NullLogger<BundleService>.Instance
        );
    }



    [Theory]
    [InlineData("Zip")]
    [InlineData("Gz")]
    [InlineData("Brotli")]
    [InlineData("Zstd")]
    [InlineData("Lz4")]
    public async Task Compress_then_decompress_single_file_round_trips_content(string formatName)
    {
        var format = Enum.Parse<CompressionFormat>(formatName);
        var sourceDir = Path.Combine(_tempDir.Path, "src");
        Directory.CreateDirectory(sourceDir);
        var content = $"round trip {format} content\nsecond line\n";
        var sourcePath = Path.Combine(sourceDir, "app.log");
        await File.WriteAllTextAsync(sourcePath, content);

        var compressResults = await NewCompressService().ExecuteAsync(new CompressionOptions
        {
            SourcePath = sourcePath,
            Format = format
        });
        Assert.True(Assert.Single(compressResults).Success);
        Assert.False(File.Exists(sourcePath));

        var extractDir = Path.Combine(_tempDir.Path, "out");
        var decompressResults = await _decompress.ExecuteAsync(new DecompressionOptions
        {
            SourcePath = sourceDir,
            OutputPath = extractDir
        });

        var result = Assert.Single(decompressResults);
        Assert.True(result.Success);
        Assert.False(File.Exists(result.SourcePath));   // archive deleted after success

        var extracted = Assert.Single(Directory.GetFiles(extractDir, "*", SearchOption.AllDirectories));
        Assert.Equal(content, await File.ReadAllTextAsync(extracted));
    }



    [Theory]
    [InlineData("Zip")]
    [InlineData("Gz")]
    [InlineData("Zstd")]
    [InlineData("Lz4")]
    [InlineData("Brotli")]
    public async Task Bundle_then_decompress_round_trips_every_entry(string formatName)
    {
        var format = Enum.Parse<CompressionFormat>(formatName);
        var sourceDir = Path.Combine(_tempDir.Path, "logs");
        Directory.CreateDirectory(sourceDir);
        var contents = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["one.log"] = "first file\n",
            ["two.log"] = "second file with more content\n",
            ["three.log"] = ""
        };
        foreach (var (name, content) in contents)
        {
            await File.WriteAllTextAsync(Path.Combine(sourceDir, name), content);
        }

        var bundleResult = await NewBundleService().ExecuteAsync(new CompressionOptions
        {
            SourcePath = sourceDir,
            Format = format
        });
        Assert.True(bundleResult.Success);

        var extractDir = Path.Combine(_tempDir.Path, "out");
        var decompressResults = await _decompress.ExecuteAsync(new DecompressionOptions
        {
            SourcePath = bundleResult.OutputPath,
            OutputPath = extractDir,
            KeepArchives = true
        });

        Assert.True(Assert.Single(decompressResults).Success);
        Assert.True(File.Exists(bundleResult.OutputPath));   // --keep-archives honoured

        foreach (var (name, content) in contents)
        {
            var extracted = Path.Combine(extractDir, name);
            Assert.True(File.Exists(extracted), $"missing entry {name}");
            Assert.Equal(content, await File.ReadAllTextAsync(extracted));
        }
    }



    public void Dispose()
    {
        _tempDir.Dispose();
    }
}
