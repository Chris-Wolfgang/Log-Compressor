using System.IO.Hashing;

namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// A read-only pass-through stream that feeds every byte read into a CRC-32 and counts them, so a
/// consumer (the tar walk) and the integrity check can share one read of the data.
/// </summary>
internal sealed class HashingReadStream : Stream
{
    private readonly Stream _inner;
    private readonly Crc32 _crc;



    /// <summary>
    /// Initializes a new instance of the <see cref="HashingReadStream"/> class.
    /// </summary>
    /// <param name="inner">The stream to read from. It is not disposed by this stream.</param>
    /// <param name="crc">The CRC-32 every byte read is appended to.</param>
    public HashingReadStream(Stream inner, Crc32 crc)
    {
        ArgumentNullException.ThrowIfNull(inner);
        ArgumentNullException.ThrowIfNull(crc);

        _inner = inner;
        _crc = crc;
    }



    /// <summary>
    /// Gets the number of bytes read through this stream.
    /// </summary>
    public long BytesRead { get; private set; }



    /// <inheritdoc />
    public override bool CanRead => true;



    /// <inheritdoc />
    public override bool CanSeek => false;



    /// <inheritdoc />
    public override bool CanWrite => false;



    /// <inheritdoc />
    public override long Length => throw new NotSupportedException();



    /// <inheritdoc />
    public override long Position
    {
        get => BytesRead;
        set => throw new NotSupportedException();
    }



    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        return Read(buffer.AsSpan(offset, count));
    }



    /// <inheritdoc />
    public override int Read(Span<byte> buffer)
    {
        var read = _inner.Read(buffer);
        Record(buffer[..read]);
        return read;
    }



    /// <inheritdoc />
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();
    }



    /// <inheritdoc />
    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var read = await _inner.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
        Record(buffer.Span[..read]);
        return read;
    }



    /// <inheritdoc />
    public override void Flush()
    {
        // Read-only: nothing to flush.
    }



    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();



    /// <inheritdoc />
    public override void SetLength(long value) => throw new NotSupportedException();



    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();



    private void Record(ReadOnlySpan<byte> data)
    {
        _crc.Append(data);
        BytesRead += data.Length;
    }
}
