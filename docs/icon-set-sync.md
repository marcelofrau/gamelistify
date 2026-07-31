---
layout: default
title: Icon Set Sync
published: false
---

# Icon Set Sync

Gamelistify uses the **Numix icon theme** for all UI icons.

## Local Path

```text
F:\workspace\numix-icon-theme\
```

Colored sources: `Numix\32\` and `Numix\scalable\`.
Monochrome symbolic sources: `Numix\scalable\actions\*-symbolic.svg` and `Numix-Light\scalable\`.

## Recommended Approaches

1. local clone only during development (never commit the theme itself)
2. render PNGs into `Gamelistify/Assets/Views/{ViewName}/` following `docs/assets-guide.md`
3. commit only the rendered PNGs, never the raw SVG sources

## Checklist Before Committing Icons

- confirm the Numix GPL-3.0 license matches the project (it does)
- keep the attribution page (`docs/attributions.md`) in sync
- keep filenames aligned with `docs/assets-guide.md`
- avoid absolute paths inside project files
- follow Numix symlinks to real SVG files before rendering
