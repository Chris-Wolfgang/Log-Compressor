namespace Wolfgang.LogCompressor.Abstraction;

/// <summary>
/// Verifies the integrity of compressed archives.
/// </summary>
internal interface IArchiveVerifier
{
    /// <summary>
    /// Verifies that the archive at the specified path can be read successfully.
    /// </summary>
    /// <param name="archivePath">The path to the archive file.</param>
    /// <param name="format">The compression format extension (e.g. "zip", "gz").</param>
    /// <returns><see langword="true"/> if the archive is valid; otherwise, <see langword="false"/>.</returns>
    Task<bool> VerifyAsync(string archivePath, string format);
}
