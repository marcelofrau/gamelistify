---
layout: default
title: Feature Specifications
---

# Feature Specifications

Detailed behavior specifications for the implemented feature set. Each spec lists
the observable behavior, the owning service/view-model, and the acceptance criteria.

## Backup and Restore

**Owners:** `BackupService`, `MainViewModel` (`SaveAsync`, `RestoreBackupAsync`, `ReloadAsync`)

### Backup creation

- Backups live in a hidden `gamelists_backup` subfolder **next to** the `gamelist.xml`.
- The folder is created on first backup and marked with the `Hidden` file attribute.
- Backup file name: `gamelist.<timestamp>.xml.bak`, where timestamp is
  `yyyy-MM-ddTHH-mm-ss.fff` (local time).
- Every save, optimize, and cleanup that rewrites the source file writes a backup first.
- The first save after loading a file produces a backup; the backup path is reported in the status bar.

### Backup listing

- `BackupService.GetBackups(sourcePath)` returns all backups for the given gamelist.
- Each entry exposes the file path, the parsed timestamp, a human-readable
  date/time (`MMM d, yyyy HH:mm:ss`), and a formatted file size (B/KB/MB).
- Results are sorted newest first.

### Restore

- The toolbar **Restore** button opens a picker window listing all backups with
  date/time and size.
- Only backups of the current gamelist file are shown (matched by file prefix).
- Restoring a backup first creates a safety backup of the current gamelist, then
  copies the selected backup over the source file.
- The document is reloaded immediately after a successful restore.
- Restore is disabled until a gamelist is loaded; a message is shown when no
  backups exist.

### Discard / reload

- The toolbar **Revert** button discards unsaved changes and reloads the file
  from disk.
- It is only enabled while the document has unsaved changes (`IsDirty`).
- A confirmation dialog is shown before discarding.

### Acceptance criteria

- Loading, editing, saving, and restoring never lose the safety backup chain.
- Restore always leaves at least the pre-restore state available as a backup.

## XML Load and Save

**Owners:** `GamelistService`, `GamelistDocument`, `GamelistEntry`

### Load

- Parse `gamelist.xml` preserving every entry as a game or folder.
- Supported metadata fields are mapped into `GamelistEntry` (name, path, genres,
  rating, playcount, favorite, hidden, kid, dates, and media paths).
- Unknown elements are preserved on save where feasible (semantic-first strategy).
- Relative paths are kept usable for frontend consumption.

### Save

- Output is normalized: stable element ordering, readable indentation.
- `compact` mode writes minified XML for the Optimize action.
- Boolean fields are written predictably.
- Comments and whitespace are **not** guaranteed to round-trip.

### Acceptance criteria

- `gamelist.xml` produced by the app opens correctly in EmulationStation-compatible
  frontends (RetroBat, Batocera, ES-DE, EmulationStation).
- Parser and writer behavior is covered by automated tests.

## ROM Scan

**Owners:** `RomScannerService`, `MainViewModel` (`ScanRomsAsync`), `ScanRomsViewModel`

- Scan a folder for ROM files and compare against the current gamelist.
- Entries missing from the gamelist are presented in a review window with a
  checkbox per candidate.
- The review window offers **Select All**, **Clear**, and **Add Selected**.
- Selected candidates are appended to the current document as new game entries.
- Platform can be auto-detected from the base directory when creating entries.

### Acceptance criteria

- Already-present entries are never duplicated.
- Added entries are immediately visible in the grid and are included on the
  next save.

## Scraping (Skyscraper)

**Owners:** `SkyscraperService`, `MainViewModel` (`ScrapeSelectedAsync`, `ScrapeAllAsync`),
`ScrapeOptionsViewModel`, `ScrapeProgressViewModel`

- Scraping shells out to the external Skyscraper binary.
- The binary path and ScreenScraper credentials come from application settings.
- Scrape options (platform, extra arguments) are chosen in a dialog before launch.
- Scraping runs for the selected entries or the whole library.
- A progress window shows live log output, a progress bar, and parsed progress
  (percent / ratio / current game).
- Long-running jobs can be cancelled; post-scrape the library view refreshes.

### Acceptance criteria

- Without configured credentials or a valid Skyscraper binary, the action is
  blocked with a clear status message.
- Cancellation terminates the child process and returns control to the app.

## Media

**Owners:** `MediaResolverService`, `MainViewModel` (`BrowseMediaAsync`, `FindOrphanMedia`)

- The detail pane previews the selected entry's image when a resolvable asset exists.
- Media path fields (image, video, marquee, wheel, fanart, thumbnail, screenshot)
  can be edited with a browse button.
- Orphan media scan finds media files in known media folders that no entry references.

## Settings

**Owners:** `SettingsService`, `SettingsViewModel`, `AppSettings`

- Settings are stored as JSON and load on startup.
- Persisted settings include: Skyscraper binary path, ScreenScraper credentials,
  log level, recent files, column visibility, last-used directory.
- The settings window supports runtime log-level switching without restart.
- Column visibility is persisted and applied on next launch.

## Recent Files

**Owners:** `MainViewModel`, `RecentFileViewModel`

- Opened files are tracked (bounded) and shown under the Open flyout.
- Recent files are persisted across sessions.
- Selecting a recent file reopens it; entries that no longer exist are skipped.
