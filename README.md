# Gamelistify

Gamelistify is a cross-platform desktop application for inspecting, editing, and maintaining `gamelist.xml` libraries used by EmulationStation-compatible frontends such as RetroBat, Batocera, ES-DE, and vanilla EmulationStation.

This repository contains the modern rewrite built with .NET 8 and Avalonia UI.

## Status

Initial product foundation implemented.

Current state:

- core XML domain services in place (load, save, backup)
- desktop shell with searchable grid, resizable detail pane
- full editing workflow (single-entry, bulk actions, dirty-state, recent files)
- kind filter (All Entries / Games / Folders) with favorite/hidden row tinting
- keyboard shortcuts (Ctrl+O, Ctrl+S, Ctrl+F, Delete, double-click edit)
- kid game flag and media path editor (image, video, marquee, wheel, fanart, thumbnail, screenshot)
- orphan media scan from the detail pane
- ROM scan workflow (detect missing entries, review, adopt)
- scrape workflow with progress, cancel, live output, and progress parsing
- platform auto-detection from the base directory
- star rating control with clickable 5-star UI
- confirmation dialogs for destructive actions
- settings window with runtime log level switching
- Quick Tips green flyout
- toolbar icons from personal icons8 set
- release/versioning/documentation foundation in place

## Stack

- .NET 8
- Avalonia 12
- CommunityToolkit.Mvvm
- Serilog
- xUnit

## Documentation

Project documentation lives in [`docs/`](docs/).

Start here:

- [`docs/index.md`](docs/index.md)
- [`docs/checklists/master-checklist.md`](docs/checklists/master-checklist.md)
- [`docs/architecture.md`](docs/architecture.md)
- [`docs/requirements.md`](docs/requirements.md)

## Build

```powershell
dotnet build Gamelistify.sln -c Debug
dotnet test Gamelistify.sln -c Debug
```

Release packaging details live in [`docs/packaging.md`](docs/packaging.md).

## Contributing

See [`CONTRIBUTING.md`](CONTRIBUTING.md).

## License

GPL-3.0-only.
