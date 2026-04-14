using System.Diagnostics.CodeAnalysis;

namespace Wolfgang.LogCompressor.Model.Framework;

[ExcludeFromCodeCoverage]
internal record ConfigurationFile
{
    public required string Name { get; init; }
    public bool Optional { get; init; }
    public bool ReloadOnChange { get; init; }

}