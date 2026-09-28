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
        public const string Version = "0.1.2";

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
                "Draw the remaining time on the square.");
            CrispPixels = Config.Bind("Layout", "CrispPixels", true,
                "Point-filter the vanilla icon art so the squares stay sharp instead of soft and blurry.");
            Background = Config.Bind("Layout", "Background", new Color(10f / 255f, 9f / 255f, 8f / 255f, 209f / 255f),
                "Color of the square behind each icon.");

            _harmony.PatchAll(typeof(VanillaStatusListGuard).Assembly);
            SenealStatusGuard.Patch(_harmony);
            gameObject.AddComponent<StatusSquaresHud>();
            Log.LogInfo(Name + " " + Version + " loaded. Hold Left Alt and use the arrow keys to move the icons.");
        }

        public void OnDestroy()
        {
            _harmony.UnpatchAll(Guid);
            StatusSquaresHud.RestoreFilteredTextures();
        }
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
