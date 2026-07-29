---
layout: default
title: Requirements
---

# Requirements

## Functional

| ID | Requirement | Target |
|----|-------------|--------|
| F01 | User can open an existing `gamelist.xml` file from disk | v1.0 |
| F02 | User can inspect games and folders in a searchable desktop list | v1.0 |
| F03 | User can view metadata details and image preview for current selection | v1.0 |
| F04 | User can edit supported metadata fields for a single entry | v1.0 |
| F05 | User can apply bulk actions to multiple selected entries | v1.0 |
| F06 | User can scan ROM directories and add missing entries to current gamelist | v1.0 |
| F07 | User can configure and launch Skyscraper scraping workflows | v1.0 |
| F08 | User can see live scrape log output and cancel long-running jobs | v1.0 |
| F09 | User can save changes and receive a timestamped backup before overwrite | v1.0 |
| F10 | User can manage recent files and application settings | v1.0 |
| F11 | Application supports EmulationStation, RetroBat, Batocera, and ES-DE conventions | v1.0 |
| F12 | User can toggle visible columns and sort the main list | v1.0 |
| F13 | User can refresh the current view after scrape or scan operations | v1.0 |

## Non-Functional

| ID | Requirement | Priority |
|----|-------------|----------|
| NF01 | Application targets .NET 8 | High |
| NF02 | Application uses Avalonia 12 and CommunityToolkit.Mvvm | High |
| NF03 | Application is cross-platform on Windows, Linux, and macOS | High |
| NF04 | Core XML and domain logic is covered by automated tests from day one | High |
| NF05 | All project documentation is written in English | High |
| NF06 | Documentation is maintained inside `docs/` and structured for future website publishing | High |
| NF07 | XML save behavior preserves semantic content and useful field ordering where possible | High |
| NF08 | XML comments and whitespace do not need perfect preservation | Medium |
| NF09 | Every write operation creates a timestamped backup artifact before replacement | High |
| NF10 | UI has a distinct identity inspired by scraper tools, but not visually derivative | High |
| NF11 | Project ships under GPL-3.0-only | High |

## Explicit Scope Decisions

- Image-first preview is in scope for v1.0.
- Embedded video preview is out of scope for v1.0.
- Perfect round-trip formatting preservation is out of scope for v1.0.
- XML safety and backup behavior are mandatory.
