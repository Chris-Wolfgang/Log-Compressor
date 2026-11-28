using System.Buffers.Binary;
using System.IO.Compression;
using System.IO.Hashing;

namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// A read-only gzip decompression stream that checks the gzip trailer (CRC-32 and ISIZE) of the
/// archive when the decompressed data ends, and throws <see cref="InvalidDataException"/> on a
/// mismatch.
/// </summary>
/// <remarks>
/// <see cref="GZipStream"/> returns partial data from a truncated archive instead of failing, so
/// without this check <c>decompress</c> restores a truncated file, reports success and deletes the
/// archive (#466). <see cref="ArchiveVerifier"/> makes the same check at compress time. The archive
/// stream must be seekable; it is left open. Only a single-member gzip file (what logc writes) is
/// checked correctly: for a multi-member file the last member's trailer is compared against all
/// members' data, so such a file from another tool fails and is kept.
/// </remarks>
internal sealed class GZipTrailerCheckingStream : Stream
{
    // 10-byte header + 8-byte trailer: the smallest complete gzip member.
    private const int MinimumGZipLength = 18;

    private readonly Stream _archive;
    private readonly GZipStream _gzip;
    private readonly Crc32 _crc = new();
    private long _count;
    private bool _checked;



    /// <summary>
    /// Initializes a new instance of the <see cref="GZipTrailerCheckingStream"/> class.
    /// </summary>
    /// <param name="archive">The seekable gzip archive stream, positioned at its start.</param>
    public GZipTrailerCheckingStream(Stream archive)
    {
        ArgumentNullException.ThrowIfNull(archive);
        _archive = archive;
        _gzip = new GZipStream(archive, CompressionMode.Decompress, leaveOpen: true);
    }



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
        get => _count;
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
        var read = _gzip.Read(buffer);
        return Record(buffer[..read], buffer.Length);
    }



    /// <inheritdoc />
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();
    }



    /// <inheritdoc />
    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var read = await _gzip.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
        return Record(buffer.Span[..read], buffer.Length);
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



    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        // No finalizer here or in Stream, so this only runs from Dispose().
        _gzip.Dispose();
        base.Dispose(disposing);
    }



    private int Record(ReadOnlySpan<byte> data, int requested)
    {
        // An empty read of a non-empty buffer is the end of the decompressed
        // data (a zero-length read, as TarReader issues, is not): check the
        // trailer once, before the caller treats the entry as complete.
        if (data.IsEmpty && requested > 0 && !_checked)
        {
            _checked = true;
            CheckTrailer();
        }

        _crc.Append(data);
        _count += data.Length;
        return data.Length;
    }



    private void CheckTrailer()
    {
        if (_archive.Length < MinimumGZipLength)
        {
            throw new InvalidDataException($"Gzip archive is too small to be complete: {_archive.Length} bytes.");
        }

        Span<byte> trailer = stackalloc byte[8];
        _archive.Seek(-8, SeekOrigin.End);
        _archive.ReadExactly(trailer);

        var storedCrc = BinaryPrimitives.ReadUInt32LittleEndian(trailer);
        var storedSize = BinaryPrimitives.ReadUInt32LittleEndian(trailer[4..]);

        if (storedCrc != _crc.GetCurrentHashAsUInt32() || storedSize != (uint)_count)
        {
            throw new InvalidDataException("Gzip trailer (CRC-32/length) does not match the decompressed data: the archive is truncated or corrupt.");
        }
    }
}
