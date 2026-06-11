namespace Wolfgang.LogCompressor.Abstraction;

/// <summary>
/// Abstracts file system operations for testability.
/// </summary>
internal interface IFileSystem
{
    /// <summary>
    /// Enumerates files matching the specified search pattern.
    /// </summary>
    /// <param name="path">The directory to search.</param>
    /// <param name="searchPattern">The search pattern to match.</param>
    /// <param name="searchOption">Specifies whether to search the top directory only or all subdirectories.</param>
    /// <returns>An enumerable collection of file paths.</returns>
    IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption);



    /// <summary>
    /// Gets file information for the specified path.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns>A <see cref="FileInfo"/> for the specified path.</returns>
    FileInfo GetFileInfo(string path);



    /// <summary>
    /// Opens a file for reading.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>A read-only <see cref="Stream"/>.</returns>
    Stream OpenRead(string filePath);



    /// <summary>
    /// Creates a file for writing, overwriting if it already exists.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>A writable <see cref="Stream"/>.</returns>
    Stream CreateWrite(string filePath);



    /// <summary>
    /// Deletes the specified file.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    void DeleteFile(string filePath);



    /// <summary>
    /// Determines whether the specified file exists.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns><see langword="true"/> if the file exists; otherwise, <see langword="false"/>.</returns>
    bool FileExists(string path);



    /// <summary>
    /// Determines whether the specified directory exists.
    /// </summary>
    /// <param name="path">The directory path.</param>
    /// <returns><see langword="true"/> if the directory exists; otherwise, <see langword="false"/>.</returns>
    bool DirectoryExists(string path);



    /// <summary>
    /// Creates all directories and subdirectories in the specified path.
    /// </summary>
    /// <param name="path">The directory path.</param>
    void CreateDirectory(string path);
}
