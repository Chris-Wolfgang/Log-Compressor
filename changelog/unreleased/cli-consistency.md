type: fix

`bundle` with no matching files now exits 0 ("No files matched; nothing bundled.") like `compress`, instead of 11; `decompress --report` accepts `JSON`/`CSV` in any case like the other commands; `logc init` refuses to overwrite an existing config file unless `--force` is given; the `--output` help text states each command's default.
