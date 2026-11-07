# 0.2.1

- Default `IconSize` is now 50 (fresh configs only; existing `IconSize` in your cfg file is unchanged).

# 0.2.0

- **Release for Valheim 1.0.21 (Unity 6):** status squares draw with OnGUI in screen pixels. Unity UI on the game HUD was invisible in practice; SeneaL UI is optional, not required.
- Lighter HUD path: no per-label Outline meshes, no extra overlay canvas, SeneaL scans only when SeneaL is installed.
- Startup logs explain when squares are hidden (no player, HUD toggled off, no buffs, and so on).

# 0.1.15

- Draw status squares with OnGUI in screen pixels. Valheim 1.0.21's HUD canvas was swallowing the Unity UI layer, so icons could log as drawn and still be invisible.
- Log the first reason squares are skipped (disabled, no player, HUD hidden, no effects, no icons).

# 0.1.14

- Squares now stretch across Valheim's HUD canvas, so they sit in the chosen screen corner instead of a 100x100 box in the middle of the HUD.
- Dropped Unity UI Outline and stopped rebuilding layout five times a second, which was hammering FPS.
- Skip SeneaL leftover scans when that mod is not installed. Show squares whenever the vanilla HUD is visible.

# 0.1.13

- Parent the squares under Valheim's own HUD root instead of a separate overlay canvas, so they render on Valheim 1.0.21 / Unity 6 without SeneaL UI.
- Track slot placement explicitly so recreated HUD layers still position every square.
- Log once when the game reports HUD effects but none have icons to draw.

# 0.1.12

- New status squares stay hidden until they are placed in the list, so they no longer flash at a default screen position when an effect is applied.

# 0.1.11

- Re-checked against Valheim 1.0.21 (`Hud.UpdateStatusEffects`, `SEMan.GetHUDStatusEffects`, `StatusEffect.GetIconText`).
- Cut per-frame HUD work: cache layout/fonts, skip unchanged squares, localize names once, and stop reflecting into SeneaL UI every frame.

# 0.1.10

- Verified against Valheim 1.0.21 (Release build).
- BepInEx dependency set to `denikson-BepInExPack_Valheim-5.4.2360`.

# 0.1.9

- Defaults: NameGap 1.731, Spacing 5.616, ShowName Bottom, TextScale 0.83.

# 0.1.8

- A name above or below an icon sits on the icon edge, and the next icon starts under that line.
- Defaults: ShowName Bottom, NameGap 0, TextScale 0.83, Spacing 11.

# 0.1.7

- ShowName can also place the status name Top or Bottom of the icon.

# 0.1.6

- NameGap sets the pixels between the icon and the status name, so the name can clear the comfort number.
- FillPreview draws sample squares, including Resting Comfort with 16, so the layout can be checked with no status effects. Off by default.

# 0.1.4

- Removed the Alt coordinate overlay and the Alt+arrow nudge. Position stays in the config.

# 0.1.3

- TextScale sizes timers, comfort numbers, names, and the hover tip.
- ShowName can draw the localized status name Left or Right of the icon (Off by default).
- UseVanillaFont switches those labels to Valheim's Norse font.

# 0.1.2

- Stop NullReferenceException spam on logout: SeneaL's status panel is already destroyed while LateUpdate still ran.

# 0.1.1

- Default cluster sits in the top-right with OffsetX -18, OffsetY -18, and IconSize 36.577.

# 0.1.0

- Small square status icons using the vanilla effect sprites.
- Screen corner, offset, size, columns, and spacing are configurable, including in-game nudge.
- While this mod is on, it takes the status-effect slot: SeneaL UI's pills stay hidden and the large vanilla list is not drawn.
