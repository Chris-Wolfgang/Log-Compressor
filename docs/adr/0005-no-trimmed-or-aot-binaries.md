# ADR-0005: Ship untrimmed self-contained binaries — no PublishTrimmed / PublishAot

- **Status:** Accepted
- **Date:** 2026-08-29

## Context

Fleet issue #81 asks every repo to verify its output survives .NET trimming
and Native AOT. For a library that means a trimmed consumer app; for this CLI
the equivalent is publishing `logc` itself with `<PublishTrimmed>` /
`<PublishAot>` to shrink the ~70 MB self-contained bundles.

The experiment was run (win-x64, Release, `PublishSingleFile` +
`PublishTrimmed=true`): **the publish fails** with IL2104 — McMaster
CommandLineUtils 5.1.0 produces trim warnings (its command/option binding
walks attributes and properties via unannotated reflection), and this repo
builds with warnings-as-errors. The Hosting/DI stack underneath adds more of
the same. Native AOT would fail for the same reason with a worse failure
mode (runtime `MissingMetadataException` instead of build-time warnings).

The workaround exists: suppress IL2104 and add `<TrimmerRootAssembly>` for
`logc` + McMaster so the trimmer keeps everything they touch. But rooted-
everything trimming only shaves the BCL, and every suppression converts a
build-time warning into a *potential silent behavioural change* — a member
the trimmer removed that reflection finds missing (or worse, quietly skips)
at runtime.

## Decision

`logc` ships **untrimmed** self-contained single-file binaries. No
`PublishTrimmed`, no `PublishAot`, and no IL2104 suppressions.

The deciding factor is the failure mode, not the size: this tool's job is to
**delete original files** after compressing them, unattended. A trim-induced
silent misbehaviour in option binding or verification is a category of bug we
refuse to trade for bundle size. (Compress-side damage is bounded by the
verify-then-delete pipeline — ADR-0003 — but "bounded" is not "acceptable".)

Config binding is already source-generated (`EnableConfigurationBindingGenerator`),
so the app's *own* code stays trim-clean; the blocker is the CLI framework.

## Consequences

- Release archives stay runtime-sized (tens of MB per RID). Acceptable for a
  server tool installed once.
- Revisit if/when the command layer migrates to a trim-safe CLI framework
  (System.CommandLine has first-class trim/AOT support — the migration path
  already noted in ADR-0002), or McMaster ships trim annotations.
- Fleet issue #81 is closed by this ADR rather than by a smoke workflow —
  there is nothing to smoke-test until the decision above is revisited.
