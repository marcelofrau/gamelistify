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
Models       -> gamelist entries, media, app settings, profiles
Tests        -> parser, backup rules, scanner, command builders, settings
```

## Planned Project Layout

```text
Gamelistify/
  Assets/
  Controls/
  Models/
  Services/
  ViewModels/
  Views/
Gamelistify.Tests/
build/
docs/
```

## Main Window Shape

The desktop shell will use a three-zone workbench:

1. top command bar for open, save, scan, scrape, and settings
2. central library grid for search, sorting, and multi-selection
3. resizable right detail pane for metadata inspection and image preview

`GridSplitter` is required between list and detail pane.

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

## XML Strategy

- preserve entries and supported metadata fields
- preserve useful output ordering intentionally
- allow normalized indentation on save
- do not optimize for comment/whitespace fidelity in v1.0
- always write a timestamped backup before replacing source file

## Service Map

- `GamelistService`: load, parse, mutate, save gamelist documents
- `BackupService`: create timestamped backups and retention policy later
- `RomScannerService`: detect ROM files missing from current gamelist
- `MediaResolverService`: resolve previewable image assets and known media folders
- `SkyscraperService`: build and run external scraping commands
- `SettingsService`: load and save application settings
- `Logger`: file and console logging

## Phase 1 Status

Implemented in current bootstrap:

- metadata field definitions and path rules
- `GamelistDocument` and `GamelistEntry` models
- XML load and save pipeline with unknown element preservation
- mandatory timestamped backup before overwrite
- ROM scanner service
- image/media resolver service
- Skyscraper command builder and credentials writer
- JSON settings load/save service
- unit tests covering core flows

## Testing Strategy

Required from bootstrap:

- XML parser and writer tests
- backup naming tests
- path normalization tests
- scanner tests
- scraper command construction tests
- settings serialization tests
