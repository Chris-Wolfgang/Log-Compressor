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
    private readonly FileFilterService _fileFilter = new(TimeProvider.System);
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
            new FileNamingService(TimeProvider.System),
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
            new FileNamingService(TimeProvider.System),
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
    public async Task ExecuteAsync_when_decompressCompressedFile_expected_contentRoundTrips(string formatName)
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
    public async Task ExecuteAsync_when_decompressBundle_expected_everyEntryRoundTrips(string formatName)
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



    [Theory]
    [InlineData("Zip")]
    [InlineData("Gz")]
    public async Task ExecuteAsync_when_recursiveBundleHasSameNamedFiles_expected_bothRestoredInTheirSubdirectories(string formatName)
    {
        // #441: entries were stored under the bare file name, so a/app.log and
        // b/app.log became duplicate entries and one was lost on --force.
        var format = Enum.Parse<CompressionFormat>(formatName);
        var sourceDir = Path.Combine(_tempDir.Path, "logs");
        Directory.CreateDirectory(Path.Combine(sourceDir, "a"));
        Directory.CreateDirectory(Path.Combine(sourceDir, "b"));
        await File.WriteAllTextAsync(Path.Combine(sourceDir, "a", "app.log"), "AAA\n");
        await File.WriteAllTextAsync(Path.Combine(sourceDir, "b", "app.log"), "BBB\n");

        var bundleResult = await NewBundleService().ExecuteAsync(new CompressionOptions
        {
            SourcePath = sourceDir,
            Format = format,
            Recurse = true
        });
        Assert.True(bundleResult.Success, bundleResult.ErrorMessage);

        var extractDir = Path.Combine(_tempDir.Path, "out");
        var decompressResults = await _decompress.ExecuteAsync(new DecompressionOptions
        {
            SourcePath = bundleResult.OutputPath,
            OutputPath = extractDir
        });

        Assert.True(Assert.Single(decompressResults).Success);
        Assert.Equal("AAA\n", await File.ReadAllTextAsync(Path.Combine(extractDir, "a", "app.log")));
        Assert.Equal("BBB\n", await File.ReadAllTextAsync(Path.Combine(extractDir, "b", "app.log")));
    }



    [Fact]
    public async Task ExecuteAsync_when_archiveCorrupt_expected_noFileAtFinalNameAndRerunNotBlocked()
    {
        // #444: a failed extraction left a truncated file at the final name,
        // and every re-run then failed with "Extraction target already exists".
        var sourceDir = Path.Combine(_tempDir.Path, "src");
        Directory.CreateDirectory(sourceDir);
        var random = new Random(444);
        var content = new string(Enumerable.Range(0, 300_000).Select(_ => (char)('a' + random.Next(26))).ToArray());
        await File.WriteAllTextAsync(Path.Combine(sourceDir, "big.log"), content);
        var compressResult = Assert.Single(await NewCompressService().ExecuteAsync(new CompressionOptions
        {
            SourcePath = Path.Combine(sourceDir, "big.log"),
            Format = CompressionFormat.Zstd
        }));
        Assert.True(compressResult.Success);

        // A flipped byte in the middle: the decoder writes most of the entry,
        // then the frame checksum (#448) fails it.
        var archive = compressResult.OutputPath;
        var bytes = await File.ReadAllBytesAsync(archive);
        var corrupt = bytes.ToArray();
        corrupt[corrupt.Length / 2] ^= 0x01;
        await File.WriteAllBytesAsync(archive, corrupt);
        var destination = archive[..^".zst".Length];

        var first = Assert.Single(await _decompress.ExecuteAsync(new DecompressionOptions { SourcePath = archive }));

        Assert.False(first.Success);
        Assert.True(File.Exists(archive));
        Assert.False(File.Exists(destination));
        Assert.False(File.Exists(destination + ".partial"));

        // The re-run with an intact archive is not blocked by a leftover.
        await File.WriteAllBytesAsync(archive, bytes);
        var second = Assert.Single(await _decompress.ExecuteAsync(new DecompressionOptions { SourcePath = archive }));

        Assert.True(second.Success, second.ErrorMessage);
        Assert.Equal(content, await File.ReadAllTextAsync(destination));
    }



    public void Dispose()
    {
        _tempDir.Dispose();
    }
}
