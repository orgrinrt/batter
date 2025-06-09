using System.Reflection;
using Batter.Core.Utils;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace Batter.Core.Patches;

[HarmonyPatch]
public static class HeroIsFriendPatch {
    private static MethodBase TargetMethod() {
        return AccessTools.Method(typeof(Hero), "IsFriend");
    }

    private static Boolean Prefix(Hero __instance, Hero otherHero, ref Boolean __result) {
        if (__instance == null || otherHero == null) {
            BatterLog.Info($"[SafeIsFriendPatch] Null hero in IsFriend: {__instance?.Name} vs {otherHero?.Name}");
            __result = false;
            return false; // skip original to prevent crash
        }

        return true; // allow original method to run
    }
}