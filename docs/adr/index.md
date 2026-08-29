# Architecture Decision Records

Short records of the non-obvious design choices in this repo — the context
that forced a decision, the decision, and what it trades away. New ADRs land
alongside the PR that introduces the decision and are part of that review.
Start from [TEMPLATE.md](TEMPLATE.md).

| ADR | Title | Status |
|-----|-------|--------|
| [0001](0001-distribute-self-contained-binaries-not-nuget.md) | Distribute self-contained per-RID binaries, not a NuGet package | Accepted |
| [0002](0002-mcmaster-commandlineutils-over-system-commandline.md) | McMaster.Extensions.CommandLineUtils over System.CommandLine | Accepted |
| [0003](0003-verify-then-delete-original.md) | Compress → verify → only then delete the original | Accepted |
| [0004](0004-locale-aware-datetime-parsing.md) | `--min-datetime` / `--max-datetime` parse with the local culture | Accepted |
