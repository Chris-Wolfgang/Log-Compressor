namespace Wolfgang.LogCompressor.Abstraction;

/// <summary>
/// Defines a compression strategy for a specific format.
/// </summary>
internal interface ICompressionStrategy
{
    /// <summary>
    /// Gets the file extension for this compression format (e.g. "zip", "gz", "br").
    /// </summary>
    string FileExtension { get; }



    /// <summary>
    /// Gets the file extension used when bundling multiple files (e.g. "zip", "tar.gz", "tar.br").
    /// </summary>
    string BundleFileExtension { get; }



    /// <summary>
    /// Compresses a single file into an archive.
    /// </summary>
    /// <param name="inputStream">The source file stream.</param>
    /// <param name="outputStream">The destination archive stream.</param>
    /// <param name="entryName">The name of the entry within the archive.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task CompressFileAsync
    (
        Stream inputStream,
        Stream outputStream,
        string entryName,
        CancellationToken cancellationToken = default
    );



    /// <summary>
    /// Compresses multiple files into a single archive. The sequence is enumerated
    /// lazily and the implementation takes ownership of each input stream, disposing
    /// it immediately after writing — so the caller can open one source at a time
    /// rather than holding every file handle open for the duration of a large bundle.
    /// </summary>
    /// <param name="inputs">A lazily-enumerated sequence of stream and entry name pairs. Each stream is disposed by the implementation once it has been written.</param>
    /// <param name="outputStream">The destination archive stream.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task CompressFilesAsync
    (
        IAsyncEnumerable<(Stream Stream, string EntryName)> inputs,
        Stream outputStream,
        CancellationToken cancellationToken = default
    );
}
