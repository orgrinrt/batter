using HarmonyLib;
using SafeWarLogPatch.Utils;
using TaleWorlds.CampaignSystem;

namespace SafeWarLogPatch.Patches;

[HarmonyPatch(typeof(Clan), nameof(Clan.GetRelationWithClan))]
public static class ClanGetRelationWithClanPatch
{
    private static bool Prefix(Clan __instance, Clan other, ref int __result)
    {
        if (__instance == null || other == null)
        {
            BatterLog.Info(
                $"[SafeClanRelationPatch] Skipping GetRelationWithClan: this={__instance?.Name}, other={other?.Name}");
            __result = 0;
            return false; // skip original
        }

        return true; // run original
    }
}