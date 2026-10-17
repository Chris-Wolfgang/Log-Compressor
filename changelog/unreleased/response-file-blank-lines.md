type: fix

A response file (`@file`) with blank lines, whitespace-only lines or indented `#` comments no longer fails with "Unrecognized command or argument": the file `logc init` generates now works after editing it as its header instructs.
