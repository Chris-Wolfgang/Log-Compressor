# Release Workflow

How a Log-Compressor release is built and published by `.github/workflows/release.yaml`.

Log-Compressor is an application: **nothing is published to NuGet** (`IsPackable` is `false`, see `SECURITY.md` and ADR-0001). A release ships self-contained, trimmed `logc` executables for `win-x64`, `linux-x64` and `osx-x64` as GitHub Release assets, with build provenance, an SBOM and a reproducible-build manifest.

## Trigger

The workflow runs when a GitHub Release is **published**. It builds from the release's tag.

## Jobs

```
Published GitHub Release
        │
        ▼
validate-release (windows-latest)
  • the release tag must match <Version> in the src csproj
  • Release build, all tests with coverage, coverage threshold
        │
        ▼
publish-binaries (ubuntu-latest, one job per RID: win-x64, linux-x64, osx-x64)
  • dotnet publish: self-contained, single-file, trimmed (TrimMode=partial, ADR-0006)
  • scripts/smoke-published.ps1 against the published binary
  • stage the archive (.zip on Windows, .tar.gz elsewhere): logc + AppSettings.json
  • SLSA build provenance attestation for the archive
  • attach the archive to the release
        │
        ▼
sbom-and-manifest (ubuntu-latest)
  • CycloneDX SBOM (logc.bom.json) and THIRD-PARTY-NOTICES.md
  • deterministic build of logc.dll and reproducible-build-manifest.json
  • attach them to the release
```

The template's NuGet jobs (`pack-and-validate`, `publish-nuget`, `update-release-artifacts`) are still in the file but find no packages and do nothing for this repository. `verify-docs-build` and `trigger-docs` build and deploy the DocFX site for the new version.

## One-time configuration

- **No secrets are needed.** There is no `NUGET_API_KEY`; provenance is signed with the workflow's OIDC token (`id-token: write`, `attestations: write` on `publish-binaries`).
- **Branch protection:** the `main` ruleset requires the PR checks from `pr.yaml` to pass, conversation resolution, and forbids force-push and deletion.

## Release checklist

1. Merge everything for the release to `main`.
2. Assemble the changelog from the fragments and bump the version: `pwsh ./scripts/changelog.ps1 assemble` (see `changelog/unreleased/README.md`), and set `<Version>` in `src/Wolfgang.LogCompressor/Wolfgang.LogCompressor.csproj` to the same version. Merge that as a release PR.
3. Locally, `pwsh ./scripts/build-pr.ps1` passes on `main`.
4. Draft a GitHub Release with tag `v<version>` targeting `main`, then publish it.

After the workflow finishes:

- The release has three archives (one per RID), `logc.bom.json`, `THIRD-PARTY-NOTICES.md` and `reproducible-build-manifest.json` attached; the provenance attestations are recorded on GitHub (not as assets).
- `gh attestation verify <archive> --repo Chris-Wolfgang/Log-Compressor` succeeds for each archive.
- A downloaded archive runs: `logc --version` prints the release version.

## Troubleshooting

- **Tag/version mismatch** fails `validate-release` first: the tag (without the leading `v`) must equal the csproj `<Version>`.
- **Smoke test failure** in `publish-binaries` means the trimmed binary misbehaves on that RID even though the tests passed; usually a new reflection dependency (ADR-0006). Reproduce locally with `dotnet publish -r <rid> --self-contained` and `pwsh ./scripts/smoke-published.ps1 -Exe <path>`.
- **Coverage below the threshold** fails `validate-release`; add tests rather than lowering the gate.
- Logs and uploaded artifacts are on the workflow run in the Actions tab.
