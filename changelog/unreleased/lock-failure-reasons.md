type: fix

A lock that cannot be taken for a reason other than another instance no longer reports "Another instance is already processing this directory." with exit 2: a source path whose directory does not exist now prints `Source path not found` and a lock file that cannot be created (read-only or access-denied directory) prints the reason and the `--no-lock` hint, both with exit 11.
