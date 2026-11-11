using System.Collections.Generic;
using UnityEngine;

namespace CompactStatusSquares
{
    public class StatusSquaresHud : MonoBehaviour
    {
        static readonly Dictionary<Texture, FilterMode> Filtered = new Dictionary<Texture, FilterMode>();
        static readonly Dictionary<string, string> Localized = new Dictionary<string, string>();
        static readonly string[] PreviewNames = { "Megingjord", "Fire", "Shelter", "Resting Comfort", "Rested" };
        static readonly string[] PreviewTimes = { "", "", "", "16", "" };
        static readonly Color[] PreviewTints =
        {
            new Color(0.75f, 0.62f, 0.28f),
            new Color(0.85f, 0.35f, 0.2f),
            new Color(0.45f, 0.7f, 0.4f),
            new Color(0.85f, 0.75f, 0.45f),
            new Color(0.55f, 0.7f, 0.85f)
        };

        readonly List<StatusEffect> _effects = new List<StatusEffect>(16);
        readonly List<DrawItem> _draw = new List<DrawItem>(16);
        Texture2D _pixel;
        GUIStyle _label;
        GUIStyle _tipStyle;
        bool _hidVanilla;
        bool _vanillaWasActive;
        bool _loggedDraw;
        string _skip;
        string _lastSkip;
        string _hover;
        float _size;
        int _columns;
        float _stepX;
        float _stepY;
        bool _right;
        bool _top;
        Color _backColor;

        struct DrawItem
        {
            public string Name;
            public string Time;
            public Sprite Icon;
            public Color Tint;
        }

        void Awake()
        {
            _pixel = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            _pixel.SetPixel(0, 0, Color.white);
            _pixel.Apply();
            _pixel.hideFlags = HideFlags.HideAndDontSave;
            if (Plugin.Instance != null)
                Plugin.Instance.Config.SettingChanged += OnConfigChanged;
        }

        void OnDestroy()
        {
            if (Plugin.Instance != null)
                Plugin.Instance.Config.SettingChanged -= OnConfigChanged;
        }

        void OnConfigChanged(object sender, System.EventArgs e)
        {
            Localized.Clear();
            _loggedDraw = false;
            _lastSkip = null;
            SenealStatusGuard.ForgetBox();
        }

        void LateUpdate()
        {
            ApplyVanillaRoot();
            if (SenealStatusGuard.Installed)
                SenealStatusGuard.HideLeftoverPanel();

            _draw.Clear();
            _skip = WhyHidden();
            if (_skip != null)
            {
                LogSkip(_skip);
                return;
            }

            RefreshLayoutCache();
            bool flashOn = ((int)(Time.unscaledTime * 5f) & 1) == 1;
            _effects.Clear();
            Player.m_localPlayer.GetSEMan().GetHUDStatusEffects(_effects);
            for (int i = 0; i < _effects.Count; i++)
            {
                var effect = _effects[i];
                if (effect == null || effect.m_icon == null)
                    continue;
                _draw.Add(new DrawItem
                {
                    Name = LocalName(effect.m_name),
                    Time = Plugin.ShowTimer.Value ? effect.GetIconText() : null,
                    Icon = effect.m_icon,
                    Tint = effect.m_flashIcon && flashOn ? new Color(1f, 0.45f, 0.45f, 1f) : Color.white
                });
                ApplyFilter(effect.m_icon);
            }

            if (Plugin.FillPreview.Value)
            {
                int count = Mathf.Max(PreviewNames.Length, _columns);
                int remainder = count % _columns;
                if (remainder != 0)
                    count += _columns - remainder;
                for (int i = 0; i < count; i++)
                {
                    int sample = i % PreviewNames.Length;
                    _draw.Add(new DrawItem
                    {
                        Name = PreviewNames[sample],
                        Time = PreviewTimes[sample],
                        Icon = null,
                        Tint = PreviewTints[sample]
                    });
                }
            }

            if (_draw.Count == 0)
                LogSkip(_effects.Count == 0
                    ? "no HUD status effects on the player"
                    : _effects.Count + " HUD effect(s) had no icons");
            else if (!_loggedDraw)
            {
                _loggedDraw = true;
                _lastSkip = null;
                Plugin.Log.LogInfo("Drawing " + _draw.Count + " status square(s) with OnGUI.");
            }
        }

