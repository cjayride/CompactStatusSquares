using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace CompactStatusSquares
{
    [BepInPlugin(Guid, Name, Version)]
    [BepInProcess("valheim.exe")]
    [BepInDependency("seneaL.valheim.ui", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Guid = "cjayride.CompactStatusSquares";
        public const string Name = "Compact Status Squares";
        public const string Version = "0.1.6";

        public static Plugin Instance { get; private set; }
        public static ManualLogSource Log;
        public static ConfigEntry<bool> EnabledIcons;
        public static ConfigEntry<ScreenCorner> Corner;
        public static ConfigEntry<float> OffsetX;
        public static ConfigEntry<float> OffsetY;
        public static ConfigEntry<float> IconSize;
        public static ConfigEntry<int> Columns;
        public static ConfigEntry<float> Spacing;
        public static ConfigEntry<bool> ShowTimer;
        public static ConfigEntry<float> TextScale;
        public static ConfigEntry<StatusNameSide> ShowName;
        public static ConfigEntry<float> NameGap;
        public static ConfigEntry<bool> FillPreview;
        public static ConfigEntry<bool> UseVanillaFont;
        public static ConfigEntry<bool> CrispPixels;
        public static ConfigEntry<Color> Background;

        readonly Harmony _harmony = new Harmony(Guid);

        public void Awake()
        {
            Instance = this;
            Log = Logger;

            EnabledIcons = Config.Bind("General", "Enabled", true,
                "Draw the small status squares and keep SeneaL UI from drawing its status pills.");
            Corner = Config.Bind("Layout", "Corner", ScreenCorner.TopRight,
                "Screen corner the first icon sits in. Extra icons flow away from that corner.");
            OffsetX = Config.Bind("Layout", "OffsetX", -18f,
                "Horizontal distance from the chosen corner, in pixels. Negative moves left.");
            OffsetY = Config.Bind("Layout", "OffsetY", -18f,
                "Vertical distance from the chosen corner, in pixels. Negative moves down.");
            IconSize = Config.Bind("Layout", "IconSize", 36.577f,
                new ConfigDescription("Width and height of each square.", new AcceptableValueRange<float>(16f, 96f)));
            Columns = Config.Bind("Layout", "Columns", 1,
                new ConfigDescription("Icons per row before wrapping. 1 is a vertical stack.", new AcceptableValueRange<int>(1, 16)));
            Spacing = Config.Bind("Layout", "Spacing", 4f,
                new ConfigDescription("Gap between squares.", new AcceptableValueRange<float>(0f, 32f)));
            ShowTimer = Config.Bind("Layout", "ShowTimer", true,
                "Draw the remaining time or comfort number on the square.");
            TextScale = Config.Bind("Layout", "TextScale", 1f,
                new ConfigDescription("Size of timers, comfort numbers, and optional status names. 1 is the original size.", new AcceptableValueRange<float>(0.4f, 3f)));
            ShowName = Config.Bind("Layout", "ShowName", StatusNameSide.Off,
                "Draw the localized status name beside the icon. Off keeps the square-only layout.");
            NameGap = Config.Bind("Layout", "NameGap", 6f,
                new ConfigDescription("Pixels between the icon edge and the status name. Raise this if a name covers the comfort number on the square.", new AcceptableValueRange<float>(0f, 120f)));
            FillPreview = Config.Bind("Layout", "FillPreview", false,
                "Fill the bar with sample squares, including a comfort number, so the layout can be checked with no status effects. Turn this off when you are done testing.");
            UseVanillaFont = Config.Bind("Layout", "UseVanillaFont", false,
                "Use Valheim's Norse font for timers and names. Off uses the compact UI font.");
            CrispPixels = Config.Bind("Layout", "CrispPixels", true,
                "Point-filter the vanilla icon art so the squares stay sharp instead of soft and blurry.");
            Background = Config.Bind("Layout", "Background", new Color(10f / 255f, 9f / 255f, 8f / 255f, 209f / 255f),
                "Color of the square behind each icon.");

            _harmony.PatchAll(typeof(VanillaStatusListGuard).Assembly);
            SenealStatusGuard.Patch(_harmony);
            gameObject.AddComponent<StatusSquaresHud>();
            Log.LogInfo(Name + " " + Version + " loaded.");
        }

        public void OnDestroy()
        {
            _harmony.UnpatchAll(Guid);
            StatusSquaresHud.RestoreFilteredTextures();
        }
    }

    public enum StatusNameSide
    {
        Off,
        Left,
        Right
    }

    public enum ScreenCorner
    {
        TopRight,
        TopLeft,
        BottomRight,
        BottomLeft
    }

    [HarmonyPatch(typeof(Hud), "UpdateStatusEffects")]
    static class VanillaStatusListGuard
    {
        static bool Prefix()
        {
            return !Plugin.EnabledIcons.Value;
        }
    }
}
