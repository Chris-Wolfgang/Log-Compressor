using System.IO.Compression;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Unit.Service;

public sealed class DecompressServiceTests : IDisposable
{
    private readonly TempDirectory _tempDir = new();
    private readonly IFileSystem _fileSystem = Substitute.For<IFileSystem>();
    private readonly DecompressService _sut;
    private readonly Dictionary<string, MemoryStream> _written = new(StringComparer.Ordinal);



    public DecompressServiceTests()
    {
        var fileFilter = new FileFilterService();
        _sut = new DecompressService(_fileSystem, fileFilter, Substitute.For<ILogger<DecompressService>>());

        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(call =>
        {
            var stream = new MemoryStream();
            _written[call.Arg<string>()] = stream;
            return stream;
        });
    }



    private static byte[] ZipBytes(params (string Name, string Content)[] entries)
    {
        using var buffer = new MemoryStream();
        using (var zip = new ZipArchive(buffer, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var (name, content) in entries)
            {
                var entry = zip.CreateEntry(name);
                using var stream = entry.Open();
                var bytes = System.Text.Encoding.UTF8.GetBytes(content);
                stream.Write(bytes);
            }
        }

        return buffer.ToArray();
    }



    private static byte[] GzBytes(string content)
    {
        using var buffer = new MemoryStream();
        using (var gz = new GZipStream(buffer, CompressionLevel.Fastest, leaveOpen: true))
        {
            gz.Write(System.Text.Encoding.UTF8.GetBytes(content));
        }

        return buffer.ToArray();
    }



    private string SetupSingleArchive(string name, byte[] bytes)
    {
        // A REAL file backs the FileInfo: the service materializes Length,
        // and real paths keep Windows/Unix path semantics honest.
        var path = Path.Combine(_tempDir.Path, name);
        File.WriteAllBytes(path, bytes);
        _fileSystem.FileExists(path).Returns(returnThis: true);
        _fileSystem.GetFileInfo(path).Returns(new FileInfo(path));
        _fileSystem.OpenRead(path).Returns(_ => new MemoryStream(bytes));
        _fileSystem.DirectoryExists(Arg.Any<string>()).Returns(returnThis: true);
        return path;
    }



    public void Dispose()
    {
        _tempDir.Dispose();
    }



