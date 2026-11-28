using System.IO.Compression;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Unit.Service;

public sealed class GZipTrailerCheckingStreamTests
{
    private static readonly byte[] Data = "the quick brown fox jumps over the lazy dog"u8.ToArray();



    private static byte[] GZip(byte[] content)
    {
        using var buffer = new MemoryStream();
        using (var gzip = new GZipStream(buffer, CompressionLevel.Optimal, leaveOpen: true))
        {
            gzip.Write(content);
        }

        return buffer.ToArray();
    }



    [Fact]
    public async Task ReadAsync_when_archiveIntact_expected_dataReadAndNoError()
    {
        await using var sut = new GZipTrailerCheckingStream(new MemoryStream(GZip(Data)));
        using var output = new MemoryStream();

        await sut.CopyToAsync(output);

        Assert.Equal(Data, output.ToArray());
        Assert.Equal(Data.Length, sut.Position);
    }



    [Fact]
    public async Task ReadAsync_when_trailerDoesNotMatch_expected_invalidDataException()
    {
        // #466: GZipStream returns the data without error; only the trailer
        // tells that it is not what was compressed.
        var archive = GZip(Data);
        archive[^1] ^= 0x01;
        await using var sut = new GZipTrailerCheckingStream(new MemoryStream(archive));

        await Assert.ThrowsAsync<InvalidDataException>(() => sut.CopyToAsync(Stream.Null));
    }



    [Fact]
    public async Task ReadAsync_when_onlyTheLengthDoesNotMatch_expected_invalidDataException()
    {
        var archive = GZip(Data);
        archive[^4] ^= 0x01;
        await using var sut = new GZipTrailerCheckingStream(new MemoryStream(archive));

        await Assert.ThrowsAsync<InvalidDataException>(() => sut.CopyToAsync(Stream.Null));
    }



    [Fact]
    public async Task ReadAsync_when_readAgainAfterTheEnd_expected_zeroWithoutASecondCheck()
    {
        var archive = GZip(Data);
        var source = new MemoryStream(archive);
        await using var sut = new GZipTrailerCheckingStream(source);
        await sut.CopyToAsync(Stream.Null);
        source.Position = 0;
        source.SetLength(0);

        // A second check would find a 0-byte archive and throw.
        Assert.Equal(0, await sut.ReadAsync(new byte[8].AsMemory()));
    }



    [Fact]
    public async Task ReadAsync_when_archiveTruncated_expected_invalidDataException()
    {
        var content = new byte[200_000];
        new Random(466).NextBytes(content);
        var archive = GZip(content);
        await using var sut = new GZipTrailerCheckingStream(new MemoryStream(archive[..(archive.Length / 2)]));

        await Assert.ThrowsAsync<InvalidDataException>(() => sut.CopyToAsync(Stream.Null));
    }



    [Fact]
    public void Read_when_archiveSmallerThanMinimalMember_expected_invalidDataException()
    {
        // A gzip header and an empty deflate block with no trailer (12 bytes):
        // GZipStream returns no data and no error.
        byte[] archive = [0x1F, 0x8B, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x03, 0x00];
        using var sut = new GZipTrailerCheckingStream(new MemoryStream(archive));

        Assert.Throws<InvalidDataException>(() => sut.Read(new byte[16], 0, 16));
    }



    [Fact]
    public void Read_when_syncOverloads_expected_dataAndTrailerChecked()
    {
        using var sut = new GZipTrailerCheckingStream(new MemoryStream(GZip(Data)));
        var buffer = new byte[Data.Length + 8];

        var first = sut.Read(buffer, 0, 4);
        var rest = sut.Read(buffer.AsSpan(4));
        var end = sut.Read(buffer.AsSpan(0, 1));

        Assert.Equal(Data.Length, first + rest);
        Assert.Equal(0, end);
    }



    [Fact]
    public async Task ReadAsync_when_zeroLengthReadBeforeTheEnd_expected_noPrematureTrailerCheck()
    {
        // TarReader issues zero-length reads mid-stream; they are not the end.
        await using var sut = new GZipTrailerCheckingStream(new MemoryStream(GZip(Data)));

        Assert.Equal(0, await sut.ReadAsync(Memory<byte>.Empty));
        using var output = new MemoryStream();
        await sut.CopyToAsync(output);

        Assert.Equal(Data, output.ToArray());
    }



    [Fact]
    public async Task ReadAsync_when_arrayOverload_expected_data()
    {
        await using var sut = new GZipTrailerCheckingStream(new MemoryStream(GZip(Data)));
        var buffer = new byte[Data.Length + 8];

        var read = await sut.ReadAsync(buffer, 0, buffer.Length, CancellationToken.None);

        Assert.True(read > 0);
    }



    [Fact]
    public void Members_when_notSupported_expected_throwOrReportCapabilities()
    {
        using var sut = new GZipTrailerCheckingStream(new MemoryStream(GZip(Data)));

        Assert.True(sut.CanRead);
        Assert.False(sut.CanSeek);
        Assert.False(sut.CanWrite);
        Assert.Throws<NotSupportedException>(() => sut.Length);
        Assert.Throws<NotSupportedException>(() => sut.Position = 1);
        Assert.Throws<NotSupportedException>(() => sut.Seek(0, SeekOrigin.Begin));
        Assert.Throws<NotSupportedException>(() => sut.SetLength(1));
        Assert.Throws<NotSupportedException>(() => sut.Write([1], 0, 1));
        sut.Flush();
    }



    [Fact]
    public void Constructor_when_archiveNull_expected_argumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new GZipTrailerCheckingStream(null!));
    }
}
