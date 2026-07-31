---
layout: default
title: Master Checklist
---

# Master Checklist

## Phase 0 - Bootstrap and Specification

- [x] Confirm project name: `Gamelistify`
- [x] Confirm stack: .NET 8 + Avalonia 12 + MVVM
- [x] Confirm English-only documentation requirement
- [x] Confirm image-first preview strategy
- [x] Confirm XML preservation strategy and mandatory backups
- [x] Create repository scaffold
- [x] Create documentation baseline in `docs/`
- [x] Finalize solution structure and package set
- [x] Add CI baseline

## Phase 1 - Core Domain Foundation

- [x] Define core models
- [x] Define supported metadata fields
- [x] Implement XML load service
- [x] Implement XML save service
- [x] Implement timestamped backup service
- [x] Implement ROM scanner
- [x] Implement image/media resolver
- [x] Add unit tests for core services

## Phase 2 - Desktop Shell

- [x] Build main window workbench layout
- [x] Add command toolbar
- [x] Add searchable/sortable grid
- [x] Add resizable detail pane
- [x] Add image preview surface
- [x] Add theme resource dictionaries

## Phase 3 - Editing Workflow

- [x] Add single-entry editing workflow
- [x] Add bulk actions workflow
- [x] Add dirty-state indicators
- [x] Add recent files support
- [x] Add settings window (log level combo, taller)
- [x] Add Quick Tips flyout (green, styled FlyoutPresenter)
- [x] Replace toolbar icons with personal icons8 set
- [x] Add star rating control (5 clickable stars)
- [x] Add confirmation dialogs (discard, remove)
- [x] Add remove entry from detail view
- [x] Add remove selected entries toolbar button
- [x] Add BoolToStarBrush converter
- [x] Add ConfirmWindow/ConfirmViewModel
- [x] Add Serilog runtime level switching

## Phase 4 - Scraping Workflow

- [x] Implement Skyscraper settings
- [x] Implement scrape selected flow
- [x] Implement scrape all flow
- [x] Implement log/progress UI
- [x] Implement cancel handling
- [x] Implement post-scrape refresh flow

## Phase 5 - OSS Polish

- [x] Add release build scripts
- [x] Add packaging matrix
- [ ] Add screenshots
- [x] Expand docs for website publishing
- [x] Add contribution templates

## Phase 6 - Feature Parity Gaps

- [x] Add kind filter combo (All Entries / Games / Folders)
- [x] Add per-row tint for favorite / hidden entries
- [x] Add keyboard shortcuts (Ctrl+O, Ctrl+S, Ctrl+F, Delete, double-click edit)
- [x] Add kid game boolean editing
- [x] Add media path editor (image/video/marquee/wheel/fanart/thumbnail/screenshot) with browse buttons
- [x] Add orphan media scan from detail pane
- [x] Add progress parsing (percent / ratio / current game) with progress bar
- [x] Add platform auto-detection from base directory
- [ ] Add screenshots (deferred)
