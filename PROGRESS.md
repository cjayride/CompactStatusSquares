# Compact Status Squares — 0.2.0

## Goal
- Shippable release: visible squares without SeneaL on Valheim 1.0.21, acceptable FPS.

## Status
- **Completed.** User confirmed icons visible with 0.1.15; version bumped to 0.2.0 for release.

## Root cause
- Valheim 1.0.21 / Unity 6: custom Unity UI (overlay or under Hud root) did not show on screen.
- OnGUI in screen pixels works. Earlier Unity UI path also had Outline + layout churn hurting FPS.

## 0.2.0 deliverables
- Version 0.2.0 across Plugin, csproj, manifests.
- README: SeneaL optional, 1.0.21 called out.
- Thunderstore zip in `/dist`.

## Commands
- `dotnet build -c Release -p:SkipGameCopy=true` — success.

## Output
- DLL: `D:\AI-Agent-Projects\cjaycraft\Cjayride.CompactStatusSquares\bin\Release\net480\CompactStatusSquares.dll`
- Zip: `D:\AI-Agent-Projects\cjaycraft\Cjayride.CompactStatusSquares\dist\Cjayride.CompactStatusSquares-0.2.0.zip`
