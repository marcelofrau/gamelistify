---
layout: default
title: Branching and Versioning
---

# Branching and Versioning

## Semantic Versioning

Gamelistify follows **Semantic Versioning 2.0.0**.

Version shape:

```text
MAJOR.MINOR.PATCH
```

Rules:

- `MAJOR`: breaking change
- `MINOR`: backward-compatible feature
- `PATCH`: backward-compatible fix, refactor, docs, or polish

Before `1.0.0`, minor releases may still include breaking changes.

## Build Number

Gamelistify also tracks an incremental build number.

Current version metadata is defined in:

```text
Directory.Build.props
```

Fields:

- `VersionMajor`
- `VersionMinor`
- `VersionPatch`
- `BuildNumber`

Generated outputs:

- `Version`: semantic version
- `FileVersion`: semantic version plus build number
- `InformationalVersion`: semantic version plus `+build.N`

Example:

```text
Version            0.1.0
BuildNumber        1
InformationalVersion 0.1.0+build.1
```

## Version Source of Truth

Canonical project-wide version source:

```text
Directory.Build.props
```

## Incrementing the Build Number

Use helper script:

```powershell
.\build\increment-build-number.ps1
```

This increments `BuildNumber` in `Directory.Build.props`.

## Release Workflow

1. choose semantic version bump
2. update `VersionMajor`, `VersionMinor`, or `VersionPatch`
3. increment `BuildNumber`
4. commit version change
5. tag release as `vMAJOR.MINOR.PATCH`

## Branches

- `main`: always intended to stay buildable
- `feat/<name>`: new feature work
- `fix/<name>`: bug fixes
- `chore/<name>`: tooling, cleanup, refactors
- `docs/<name>`: documentation-only work

## Commits

Use Conventional Commits:

- `feat:`
- `fix:`
- `chore:`
- `docs:`
- `perf:`
- `style:`

## Application Display

The application About window should display:

- semantic version
- build label
- repository link
- issue tracker link
- license
