using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CompactStatusSquares
{
    public class StatusSquaresHud : MonoBehaviour
    {
        static readonly Dictionary<Texture, FilterMode> Filtered = new Dictionary<Texture, FilterMode>();
        static Font _font;

        readonly List<StatusEffect> _effects = new List<StatusEffect>(16);
        readonly List<Slot> _slots = new List<Slot>(16);

        Canvas _canvas;
        RectTransform _root;
        RectTransform _tipRoot;
        Text _tip;
        Text _hint;
        Sprite _pixel;
        bool _loggedDraw;
        bool _hidVanilla;
        bool _vanillaWasActive;
        float _nextNudge;
        Slot _hover;

        void Awake()
        {
            _pixel = Pixel();
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (_font == null)
                _font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            var canvasGo = new GameObject("CompactStatusSquares");
            canvasGo.transform.SetParent(transform, false);
            _canvas = canvasGo.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 450;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            _root = Stretch(canvasGo.transform, "icons");
            _tipRoot = MakeTip();
            _hint = MakeHint();
        }

        void LateUpdate()
        {
            Nudge();
            SenealStatusGuard.HideLeftoverPanel();
            ApplyVanillaRoot();

            var player = Player.m_localPlayer;
            bool show = Plugin.EnabledIcons.Value
                && player != null
                && Hud.instance != null
                && Hud.instance.IsVisible()
                && !MapOpen()
                && !InventoryOpen()
                && !MenuOpen()
                && !ConsoleOpen()
                && !player.IsDead();

            if (!show)
            {
                _canvas.enabled = false;
                SetHover(null);
                return;
            }

            _canvas.enabled = true;
            _effects.Clear();
            player.GetSEMan().GetHUDStatusEffects(_effects);
            Layout();
        }

        void Layout()
        {
            int shown = 0;
            float size = Mathf.Clamp(Plugin.IconSize.Value, 16f, 96f);
            int columns = Mathf.Clamp(Plugin.Columns.Value, 1, 16);
            float gap = Mathf.Max(0f, Plugin.Spacing.Value);
            float step = size + gap;
            var corner = Plugin.Corner.Value;
            bool right = corner == ScreenCorner.TopRight || corner == ScreenCorner.BottomRight;
            bool top = corner == ScreenCorner.TopRight || corner == ScreenCorner.TopLeft;
            bool flashOn = Mathf.Sin(Time.time * 10f) > 0f;
            var back = Plugin.Background.Value;

            for (int i = 0; i < _effects.Count; i++)
            {
                var effect = _effects[i];
                if (effect == null || effect.m_icon == null)
                    continue;

                var slot = SlotAt(shown);
                slot.Root.gameObject.SetActive(true);
                slot.Effect = effect;
                int col = shown % columns;
                int row = shown / columns;
                float x = Plugin.OffsetX.Value + (right ? -col * step : col * step);
                float y = Plugin.OffsetY.Value + (top ? -row * step : row * step);
                Place(slot.Root, corner, x, y, size);

                slot.Back.color = back;
                if (slot.Icon.sprite != effect.m_icon)
                {
                    slot.Icon.sprite = effect.m_icon;
                    ApplyFilter(effect.m_icon);
                }
                slot.Icon.color = effect.m_flashIcon && flashOn
                    ? new Color(1f, 0.45f, 0.45f, 1f)
                    : Color.white;

                string time = effect.GetIconText();
                bool timer = Plugin.ShowTimer.Value && !string.IsNullOrEmpty(time);
                slot.Timer.gameObject.SetActive(timer);
                if (timer)
                    slot.Timer.text = time;

                shown++;
            }

            for (int i = shown; i < _slots.Count; i++)
            {
                if (_slots[i].Root.gameObject.activeSelf)
                    _slots[i].Root.gameObject.SetActive(false);
                if (_hover == _slots[i])
                    SetHover(null);
            }

            if (shown > 0 && !_loggedDraw)
            {
                _loggedDraw = true;
                Plugin.Log.LogInfo("Drawing " + shown + " status icon" + (shown == 1 ? "" : "s") + ".");
            }

            UpdateTip();
        }

        void Nudge()
        {
            bool alt = ZInput.GetKey(KeyCode.LeftAlt) || ZInput.GetKey(KeyCode.RightAlt);
            _hint.gameObject.SetActive(alt && Plugin.EnabledIcons.Value);
            if (!alt)
                return;

            _hint.text = "Status squares  x " + Plugin.OffsetX.Value.ToString("0") + "   y " + Plugin.OffsetY.Value.ToString("0");
            if (Player.m_localPlayer == null || Typing())
                return;

            float x = 0f;
            float y = 0f;
            if (ZInput.GetKey(KeyCode.LeftArrow)) x -= 1f;
            if (ZInput.GetKey(KeyCode.RightArrow)) x += 1f;
            if (ZInput.GetKey(KeyCode.DownArrow)) y -= 1f;
            if (ZInput.GetKey(KeyCode.UpArrow)) y += 1f;
            if (x == 0f && y == 0f)
                return;
            if (Time.unscaledTime < _nextNudge)
                return;

            _nextNudge = Time.unscaledTime + 0.045f;
            float step = ZInput.GetKey(KeyCode.LeftShift) || ZInput.GetKey(KeyCode.RightShift) ? 1f : 8f;
            Plugin.OffsetX.Value += x * step;
            Plugin.OffsetY.Value += y * step;
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

        void UpdateTip()
        {
            if (_hover == null || !_hover.Root.gameObject.activeSelf || _hover.Effect == null)
            {
                _tipRoot.gameObject.SetActive(false);
                return;
            }

            var effect = _hover.Effect;
            string name = effect.m_name;
            if (Localization.instance != null)
                name = Localization.instance.Localize(name);
            string time = effect.GetIconText();
            _tip.text = string.IsNullOrEmpty(time) ? name : name + "   " + time;
            _tipRoot.gameObject.SetActive(true);

            var corner = Plugin.Corner.Value;
            bool right = corner == ScreenCorner.TopRight || corner == ScreenCorner.BottomRight;
            var slot = _hover.Root;
            _tipRoot.anchorMin = slot.anchorMin;
            _tipRoot.anchorMax = slot.anchorMax;
            _tipRoot.pivot = new Vector2(right ? 1f : 0f, slot.pivot.y);
            float size = Mathf.Clamp(Plugin.IconSize.Value, 16f, 96f);
            _tipRoot.anchoredPosition = slot.anchoredPosition + new Vector2(right ? -(size + 8f) : size + 8f, 0f);
            _tipRoot.sizeDelta = new Vector2(Mathf.Clamp(_tip.text.Length * 9f + 20f, 80f, 460f), 28f);
        }

        public void SetHover(Slot slot)
        {
            _hover = slot;
        }

        public void ClearHover(Slot slot)
        {
            if (_hover == slot)
                _hover = null;
        }

        Slot SlotAt(int index)
        {
            while (_slots.Count <= index)
                _slots.Add(CreateSlot());
            return _slots[index];
        }

        Slot CreateSlot()
        {
            var go = new GameObject("square");
            go.transform.SetParent(_root, false);
            var rect = go.AddComponent<RectTransform>();
            var back = go.AddComponent<Image>();
            back.sprite = _pixel;
            back.type = Image.Type.Simple;
            back.color = Plugin.Background.Value;

            var iconGo = new GameObject("icon");
            iconGo.transform.SetParent(go.transform, false);
            var iconRect = iconGo.AddComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = new Vector2(3f, 3f);
            iconRect.offsetMax = new Vector2(-3f, -3f);
            var icon = iconGo.AddComponent<Image>();
            icon.preserveAspect = true;
            icon.raycastTarget = false;

            var timerGo = new GameObject("time");
            timerGo.transform.SetParent(go.transform, false);
            var timerRect = timerGo.AddComponent<RectTransform>();
            timerRect.anchorMin = new Vector2(0f, 0f);
            timerRect.anchorMax = new Vector2(1f, 0f);
            timerRect.pivot = new Vector2(0.5f, 0f);
            timerRect.sizeDelta = new Vector2(-2f, 14f);
            timerRect.anchoredPosition = new Vector2(0f, 1f);
            var timer = timerGo.AddComponent<Text>();
            timer.font = _font;
            timer.fontSize = 12;
            timer.alignment = TextAnchor.LowerCenter;
            timer.color = Color.white;
            timer.horizontalOverflow = HorizontalWrapMode.Overflow;
            timer.verticalOverflow = VerticalWrapMode.Overflow;
            timer.raycastTarget = false;
            timer.resizeTextForBestFit = true;
            timer.resizeTextMinSize = 8;
            timer.resizeTextMaxSize = 13;
            var outline = timerGo.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.95f);
            outline.effectDistance = new Vector2(1f, -1f);

            var slot = go.AddComponent<Slot>();
            slot.Hud = this;
            slot.Root = rect;
            slot.Back = back;
            slot.Icon = icon;
            slot.Timer = timer;
            return slot;
        }

        RectTransform MakeTip()
        {
            var go = new GameObject("tip");
            go.transform.SetParent(_root, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(160f, 28f);
            var back = go.AddComponent<Image>();
            back.sprite = _pixel;
            back.color = new Color(0.05f, 0.04f, 0.035f, 0.92f);
            back.raycastTarget = false;

            var textGo = new GameObject("label");
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(8f, 2f);
            textRect.offsetMax = new Vector2(-8f, -2f);
            _tip = textGo.AddComponent<Text>();
            _tip.font = _font;
            _tip.fontSize = 16;
            _tip.alignment = TextAnchor.MiddleLeft;
            _tip.color = new Color(0.95f, 0.9f, 0.78f, 1f);
            _tip.raycastTarget = false;
            go.SetActive(false);
            return rect;
        }

        Text MakeHint()
        {
            var go = new GameObject("nudge");
            go.transform.SetParent(_root, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(520f, 28f);
            rect.anchoredPosition = new Vector2(0f, -8f);
            var text = go.AddComponent<Text>();
            text.font = _font;
            text.fontSize = 16;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(1f, 0.92f, 0.7f, 1f);
            text.raycastTarget = false;
            var outline = go.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.9f);
            outline.effectDistance = new Vector2(1f, -1f);
            go.SetActive(false);
            return text;
        }

        void ApplyFilter(Sprite sprite)
        {
            if (!Plugin.CrispPixels.Value || sprite == null || sprite.texture == null)
                return;
            var tex = sprite.texture;
            if (!Filtered.ContainsKey(tex))
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

        static void Place(RectTransform rect, ScreenCorner corner, float x, float y, float size)
        {
            Vector2 anchor;
            switch (corner)
            {
                case ScreenCorner.TopLeft: anchor = new Vector2(0f, 1f); break;
                case ScreenCorner.BottomLeft: anchor = new Vector2(0f, 0f); break;
                case ScreenCorner.BottomRight: anchor = new Vector2(1f, 0f); break;
                default: anchor = new Vector2(1f, 1f); break;
            }
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.sizeDelta = new Vector2(size, size);
            rect.anchoredPosition = new Vector2(x, y);
        }

        static RectTransform Stretch(Transform parent, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return rect;
        }

        static Sprite Pixel()
        {
            var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            tex.hideFlags = HideFlags.HideAndDontSave;
            var sprite = Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            sprite.hideFlags = HideFlags.HideAndDontSave;
            return sprite;
        }

        static bool MapOpen()
        {
            return Minimap.IsOpen();
        }

        static bool InventoryOpen()
        {
            return InventoryGui.IsVisible();
        }

        static bool Typing()
        {
            if (TextInput.IsVisible() || Console.IsVisible() || Menu.IsVisible())
                return true;
            return Chat.instance != null && Chat.instance.HasFocus();
        }

        static bool MenuOpen()
        {
            return Menu.IsVisible();
        }

        static bool ConsoleOpen()
        {
            return Console.IsVisible();
        }

        public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
        {
            public StatusSquaresHud Hud;
            public RectTransform Root;
            public Image Back;
            public Image Icon;
            public Text Timer;
            public StatusEffect Effect;

            public void OnPointerEnter(PointerEventData eventData)
            {
                Hud.SetHover(this);
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                if (Hud != null)
                    Hud.ClearHover(this);
            }
        }
    }
}
