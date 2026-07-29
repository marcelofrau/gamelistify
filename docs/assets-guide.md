---
layout: default
title: Assets Guide
---

# Assets Guide

## Directory Structure

```text
Gamelistify/Assets/
├── Fonts/
├── Icons/
│   └── app.ico
├── Themes/
│   ├── WorkbenchResources.axaml
│   └── WorkbenchStyles.axaml
└── Views/
    ├── AboutWindow/
    └── MainWindow/
```

Each view or window that needs its own PNG assets should get a dedicated subfolder under `Assets/Views/`.

## Naming Convention

Pattern:

```text
{viewname}-{descriptor}[-{size}].png
```

Examples:

- `mainwindow-open-32.png`
- `mainwindow-save-32.png`
- `aboutwindow-logo-48.png`
- `aboutwindow-close-32.png`

Rules:

- always lowercase
- use hyphens only
- use PNG for UI icons
- reserve `.ico` for `Assets/Icons/app.ico`

## Personal Icon Set

Developer local source path:

```text
D:\workspace\_non_work_\icons8-personal-set
```

Observed size directories currently available:

- `32x32`
- `48x48`

If additional sizes are added later, keep this document updated.

## Import Workflow

1. choose icon size based on UI context
2. copy PNG from local personal set
3. rename to Gamelistify naming convention
4. place under `Gamelistify/Assets/Views/{ViewName}/`
5. reference via `avares://Gamelistify/Assets/Views/{ViewName}/{filename}`

## Size Guidance

| Context | Preferred size |
|---------|----------------|
| toolbar action | 32x32 |
| dialog action | 32x32 |
| feature tile / body illustration | 48x48 |
| app icon | `.ico` only |

## Redistribution Rule

Do not commit icon binaries unless redistribution is explicitly allowed.

When icons are bundled, add attribution details to project documentation.
