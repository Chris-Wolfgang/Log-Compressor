using System.IO.Hashing;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Unit.Service;

public sealed class HashingReadStreamTests
{
    private static readonly byte[] Data = "the quick brown fox"u8.ToArray();



    [Fact]
    public async Task ReadAsync_when_drained_expected_crcAndCountMatchTheData()
    {
        var crc = new Crc32();
        await using var sut = new HashingReadStream(new MemoryStream(Data), crc);

        var buffer = new byte[5];
        while (await sut.ReadAsync(buffer.AsMemory()) > 0)
        {
        }

        Assert.Equal(Data.Length, sut.BytesRead);
        Assert.Equal(sut.BytesRead, sut.Position);
        Assert.Equal(Crc32.HashToUInt32(Data), crc.GetCurrentHashAsUInt32());
    }



    [Fact]
    public async Task ReadAsync_when_arrayOverload_expected_recordsTheBytes()
    {
        var crc = new Crc32();
        await using var sut = new HashingReadStream(new MemoryStream(Data), crc);

        var buffer = new byte[Data.Length + 4];
        var read = await sut.ReadAsync(buffer, 2, Data.Length, CancellationToken.None);

        Assert.Equal(Data.Length, read);
        Assert.Equal(Crc32.HashToUInt32(Data), crc.GetCurrentHashAsUInt32());
    }



    [Fact]
    public void Read_when_syncOverloads_expected_recordsTheBytes()
    {
        var crc = new Crc32();
        using var sut = new HashingReadStream(new MemoryStream(Data), crc);

        var first = sut.Read(new byte[4], 0, 4);
        var rest = sut.Read(new byte[Data.Length].AsSpan());

        Assert.Equal(Data.Length, first + rest);
        Assert.Equal(Crc32.HashToUInt32(Data), crc.GetCurrentHashAsUInt32());
    }



    [Fact]
    public void Capabilities_when_queried_expected_readOnlyAndNotSeekable()
    {
        using var sut = new HashingReadStream(new MemoryStream(Data), new Crc32());

        Assert.True(sut.CanRead);
        Assert.False(sut.CanSeek);
        Assert.False(sut.CanWrite);
        sut.Flush();
    }



    [Fact]
    public void UnsupportedMembers_when_called_expected_throwNotSupported()
    {
        using var sut = new HashingReadStream(new MemoryStream(Data), new Crc32());

        Assert.Throws<NotSupportedException>(() => sut.Length);
        Assert.Throws<NotSupportedException>(() => sut.Position = 1);
        Assert.Throws<NotSupportedException>(() => sut.Seek(0, SeekOrigin.Begin));
        Assert.Throws<NotSupportedException>(() => sut.SetLength(1));
        Assert.Throws<NotSupportedException>(() => sut.Write(new byte[1], 0, 1));
    }



    [Fact]
    public void Ctor_when_argumentNull_expected_throwsWithParamName()
    {
        Assert.Equal("inner", Assert.Throws<ArgumentNullException>(() => new HashingReadStream(null!, new Crc32())).ParamName);
        Assert.Equal("crc", Assert.Throws<ArgumentNullException>(() => new HashingReadStream(new MemoryStream(), null!)).ParamName);
    }
}