        string WhyHidden()
        {
            if (!Plugin.EnabledIcons.Value)
                return "Enabled is false";
            if (Player.m_localPlayer == null)
                return "no local player yet";
            if (Hud.instance == null)
                return "Hud is not ready";
            if (!Hud.instance.IsVisible())
                return "vanilla HUD is hidden";
            if (Player.m_localPlayer.IsDead())
                return "player is dead";
            // 0.2.2: these squares are drawn with OnGUI, and Unity composites IMGUI AFTER every
            // canvas pass - no canvas sorting order, nested canvas, or raised root can ever paint
            // over them. That is why the custom loading screen lost to the squares during portal
            // trips. The only fix is not drawing them while a trip hides the HUD.
            if (Player.m_localPlayer.IsTeleporting())
                return "player is teleporting";
            return null;
        }

        void LogSkip(string reason)
        {
            if (_lastSkip == reason)
                return;
            _lastSkip = reason;
            Plugin.Log.LogInfo("Status squares not drawn: " + reason + ".");
        }

        void OnGUI()
        {
            if (_draw.Count == 0)
                return;

            EnsureStyles();
            _hover = null;
            Vector2 mouse = Event.current.mousePosition;
            var nameSide = Plugin.ShowName.Value;

            for (int i = 0; i < _draw.Count; i++)
            {
                int col = i % _columns;
                int row = i / _columns;
                float x = _right
                    ? Screen.width - _size + Plugin.OffsetX.Value - col * _stepX
                    : Plugin.OffsetX.Value + col * _stepX;
                float y = _top
                    ? -Plugin.OffsetY.Value + row * _stepY
                    : Screen.height - _size + Plugin.OffsetY.Value - row * _stepY;
                var box = new Rect(x, y, _size, _size);
                var item = _draw[i];

                GUI.color = _backColor;
                GUI.DrawTexture(box, _pixel);
                GUI.color = item.Tint;
                if (item.Icon != null)
                    DrawSprite(Inset(box, 3f), item.Icon);
                else
                    GUI.DrawTexture(Inset(box, 3f), _pixel);
                GUI.color = Color.white;

                if (Plugin.ShowTimer.Value && !string.IsNullOrEmpty(item.Time))
                    GUI.Label(new Rect(box.x, box.yMax - 16f, box.width, 16f), item.Time, _label);

                if (nameSide != StatusNameSide.Off && !string.IsNullOrEmpty(item.Name))
                    GUI.Label(NameRect(box, nameSide), item.Name, _label);

                if (box.Contains(mouse))
                    _hover = string.IsNullOrEmpty(item.Time) ? item.Name : item.Name + "   " + item.Time;
            }

            if (!string.IsNullOrEmpty(_hover))
            {
                var size = _tipStyle.CalcSize(new GUIContent(_hover));
                var tip = new Rect(mouse.x - size.x - 12f, mouse.y + 16f, size.x + 16f, size.y + 8f);
                GUI.color = new Color(0.05f, 0.04f, 0.035f, 0.92f);
                GUI.DrawTexture(tip, _pixel);
                GUI.color = Color.white;
                GUI.Label(new Rect(tip.x + 8f, tip.y, tip.width - 8f, tip.height), _hover, _tipStyle);
            }
        }

        void EnsureStyles()
        {
            if (_label != null)
                return;
            _label = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.LowerCenter,
                fontSize = Mathf.Clamp(Mathf.RoundToInt(12f * Plugin.TextScale.Value), 8, 36),
                fontStyle = FontStyle.Bold,
                clipping = TextClipping.Overflow
            };
            _label.normal.textColor = Color.white;
            _tipStyle = new GUIStyle(_label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = Mathf.Clamp(Mathf.RoundToInt(14f * Plugin.TextScale.Value), 10, 40)
            };
            _tipStyle.normal.textColor = new Color(0.95f, 0.9f, 0.78f, 1f);
        }

