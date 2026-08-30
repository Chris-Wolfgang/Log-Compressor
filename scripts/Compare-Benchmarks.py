#!/usr/bin/env python3
"""Compare two BenchmarkDotNet JSON reports for the per-PR delta gate (#92).

Usage: Compare-Benchmarks.py <base-report.json> <head-report.json> <output.md>

Exit codes:
  0 - no gated regression (advisory time flags may still appear in the table)
  1 - allocation regression beyond the threshold on at least one benchmark

Policy (fleet-validated on ETL-FixedWidth): allocated bytes per operation are
deterministic managed-heap numbers, so they gate HARD at +50%. Wall-clock on a
shared GitHub-hosted runner is too noisy to fail a PR on, so time deltas past
+20% are flagged in the table but never fail the run.
"""
import json
import sys

ALLOC_GATE = 1.50   # fail when head allocates > 150% of base
TIME_FLAG = 1.20    # advisory marker when head is > 120% of base wall-clock


def load(path):
    with open(path, encoding="utf-8-sig") as f:
        report = json.load(f)
    cases = {}
    for b in report.get("Benchmarks", []):
        name = b.get("FullName") or b.get("DisplayInfo")
        stats = b.get("Statistics") or {}
        memory = b.get("Memory") or {}
        cases[name] = {
            "mean_ns": stats.get("Mean"),
            "alloc_b": memory.get("BytesAllocatedPerOperation"),
        }
    return cases


def fmt_ns(ns):
    if ns is None:
        return "-"
    for unit, div in (("s", 1e9), ("ms", 1e6), ("us", 1e3)):
        if ns >= div:
            return f"{ns / div:,.2f} {unit}"
    return f"{ns:,.0f} ns"


def fmt_b(b):
    return "-" if b is None else f"{b:,.0f} B"


def ratio(head, base):
    if head is None or base is None or base == 0:
        return None
    return head / base


def main():
    base_path, head_path, out_path = sys.argv[1], sys.argv[2], sys.argv[3]
    base, head = load(base_path), load(head_path)

    rows = []
    gate_failures = []
    for name in sorted(head):
        h, b = head[name], base.get(name)
        short = name.split(".")[-1]
        if b is None:
            rows.append(f"| {short} | new | {fmt_ns(h['mean_ns'])} | new | {fmt_b(h['alloc_b'])} | 🆕 |")
            continue

        t_ratio = ratio(h["mean_ns"], b["mean_ns"])
        a_ratio = ratio(h["alloc_b"], b["alloc_b"])
        marks = []
        if a_ratio is not None and a_ratio > ALLOC_GATE:
            marks.append(f"❌ alloc ×{a_ratio:.2f}")
            gate_failures.append(f"{short}: allocations ×{a_ratio:.2f} (gate ×{ALLOC_GATE})")
        if t_ratio is not None and t_ratio > TIME_FLAG:
            marks.append(f"⚠️ time ×{t_ratio:.2f} (advisory)")
        if not marks:
            marks.append("✅")

        rows.append(
            f"| {short} | {fmt_ns(b['mean_ns'])} | {fmt_ns(h['mean_ns'])}"
            f" | {fmt_b(b['alloc_b'])} | {fmt_b(h['alloc_b'])} | {' '.join(marks)} |"
        )

    removed = sorted(set(base) - set(head))
    lines = [
        "<!-- pr-benchmarks -->",
        "## Benchmark delta (PerfSmoke, base vs HEAD)",
        "",
        "| Benchmark | Base time | HEAD time | Base alloc | HEAD alloc | Verdict |",
        "|---|---|---|---|---|---|",
        *rows,
    ]
    if removed:
        lines += ["", "Removed vs base: " + ", ".join(n.split(".")[-1] for n in removed)]
    lines += [
        "",
        f"Gate: allocations > ×{ALLOC_GATE} fail (deterministic); time > ×{TIME_FLAG} is advisory only"
        " (shared-runner wall-clock noise). Label the PR `perf-impact-acknowledged` to accept a flagged"
        " allocation change.",
    ]

    with open(out_path, "w", encoding="utf-8") as f:
        f.write("\n".join(lines) + "\n")

    if gate_failures:
        print("ALLOCATION GATE FAILED:")
        for g in gate_failures:
            print("  " + g)
        return 1

    print("No gated regressions.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
