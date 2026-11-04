# Compact Status Squares — 0.1.11

## Goal
- Confirm the mod still matches latest Valheim (1.0.21).
- Reduce per-frame HUD work for better FPS / less lag.

## Status
- Completed (source + Release build). Not committed.

## Compatibility
- Local game: `D:\SteamLibrary\steamapps\common\Valheim` (assembly dated 2026-10-28, patch 1.0.21).
- Public latest is also 1.0.21.
- Still present: `Hud.UpdateStatusEffects(List<StatusEffect>)`, `Hud.m_statusEffectListRoot`, `Hud.IsVisible()`, `SEMan.GetHUDStatusEffects`, `StatusEffect.GetIconText` / `m_icon` / `m_flashIcon`.
- Harmony prefix with no extra args still intercepts the list-taking vanilla method.

## Changes
- Version 0.1.10 → 0.1.11.
- Cache layout, fonts, outline sizes; only rebuild square geometry when config or slot set changes.
- Cache localized names; apply Point filter once per texture.
- Skip canvas work while HUD is hidden; skip `GetIconText` when timers are off (hover still reads it).
- SeneaL leftover panel: cache the box GameObject and rescan at most every 2s.
- Patch only `VanillaStatusListGuard` instead of the whole assembly.

## Commands
- `dotnet build -c Release -p:SkipGameCopy=true` — succeeded, 0 warnings, 0 errors.

## Output
- DLL: `D:\AI-Agent-Projects\cjaycraft\Cjayride.CompactStatusSquares\bin\Release\net480\CompactStatusSquares.dll` (0.1.11)
- Thunderstore zip: `D:\AI-Agent-Projects\cjaycraft\Cjayride.CompactStatusSquares\dist\Cjayride.CompactStatusSquares-0.1.11.zip`

## Remaining
- In-game FPS comparison was not run (no live session).
