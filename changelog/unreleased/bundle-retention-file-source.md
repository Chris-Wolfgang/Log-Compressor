type: fix

`bundle <file> --delete-archives-older-than N` now deletes that file's old archives: the retention target is resolved before the bundle runs, instead of after the source file was deleted, when it was taken for a directory and matched nothing.
