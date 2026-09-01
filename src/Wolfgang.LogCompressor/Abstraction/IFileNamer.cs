using Wolfgang.LogCompressor.Model;

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
    /// <param name="timestampSource">Which timestamp to embed in the name.</param>
    /// <param name="namePrefix">Custom base name replacing the source file's name, or <see langword="null"/> for the default.</param>
    /// <returns>The output file name including extension.</returns>
    string GetCompressedFileName(FileInfo sourceFile, string extension, TimestampSource timestampSource = TimestampSource.Modified, string? namePrefix = null);



    /// <summary>
    /// Generates the output file name for a bundle of compressed files.
    /// </summary>
    /// <param name="folderName">The source folder name.</param>
    /// <param name="files">The files being bundled.</param>
    /// <param name="extension">The compression format extension (e.g. "zip").</param>
    /// <param name="timestampSource">Which timestamp to embed in the name.</param>
    /// <param name="namePrefix">Custom base name replacing the folder name, or <see langword="null"/> for the default.</param>
    /// <returns>The output file name including extension.</returns>
    string GetBundleFileName(string folderName, IReadOnlyList<FileInfo> files, string extension, TimestampSource timestampSource = TimestampSource.Modified, string? namePrefix = null);
}
