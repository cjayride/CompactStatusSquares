# Compact Status Squares — 0.1.12

## Goal
- Stop new status squares from flashing at a default screen position before they appear in the compact list.

## Status
- Completed (source + Release build). Not committed.

## Cause
- New slots were created active at Unity's default RectTransform.
- `Place()` only ran when `_layoutDirty` was true, so a newly applied effect could sit at the default overlay position for up to ~0.2s (next flash tick) before jumping into the list.

## Changes
- Version 0.1.11 → 0.1.12.
- Create slots inactive; position them, then enable.

## Commands
- `dotnet build -c Release -p:SkipGameCopy=true` — succeeded, 0 warnings, 0 errors.

## Output
- DLL: `D:\AI-Agent-Projects\cjaycraft\Cjayride.CompactStatusSquares\bin\Release\net480\CompactStatusSquares.dll` (0.1.12)
- Thunderstore zip: `D:\AI-Agent-Projects\cjaycraft\Cjayride.CompactStatusSquares\dist\Cjayride.CompactStatusSquares-0.1.12.zip`

## Remaining
- In-game confirmation: apply a new status and confirm no flash at cursor/center.
