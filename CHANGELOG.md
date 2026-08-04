# Changelog

All notable changes to Gamelistify are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.9.9] - 2026-08-04

### First public beta

This is the initial public beta release of Gamelistify — a cross-platform
desktop workbench for inspecting, editing, and maintaining `gamelist.xml`
libraries used by EmulationStation-compatible frontends (RetroBat, Batocera,
ES-DE, vanilla EmulationStation).

### Added

#### Library management

- Open any `gamelist.xml` and browse entries in a dense, searchable, sortable grid
- Kind filter (All Entries / Games / Folders) and a **Show Hidden** toggle
- Single-entry editing in a resizable detail pane: name, path, genres, ratings,
  dates, players, and more
- Clickable 5-star rating control
- Bulk actions on the selection: hide, unhide, favorite, unfavorite, remove
- Save As via the Save flyout (retargets the document to the new file)
- Recent files list with quick reopen
- Keyboard shortcuts: Ctrl+O, Ctrl+S, Ctrl+F, Ctrl+A, Ctrl+I, H/F/U/G, Delete,
  double-click to edit

#### Safety and backups

- Automatic timestamped backup before every save (`gamelist.<timestamp>.xml.bak`)
  in a hidden `gamelists_backup` folder next to the gamelist
- Restore from any backup with a date/time and size picker — a safety backup of
  the current file is always taken first
- Discard changes and reload from disk when you have unsaved edits
- Confirmation dialogs for every destructive action, including an exit prompt
  when closing with unsaved changes

#### Library hygiene (Tools)

- Set Name from Filename for the selection
- Batch Favorite by Names (one name per line, case-insensitive matching)
- Detect & Hide Duplicates (same file, multiple entries)
- Detect & Hide Bad Versions (regional/edition variants of the same game)
- Review Hidden & Favorites with live preview before applying
- Preferences for favorites of hidden entries are transferred to the kept entry

#### Scraping and scanning

- ROM scan workflow: detect files missing from the gamelist, review candidates,
  and add them in one go
- Skyscraper integration: scrape metadata for selected entries or the whole library
- Live scrape progress with output parsing (percent, ratio, current game) and
  cancellation
- Orphan media scan from the detail pane
- Platform auto-detection from the base directory

#### Media

- Image preview for the selected entry
- Media path editor for image, video, marquee, wheel, fanart, thumbnail, and
  screenshot
- Media resolver that finds previewable assets in known media folders

#### Experience and polish

- Dark, tool-oriented workbench theme with colored Numix icons
- Settings window with runtime log-level switching
- Optimize (minified XML) and Cleanup (remove entries pointing to missing files)
- Quick Tips flyout
- Animated About window
- Visual dimming of the main window while any dialog is open, to keep focus
  on the active task
- Deterministic build number derived from the commit history

### Fixed

- The About window no longer shows the raw Git commit hash in the Build field —
  it now displays a clean, auto-incremented build number
- Split Open/Save toolbar buttons rebuilt as clean single buttons with flyouts
- Removed orphaned chevron styling and icon resources

### Notes

- Backup locations, restore safety, and the full feature specifications are
  documented in the `docs/` folder.
- This is beta software. Automatic backups are enabled, but keep your own copies
  of important libraries as well.
