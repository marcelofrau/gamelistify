<p align="center">
  <img src="docs/social-preview.png" alt="Gamelistify" width="100%" />
</p>

<p align="center">
  <strong>A friendly desktop tool for your retro game library.</strong><br />
  Open, fix, clean, and fill the <code>gamelist.xml</code> used by RetroBat, Batocera, ES-DE, and other EmulationStation-style frontends — without ever touching the file by hand.
</p>

<p align="center">
  <a href="https://github.com/marcelofrau/gamelistify/releases">
    <img src="https://img.shields.io/badge/download-Windows%20|%20Linux%20|%20macOS-2E9E4F" alt="Downloads" />
  </a>
  <img src="https://img.shields.io/badge/version-0.9.9%20beta-2E9E4F" alt="Version 0.9.9" />
  <a href="https://github.com/marcelofrau/gamelistify/actions/workflows/build.yml">
    <img src="https://img.shields.io/github/actions/workflow/status/marcelofrau/gamelistify/build.yml?branch=main&label=CI%20build" alt="CI build" />
  </a>
  <a href="https://github.com/marcelofrau/gamelistify/blob/main/LICENSE">
    <img src="https://img.shields.io/github/license/marcelofrau/gamelistify" alt="License: GPL-3.0" />
  </a>
</p>

> **Beta notice.** Gamelistify is in its first public beta. Every save is automatically
> backed up, but keep your own copies of important libraries too. Found a bug or want
> a feature? [Open an issue](https://github.com/marcelofrau/gamelistify/issues).

---

## What is Gamelistify?

Retro game frontends like [RetroBat](https://www.retrobat.org/),
[Batocera](https://batocera.org/), and [ES-DE](https://es-de.org/) keep their game
lists in a file called `gamelist.xml`. Normally you either edit that file by hand
(tedious and risky) or never look at it at all.

**Gamelistify gives you a friendly window into that file.** You can browse your whole
library, fix messy entries, hide duplicates and bad versions, mark favorites, and even
download metadata for games — all from one safe, visual interface. Think of it as a
workbench for keeping your retro library tidy.

## What you can do

- **See everything at a glance** — search, sort, and filter your games and folders in a clean table.
- **Fix entries quickly** — edit names, paths, genres, ratings, and dates; star your favorites.
- **Clean up your library** — detect duplicate entries and bad regional versions, then hide them in one click.
- **Bulk edit** — hide, unhide, favorite, unfavorite, or remove many games at once.
- **Fill in missing games** — scan your ROM folders and add the ones your gamelist is missing.
- **Add metadata automatically** — scrape game information with Skyscraper from right inside the app.
- **Never lose work** — every save creates a backup automatically, and you can restore any of them.

## Getting started

1. **Download the app** for your system from the [Releases page](https://github.com/marcelofrau/gamelistify/releases).
2. **Open it.** No install wizard needed — unzip and run.
3. **Open your gamelist.** In the toolbar click **Open** and choose your `gamelist.xml` (it lives inside your frontend's game folder).
4. **Explore.** Click any game to edit it in the panel on the right. Use the search box to find entries.
5. **Save.** Click **Save** — a backup is created automatically before every save.

That's it. Everything else (restore, cleaning, scanning, scraping) lives in the
**Tools** menu and the right-click context menus, so you can discover it as you go.

### A quick tour of the toolbar

| Button | What it does |
| --- | --- |
| **Open** | Open a gamelist file — or a recent one from the list. |
| **Save** | Save your changes (auto-backup first), or Save As to a new file. |
| **Add** | Add a new entry to the list. |
| **Remove / Hide / Unhide** | Delete or hide/unhide the selected games. |
| **Favorite / Unfavorite** | Mark or unmark favorites (star). |
| **Scrape** | Pull metadata for the selection or the whole library via Skyscraper. |
| **Scan** | Find ROM files that are not yet in your gamelist and add them. |
| **Tools** | Set name from filename, batch favorite by names, detect & hide duplicates and bad versions, review hidden entries & favorites. |

## Safety first

- Every save stores a **timestamped backup** (`gamelist.<timestamp>.xml.bak`) in a hidden
  `gamelists_backup` folder next to your gamelist.
- **Restore Backup** (in **Tools**) shows every backup with its date and size. Restoring
  takes a safety backup of your current file first — you can always undo.
- Destructive actions always ask for confirmation before they happen.

## Requirements

- **Windows, Linux, or macOS**
- To **use** it: just the downloaded app — no extra software needed.
- To **build it yourself**: the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).
- To **scrape metadata** (optional): [Skyscraper](https://github.com/muldjord/skyscraper) configured on your machine.

## Build from source

```powershell
git clone https://github.com/marcelofrau/gamelistify.git
cd gamelistify
dotnet build Gamelistify.sln -c Debug
dotnet test Gamelistify.sln -c Debug
```

Run the app after a build:

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

Built with [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0) and
[Avalonia UI](https://avaloniaui.net), using [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/),
[Serilog](https://serilog.net), and [xUnit](https://xunit.net).

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines, and the
[issue templates](.github/ISSUE_TEMPLATE/) when reporting bugs or requesting features.

## License and attributions

Gamelistify is released under the **GPL-3.0-only** license. See [LICENSE](LICENSE).

UI icons are rendered from the [Numix icon theme](https://github.com/numixproject/numix-icon-theme)
(GPL-3.0). See [docs/attributions.md](docs/attributions.md).
