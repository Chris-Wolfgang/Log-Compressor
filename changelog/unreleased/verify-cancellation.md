type: fix

Ctrl+C during archive verification now stops the run promptly instead of after the whole archive has been re-read, a corrupt `.zst` archive is skipped under `--on-error skip` instead of aborting the whole `decompress` run, and `.tar.gz` bundles are verified in one read instead of two.
