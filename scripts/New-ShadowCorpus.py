#!/usr/bin/env python3
"""Generate the shadow-testing corpus (#69): a realistic mixed-shape log tree.

Usage: New-ShadowCorpus.py <output-dir> [seed]

Deterministic for a given seed so the current build and the baseline release
binary compress IDENTICAL input. Shapes chosen to mirror real log dirs rather
than benchmark-friendly uniformity: many small files, a few large ones, one
empty file, nested subdirectories, and mixed extensions.
"""
import random
import sys
from pathlib import Path

LINE = (
    "2026-03-15 23:00:15.123 [{lvl}] Processing request id={rid:08x} "
    "method={m} path={p} status={s} duration={d}ms\n"
)
LEVELS = ["INF"] * 5 + ["DBG"] * 2 + ["WRN", "ERR"]
METHODS = ["GET", "POST", "PUT", "DELETE"]
PATHS = ["/api/data", "/api/users", "/api/orders", "/health", "/auth/token"]


def write_log(path: Path, size: int, rnd: random.Random) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    with open(path, "w", encoding="utf-8", newline="\n") as f:
        written = 0
        while written < size:
            line = LINE.format(
                lvl=rnd.choice(LEVELS),
                rid=rnd.randrange(2**31),
                m=rnd.choice(METHODS),
                p=rnd.choice(PATHS),
                s=500 if rnd.randrange(10) == 0 else 200,
                d=rnd.randrange(1, 1200),
            )
            f.write(line)
            written += len(line)


def main() -> int:
    out = Path(sys.argv[1])
    seed = int(sys.argv[2]) if len(sys.argv) > 2 else 42
    rnd = random.Random(seed)

    # 20 small files (2-50 KB), 3 medium (1-4 MB), 1 large (16 MB),
    # 1 empty, nested dirs, mixed extensions.
    for i in range(20):
        write_log(out / f"app-{i:02}.log", rnd.randrange(2_000, 50_000), rnd)
    for i in range(3):
        write_log(out / "service" / f"service-{i}.log", rnd.randrange(1, 4) * 1_048_576, rnd)
    write_log(out / "archive" / "big.log", 16 * 1_048_576, rnd)
    (out / "empty.log").write_text("")
    write_log(out / "notes.txt", 5_000, rnd)

    total = sum(p.stat().st_size for p in out.rglob("*") if p.is_file())
    count = sum(1 for p in out.rglob("*") if p.is_file())
    print(f"corpus: {count} files, {total:,} bytes (seed={seed})")
    return 0


if __name__ == "__main__":
    sys.exit(main())
