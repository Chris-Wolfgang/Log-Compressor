namespace Wolfgang.LogCompressor.Abstraction;

/// <summary>
/// Generates output file names for compressed archives.
/// </summary>
internal interface IFileNamer
{
    /// <summary>
    /// Generates the output file name for a single compressed file.
    /// </summary>
    /// <param name="sourceFile">The source file being compressed.</param>
    /// <param name="extension">The compression format extension (e.g. "zip").</param>
    /// <returns>The output file name including extension.</returns>
    string GetCompressedFileName(FileInfo sourceFile, string extension);



    /// <summary>
    /// Generates the output file name for a bundle of compressed files.
    /// </summary>
    /// <param name="folderName">The source folder name.</param>
    /// <param name="files">The files being bundled.</param>
    /// <param name="extension">The compression format extension (e.g. "zip").</param>
    /// <returns>The output file name including extension.</returns>
    string GetBundleFileName(string folderName, IReadOnlyList<FileInfo> files, string extension);
}
