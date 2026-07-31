<p align="center">
  <img src="docs/social-preview.png" alt="Gamelistify" width="80%" />
</p>

<p align="center">
  <a href="https://github.com/marcelofrau/gamelistify/actions/workflows/build.yml">
    <img src="https://img.shields.io/github/actions/workflow/status/marcelofrau/gamelistify/build.yml?branch=main&label=CI%20build" alt="CI build" />
  </a>
  <a href="https://github.com/marcelofrau/gamelistify/blob/main/LICENSE">
    <img src="https://img.shields.io/github/license/marcelofrau/gamelistify" alt="License: GPL-3.0" />
  </a>
  <a href="https://dotnet.microsoft.com/download/dotnet/8.0">
    <img src="https://img.shields.io/badge/.NET-8.0-512BD4" alt=".NET 8" />
  </a>
  <a href="https://avaloniaui.net">
    <img src="https://img.shields.io/badge/Avalonia-12-8B5CF6" alt="Avalonia 12" />
  </a>
  <img src="https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-2E9E4F" alt="Platforms: Windows, Linux, macOS" />
  <img src="https://img.shields.io/badge/version-0.1.0-2E9E4F" alt="Version 0.1.0" />
</p>

# Gamelistify

Gamelistify is a cross-platform desktop workbench for **inspecting, editing, and maintaining `gamelist.xml` libraries** used by EmulationStation-compatible frontends such as [RetroBat](https://www.retrobat.org/), [Batocera](https://batocera.org/), [ES-DE](https://es-de.org/), and vanilla EmulationStation.

It is a modern rewrite built with **.NET 8 and Avalonia UI**, designed to feel like a maintenance workstation for retro game libraries rather than a storefront browser.

## Why Gamelistify?

Editing `gamelist.xml` by hand is error-prone and tedious. Gamelistify gives you a desktop tool to do it safely:

- **Safe by default** — every save and restore creates a timestamped backup in a hidden `gamelists_backup` folder next to your gamelist, so you can always roll back.
- **Know your library** — search, sort, filter, and inspect every game or folder entry in a dense grid with a metadata detail pane.
- **Bulk editing** — hide, unhide, favorite, unfavorite, and remove multiple entries at once.
- **Fill your library** — scan ROM folders for missing entries and scrape metadata with Skyscraper, right from the app.

## Features

### Library management

- Open any `gamelist.xml` and browse entries in a searchable, sortable, filterable grid
- Kind filter: All Entries / Games / Folders
- Single-entry editing with a resizable detail pane (name, path, genres, ratings, dates, and more)
- Star rating control with clickable 5-star UI
- Bulk actions: hide, unhide, favorite, unfavorite, remove selected
- Keyboard shortcuts (Ctrl+O, Ctrl+S, Ctrl+F, Delete, double-click to edit)
- Recent files with quick reopen

### Safety and backups

- Timestamped backups created automatically before every save (`gamelist.<timestamp>.xml.bak`)
- Hidden `gamelists_backup` folder stored next to the gamelist
- Discard changes and reload from disk (only enabled when you have unsaved changes)
- Restore from any backup with a picker that shows date/time and file size — a safety backup is taken first
- Confirmation dialogs for every destructive action

### Scraping and scanning

- ROM scan workflow: detect entries missing from your gamelist, review them, and add them
- Skyscraper integration: scrape selected entries or the whole library
- Live scrape progress with output parsing (percent, ratio, current game) and cancellation
- Orphan media scan from the detail pane
- Platform auto-detection from the base directory

### Media

- Image preview for the selected entry
- Media path editor for image, video, marquee, wheel, fanart, thumbnail, and screenshot
- Media resolver that finds previewable assets in known media folders

### And more

- Settings window with runtime log-level switching
- Optimize (minified XML) and Cleanup (remove entries pointing to missing files)
- Quick Tips flyout
- Dark, tool-oriented theme

## Requirements

- **Windows, Linux, or macOS**
- **.NET 8** SDK to build from source (or a prebuilt binary from [Releases](https://github.com/marcelofrau/gamelistify/releases), when published)
- **Skyscraper** (optional, only for scraping metadata)

## Build from source

```powershell
git clone https://github.com/marcelofrau/gamelistify.git
cd gamelistify
dotnet build Gamelistify.sln -c Debug
dotnet test Gamelistify.sln -c Debug
```

To run the app after a build:

```powershell
dotnet run --project Gamelistify\Gamelistify.csproj
```

Packaging and release-build instructions live in [docs/packaging.md](docs/packaging.md).

## Documentation

- [Documentation index](docs/index.md)
- [Requirements](docs/requirements.md)
- [Feature specifications](docs/specs.md)
- [Architecture](docs/architecture.md)
- [Roadmap](docs/roadmap.md)
- [XML compatibility](docs/xml-compatibility.md)
- [Theme](docs/theme.md)
- [Assets guide](docs/assets-guide.md)
- [Packaging](docs/packaging.md)
- [Branching and versioning](docs/branching-and-versioning.md)
- [Master checklist](docs/checklists/master-checklist.md)

## Tech stack

- [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Avalonia 12](https://avaloniaui.net)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
- [Serilog](https://serilog.net)
- [xUnit](https://xunit.net)

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines, and the [issue templates](.github/ISSUE_TEMPLATE/) when reporting bugs or requesting features.

## License and attributions

Gamelistify is released under the **GPL-3.0-only** license. See [LICENSE](LICENSE).

UI icons are rendered from the [Numix icon theme](https://github.com/numixproject/numix-icon-theme) (GPL-3.0). See [docs/attributions.md](docs/attributions.md).
