# Gamelistify

Gamelistify is a cross-platform desktop application for inspecting, editing, and maintaining `gamelist.xml` libraries used by EmulationStation-compatible frontends such as RetroBat, Batocera, ES-DE, and vanilla EmulationStation.

This repository contains the modern rewrite built with .NET 8 and Avalonia UI.

## Status

Initial product foundation implemented.

Current state:

- core XML domain services in place
- desktop shell, detail pane, settings, and scan workflow working
- scrape workflow wired with progress and cancel support
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
