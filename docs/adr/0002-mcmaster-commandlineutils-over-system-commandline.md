# ADR-0002: McMaster.Extensions.CommandLineUtils over System.CommandLine

- **Status:** Accepted
- **Date:** 2026-08-29 (decision originally made during initial development; recorded retroactively)

## Context

The CLI needs sub-commands (`compress`, `bundle`, `init`), shared options
across sub-commands, validation, and config-file support so unattended jobs
can keep their arguments in a versioned file rather than a brittle scheduler
command line. The two mainstream choices were Microsoft's System.CommandLine
and McMaster.Extensions.CommandLineUtils.

## Decision

We use **McMaster.Extensions.CommandLineUtils** (with
`McMaster.Extensions.Hosting.CommandLine` for generic-host integration).
Attribute-based command/option declaration keeps each sub-command a plain
class (`SharedOptions` base class carries the common flags), validation hooks
are first-class, and **response files** (`@args.rsp`, line-separated) provide
the config-file mechanism the `init` sub-command generates — no custom config
parsing layer needed.

At the time of the decision System.CommandLine was still churning through
pre-2.0 API redesigns, and its binding model would have required more
hand-rolled plumbing for the shared-options inheritance shape this app uses.

## Consequences

- Sub-commands stay small, declarative classes; shared flags live once in
  `SharedOptions`.
- Response-file support came for free and became the `init` workflow.
- McMaster is community-maintained; if it goes dormant, migrating to
  System.CommandLine (once stable) is a contained change — the command layer
  is thin and the service layer below it is CLI-framework-agnostic.
- Generic-host integration means DI, logging, and configuration behave like
  any other hosted .NET app.
