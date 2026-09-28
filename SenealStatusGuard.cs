using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace CompactStatusSquares
{
    /// <summary>
    /// SeneaL UI draws its own status pills and, while its interface is on,
    /// skips Hud.UpdateStatusEffects even if the pills element is switched off.
    /// This guard stops only that panel. The rest of SeneaL UI keeps running.
    /// </summary>
    static class SenealStatusGuard
    {
        static FieldInfo _statusField;
        static FieldInfo _boxField;

        public static void Patch(Harmony harmony)
        {
            var view = AccessTools.TypeByName("SeneaLUI.Hud.StatusEffectsView");
            var tick = view == null ? null : AccessTools.Method(view, "Tick");
            if (tick == null)
            {
                Plugin.Log.LogInfo("SeneaL UI status panel was not found. Squares will still replace the vanilla list.");
                return;
            }

            harmony.Patch(tick, prefix: new HarmonyMethod(typeof(SenealStatusGuard), nameof(BlockTick)));
            _statusField = AccessTools.Field(AccessTools.TypeByName("SeneaLUI.Hud.HudRoot"), "_status");
            Plugin.Log.LogInfo("SeneaL UI status pills blocked. Compact Status Squares owns that slot.");
        }

        static bool BlockTick()
        {
            return !Plugin.EnabledIcons.Value;
        }

        public static void HideLeftoverPanel()
        {
            if (!Plugin.EnabledIcons.Value || _statusField == null || Hud.instance == null)
                return;

            var status = _statusField.GetValue(null);
            if (status == null)
                return;

            if (status is UnityEngine.Object statusObj && !statusObj)
                return;

            if (_boxField == null)
                _boxField = AccessTools.Field(status.GetType(), "_box");

            var box = _boxField?.GetValue(status) as Component;
            if (!box)
                return;

            GameObject go = box.gameObject;
            if (go != null && go.activeSelf)
                go.SetActive(false);
        }
    }
}
