type: fix

`bundle --recurse` now stores each file under its path relative to the source directory, so same-named files in different subdirectories are no longer written as duplicate entries and the layout is restored by `decompress`. `decompress` fails an archive that holds the same entry twice, even with `--force`, and keeps it, instead of extracting the copies over each other and deleting the archive.
