using System.Reflection;
using HarmonyLib;
using SafeWarLogPatch.Utils;
using TaleWorlds.CampaignSystem;

namespace SafeWarLogPatch.Patches;

[HarmonyPatch]
public static class DiplomacyModHasMarriedClanLeaderRelationPatch
{
    private static MethodBase TargetMethod()
    {
        var type = AccessTools.TypeByName("Diplomacy.Extensions.ClanExtensions");
        return AccessTools.Method(type, "HasMarriedClanLeaderRelation");
    }

    private static bool Prefix(object clan, object other, ref bool __result)
    {
        //SafeLog.Info("SafeClanExtensionsPatch Prefix called!");

        try
        {
            if (clan is Clan c1 && other is Clan c2)
            {
                if (c1.Leader == null || c2.Leader == null)
                {
                    BatterLog.Info($"[SafeClanExtensionsPatch] Null leader: {c1?.Name} / {c2?.Name}");
                    __result = false;
                    return false;
                }

                if (c1.Leader.Spouse == null || c2.Leader.Spouse == null)
                {
                    BatterLog.Info(
                        $"[SafeClanExtensionsPatch] Can not have a spouse relation; Null spouse: either clan1: {c1.Leader?.Name} or clan2: {c2.Leader?.Name}. Returning false on HasMarriedClanLeaderRelation.");
                    __result = false;
                    return false;
                }
            }
            else
            {
                BatterLog.Info($"[SafeClanExtensionsPatch] Invalid cast: {clan?.GetType()} / {other?.GetType()}");
                __result = false;
                return false;
            }
        }
        catch (Exception e)
        {
            BatterLog.Info($"[SafeClanExtensionsPatch] Exception in Prefix: {e}");
            __result = false;
            return false;
        }

        return true; // Continue to original method
    }
}