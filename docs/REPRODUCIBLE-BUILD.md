# Reproducible builds — verify it yourself

`logc` is built deterministically, so anyone can rebuild the exact same
compiled assembly from the tagged source and confirm a release was built from
that source and nothing else. This page is the **consumer side** of that
guarantee: how *you* independently verify a release.

> Related: [`reproducible-build.yaml`](../.github/workflows/reproducible-build.yaml)
> proves the build is reproducible on every PR (it builds the same commit twice
> in different paths and fails if the assemblies differ). This document is about
> a third party reproducing it out-of-band.

## What is reproducible — and what is not

The unit of verification is the **managed assembly `logc.dll`**
(single target, `net10.0`). It is byte-for-byte reproducible because the build
sets:

- `Deterministic` (default) — no timestamps or random GUIDs baked into the IL;
- `ContinuousIntegrationBuild=true` on CI — normalizes source paths to `/_/` so
  the checkout directory doesn't affect output;
- `EmbedUntrackedSources` + SourceLink — provenance is embedded deterministically.

> **The downloadable release archives (`logc-win-x64.zip`,
> `logc-linux-x64.tar.gz`, `logc-osx-x64.tar.gz`) are *not* byte-for-byte
> reproducible.** A self-contained single-file bundle embeds the .NET runtime
> and the archives record per-entry timestamps, so their hashes vary between
> builds. The manifest lists their hashes for **download-integrity** checks;
> the *build* is verified against the assembly.

For the release archives, the stronger guarantee is the **SLSA provenance
attestation**: every archive is attested at build time with the workflow's
OIDC identity, binding its SHA-256 to this repo, commit and workflow run.
Verify a downloaded archive with:

```bash
gh attestation verify logc-win-x64.zip --repo Chris-Wolfgang/Log-Compressor
```

## The per-release manifest

Every GitHub release attaches **`reproducible-build-manifest.json`**, produced
by [`release.yaml`](../.github/workflows/release.yaml) from the exact build
that was published. It records the expected assembly hash plus the toolchain
that produced it:

```jsonc
{
  "tag": "v0.2.0",
  "commit": "…",
  "dotnetSdk": "10.0.100",          // the SDK you must use to reproduce
  "runnerOs": "Linux",
  "buildConfiguration": "Release",
  "assemblies": [
    { "tfm": "net10.0", "file": "logc.dll", "sha256": "…" }
  ],
  "releaseAssets": [                 // download-integrity only — not byte-reproducible
    { "file": "logc-win-x64.zip", "sha256": "…" }
  ],
  "sbom": { "file": "logc.bom.json", "sha256": "…" }
}
```

Download it from the release page, or with the CLI:

```bash
gh release download v0.2.0 --repo Chris-Wolfgang/Log-Compressor --pattern reproducible-build-manifest.json
```

## Reproduce it

You need the **same .NET SDK version** the manifest records in `dotnetSdk`
(a different SDK ships a different Roslyn and may emit different IL). Install
it from <https://dotnet.microsoft.com/download/dotnet>.

```bash
# 1. Clone the source at the exact published tag.
git clone --depth 1 --branch v0.2.0 https://github.com/Chris-Wolfgang/Log-Compressor
cd Log-Compressor

# 2. Build Release with the CI determinism flag (matches the release build).
dotnet build src/Wolfgang.LogCompressor/Wolfgang.LogCompressor.csproj \
  -c Release -p:ContinuousIntegrationBuild=true

# 3. Hash the assembly and compare against the manifest.
sha256sum src/Wolfgang.LogCompressor/bin/Release/net10.0/logc.dll
```

If your `sha256` matches the manifest's `assemblies[0].sha256`, the published
release is reproducible from source on your machine.

## If a hash does not match

A mismatch is worth reporting — it may be a determinism regression, a
toolchain difference, or something more serious.

1. Double-check you used the **exact** `dotnetSdk` from the manifest
   (`dotnet --version` must match) and passed `-p:ContinuousIntegrationBuild=true`.
2. [Open an issue](https://github.com/Chris-Wolfgang/Log-Compressor/issues/new)
   titled `reproducible-build discrepancy: <tag>` and include: the tag, your
   `dotnet --version`, your OS, and the assembly hash you got versus the
   manifest's. Label it `maintenance - security`.

## Publish a third-party verification attestation

Independent verifications make the guarantee stronger than a single
publisher's claim. If you reproduced a release, you can publish an attestation
others can find:

- **Reproducible Builds conventions** — follow
  <https://reproducible-builds.org/docs/> to record your environment and result.
- **[vouchsafe.io](https://vouchsafe.io/)** (or a similar attestation service) —
  publish a signed statement that tag `vX.Y.Z` reproduced to the manifest hash
  in your environment, and link it back on the release discussion.

Link your attestation in a comment on the release so future consumers can find
corroborating verifications.
