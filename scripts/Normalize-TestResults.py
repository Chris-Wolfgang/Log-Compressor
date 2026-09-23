#!/usr/bin/env python3
"""Normalize a TRX test-results file to a platform-comparable outcome list (#78).

Usage: Normalize-TestResults.py <results.trx> <output.txt>

Emits one sorted line per test: "<outcome>\t<fully-qualified test name>".
Everything platform-variable (timings, machine names, output text, absolute
paths) is discarded — two runs on different OS/arch legs must produce
byte-identical output unless a test genuinely passed on one leg and failed on
another, which is exactly the divergence the differential workflow exists to
catch.

Deliberately outcome-level rather than message-level (a scoped-down reading
of #78's "assertion messages and reported numbers"): passing tests carry no
assertion text, and failing tests' messages embed platform-variable paths and
values that would make every real divergence drown in noise. A test that
fails on one leg surfaces as an outcome divergence; that leg's own test log
carries the message.
"""
import sys
import xml.etree.ElementTree as ET  # nosemgrep

NS = {"t": "http://microsoft.com/schemas/VisualStudio/TeamTest/2010"}


def main():
    if len(sys.argv) != 3:
        print(__doc__, file=sys.stderr)
        return 2
    trx_path, out_path = sys.argv[1], sys.argv[2]

    # xml.etree is safe against external-entity expansion and DTD retrieval; what it does not
    # guard on its own is entity-expansion denial of service (billion laughs, quadratic blowup),
    # which is what Semgrep's use-defused-xml rule is about. On the Python this workflow pins
    # (3.12) expat already refuses those - a bomb raises "limit on input amplification factor
    # (from DTD and entities) breached" - so this is defence in depth, not a live hole: it makes
    # the refusal explicit and keeps holding if the pinned version ever moves. Both vectors need
    # a DTD with entity definitions, so refusing a DOCTYPE closes them without pulling defusedxml
    # into CI; this script runs on setup-python with no pip step, and adding a dependency would
    # be more supply-chain surface than the vector it removes. A TRX emitted by dotnet test never
    # carries a DOCTYPE; one that does is not a TRX we should be reading.
    with open(trx_path, 'rb') as probe:
        head = probe.read(8192)
    if b'<!DOCTYPE' in head or b'<!ENTITY' in head:
        print(f'{trx_path}: refusing to parse - the file declares a DTD or entities', file=sys.stderr)
        return 2

    tree = ET.parse(trx_path)  # nosemgrep

    lines = []
    for result in tree.getroot().iter(f"{{{NS['t']}}}UnitTestResult"):
        name = result.get("testName") or ""
        outcome = result.get("outcome") or "Unknown"
        # Theory rows repeat testName with parameters embedded — keep as-is,
        # they're deterministic. NotExecuted == skipped; keep visible so a
        # test silently skipping on ONE platform also counts as divergence.
        lines.append(f"{outcome}\t{name}")

    lines.sort()
    with open(out_path, "w", encoding="utf-8", newline="\n") as f:
        f.write("\n".join(lines) + "\n")

    print(f"{len(lines)} test outcomes -> {out_path}")
    if not lines:
        print("ERROR: no test results found in TRX — treat as failure", file=sys.stderr)
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
