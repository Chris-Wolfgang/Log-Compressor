type: fix

A corrupt `.br` or `.tar.br` archive no longer aborts the whole `decompress` run with exit 11: it fails that archive under `--on-error` like any other bad archive, and the run continues.
