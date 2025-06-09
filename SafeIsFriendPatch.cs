using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SafeWarLogPatch
{
    [HarmonyPatch]
    public static class SafeIsFriendPatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(Hero), "IsFriend");
        }

        static bool Prefix(Hero __instance, Hero otherHero, ref bool __result)
        {
            if (__instance == null || otherHero == null)
            {
                SafeLog.Info($"[SafeIsFriendPatch] Null hero in IsFriend: {__instance?.Name} vs {otherHero?.Name}");
                __result = false;
                return false; // skip original to prevent crash
            }

            return true; // allow original method to run
        }
    }
}
