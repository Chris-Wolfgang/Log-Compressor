# API Reference

This section documents the internal types that make up `logc`.

`logc` is a CLI application rather than a library — all of its types are
`internal` — so this reference is aimed at **contributors** working on the code,
not at consumers calling a public API. (If you just want to *use* `logc`, see
[Getting Started](~/docs/getting-started.md).)

The architecture is organized in three layers:

- **Command** — the McMaster CommandLineUtils entry points (`Compress`, `Bundle`, `Init`) and shared option binding.
- **Service** — the compression engine (`CompressService`, `BundleService`), filtering, naming, retention, archive verification, and the compression strategies.
- **Abstraction** — the interfaces (`IFileSystem`, `ICompressionStrategy`, `IFileFilter`, `IFileNamer`, `IArchiveVerifier`) the services depend on.

Browse the namespaces in the table of contents on the left for the full type listing.