    [Fact]
    public async Task ExecuteAsync_when_nullOptions_expected_throws()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.ExecuteAsync(null!));
    }



    [Fact]
    public async Task ExecuteAsync_when_sourceMissing_expected_fileNotFound()
    {
        _fileSystem.FileExists("/tmp/nope").Returns(returnThis: false);
        _fileSystem.DirectoryExists("/tmp/nope").Returns(returnThis: false);

        await Assert.ThrowsAsync<FileNotFoundException>(
            () => _sut.ExecuteAsync(new DecompressionOptions { SourcePath = "/tmp/nope" }));
    }



    [Fact]
    public async Task ExecuteAsync_when_zipWithEntries_expected_allExtractedAndArchiveDeleted()
    {
        var archive = SetupSingleArchive("bundle.zip", ZipBytes(("a.log", "alpha"), ("sub/b.log", "beta")));
        // Substitute default: FileExists is false for every other path.

        var results = await _sut.ExecuteAsync(new DecompressionOptions { SourcePath = archive });

        var result = Assert.Single(results);
        Assert.True(result.Success);
        Assert.Equal(2, _written.Count);
        Assert.Contains(_written.Keys, k => k.EndsWith("a.log", StringComparison.Ordinal));
        Assert.Contains(_written.Keys, k => k.Replace('\\', '/').EndsWith("sub/b.log", StringComparison.Ordinal));
        Assert.Equal("alpha", System.Text.Encoding.UTF8.GetString(
            _written.First(kv => kv.Key.EndsWith("a.log", StringComparison.Ordinal)).Value.ToArray()));
        _fileSystem.Received(1).DeleteFile(archive);
    }



    [Fact]
    public async Task ExecuteAsync_when_keepArchives_expected_archiveNotDeleted()
    {
        var archive = SetupSingleArchive("bundle.zip", ZipBytes(("a.log", "alpha")));
        // Substitute default: FileExists is false for every other path.

        var results = await _sut.ExecuteAsync(
            new DecompressionOptions { SourcePath = archive, KeepArchives = true });

        Assert.True(Assert.Single(results).Success);
        _fileSystem.DidNotReceive().DeleteFile(Arg.Any<string>());
    }



    [Fact]
    public async Task ExecuteAsync_when_targetExists_withoutForce_expected_failureAndArchiveKept()
    {
        var archive = SetupSingleArchive("bundle.zip", ZipBytes(("a.log", "alpha")));
        // Every extraction target already exists.
        _fileSystem.FileExists(Arg.Is<string>(p => !string.Equals(p, archive, StringComparison.Ordinal))).Returns(returnThis: true);

        var results = await _sut.ExecuteAsync(new DecompressionOptions { SourcePath = archive });

        var result = Assert.Single(results);
        Assert.False(result.Success);
        Assert.Contains("--force", result.ErrorMessage, StringComparison.Ordinal);
        _fileSystem.DidNotReceive().DeleteFile(Arg.Any<string>());
    }



    [Fact]
    public async Task ExecuteAsync_when_targetExists_withForce_expected_overwritten()
    {
        var archive = SetupSingleArchive("bundle.zip", ZipBytes(("a.log", "alpha")));
        _fileSystem.FileExists(Arg.Is<string>(p => !string.Equals(p, archive, StringComparison.Ordinal))).Returns(returnThis: true);

        var results = await _sut.ExecuteAsync(
            new DecompressionOptions { SourcePath = archive, Force = true });

        Assert.True(Assert.Single(results).Success);
        Assert.Single(_written);
    }



    [Fact]
    public async Task ExecuteAsync_when_zipSlipEntry_expected_failureAndArchiveKept()
    {
        var archive = SetupSingleArchive("evil.zip", ZipBytes(("../evil.txt", "pwned")));
        // Substitute default: FileExists is false for every other path.

        var results = await _sut.ExecuteAsync(new DecompressionOptions { SourcePath = archive });

        var result = Assert.Single(results);
        Assert.False(result.Success);
        Assert.Contains("escapes", result.ErrorMessage, StringComparison.Ordinal);
        Assert.Empty(_written);
        _fileSystem.DidNotReceive().DeleteFile(Arg.Any<string>());
    }



    [Fact]
    public async Task ExecuteAsync_when_rawGz_expected_suffixStrippedOutputName()
    {
        var archive = SetupSingleArchive("app-2026-01-05_09-30-00.gz", GzBytes("log line"));
        // Substitute default: FileExists is false for every other path.

        var results = await _sut.ExecuteAsync(new DecompressionOptions { SourcePath = archive });

        Assert.True(Assert.Single(results).Success);
        var written = Assert.Single(_written);
        Assert.EndsWith("app-2026-01-05_09-30-00", written.Key, StringComparison.Ordinal);
        Assert.Equal("log line", System.Text.Encoding.UTF8.GetString(written.Value.ToArray()));
    }



    [Fact]
    public async Task ExecuteAsync_when_unknownExtension_withGzipMagic_expected_sniffedAndExtracted()
    {
        var archive = SetupSingleArchive("mystery.bin", GzBytes("sniffed"));
        // Substitute default: FileExists is false for every other path.

        var results = await _sut.ExecuteAsync(new DecompressionOptions { SourcePath = archive });

        Assert.True(Assert.Single(results).Success);
        var written = Assert.Single(_written);
        Assert.EndsWith("mystery.bin.extracted", written.Key, StringComparison.Ordinal);
        Assert.Equal("sniffed", System.Text.Encoding.UTF8.GetString(written.Value.ToArray()));
    }



    [Fact]
    public async Task ExecuteAsync_when_unknownExtension_withUnknownMagic_expected_failure()
    {
        var archive = SetupSingleArchive("mystery.bin", [0x00, 0x01, 0x02, 0x03, 0x04]);

        var results = await _sut.ExecuteAsync(new DecompressionOptions { SourcePath = archive });

        var result = Assert.Single(results);
        Assert.False(result.Success);
        Assert.Contains("brotli", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }



    private static byte[] ZstdBytes(string content)
    {
        using var buffer = new MemoryStream();
        using (var z = new ZstdSharp.CompressionStream(buffer, leaveOpen: true))
        {
            z.Write(System.Text.Encoding.UTF8.GetBytes(content));
        }

        return buffer.ToArray();
    }



    private static byte[] Lz4Bytes(string content)
    {
        using var buffer = new MemoryStream();
        using (var l = K4os.Compression.LZ4.Streams.LZ4Stream.Encode(buffer, leaveOpen: true))
        {
            l.Write(System.Text.Encoding.UTF8.GetBytes(content));
        }

        return buffer.ToArray();
    }



    [Theory]
    [InlineData("zip")]
    [InlineData("zst")]
    [InlineData("lz4")]
    public async Task ExecuteAsync_when_unknownExtension_withKnownMagic_expected_sniffed(string flavor)
    {
        var bytes = flavor switch
        {
            "zip" => ZipBytes(("a.log", "z")),
            "zst" => ZstdBytes("z"),
            _ => Lz4Bytes("z")
        };
        var archive = SetupSingleArchive($"mystery-{flavor}.dat", bytes);

        var results = await _sut.ExecuteAsync(new DecompressionOptions { SourcePath = archive });

        Assert.True(Assert.Single(results).Success);
    }



    [Fact]
    public async Task ExecuteAsync_when_fileTooSmallToSniff_expected_failure()
    {
        var archive = SetupSingleArchive("tiny.dat", [0x01, 0x02]);

        var results = await _sut.ExecuteAsync(new DecompressionOptions { SourcePath = archive });

        var result = Assert.Single(results);
        Assert.False(result.Success);
        Assert.Contains("too small", result.ErrorMessage, StringComparison.Ordinal);
    }



    [Fact]
    public async Task ExecuteAsync_when_entryParentDirectoryMissing_expected_created()
    {
        var archive = SetupSingleArchive("nested.zip", ZipBytes(("deep/nested/a.log", "x")));
        // Output dir exists, but the entry's nested parents do not.
        _fileSystem.DirectoryExists(Arg.Is<string>(p => p.Contains("deep", StringComparison.Ordinal)))
            .Returns(returnThis: false);

        var results = await _sut.ExecuteAsync(new DecompressionOptions { SourcePath = archive });

        Assert.True(Assert.Single(results).Success);
        _fileSystem.Received().CreateDirectory(Arg.Is<string>(p => p.Contains("deep", StringComparison.Ordinal)));
    }



    [Fact]
    public async Task ExecuteAsync_when_outputPathHasTrailingSeparator_expected_entriesExtracted()
    {
        // Regression (review finding): GetFullPath preserves a trailing
        // separator, and the old prefix check built "root//" which rejected
        // every valid entry as an escape.
        var archive = SetupSingleArchive("t.zip", ZipBytes(("a.log", "x")));
        var outputDir = Path.Combine(_tempDir.Path, "out") + Path.DirectorySeparatorChar;

        var results = await _sut.ExecuteAsync(new DecompressionOptions
        {
            SourcePath = archive,
            OutputPath = outputDir
        });

        Assert.True(Assert.Single(results).Success);
        Assert.Single(_written);
    }



    [Fact]
    public async Task ExecuteAsync_when_onErrorRetry_expected_secondAttemptSucceeds()
    {
        var bytes = ZipBytes(("a.log", "hello"));
        var archive = SetupSingleArchive("flaky.zip", bytes);
        // Unreadable once (transient lock), readable on the retry.
        var attempts = 0;
        _fileSystem.OpenRead(archive).Returns(_ =>
            ++attempts == 1 ? throw new IOException("locked") : new MemoryStream(bytes));

        var results = await _sut.ExecuteAsync(new DecompressionOptions
        {
            SourcePath = archive,
            OnError = new ErrorPolicy { RetryCount = 1 }
        });

        var result = Assert.Single(results);
        Assert.True(result.Success);
        Assert.Equal(2, attempts);
    }



    [Fact]
    public async Task ExecuteAsync_when_onErrorFail_expected_stopsAtFirstFailure()
    {
        var dir = _tempDir.Path;
        // First archive is corrupt (garbage after the zip magic), second is valid.
        var bad = SetupSingleArchive("bad.zip", [0x50, 0x4B, 0x03, 0x04, 0xFF, 0xFF]);
        var good = SetupSingleArchive("good.zip", ZipBytes(("a.log", "a")));
        _fileSystem.FileExists(dir).Returns(returnThis: false);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).Returns([bad, good]);

        var results = await _sut.ExecuteAsync(new DecompressionOptions
        {
            SourcePath = dir,
            OnError = new ErrorPolicy { Mode = OnErrorMode.Fail }
        });

        var result = Assert.Single(results);
        Assert.False(result.Success);
        _fileSystem.DidNotReceive().OpenRead(good);
    }



    [Fact]
    public async Task ExecuteAsync_when_directorySource_expected_onlyRecognizedArchivesSelected()
    {
        var dir = _tempDir.Path;
        var zipPath = SetupSingleArchive("a.zip", ZipBytes(("a.log", "a")));
        var gzPath = SetupSingleArchive("b.gz", GzBytes("b"));
        var txtPath = Path.Combine(dir, "readme.txt");
        File.WriteAllText(txtPath, "not an archive");
        _fileSystem.FileExists(dir).Returns(returnThis: false);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly)
            .Returns([zipPath, txtPath, gzPath]);

        var results = await _sut.ExecuteAsync(new DecompressionOptions { SourcePath = dir });

        Assert.Equal(2, results.Count);
        Assert.All(results, r => Assert.True(r.Success));
        _fileSystem.DidNotReceive().OpenRead(txtPath);
    }
}
