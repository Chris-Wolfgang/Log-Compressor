using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace Wolfgang.LogCompressor.Framework;

/// <summary>
/// Binds the <c>Logging</c> configuration section to <see cref="LoggingOptions"/> by hand.
/// </summary>
/// <remarks>
/// Replaces <c>IConfiguration.Get&lt;LoggingOptions&gt;()</c>. The reflection-based binder is not
/// trim-safe, and the source-generated one writes its code with the build machine's newline, which
/// made <c>logc.dll</c> differ between Windows and Linux builds. This section is small and fixed,
/// so reading it directly costs nothing. Behaviour follows the framework binder: keys are
/// case-insensitive, a missing key keeps the default, and a value that does not convert throws.
/// </remarks>
internal static class LoggingOptionsBinder
{
    /// <summary>
    /// Reads <paramref name="section"/> into a new <see cref="LoggingOptions"/>.
    /// </summary>
    /// <param name="section">The <c>Logging</c> configuration section.</param>
    /// <returns>The options; all defaults when the section does not exist.</returns>
    /// <exception cref="InvalidOperationException">A value cannot be converted to its property's type.</exception>
    internal static LoggingOptions Bind(IConfigurationSection section)
    {
        ArgumentNullException.ThrowIfNull(section);

        var options = new LoggingOptions();
        if (!section.Exists())
        {
            return options;
        }

        options.MinimumLevel = ReadEnum(section, nameof(LoggingOptions.MinimumLevel), options.MinimumLevel);

        foreach (var entry in section.GetSection(nameof(LoggingOptions.Overrides)).GetChildren())
        {
            if (entry.Value is { Length: > 0 } text)
            {
                options.Overrides[entry.Key] = Parse<Serilog.Events.LogEventLevel>(entry, text);
            }
        }

        var console = section.GetSection(nameof(LoggingOptions.Console));
        options.Console.Enabled = ReadBool(console, nameof(ConsoleSinkOptions.Enabled), options.Console.Enabled);
        options.Console.MinimumLevel = ReadEnum(console, nameof(ConsoleSinkOptions.MinimumLevel), options.Console.MinimumLevel);
        options.Console.OutputTemplate = ReadString(console, nameof(ConsoleSinkOptions.OutputTemplate), options.Console.OutputTemplate);

        var file = section.GetSection(nameof(LoggingOptions.File));
        options.File.Enabled = ReadBool(file, nameof(FileSinkOptions.Enabled), options.File.Enabled);
        options.File.Path = ReadString(file, nameof(FileSinkOptions.Path), options.File.Path);
        options.File.MinimumLevel = ReadEnum(file, nameof(FileSinkOptions.MinimumLevel), options.File.MinimumLevel);
        options.File.RollingInterval = ReadEnum(file, nameof(FileSinkOptions.RollingInterval), options.File.RollingInterval);
        options.File.RollOnFileSizeLimit = ReadBool(file, nameof(FileSinkOptions.RollOnFileSizeLimit), options.File.RollOnFileSizeLimit);
        options.File.FileSizeLimitBytes = ReadLong(file, nameof(FileSinkOptions.FileSizeLimitBytes), options.File.FileSizeLimitBytes);
        options.File.OutputTemplate = ReadString(file, nameof(FileSinkOptions.OutputTemplate), options.File.OutputTemplate);

        return options;
    }



    // A key that is absent keeps the default. An empty value for a non-string property is treated as absent too.
    private static string ReadString(IConfigurationSection parent, string key, string current)
    {
        return parent[key] ?? current;
    }



    private static bool ReadBool(IConfigurationSection parent, string key, bool current)
    {
        var text = parent[key];
        if (string.IsNullOrEmpty(text))
        {
            return current;
        }

        return bool.TryParse(text, out var value) ? value : throw ConversionFailed(parent.GetSection(key), text, typeof(bool));
    }



    private static long ReadLong(IConfigurationSection parent, string key, long current)
    {
        var text = parent[key];
        if (string.IsNullOrEmpty(text))
        {
            return current;
        }

        return long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : throw ConversionFailed(parent.GetSection(key), text, typeof(long));
    }



    private static T ReadEnum<T>(IConfigurationSection parent, string key, T current)
        where T : struct, Enum
    {
        var text = parent[key];
        return string.IsNullOrEmpty(text) ? current : Parse<T>(parent.GetSection(key), text);
    }



    private static T Parse<T>(IConfigurationSection at, string text)
        where T : struct, Enum
    {
        return Enum.TryParse<T>(text, ignoreCase: true, out var value)
            ? value
            : throw ConversionFailed(at, text, typeof(T));
    }



    private static InvalidOperationException ConversionFailed(IConfigurationSection at, string text, Type type)
    {
        return new InvalidOperationException($"Failed to convert configuration value '{text}' at '{at.Path}' to type '{type}'.");
    }
}
