using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO.Compression;
using McMaster.Extensions.CommandLineUtils;
using Wolfgang.LogCompressor.Model;

namespace Wolfgang.LogCompressor.Command;

/// <summary>
/// Base class providing shared CLI options for compression commands.
/// </summary>
internal abstract class SharedOptions
{
    private const int InvalidFormatSentinel = -1;
    private const int InvalidLevelSentinel = -1;



    /// <summary>
    /// Gets or sets the path to a file or directory to process.
    /// </summary>
    [Required]
    [Argument(0, Description = "Path to a file or directory to process")]
    public required string Path { get; set; }



    /// <summary>
    /// Gets or sets a value indicating whether to recurse into subdirectories.
    /// </summary>
    [Option
    (
        "-r|--recurse",
        Description = "Recurse into subdirectories"
    )]
    public bool Recurse { get; set; }



    /// <summary>
    /// Gets or sets the output directory.
    /// </summary>
    [Option
    (
        "-o|--output",
        Description = "Output directory for compressed files (defaults to source directory)"
    )]
    public string? Output { get; set; }



    /// <summary>
    /// Gets or sets the minimum file age in days.
    /// </summary>
    [Option
    (
        "--older-than",
        Description = "Only include files last modified N+ calendar days ago"
    )]
    [Range(1, int.MaxValue)]
    public int? OlderThan { get; set; }



    /// <summary>
    /// Gets or sets the minimum last-modified date filter.
    /// </summary>
    [Option
    (
        "--min-datetime",
        Description = "Only include files modified on or after this date/time"
    )]
    public string? MinDateTime { get; set; }



    /// <summary>
    /// Gets or sets the maximum last-modified date filter.
    /// </summary>
    [Option
    (
        "--max-datetime",
        Description = "Only include files modified on or before this date/time"
    )]
    public string? MaxDateTime { get; set; }



    /// <summary>
    /// Gets or sets the compression format.
    /// </summary>
    [Option
    (
        "-f|--format",
        Description = "Compression format: zip, gz, brotli (default: zip)"
    )]
    public string Format { get; set; } = "zip";



    /// <summary>
    /// Gets or sets the compression level.
    /// </summary>
    [Option
    (
        "-l|--level",
        Description = "Compression level: fastest, optimal, smallest (default: smallest)"
    )]
    public string Level { get; set; } = "smallest";



    /// <summary>
    /// Validates command options and writes errors to the console.
    /// </summary>
    /// <param name="console">The console to write errors to.</param>
    /// <returns><see langword="true"/> if options are valid; otherwise, <see langword="false"/>.</returns>
    internal bool ValidateOptions(IConsole console)
    {
        var resolvedPath = System.IO.Path.GetFullPath(Path);
        if (!resolvedPath.Equals(Path, StringComparison.Ordinal))
        {
            Path = resolvedPath;
        }

        if (Output != null)
        {
            Output = System.IO.Path.GetFullPath(Output);
        }

        if (OlderThan.HasValue && (MinDateTime != null || MaxDateTime != null))
        {
            console.Error.WriteLine("Error: --older-than cannot be used with --min-datetime or --max-datetime.");
            return false;
        }

        if (!IsValidDateTime(MinDateTime))
        {
            console.Error.WriteLine($"Error: Could not parse --min-datetime value: '{MinDateTime}'");
            return false;
        }

        if (!IsValidDateTime(MaxDateTime))
        {
            console.Error.WriteLine($"Error: Could not parse --max-datetime value: '{MaxDateTime}'");
            return false;
        }

        if (!TryParseFormat(Format, out _))
        {
            console.Error.WriteLine($"Error: Unsupported compression format: '{Format}'. Supported: zip, gz, brotli");
            return false;
        }

        if (!TryParseLevel(Level, out _))
        {
            console.Error.WriteLine($"Error: Unsupported compression level: '{Level}'. Supported: fastest, optimal, smallest");
            return false;
        }

        return true;
    }



    /// <summary>
    /// Builds a <see cref="CompressionOptions"/> from the current CLI values.
    /// </summary>
    /// <returns>A populated <see cref="CompressionOptions"/> instance.</returns>
    internal CompressionOptions BuildOptions()
    {
        TryParseFormat(Format, out var format);
        TryParseLevel(Level, out var level);

        return new CompressionOptions
        {
            SourcePath = System.IO.Path.GetFullPath(Path),
            OutputPath = Output != null ? System.IO.Path.GetFullPath(Output) : null,
            Recurse = Recurse,
            OlderThanDays = OlderThan,
            MinDateTime = ParseDateTime(MinDateTime),
            MaxDateTime = ParseDateTime(MaxDateTime),
            Format = format,
            Level = level
        };
    }



    private static DateTime? ParseDateTime(string? value)
    {
        if (value == null)
        {
            return null;
        }

        return DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out var result)
            ? result
            : null;
    }



    internal static bool IsValidDateTime(string? value)
    {
        return value == null || DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out _);
    }



    private static bool TryParseFormat(string value, out CompressionFormat format)
    {
        format = value.ToLowerInvariant() switch
        {
            "zip" => CompressionFormat.Zip,
            "gz" or "gzip" => CompressionFormat.Gz,
            "br" or "brotli" => CompressionFormat.Brotli,
            _ => (CompressionFormat)InvalidFormatSentinel
        };

        return (int)format != InvalidFormatSentinel;
    }



    private static bool TryParseLevel(string value, out CompressionLevel level)
    {
        level = value.ToLowerInvariant() switch
        {
            "fastest" => CompressionLevel.Fastest,
            "optimal" => CompressionLevel.Optimal,
            "smallest" => CompressionLevel.SmallestSize,
            _ => (CompressionLevel)InvalidLevelSentinel
        };

        return (int)level != InvalidLevelSentinel;
    }
}
