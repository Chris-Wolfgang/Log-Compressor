using System.Globalization;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Model;

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



    // Captured once on first use so every archive named during one run embeds
    // the SAME "compressed" timestamp — per-call now could straddle a second
    // boundary and scatter a batch across different suffixes. logc is a
    // one-shot CLI, so instance lifetime == run lifetime.
    private readonly Lazy<DateTime> _runTimestamp;



    /// <summary>
    /// Initializes a new instance of the <see cref="FileNamingService"/> class.
    /// </summary>
    /// <param name="timeProvider">The time source for compressed-mode timestamps.</param>
    public FileNamingService(TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);

        _runTimestamp = new Lazy<DateTime>(() => timeProvider.GetLocalNow().DateTime);
    }



    /// <inheritdoc />
    public string GetCompressedFileName(FileInfo sourceFile, string extension, TimestampSource timestampSource = TimestampSource.Modified, string? namePrefix = null)
    {
        ArgumentNullException.ThrowIfNull(sourceFile);
        ArgumentException.ThrowIfNullOrWhiteSpace(extension);

        var baseName = namePrefix ?? Path.GetFileNameWithoutExtension(sourceFile.Name);
        var timestamp = timestampSource == TimestampSource.Compressed
            ? _runTimestamp.Value
            : sourceFile.LastWriteTime;

        return $"{baseName}-{timestamp.ToString(DateTimeFormat, CultureInfo.InvariantCulture)}.{extension}";
    }



    /// <inheritdoc />
    public string GetBundleFileName(string folderName, IReadOnlyList<FileInfo> files, string extension, TimestampSource timestampSource = TimestampSource.Modified, string? namePrefix = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderName);
        ArgumentNullException.ThrowIfNull(files);
        ArgumentException.ThrowIfNullOrWhiteSpace(extension);

        if (files.Count == 0)
        {
            throw new ArgumentException("At least one file is required to generate a bundle name.", nameof(files));
        }

        var baseName = namePrefix ?? folderName;

        if (timestampSource == TimestampSource.Compressed)
        {
            // One archive, one job time — a min/max range adds nothing here.
            var now = _runTimestamp.Value.ToString(DateTimeFormat, CultureInfo.InvariantCulture);
            return $"{baseName}-{now}.{extension}";
        }

        var minModified = files.Min(f => f.LastWriteTime).ToString(DateTimeFormat, CultureInfo.InvariantCulture);
        var maxModified = files.Max(f => f.LastWriteTime).ToString(DateTimeFormat, CultureInfo.InvariantCulture);

        return $"{baseName}-{minModified} to {maxModified}.{extension}";
    }
}
