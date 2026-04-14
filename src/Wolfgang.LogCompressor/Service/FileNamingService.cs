using Wolfgang.LogCompressor.Abstraction;

namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// Generates output file names with embedded timestamps.
/// </summary>
internal sealed class FileNamingService : IFileNamer
{
    private const string DateTimeFormat = "yyyy-MM-dd_HH-mm-ss";



    /// <inheritdoc />
    public string GetCompressedFileName(FileInfo sourceFile, string extension)
    {
        ArgumentNullException.ThrowIfNull(sourceFile);

        var baseName = System.IO.Path.GetFileNameWithoutExtension(sourceFile.Name);
        var modified = sourceFile.LastWriteTime.ToString(DateTimeFormat);

        return $"{baseName}-{modified}.{extension}";
    }



    /// <inheritdoc />
    public string GetBundleFileName(string folderName, IReadOnlyList<FileInfo> files, string extension)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderName);
        ArgumentNullException.ThrowIfNull(files);

        if (files.Count == 0)
        {
            throw new ArgumentException("At least one file is required to generate a bundle name.", nameof(files));
        }

        var minModified = files.Min(f => f.LastWriteTime).ToString(DateTimeFormat);
        var maxModified = files.Max(f => f.LastWriteTime).ToString(DateTimeFormat);

        return $"{folderName}-{minModified} to {maxModified}.{extension}";
    }
}
