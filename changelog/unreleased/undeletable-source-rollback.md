type: fix

When `compress` cannot delete a source after archiving it (a file another process holds open, a read-only file), the archive is now removed again, so the failed file no longer leaves an archive that makes every later run refuse it with "Output archive already exists". `bundle` does the same when its first source cannot be deleted; once a source is deleted the archive is its only copy and is kept.
