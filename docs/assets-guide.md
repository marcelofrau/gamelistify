---
layout: default
title: Assets Guide
---

# Assets Guide

## Directory Structure

```text
Gamelistify/Assets/
├── Themes/
│   ├── WorkbenchResources.axaml
│   └── WorkbenchStyles.axaml
└── Views/
    ├── AboutWindow/
    └── MainWindow/
```

Each view or window that needs its own PNG assets gets a dedicated subfolder under `Assets/Views/`.

## Naming Convention

Pattern:

```text
mainwindow-{descriptor}-32.png
```

Examples:

- `mainwindow-open-32.png`
- `mainwindow-save-32.png`
- `mainwindow-restore-32.png`
- `aboutwindow-logo-48.png`

Rules:

- always lowercase
- use hyphens only
- use PNG for UI icons
- always 32px source renders (scaled down by style classes at use site)

## Icon Source: Numix Icon Theme

All UI icons come from the **Numix icon theme** local clone:

```text
F:\workspace\numix-icon-theme\
```

- Colored icons: `Numix\32\` (PNG) and `Numix\scalable\` (SVG sources)
- Monochrome symbolic icons: `Numix\scalable\actions\*-symbolic.svg`, light variant `Numix-Light\scalable\`
- License: **GPL-3.0** — matches this app's license. Redistribution is allowed with attribution.
- Always extract from the local clone before hand-drawing geometry.
- Prefer colored Numix icons over monochrome. Dialog and toolbar buttons use colored renders.

Some Numix filenames are symlinks. When following them, use the real file:

| Symlink | Real file |
|---------|-----------|
| `gtk-revert-to-saved-ltr.svg` | `document-revert.svg` |
| `document-open-recent.svg` | `default-document-open-recent.svg` |
| `folder.svg` | `default-folder.svg` |

## Geometry Resources

Static `Geometry` resources remain in `Assets\Themes\WorkbenchResources.axaml` under `Icon*` keys (`IconPlus`, `IconFolder`, `IconSearch`, `IconStar`, `IconChevronDown`, ...). Use them **only** for in-place glyphs (search box, chevrons, star column) that have no colored equivalent. Buttons use PNG images.

## Render Workflow

1. pick the Numix SVG source (follow symlinks)
2. set the fill to the appropriate color (e.g. green `#859900`, red `#dc322f`,
   blue `#268bd2`, folder orange `#F2BB64`/`#EA9036`)
3. export to PNG at 32px into `Gamelistify/Assets/Views/{ViewName}/` as `mainwindow-{descriptor}-32.png`
4. reference via `avares://Gamelistify/Assets/Views/{ViewName}/{filename}`
5. update `docs/attributions.md` icon inventory

## Style Classes

| Context | Class | Rendered size |
|---------|-------|---------------|
| toolbar action icon | `Image.toolbar-icon` | ~24-26px |
| dialog action icon | `Image.dialog-icon` | ~16-18px |
| in-place glyph | `Path` with `Icon*` Geometry | varies |

## Redistribution Rule

All bundled icons are GPL-3.0 from Numix and may be redistributed with the app.
Keep the attribution page (`docs/attributions.md`) in sync with the asset inventory.
