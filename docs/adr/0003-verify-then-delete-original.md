# ADR-0003: Compress → verify → only then delete the original

- **Status:** Accepted
- **Date:** 2026-08-29 (decision originally made during initial development; recorded retroactively)

## Context

`logc`'s whole value is reclaiming disk space, so after compressing a log file
the original must be removed. Deleting a source file is the one irreversible
action in the tool: if the archive it just wrote is truncated or corrupt (full
disk, torn write, flaky network share), deleting the original destroys the
only good copy. The primary use case is *unattended* scheduled jobs — nobody
is watching to notice a bad archive before the original is gone.

## Decision

The pipeline is **compress → flush/close the archive → verify the archive →
delete the original**, in that order:

- Verification re-opens the written archive and checks it is structurally
  sound for its format (`ArchiveVerifier`).
- If verification fails, the original is kept, the result is reported as a
  failure, and the (suspect) archive is left for inspection.
- `--no-verify` exists as an explicit opt-out for callers who prefer speed
  over the safety re-read (e.g. enormous archives on slow storage) — the
  default is always verify.

## Consequences

- The failure mode "archive corrupt AND original deleted" is designed out of
  the default path; unattended jobs fail safe.
- Every file is read twice by default (compress + verify), roughly doubling
  I/O per file. For scheduled off-peak compression jobs this cost is
  acceptable; `--no-verify` is the pressure valve.
- Verification checks archive integrity, not byte-for-byte content equality —
  a format-level check is the pragmatic middle ground between "no check" and
  "decompress and diff everything" (which would triple I/O).
