# AGENTS.md — Gamelistify

Avalonia 12 / .NET 8 desktop editor for EmulationStation `gamelist.xml` libraries. Build per-csproj: `dotnet build Gamelistify\Gamelistify.csproj`. Tests: `dotnet test Gamelistify.Tests\Gamelistify.Tests.csproj`. User runs/tests the app manually.

## Icons
- All images come from the Numix theme. Local clone: `D:\workspace\numix-icon-theme\` (outside this repo).
  - Colored icons: `D:\workspace\numix-icon-theme\Numix\32\` (and `Numix\scalable\` SVG sources). Monochrome symbolic: `Numix\scalable\actions\*-symbolic.svg`, light variant `Numix-Light\scalable\`.
  - License GPL-3.0 — matches this app's license. Always extract from the local clone before hand-drawing geometry.
  - Prefer colored Numix icons over monochrome. Dialog and toolbar buttons use colored renders (PNG, 32px source scaled to the button size).
- Toolbar/status/dialog button images are PNG files under `Assets\Views\MainWindow\` named `mainwindow-*.png`, referenced via `avares://Gamelistify/Assets/Views/MainWindow/<name>.png`.
  - Toolbar button icons: `Path.toolbar-icon` / `Image.toolbar-icon` style, 32px source rendered at ~24-26px.
  - Dialog button icons: `Image.dialog-icon` style, rendered ~16-18px.
- Static `Geometry` resources still exist in `Assets\Themes\WorkbenchResources.axaml` under `Icon*` keys (`IconPlus`, `IconFolder`, `IconSearch`, `IconStar`, `IconChevronDown`, ...). Use them only for in-place glyphs (search box, chevrons, star column) that have no colored equivalent.
- Numix SVG source notes: some filenames are symlinks. `gtk-revert-to-saved-ltr.svg` → real file `document-revert.svg`; `document-open-recent.svg` → `default-document-open-recent.svg`; `folder.svg` → `default-folder.svg`. When a render needs colors, open the SVG and adjust the fill(s) to a Numix color (e.g. green `#859900`, red `#dc322f`, blue `#268bd2`, folder orange `#F2BB64`/`#EA9036`) before exporting to PNG.
- To add an icon: find the Numix SVG (follow symlinks), set the fill to the appropriate color, export to PNG into `Assets\Views\MainWindow\` as `mainwindow-<name>-32.png`, and reference it with `avares://...`. Do not invent custom geometries.

## Backups
- Backups of a gamelist are stored in a hidden `gamelists_backup\` subfolder next to the `gamelist.xml`, named `gamelist.<timestamp>.xml.bak`. Managed by `BackupService` (`GetBackupDirectory`, `CreateBackupAsync`, `GetBackups`, `RestoreBackupAsync`). Restore always creates a safety backup of the current file first.
