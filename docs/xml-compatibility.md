---
layout: default
title: XML Compatibility
---

# XML Compatibility

## Target Ecosystems

- EmulationStation
- RetroBat
- Batocera
- ES-DE

## Save Philosophy

Gamelistify is not aiming for byte-perfect round-trip output.

It is aiming for:

- semantic preservation
- stable and readable output
- safe backups before replacement
- compatibility with common frontend expectations

## v1.0 Guarantees

- existing entries remain represented correctly after load/save
- edited fields remain serialized consistently
- boolean fields are written predictably
- relative paths remain usable for frontend consumption
- backup file is created before overwrite

## v1.0 Non-Goals

- exact comment preservation
- exact whitespace preservation
- exact original child node ordering when normalization is required

## Open Questions

- whether to preserve unknown child elements automatically
- whether to preserve folder-only quirks per frontend profile
- whether to maintain profile-specific media path preferences
