using System.Globalization;
using Wolfgang.LogCompressor.Abstraction;

namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// Generates output file names with embedded timestamps.
/// </summary>
internal sealed class FileNamingService : IFileNamer
{
    // Always formatted with the invariant culture: archive names must be
    // byte-identical regardless of the host's locale. Cultures with a
    // non-Gregorian default calendar (ar-SA → Umm al-Qura) or native digit
    // shaping would otherwise change the embedded timestamp.
    private const string DateTimeFormat = "yyyy-MM-dd_HH-mm-ss";



    /// <inheritdoc />
    public string GetCompressedFileName(FileInfo sourceFile, string extension)
    {
        ArgumentNullException.ThrowIfNull(sourceFile);
        ArgumentException.ThrowIfNullOrWhiteSpace(extension);

        var baseName = System.IO.Path.GetFileNameWithoutExtension(sourceFile.Name);
        var modified = sourceFile.LastWriteTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture);

        return $"{baseName}-{modified}.{extension}";
    }



    /// <inheritdoc />
    public string GetBundleFileName(string folderName, IReadOnlyList<FileInfo> files, string extension)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderName);
        ArgumentNullException.ThrowIfNull(files);
        ArgumentException.ThrowIfNullOrWhiteSpace(extension);

        if (files.Count == 0)
        {
            throw new ArgumentException("At least one file is required to generate a bundle name.", nameof(files));
        }

        var minModified = files.Min(f => f.LastWriteTime).ToString(DateTimeFormat, CultureInfo.InvariantCulture);
        var maxModified = files.Max(f => f.LastWriteTime).ToString(DateTimeFormat, CultureInfo.InvariantCulture);

        return $"{folderName}-{minModified} to {maxModified}.{extension}";
    }
}
