# Compact Status Squares

Small square buff and debuff icons for Valheim, drawn with the game's own effect art. The cluster sits in a screen corner you choose, and you can nudge it in game.

Made to live next to [SeneaL UI](https://thunderstore.io/c/valheim/p/seneaL/SeneaL_UI/). SeneaL UI skips the game's status list whenever its interface is on, even if its own status pills are switched off. This mod draws the icons itself and keeps that status slot, so SeneaL's pills stay hidden. The rest of SeneaL UI is left alone.

**Client only.** The dedicated server does not need this mod.

Do not run this together with another mod that also replaces the status-effect HUD.

## Install

Requires [BepInExPack for Valheim](https://thunderstore.io/c/valheim/p/denikson/BepInExPack_Valheim/).

**Thunderstore, Gale, or r2modman:** install the package. The DLL lands in `BepInEx/plugins/cjayride-CompactStatusSquares/`.

**Nexus or a manual zip:** copy the folder `plugins/cjayride-CompactStatusSquares` into your Valheim `BepInEx/plugins` folder. You should end up with:

```
Valheim/BepInEx/plugins/cjayride-CompactStatusSquares/CompactStatusSquares.dll
```

Do not copy `manifest.json`, `README.md`, or `icon.png` into the game.

## Use

Load a character and pick up a buff (Rested, a potion, Wet, Cold). The icons are small squares. Hover one for its name.

Move the cluster with `OffsetX` and `OffsetY` in:

`BepInEx/config/cjayride.CompactStatusSquares.cfg`

Size, text scale, name beside the icon, vanilla font, columns, spacing, corner, timer, and crisp pixels in that file apply immediately.

Turn `[General] Enabled` off to hand the status list back to the game (or to SeneaL, if that mod's status pills are turned on).
