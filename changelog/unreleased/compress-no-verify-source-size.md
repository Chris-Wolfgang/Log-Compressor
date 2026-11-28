type: fix

`compress --timestamp compressed --no-verify` no longer aborts the run with "Could not find file" after archiving the first file: the source size is read before the source is deleted.
