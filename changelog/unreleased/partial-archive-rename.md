type: fix

A failed, interrupted or unverifiable compress or bundle no longer leaves an archive at its final name that blocks every later run with "Output archive already exists": archives are written as `.partial` and renamed only once verified, and an archive that fails verification is kept as `.failed` for inspection.
