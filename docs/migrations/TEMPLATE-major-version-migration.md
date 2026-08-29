# Migrating from vX to vY

> Copy this template to `docs/migrations/vX-to-vY.md` during release prep for
> any major version (or any 0.x release with breaking changes) — not after it
> ships. Link the finished guide from the GitHub Release notes.
>
> `logc` is a CLI, so "breaking change" means the **command-line contract**:
> flags renamed/removed, defaults changed, exit codes changed, output-file
> naming changed, report schema changed, or supported-platform changes. There
> is no library API surface.

## Summary

One paragraph: who is affected and how much work the upgrade is.

## Breaking-change inventory

| Change | What breaks | Replacement |
|--------|-------------|-------------|
| e.g. `--older-than` renamed to `--min-age` | Scheduled jobs / `.rsp` files using the old flag fail to parse | Use `--min-age <days>` |

## Before / after

For each change, show the old and new invocation side by side:

```bash
# vX
logc compress /var/log/app --older-than 30

# vY
logc compress /var/log/app --min-age 30
```

Include `.rsp` file diffs when a flag rename affects generated config files —
unattended jobs are this tool's primary consumers, and their `.rsp` files
outlive releases.

## Deprecation timeline

Which flags are deprecated-but-working in vY (with warnings), and in which
version they will be removed.

## Verification

How to confirm the migrated job behaves identically (e.g. run with
`--report` on a scratch directory and diff the report against the old
version's output).
