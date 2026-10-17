type: internal

The JSON report is written directly with `Utf8JsonWriter`, and the `Logging` configuration section is read by hand, instead of by two .NET source generators. Behaviour and the report output are unchanged (byte for byte); the generators wrote their code with the build machine's newline, which made `logc.dll` differ between Windows and Linux builds.
