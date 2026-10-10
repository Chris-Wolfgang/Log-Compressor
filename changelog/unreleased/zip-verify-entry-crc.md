type: fix

Archive verification now checks each zip entry's recorded CRC-32 and length, so a zip whose entry data is corrupt (but whose directory is intact) fails verification and the original file is kept instead of deleted.
