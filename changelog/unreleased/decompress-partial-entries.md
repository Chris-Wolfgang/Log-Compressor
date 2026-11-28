type: fix

`decompress` now writes each extracted file under a `.partial` name and gives it its final name only once it is complete, so a failed or interrupted extraction no longer leaves a truncated file that blocks the re-run with "Extraction target already exists", and `--force` replaces an existing file only with a complete one.
