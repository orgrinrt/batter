using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SafeWarLogPatch
{
    [HarmonyPatch]
    public static class SafeClanExtensionsPatch
    {
        static MethodBase TargetMethod()
        {
            var type = AccessTools.TypeByName("Diplomacy.Extensions.ClanExtensions");
            return AccessTools.Method(type, "HasMarriedClanLeaderRelation");
        }

        static bool Prefix(object clan, object other, ref bool __result)
        {
            //SafeLog.Info("SafeClanExtensionsPatch Prefix called!");

            try
            {

                if (clan is Clan c1 && other is Clan c2)
                {

                    if (c1.Leader == null || c2.Leader == null)
                    {
                        SafeLog.Info($"[SafeClanExtensionsPatch] Null leader: {c1?.Name} / {c2?.Name}");
                        __result = false;
                        return false;
                    }

                    if (c1.Leader.Spouse == null || c2.Leader.Spouse == null)
                    {
                        SafeLog.Info($"[SafeClanExtensionsPatch] Can not have a spouse relation; Null spouse: either clan1: {c1.Leader?.Name} or clan2: {c2.Leader?.Name}. Returning false on HasMarriedClanLeaderRelation.");
                        __result = false;
                        return false;
                    }
                }
                else
                {
                    SafeLog.Info($"[SafeClanExtensionsPatch] Invalid cast: {clan?.GetType()} / {other?.GetType()}");
                    __result = false;
                    return false;
                }
            }
            catch (Exception e)
            {
                SafeLog.Info($"[SafeClanExtensionsPatch] Exception in Prefix: {e}");
                __result = false;
                return false;
            }

            return true; // Continue to original method
        }

    }
    [HarmonyPatch(typeof(Clan), nameof(Clan.GetRelationWithClan))]
    public static class SafeClanRelationPatch
    {
        static bool Prefix(Clan __instance, Clan other, ref int __result)
        {
            if (__instance == null || other == null)
            {
                SafeLog.Info($"[SafeClanRelationPatch] Skipping GetRelationWithClan: this={__instance?.Name}, other={other?.Name}");
                __result = 0;
                return false; // skip original
            }

            return true; // run original
        }
    }
}
