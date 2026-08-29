# ADR-0004: `--min-datetime` / `--max-datetime` parse with the local culture

- **Status:** Accepted
- **Date:** 2026-08-29 (decision originally made during initial development; recorded retroactively)

## Context

The date-range filter flags take datetime strings from the command line.
Library code in this codebase follows the usual rule — machine-facing parsing
uses `CultureInfo.InvariantCulture` — and an automated reviewer flagged the
`CultureInfo.CurrentCulture` parse in `SharedOptions` as a bug on that basis.
But these values are typed by a human operator (or put in an `.rsp` file by
one) on the machine where the job runs.

## Decision

`--min-datetime` and `--max-datetime` are parsed with
**`CultureInfo.CurrentCulture`, deliberately**. An operator on a German server
writes `31.01.2026`, a US operator writes `1/31/2026`, and both mean the same
day; forcing invariant (US-shaped) parsing would silently misread or reject
the local format. ISO 8601 (`2026-01-31`) parses correctly under every
culture, so scripts that want culture-independence simply use ISO — which is
what the documentation and examples show.

This is a **documented allowlist entry**: these two flags are the
intentionally culture-sensitive surface of the CLI. Everything else (file
naming timestamps, report output, size formatting) is culture-invariant by
contract.

## Consequences

- Human-entered dates behave the way the operator's own OS does — least
  surprise for the interactive case.
- The same `.rsp` file can parse differently on machines with different
  locales. Mitigation: examples use ISO 8601, which is locale-proof.
- Culture-invariance tests (fleet issue #83) must exempt these two flags and
  assert the rest of the surface is invariant.
