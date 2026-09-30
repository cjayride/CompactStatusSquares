# Compact Status Squares

Small square buff and debuff icons for Valheim, using the game's own effect art. Built to sit next to [SeneaL UI](https://thunderstore.io/c/valheim/p/seneaL/SeneaL_UI/) without letting that mod draw the status list.

SeneaL UI skips the game's status-effect update whenever its interface is on, even if its own status pills are switched off. This mod draws the icons itself and blocks SeneaL's status panel, so that slot stays ours. The rest of SeneaL UI is left alone.

## Try it

The test build is already copied into the Gale profiles that have SeneaL UI:

- `beta_cjaycraft_CLIENT`
- `sennealteest`

Launch that profile, load a character, and pick up a buff (Rested, a potion, Wet, Cold). The icons should be small squares. SeneaL's pills should not appear, including if `[Elements] StatusEffects` is turned back on.

Move the cluster with `OffsetX` and `OffsetY` in:

`BepInEx/config/cjayride.CompactStatusSquares.cfg`

Other settings in that file (size, text scale, name beside the icon, vanilla font, columns, spacing, corner, timer, crisp pixels) apply immediately. Hover an icon for its name.

Turn `[General] Enabled` off to hand the status list back.

Do not run this together with another mod that also replaces the status-effect HUD.

## Build

From this folder:

```
dotnet build -c Release
```

The game install's BepInEx plugins folder receives `CompactStatusSquares.dll` after a successful build. Copy that dll into a Gale profile's `BepInEx/plugins/cjayride-CompactStatusSquares/` folder when testing from Gale.
