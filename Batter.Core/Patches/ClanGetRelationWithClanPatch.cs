using Batter.Core.Utils;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace Batter.Core.Patches;

[HarmonyPatch(typeof(Clan), nameof(Clan.GetRelationWithClan))]
public static class ClanGetRelationWithClanPatch {
    private static Boolean Prefix(Clan __instance, Clan other, ref Int32 __result) {
        if (__instance == null || other == null) {
            BatterLog.Info(
                $"[SafeClanRelationPatch] Skipping GetRelationWithClan: this={__instance?.Name}, other={other?.Name}");
            __result = 0;
            return false; // skip original
        }

        return true; // run original
    }
}