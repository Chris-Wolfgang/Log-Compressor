type: fix

Zstandard and LZ4 archives are now written with a content checksum (LZ4 also with block checksums), so a corrupted `.zst`, `.lz4`, `.tar.zst` or `.tar.lz4` fails verification and `decompress` instead of extracting damaged data and deleting the archive. Archives written by earlier versions have no checksum and are read as before.
