---
layout: default
title: Packaging
---

# Packaging

## Local Development

Run app:

```powershell
.\build\run.ps1
```

Build solution:

```powershell
.\build\build.ps1
```

## Release Archives

Windows:

```powershell
.\build\build-release.ps1 -Version 0.1.0 -Rid win-x64
```

Linux/macOS:

```bash
./build/build-release.sh 0.1.0 linux-x64
./build/build-release.sh 0.1.0 osx-arm64
```

Artifacts are written to:

```text
build/dist/
```

## Current Release Targets

- `win-x64`
- `linux-x64`
- `osx-arm64`

Additional RIDs can be added later if needed.

## Automated Release Pipeline

Pushing a tag `vMAJOR.MINOR.PATCH` triggers the GitHub Actions workflow
(`.github/workflows/build.yml`):

1. **test** — build and run the test suite on Windows, Linux, and macOS.
2. **release** — publish self-contained archives per target RID and verify the
   native executable exists inside each ZIP.
3. **publish** — collect all archives, generate SHA256 checksums, create the
   GitHub Release with the checksums in the body, then run a VirusTotal scan on
   each ZIP and append the results to the release.

### Requirements

- A tag push such as `git tag v0.1.0 && git push origin v0.1.0`.
- The repository secret `VT_API_KEY` (VirusTotal API key) configured under
  **Settings → Secrets and variables → Actions**. Without it the VirusTotal step
  is skipped; the release itself still succeeds.

### Notes

- Version tags must be prefixed with `v` (e.g. `v0.1.0`); the build scripts strip
  the prefix.
- A VirusTotal scan of a newly uploaded sample can take minutes to complete; the
  workflow waits and appends the result to the release body.
