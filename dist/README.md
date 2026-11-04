# 🟦 Compact Status Squares

**Small, configurable buff and debuff icons for Valheim.**

Compact Status Squares displays Valheim's own status-effect artwork as small square icons, giving you a cleaner HUD without the large vanilla status list.

Designed to work alongside [SeneaL UI](https://thunderstore.io/c/valheim/p/seneaL/SeneaL_UI/), this mod takes control of the status-effect HUD slot while leaving the rest of SeneaL UI untouched.

## ✨ Features

- **Compact square icons** using vanilla status-effect sprites.
- **Configurable positioning** with screen corners and X/Y offsets.
- **Flexible layouts** with adjustable icon size, columns, and spacing.
- **Status labels** displayed to the left, right, above, or below icons.
- **Scalable text** for status names, timers, comfort numbers, and hover tips.
- **Vanilla font option** using Valheim's Norse font.
- **Live configuration updates** so settings can be adjusted without restarting.
- **Hover for details** to see a status effect's name.
- **Preview mode** to check layouts without active status effects.

## 🔧 Compatibility with SeneaL UI

SeneaL UI can skip vanilla status-effect updates whenever its interface is enabled, even when its own status pills are switched off.

Compact Status Squares handles the status icons itself and prevents SeneaL UI's status panel from occupying the same HUD area.

While this mod is enabled:

- Compact Status Squares displays the status-effect icons.
- SeneaL UI's status pills remain hidden, even if `[Elements] StatusEffects` is enabled.
- The large vanilla status list is not drawn.
- Other SeneaL UI features remain unchanged.

**Do not run this alongside another mod that replaces the status-effect HUD.**

## 🧪 Testing

A test build has already been copied into these Gale profiles that have SeneaL UI installed:

- `beta_cjaycraft_CLIENT`
- `sennealteest`

### Quick Test

1. Launch one of the profiles and load a character.
2. Pick up a status effect, such as Rested, a potion buff, Wet, or Cold.
3. Confirm the effects appear as small square icons.
4. Check that SeneaL UI's status pills remain hidden.
5. Adjust the layout settings to suit your HUD.

### Configuration

Edit:

`BepInEx/config/cjayride.CompactStatusSquares.cfg`

The settings include:

| Setting | Purpose |
|---|---|
| `OffsetX` / `OffsetY` | Move the icon cluster |
| `IconSize` | Adjust icon dimensions |
| `TextScale` | Scale names, timers, comfort numbers, and hover tips |
| `ShowName` | Display names Left, Right, Top, or Bottom of an icon |
| `UseVanillaFont` | Use Valheim's Norse font for labels |
| `Columns` | Set the number of icons per row |
| `Spacing` | Adjust the distance between icons |
| `Corner` | Choose the screen corner used for positioning |
| `Timer` | Configure timer display |
| `CrispPixels` | Configure crisp icon rendering |
| `NameGap` | Adjust the gap between an icon and its name |
| `FillPreview` | Display sample squares for layout testing |

Most settings apply immediately.

To return status-list control to the other HUD systems, set:

`[General] Enabled = false`

## 🛠️ Build

Build from the project directory:

```bash
dotnet build -c Release
```

After a successful build, `CompactStatusSquares.dll` is copied to the game's BepInEx plugins folder.

For testing through Gale, copy the DLL into the profile's plugin directory:

`BepInEx/plugins/cjayride-CompactStatusSquares/`

## 📄 Credits

Compact Status Squares uses Valheim's existing status-effect artwork and is designed to complement [SeneaL UI](https://thunderstore.io/c/valheim/p/seneaL/SeneaL_UI/).

