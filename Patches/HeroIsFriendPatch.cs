using System.Reflection;
using HarmonyLib;
using SafeWarLogPatch.Utils;
using TaleWorlds.CampaignSystem;

namespace SafeWarLogPatch.Patches;

[HarmonyPatch]
public static class HeroIsFriendPatch
{
    private static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(Hero), "IsFriend");
    }

    private static bool Prefix(Hero __instance, Hero otherHero, ref bool __result)
    {
        if (__instance == null || otherHero == null)
        {
            BatterLog.Info($"[SafeIsFriendPatch] Null hero in IsFriend: {__instance?.Name} vs {otherHero?.Name}");
            __result = false;
            return false; // skip original to prevent crash
        }

        return true; // allow original method to run
    }
}