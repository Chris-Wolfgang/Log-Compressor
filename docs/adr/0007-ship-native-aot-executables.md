# ADR-0007: Ship logc as a Native AOT executable per RID

- **Status:** Accepted
- **Date:** 2026-11-12 (recorded 2026-11-28; the decision shipped in #375, `a54e933`)

## Context

The native binaries are logc's product ([ADR-0001](0001-distribute-self-contained-binaries-not-nuget.md)),
and the fleet's aot-smoke standard (decision 2026-10-30) asks every shipped executable to publish
with Native AOT and to prove the native image works by running it.
[ADR-0006](0006-trim-self-contained-binaries-in-partial-mode.md) had kept Native AOT out because
McMaster.Extensions.CommandLineUtils 5.1's AOT source generator does not compile against this
app's command layer. That generator turns on by itself when `PublishAot=true`, so adopting AOT
meant working around it rather than waiting for a move off McMaster
([ADR-0002](0002-mcmaster-commandlineutils-over-system-commandline.md)).

## Decision

`logc` publishes as a Native AOT executable for each release RID (`win-x64`, `linux-x64`,
`linux-arm64`, `osx-x64`, `osx-arm64`). The load-bearing details, all in
`src/Wolfgang.LogCompressor/Wolfgang.LogCompressor.csproj` and `Program.cs`:

- `PublishAot`, `PublishTrimmed`, `IsAotCompatible` and `TrimMode=partial` (the partial mode of
  ADR-0006 is kept). `InvariantGlobalization` is not set: [ADR-0004](0004-locale-aware-datetime-parsing.md)
  parses `--min-datetime` / `--max-datetime` with the current culture.
- **McMaster's AOT generator is removed** before compilation (`_RemoveMcMasterAotGenerator`). It
  fails with CS7036 / CS9035 / CS0122 / CS1503 against this command layer and drops the options
  `Compress` and `Bundle` inherit from `SharedOptions`. McMaster binds by reflection, as in the
  JIT build.
- **`logc` and McMaster are rooted** (`TrimmerRootAssembly`), so the reflection metadata McMaster
  looks up by name (commands, options, `OnExecute`, the version member) survives. Without the
  roots every command fails at startup.
- **`Program.PrecompileValueParsers`** calls `GetParser<int?>()` so the generic instantiation
  McMaster makes through `MakeGenericMethod` exists in the native image.
- McMaster's trim / AOT warnings are reported as one summary per assembly (`IL2104` / `IL3053`,
  `_McMasterSingleWarn`) and only those two are in `WarningsNotAsErrors`; every other assembly,
  logc included, still fails the Release build on any IL warning.
- Native AOT cannot cross-compile between operating systems, so `release.yaml` publishes each RID
  on a runner of its own OS and architecture, and runs `scripts/smoke-published.ps1` against
  each native binary before it is archived and attested.

## Consequences

- The release binaries no longer contain a bundled .NET runtime or a `logc.dll`; they are native
  executables. The deterministic-build hash in `reproducible-build-manifest.json` is the hash of
  the **managed** `logc.dll` built from the tagged source. It proves the source builds
  reproducibly; it does not prove anything about the bytes of the shipped executables, whose
  integrity is covered by the SLSA provenance attestation (see `docs/REPRODUCIBLE-BUILD.md`).
- The smoke test remains the safeguard for McMaster's reflection use, now against the native
  image on every RID.
- A new dependency must be trim- and AOT-safe, and a new McMaster version must be checked against
  the generator removal and the roots.
- Revisit when the CLI layer moves to a trim-safe parser (ADR-0002) or McMaster's generator
  handles this app: the generator removal, the roots, `PrecompileValueParsers` and the
  `IL2104` / `IL3053` exception can then go.
