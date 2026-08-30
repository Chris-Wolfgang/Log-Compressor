# Security Policy

## Reporting a Vulnerability

If you discover a security vulnerability, please follow these steps:

1. **Do not** create a public issue on this repository.
2. In the top navigation of this repository, click the **Security** tab.
3. In the top right, click the **Report a vulnerability** button.
4. Fill out the provided form with:
   - A description of the vulnerability
   - Steps to reproduce the issue
   - Potential impact
   - Suggested fix (if you have one)

## Response Timeline

We will acknowledge your report within 48 hours and provide an estimated timeline for a fix.

## Thank You

Your help is greatly appreciated!
Responsible disclosure of security vulnerabilities helps protect our entire community.

## Release path & compromise scope

Facts a maintainer would need at 2am if the release identity is compromised. Generic incident-response steps (rotating credentials, revoking OAuth apps, publishing advisories) are not duplicated here — GitHub's own docs update faster than a checked-in runbook.

- **Release path**: `logc` ships as self-contained per-RID binaries (win-x64, linux-x64, osx-x64) attached to the GitHub Release by `.github/workflows/release.yaml` on the `release: published` trigger, using the workflow's own `GITHUB_TOKEN` (`contents: write`). **This repo publishes nothing to NuGet** — `<IsPackable>false</IsPackable>` makes the workflow's NuGet jobs skip via the `has-packages` gate (see [ADR-0001](docs/adr/0001-distribute-self-contained-binaries-not-nuget.md)). The skipped NuGet job references a `NUGET_API_KEY` secret; if that secret exists in this repo's settings it is dead weight — during an incident, delete it.
- **Fallback**: none. There is no publish identity outside GitHub itself — a compromise of the release path IS a compromise of the GitHub account or repository (branch/rule tampering, malicious Release asset swap).
- **Owner**: @Chris-Wolfgang.
- **Downstream consumers**: no known Wolfgang.* dependents (this is an end-user CLI, not a library). Unknown third parties may have downloaded release binaries — a compromised release requires editing the Release notes with a warning and deleting/replacing the affected assets, since binaries cannot be recalled.
- **Package coordinates for unlisting**: none on nuget.org. The unit of revocation is the GitHub Release asset: delete the compromised asset(s) from the release and, if the tag itself is suspect, delete and re-tag from an audited commit.
