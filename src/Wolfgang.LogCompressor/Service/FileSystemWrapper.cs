using System.Diagnostics.CodeAnalysis;
using Wolfgang.LogCompressor.Abstraction;

namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// Thin wrapper around <see cref="System.IO"/> for testability.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class FileSystemWrapper : IFileSystem
{
    /// <inheritdoc />
    public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
    {
        return Directory.EnumerateFiles(path, searchPattern, searchOption);
    }



    /// <inheritdoc />
    public FileInfo GetFileInfo(string path)
    {
        return new FileInfo(path);
    }



    /// <inheritdoc />
    public Stream OpenRead(string filePath)
    {
        return File.OpenRead(filePath);
    }



    /// <inheritdoc />
    public Stream CreateWrite(string filePath)
    {
        return File.Create(filePath);
    }



    /// <inheritdoc />
    public void DeleteFile(string filePath)
    {
        File.Delete(filePath);
    }



    /// <inheritdoc />
    public bool FileExists(string path)
    {
        return File.Exists(path);
    }



    /// <inheritdoc />
    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }



    /// <inheritdoc />
    public void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }
}
