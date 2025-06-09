using System.Collections;
using System.Reflection;
using HarmonyLib;
using SafeWarLogPatch.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace SafeWarLogPatch.Patches;

[HarmonyPatch(typeof(CampaignEvents), "DailyTickSettlement")]
public static class CampaignEventsDailyTickSettlementPatch
{
    /// <summary>
    ///     Prefix: if settlement is null, skip the original DailyTickSettlement entirely.
    /// </summary>
    private static bool Prefix(Settlement settlement)
    {
        try
        {
            return settlement != null;
        }
        catch (Exception ex)
        {
            BatterLog.Error($"[SafeDailyTickSettlementPatch] Unexpected error in Prefix: {ex}");
            return false;
        }
    }

    /// <summary>
    ///     Finalizer: runs if the original DailyTickSettlement threw an exception.
    ///     Attempts to “repair” broken rebellion state so Bannerlord’s campaign loop does not crash.
    /// </summary>
    private static void Finalizer(Settlement settlement, Exception __exception)
    {
        if (__exception == null)
            return;

        BatterLog.Warn($"[SafeDailyTickSettlementPatch] Suppressed exception in DailyTickSettlement:\n  {__exception}");

        if (settlement == null)
            return;

        BatterLog.Warn(
            $"[SafeDailyTickSettlementPatch] Attempting to repair broken settlement: {settlement.Name?.ToString() ?? "[unknown]"}");

        try
        {
            // 1) If the settlement ever spawned a rebel party, try to find and remove it.
            //    (The RebellionsCampaignBehavior keeps track of rebel parties internally.)
            //    We attempt to locate any rebel party that references this settlement,
            //    then disband it or detach it so it can’t crash later.

            var allRebelBehaviors = Campaign.Current.GetCampaignBehaviors<RebellionsCampaignBehavior>();

            // If there’s at least one, run your cleanup logic on each
            foreach (var rebellionsBehavior in allRebelBehaviors)
            {
                if (rebellionsBehavior != null)
                {
                    // RebellionsCampaignBehavior has a private dictionary tracking rebellion parties.
                    // We can reflectively wipe out any rebel party entry for this settlement.

                    var rbType = typeof(RebellionsCampaignBehavior);
                    var fieldInfo = rbType.GetField("_settlementToRebelParty",
                        BindingFlags.Instance
                        | BindingFlags.NonPublic);

                    if (fieldInfo != null)
                    {
                        var dictionary = fieldInfo.GetValue(rebellionsBehavior)
                            as IDictionary;
                        if (dictionary != null)
                            if (dictionary.Contains(settlement))
                            {
                                var rebelParty = dictionary[settlement] as MobileParty;
                                if (rebelParty != null)
                                {
                                    BatterLog.Info(
                                        $"[SafeDailyTickSettlementPatch] Disbanding rebel party '{rebelParty.Name}' created for settlement '{settlement.Name}'");
                                    try
                                    {
                                        // If we can, remove it safely
                                        rebelParty.RemoveParty();
                                    }
                                    catch (Exception disEx)
                                    {
                                        BatterLog.Warn(
                                            $"[SafeDailyTickSettlementPatch] Unable to RemoveParty on rebel party: {disEx}");
                                    }
                                }

                                // Remove the dictionary entry so the behavior does not try again
                                dictionary.Remove(settlement);
                                BatterLog.Info(
                                    $"[SafeDailyTickSettlementPatch] Removed settlement '{settlement.Name}' from _settlementToRebelParty dictionary.");
                            }
                    }
                }


                // 2) If the settlement has any “rebellion started” flag, reset it to zero.
                //    Most likely, RebellionsCampaignBehavior tracks an integer like “LastRebellionTick”.
                //    We can zero out any such private field. Reflection again:

                var rbType2 = typeof(RebellionsCampaignBehavior);
                var fieldTickInfo = rbType2.GetField("_nextRebellionTickFor",
                    BindingFlags.Instance
                    | BindingFlags.NonPublic);
                if (fieldTickInfo != null)
                {
                    var nextRebellionDict = fieldTickInfo.GetValue(rebellionsBehavior)
                        as IDictionary;
                    if (nextRebellionDict != null && nextRebellionDict.Contains(settlement))
                    {
                        nextRebellionDict[settlement] = CampaignTime.DaysFromNow(9999);
                        // push next rebellion far into the future
                        BatterLog.Info(
                            $"[SafeDailyTickSettlementPatch] Bumped nextRebellionTickFor[{settlement.Name}] to avoid immediate retry.");
                    }
                }

                // 3) If settlement’s “RebelFaction” (private) is not null, clear it.
                var privateRebelFaction = settlement.GetType()
                    .GetField("_rebelFaction",
                        BindingFlags.Instance
                        | BindingFlags.NonPublic);
                if (privateRebelFaction != null)
                {
                    privateRebelFaction.SetValue(settlement, null);
                    BatterLog.Info(
                        $"[SafeDailyTickSettlementPatch] Cleared private _rebelFaction on settlement '{settlement.Name}'.");
                }
            }
        }
        catch (Exception repairEx)
        {
            BatterLog.Error(
                $"[SafeDailyTickSettlementPatch] Error while repairing settlement '{settlement.Name}': {repairEx}");
        }
    }
}