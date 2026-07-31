---
layout: default
title: Roadmap
---

# Roadmap

Current version: **0.1.0** — pre-1.0. Status reflects the `main` branch.

## Phase 0 - Bootstrap and Specification

- [x] repository scaffold
- [x] project documentation baseline
- [x] solution and test project
- [x] theme direction
- [x] master checklist

## Phase 1 - Core Domain Foundation

- [x] XML models
- [x] load/save services
- [x] backup service
- [x] ROM scanner
- [x] image resolver
- [x] automated tests

## Phase 2 - Desktop Shell

- [x] main window workbench layout
- [x] command bar
- [x] searchable grid
- [x] detail pane
- [x] theme resources
- [x] real file open flow
- [x] About window with version metadata

## Phase 3 - Editing Workflow

- [x] single-entry editing
- [x] bulk actions
- [x] dirty-state handling
- [x] recent files
- [x] settings (log level, column visibility)
- [x] recent file reopen flow
- [x] Quick Tips green flyout
- [x] star rating control (5 clickable stars)
- [x] confirmation dialogs (discard, remove)
- [x] remove entry from detail view
- [x] remove selected entries toolbar
- [x] Serilog runtime level switching
- [x] CanExecute wiring for toolbar buttons

## Phase 3.5 - Library Maintenance Utilities

- [x] ROM scan review dialog
- [x] add missing ROM entries into current document

## Phase 4 - Scraping Workflow

- [x] Skyscraper configuration
- [x] scrape selected
- [x] scrape all
- [x] progress and cancellation
- [x] refresh workflow
- [x] options dialog for platform and extra arguments
- [x] live output log window
- [x] progress parsing (percent / ratio / current game)
- [x] platform auto-detection from base directory

## Phase 4.5 - Safety and Recovery

- [x] hidden `gamelists_backup` folder next to the gamelist
- [x] backup listing with date/time and size
- [x] restore-from-backup picker window
- [x] safety backup before restore
- [x] Revert (discard changes and reload), enabled only while dirty

## Phase 5 - OSS Polish

- [x] release build scripts
- [x] packaging matrix
- [x] CI on Windows, Linux, and macOS
- [x] docs expansion for website publishing
- [x] issue and pull request templates
- [x] Numix icon theme integration (GPL-3.0, attribution in docs)
- [ ] screenshots in README and docs
- [ ] website publishing from `docs/`

## Phase 6 - Pre-1.0 Hardening

- [ ] first tagged release with prebuilt binaries
- [ ] end-to-end smoke testing on all three platforms
- [ ] batch/undo editing polish (multi-edit across entries)
- [ ] backup retention policy (e.g. cap number of backups)
- [ ] localization-readiness review (string externalization)
- [ ] performance pass on large gamelists (100k+ entries)
