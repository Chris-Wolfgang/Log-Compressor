type: fix

`decompress` now checks the gzip trailer (CRC-32 and length) of `.gz` and `.tar.gz` archives, so a truncated or corrupted gzip archive fails and is kept instead of being extracted as a shorter file, reported as success and deleted.
