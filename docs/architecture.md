---
layout: default
title: Architecture
---

# Architecture

## Principles

- rewrite cleanly instead of porting Python UI structure
- keep domain logic separate from Avalonia views
- document decisions early
- preserve XML semantics first, formatting second
- make backup creation mandatory and observable

## Target Layers

```text
Views        -> Avalonia windows, user controls, dialogs
ViewModels   -> state, commands, orchestration
Services     -> XML I/O, scan, scrape, settings, backup, logging
Models       -> gamelist entries, media, app settings, backup info
Tests        -> parser, backup rules, scanner, command builders, settings
```

## Project Layout

```text
Gamelistify/
  Assets/
    Themes/        -> WorkbenchResources.axaml, WorkbenchStyles.axaml
    Views/         -> per-view PNG asset folders (MainWindow, AboutWindow)
  Helpers/         -> AppPaths, BuildInfo, Converters, ProjectInfo
  Models/          -> GamelistDocument, GamelistEntry, AppSettings, BackupInfo, ...
  Services/        -> GamelistService, BackupService, RomScannerService,
                      MediaResolverService, SkyscraperService, SettingsService, Logger
  ViewModels/      -> MainViewModel, dialog/child view-models
  Views/           -> MainWindow, AboutWindow, ConfirmWindow, ExitConfirmWindow,
                      RestoreBackupWindow, ScanRomsWindow, ScrapeOptionsWindow,
                      ScrapeProgressWindow, SettingsWindow, SplashWindow
  App.axaml(.cs)   -> composition root; wires services and dialog delegates
Gamelistify.Tests/
build/             -> build, run, and release scripts
docs/              -> project documentation
.github/           -> CI workflows and contribution templates
```

## Composition Root

`App.axaml.cs` constructs the `MainViewModel` and wires its dialog delegates
(open-file picker, confirmations, exit confirmation, restore-backup picker,
scan review, scrape options, settings). Views stay free of service creation;
ViewModels never construct windows.

## Main Window Shape

The desktop shell uses a three-zone workbench:

1. top command toolbar for open, save, add, revert, restore, scan, scrape, hide,
   favorite, remove, optimize, settings, and about
2. central library grid for search, sorting, filtering, and multi-selection
3. resizable right detail pane for metadata inspection, editing, and image preview

A `GridSplitter` separates the grid and detail pane.

## Data Flow

```text
User action
  -> View command binding
  -> ViewModel command
  -> Service call
  -> Model/result
  -> ViewModel observable state
  -> Avalonia binding refresh
```

## Service Map

- `GamelistService`: load, parse, mutate, save gamelist documents; compact save for Optimize
- `BackupService`: hidden `gamelists_backup` folder, timestamped backup creation,
  backup listing, and restore (with safety backup)
- `RomScannerService`: detect ROM files missing from the current gamelist
- `MediaResolverService`: resolve previewable image assets and known media folders
- `SkyscraperService`: build and run external scraping commands; parse progress output
- `SettingsService`: JSON load and save of application settings
- `GamelistPathHelper`: path normalization for entries and media
- `Logger`: Serilog file and console logging with runtime level switching

## Backup Strategy

- Backups are stored in a hidden `gamelists_backup` subfolder next to the source file.
- Backup name: `gamelist.<timestamp>.xml.bak`.
- Every write path (save, optimize, cleanup, restore) creates a backup first.
- Restore additionally takes a safety backup of the current file before copying.
- See [specs.md](specs) for the full backup/restore behavior.

## XML Strategy

- preserve entries and supported metadata fields
- preserve useful output ordering intentionally
- allow normalized indentation on save
- do not optimize for comment/whitespace fidelity
- always write a timestamped backup before replacing the source file

## Implemented Status

- core domain models and metadata field definitions
- `GamelistDocument` and `GamelistEntry` models
- XML load and save pipeline with unknown element preservation
- mandatory timestamped backup before overwrite, plus restore flow
- ROM scanner service and review workflow
- image/media resolver service and media path editor
- Skyscraper command builder, credentials writer, and progress parsing
- JSON settings load/save service with runtime log-level switching
- unit tests covering core flows

## Testing Strategy

Required from bootstrap and maintained since:

- XML parser and writer tests
- backup naming, listing, and restore tests
- path normalization tests
- scanner tests
- scraper command construction tests
- settings serialization tests
- progress-parsing and recent-file view-model tests

Tests run on Windows, Linux, and macOS via GitHub Actions.
