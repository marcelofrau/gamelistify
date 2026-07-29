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
