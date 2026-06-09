using System.IO.Compression;
using Microsoft.Extensions.Logging.Abstractions;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor.Tests.Integration;

/// <summary>
/// End-to-end tests that exercise the real compression pipeline (real file
/// system, real compression strategies, real archive verification) against a
/// temporary directory. These cover the compress-verify-delete round-trip that
/// the mocked unit tests cannot — the highest data-loss-risk behaviour.
/// </summary>
public sealed class CompressionRoundTripTests
{
    private static CompressService CreateCompressService()
    {
        return new CompressService
        (
            new FileSystemWrapper(),
            new FileFilterService(),
            new FileNamingService(),
            new ArchiveVerifier(NullLogger<ArchiveVerifier>.Instance),
            new CompressionStrategyFactory(),
            NullLogger<CompressService>.Instance
        );
    }



    private static BundleService CreateBundleService()
    {
        return new BundleService
        (
            new FileSystemWrapper(),
            new FileFilterService(),
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
    public async Task Compress_singleFile_creates_verifiable_archive_and_deletes_original(string formatName)
    {
        var format = Enum.Parse<CompressionFormat>(formatName);

        using var temp = new TempDirectory();
        var source = temp.WriteFile("app.log", "the quick brown fox jumps over the lazy dog");
        var sut = CreateCompressService();

        var results = await sut.ExecuteAsync
        (
            new CompressionOptions
            {
                SourcePath = source,
                Format = format,
                Verify = true
            }
        );

        var result = Assert.Single(results);
        Assert.True(result.Success, result.ErrorMessage);
        Assert.False(File.Exists(source));               // original deleted only after a verified write
        Assert.True(File.Exists(result.OutputPath));     // archive produced on disk
        Assert.True(new FileInfo(result.OutputPath).Length > 0);
    }



    [Fact]
    public async Task Compress_zipArchive_roundtrips_original_content()
    {
        using var temp = new TempDirectory();
        const string content = "line one\nline two\nline three";
        var source = temp.WriteFile("data.log", content);
        var sut = CreateCompressService();

        var result = Assert.Single
        (
            await sut.ExecuteAsync
            (
                new CompressionOptions
                {
                    SourcePath = source,
                    Format = CompressionFormat.Zip,
                    Verify = true
                }
            )
        );

        using var zip = ZipFile.OpenRead(result.OutputPath);
        var entry = Assert.Single(zip.Entries);
        using var reader = new StreamReader(entry.Open());

        Assert.Equal(content, reader.ReadToEnd());
    }



    [Theory]
    [InlineData("Zip")]
    [InlineData("Gz")]
    [InlineData("Brotli")]
    public async Task Bundle_multipleFiles_creates_single_archive_and_deletes_all_originals(string formatName)
    {
        var format = Enum.Parse<CompressionFormat>(formatName);

        using var temp = new TempDirectory();
        var first = temp.WriteFile("a.log", "alpha");
        var second = temp.WriteFile("b.log", "bravo");
        var third = temp.WriteFile("c.log", "charlie");
        var sut = CreateBundleService();

        var result = await sut.ExecuteAsync
        (
            new CompressionOptions
            {
                SourcePath = temp.Path,
                Format = format,
                Verify = true
            }
        );

        Assert.True(result.Success, result.ErrorMessage);
        Assert.False(File.Exists(first));
        Assert.False(File.Exists(second));
        Assert.False(File.Exists(third));
        Assert.True(File.Exists(result.OutputPath));
    }



    [Fact]
    public async Task Compress_withOlderThan_compresses_old_file_and_preserves_recent_file()
    {
        using var temp = new TempDirectory();
        var oldFile = temp.WriteFile("old.log", "old data");
        File.SetLastWriteTime(oldFile, DateTime.Now.AddDays(-10));
        var recentFile = temp.WriteFile("recent.log", "recent data");

        var sut = CreateCompressService();

        var results = await sut.ExecuteAsync
        (
            new CompressionOptions
            {
                SourcePath = temp.Path,
                Format = CompressionFormat.Zip,
                OlderThanDays = 7,
                Verify = true
            }
        );

        var result = Assert.Single(results);             // only the old file matched the filter
        Assert.True(result.Success, result.ErrorMessage);
        Assert.False(File.Exists(oldFile));              // old file compressed + deleted
        Assert.True(File.Exists(recentFile));            // recent file left untouched
    }



    [Fact]
    public async Task Bundle_skips_unreadable_file_preserves_it_and_returns_nonsuccess()
    {
        using var temp = new TempDirectory();
        var readable = temp.WriteFile("readable.log", "readable content");
        var locked = temp.WriteFile("locked.log", "locked content");

        // Hold an exclusive (no-share) handle so the bundler's File.OpenRead fails
        // for this one file — simulating a file locked by another process.
        using (new FileStream(locked, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            var sut = CreateBundleService();

            var result = await sut.ExecuteAsync
            (
                new CompressionOptions
                {
                    SourcePath = temp.Path,
                    Format = CompressionFormat.Zip,
                    Verify = true
                }
            );

            Assert.False(result.Success);                                                  // partial → non-success
            Assert.Contains("skipped", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
            Assert.True(File.Exists(locked));                                              // unreadable file preserved
            Assert.False(File.Exists(readable));                                           // readable file bundled + deleted
            Assert.True(File.Exists(result.OutputPath));                                   // archive still created
        }
    }



    [Fact]
    public async Task Compress_run_twice_does_not_recompress_its_own_archive()
    {
        using var temp = new TempDirectory();
        temp.WriteFile("app.log", "log contents");
        var sut = CreateCompressService();

        var first = await sut.ExecuteAsync
        (
            new CompressionOptions { SourcePath = temp.Path, Format = CompressionFormat.Zip, Verify = true }
        );
        Assert.Single(first);                                    // app.log compressed on the first run

        // Second run over the same directory: the .zip produced by the first run
        // must be ignored, not re-compressed and deleted.
        var second = await sut.ExecuteAsync
        (
            new CompressionOptions { SourcePath = temp.Path, Format = CompressionFormat.Zip, Verify = true }
        );
        Assert.Empty(second);                                    // nothing to do — the archive is not a source

        Assert.Single(Directory.GetFiles(temp.Path, "*.zip"));   // the original archive is intact
    }
}