        void RefreshLayoutCache()
        {
            _size = Mathf.Clamp(Plugin.IconSize.Value, 16f, 96f);
            _columns = Mathf.Clamp(Plugin.Columns.Value, 1, 16);
            float gap = Mathf.Max(0f, Plugin.Spacing.Value);
            _stepX = _size + gap;
            var side = Plugin.ShowName.Value;
            float namePitch = 0f;
            if (side == StatusNameSide.Top || side == StatusNameSide.Bottom)
                namePitch = Mathf.Max(10f, 14f * Plugin.TextScale.Value) + Mathf.Max(0f, Plugin.NameGap.Value);
            _stepY = _stepX + namePitch;
            var corner = Plugin.Corner.Value;
            _right = corner == ScreenCorner.TopRight || corner == ScreenCorner.BottomRight;
            _top = corner == ScreenCorner.TopRight || corner == ScreenCorner.TopLeft;
            _backColor = Plugin.Background.Value;
        }

        static Rect NameRect(Rect box, StatusNameSide side)
        {
            float gap = Mathf.Max(0f, Plugin.NameGap.Value);
            switch (side)
            {
                case StatusNameSide.Left:
                    return new Rect(box.x - 260f - gap, box.y, 260f, box.height);
                case StatusNameSide.Right:
                    return new Rect(box.xMax + gap, box.y, 260f, box.height);
                case StatusNameSide.Top:
                    return new Rect(box.x - 80f, box.y - 18f - gap, box.width + 160f, 18f);
                default:
                    return new Rect(box.x - 80f, box.yMax + gap, box.width + 160f, 18f);
            }
        }

        static Rect Inset(Rect box, float pad)
        {
            return new Rect(box.x + pad, box.y + pad, box.width - pad * 2f, box.height - pad * 2f);
        }

        static void DrawSprite(Rect box, Sprite sprite)
        {
            var tex = sprite.texture;
            if (tex == null)
                return;
            var r = sprite.textureRect;
            var uv = new Rect(r.x / tex.width, r.y / tex.height, r.width / tex.width, r.height / tex.height);
            GUI.DrawTextureWithTexCoords(box, tex, uv);
        }

        void ApplyVanillaRoot()
        {
            var hud = Hud.instance;
            if (hud == null || hud.m_statusEffectListRoot == null)
                return;

            var go = hud.m_statusEffectListRoot.gameObject;
            if (Plugin.EnabledIcons.Value)
            {
                if (!_hidVanilla)
                {
                    _vanillaWasActive = go.activeSelf;
                    _hidVanilla = true;
                }
                if (go.activeSelf)
                    go.SetActive(false);
                return;
            }

            if (_hidVanilla)
            {
                go.SetActive(_vanillaWasActive);
                _hidVanilla = false;
            }
        }

        static void ApplyFilter(Sprite sprite)
        {
            if (!Plugin.CrispPixels.Value || sprite == null || sprite.texture == null)
                return;
            var tex = sprite.texture;
            if (Filtered.ContainsKey(tex))
                return;
            Filtered[tex] = tex.filterMode;
            tex.filterMode = FilterMode.Point;
        }

        public static void RestoreFilteredTextures()
        {
            foreach (var pair in Filtered)
            {
                if (pair.Key != null)
                    pair.Key.filterMode = pair.Value;
            }
            Filtered.Clear();
        }

        static string LocalName(string key)
        {
            if (string.IsNullOrEmpty(key))
                return key;
            if (Localized.TryGetValue(key, out string cached))
                return cached;
            string value = Localization.instance != null ? Localization.instance.Localize(key) : key;
            Localized[key] = value;
            return value;
        }
    }
}
