type: fix

`--min-datetime` later than `--max-datetime` is now rejected with exit 1 (`Error: --min-datetime must not be later than --max-datetime.`) instead of matching nothing and exiting 0.
