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
