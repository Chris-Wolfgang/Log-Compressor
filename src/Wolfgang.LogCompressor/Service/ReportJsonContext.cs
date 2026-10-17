using System.Text.Json.Serialization;

namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// Source-generated serializer for <see cref="ReportDocument"/>. Same formatting the report always had:
/// indented, camelCase keys, null values omitted.
/// </summary>
[JsonSourceGenerationOptions
(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
[JsonSerializable(typeof(ReportDocument))]
internal sealed partial class ReportJsonContext : JsonSerializerContext
{
}
