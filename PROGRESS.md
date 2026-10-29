# Compact Status Squares — release 0.1.10

## Goal
- Test build against Valheim 1.0.21.
- Bump Thunderstore manifest to BepInExPack 5.4.2360.
- Changelog, Thunderstore zip in `/dist`, commit, push, tag `v0.1.10`.

## Status
- Completed.

## Changes
- Version 0.1.9 → 0.1.10 (`CompactStatusSquares.csproj`, `Plugin.cs`, `manifest.json`).
- BepInEx dependency: `denikson-BepInExPack_Valheim-5.4.2360`.
- `CHANGELOG.md`, `dist/README.md`, `package/*` synced.

## Commands
- `dotnet build -c Release` — succeeded (0 warnings, 0 errors) against `D:\SteamLibrary\steamapps\common\Valheim`.

## Output
- DLL: `D:\AI-Agent-Projects\cjaycraft\Cjayride.CompactStatusSquares\bin\Release\net480\CompactStatusSquares.dll` (0.1.10)
- Thunderstore zip: `D:\AI-Agent-Projects\cjaycraft\Cjayride.CompactStatusSquares\dist\cjayride-CompactStatusSquares-0.1.10.zip`
- GitHub release: https://github.com/cjayride/CompactStatusSquares/releases/tag/v0.1.10
