namespace Wolfgang.LogCompressor.Abstraction;

/// <summary>
/// Verifies the integrity of compressed archives.
/// </summary>
internal interface IArchiveVerifier
{
    /// <summary>
    /// Verifies that the archive at the specified path can be read successfully
    /// and is complete.
    /// </summary>
    /// <param name="archivePath">The path to the archive file.</param>
    /// <param name="format">The compression format extension (e.g. "zip", "gz").</param>
    /// <param name="expectedUncompressedSize">
    /// The expected uncompressed size in bytes, when the caller knows it (a
    /// single-file compression knows the source length). Used to detect
    /// truncated archives in formats whose decompressor does not fail on
    /// truncation — modern .NET's <see cref="System.IO.Compression.GZipStream"/>
    /// and <see cref="System.IO.Compression.BrotliStream"/> return partial data
    /// instead of throwing.
    /// </param>
    /// <returns><see langword="true"/> if the archive is valid; otherwise, <see langword="false"/>.</returns>
    Task<bool> VerifyAsync(string archivePath, string format, long? expectedUncompressedSize = null);
}
