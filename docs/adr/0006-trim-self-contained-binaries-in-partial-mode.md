# ADR-0006: Trim the self-contained binaries, in partial mode, behind a smoke test

- **Status:** Accepted
- **Date:** 2026-10-17

## Context

[ADR-0005](0005-no-trimmed-or-aot-binaries.md) shipped untrimmed binaries because
`PublishTrimmed` failed with `IL2104` from McMaster.Extensions.CommandLineUtils, and the
workaround traded a build-time warning for a possible silent run-time change in a tool that
deletes originals after compressing them. That risk is real, and it was measured rather than
assumed (win-x64, published as `release.yaml` does):

- A default trim (`TrimMode=full`) builds, but **every command fails at startup**, even
  `--version`: the trimmer removes `Program.GetVersion` and the other members McMaster looks up
  by name through reflection.
- `TrimMode=partial` trims the .NET runtime and the libraries that opt in to trimming, and
  leaves `logc` and McMaster whole. McMaster then works, because the types it reflects over are
  kept. One behaviour broke: `--report json` failed, because the report was serialized by
  reflection-based `System.Text.Json`, which is switched off in trimmed builds.

What ADR-0005 treated as a standing risk can now be checked: a script can run the published
binary through every behaviour a trim could break, and it fails the build when one does.

## Decision

Self-contained publishes are trimmed in **partial** mode (`PublishTrimmed` + `TrimMode=partial`
in the csproj, conditional on `SelfContained`).

- The JSON report is built from named records and serialized by a source-generated
  `JsonSerializerContext`. The unit tests run with reflection-based JSON switched off, so a
  regression fails an ordinary test.
- `IL2104` (McMaster, Serilog.Exceptions, System.Configuration.ConfigurationManager) is listed
  in `WarningsNotAsErrors`. **This departs from ADR-0005's "no IL2104 suppressions":** the
  warnings are not hidden and still appear in every publish. What replaces the build error as the
  safeguard is `scripts/smoke-published.ps1`, which runs the published executable (version and
  help, a compress and decompress round trip for every format with SHA-256 verification,
  bundling, the JSON report, the exit code of each kind of invalid input, and an edited `init`
  response file) and exits non-zero on any failure. CI runs it against the trimmed binary.
- Native AOT stays out. McMaster 5.1's AOT source generator does not compile against this app's
  command layer (service parameters on `OnExecuteAsync`, `required` members, a private version
  member), and the generic-host integration is not covered by it.

## Consequences

- Measured on win-x64: `logc.exe` 37.2 MB to 13.6 MB, and the release zip 32.1 MB to 8.6 MB.
- Partial mode keeps `logc` and McMaster untrimmed, so the saving comes from the runtime and
  the annotated libraries, not from the app. That is the reason it is safe, and the limit of the
  saving.
- The smoke test is now part of how a release is validated. A new feature that depends on
  reflection (a new serializer call, a new reflection-driven library) will fail it, which is the
  point; the fix is to give it a source-generated or otherwise trim-safe path.
- Not verified when this was written: the Linux and macOS binaries (the first CI run of the smoke
  test on those runners is their check), and whether Serilog.Exceptions logs the same exception
  detail when the framework types it reflects over have been trimmed.
- Revisit full trimming and AOT together with a move off McMaster (see ADR-0002), which is the
  only dependency that blocks either.
