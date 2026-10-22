type: fix

`--delete-archives-older-than` now deletes only archives named the way logc names them (and, for `bundle`, only archives of that bundle's base name), so unrelated `.zip`/`.gz`/... files in the scanned directory are no longer deleted; `compress --recurse` without `--output` now also cleans the subdirectories its archives were written to.
