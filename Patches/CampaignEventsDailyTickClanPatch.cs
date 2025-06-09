using HarmonyLib;
using SafeWarLogPatch.Behaviours;
using SafeWarLogPatch.Utils;
using TaleWorlds.CampaignSystem;

namespace SafeWarLogPatch.Patches;

[HarmonyPatch(typeof(CampaignEvents), "DailyTickClan")]
public static class CampaignEventsDailyTickClanPatch
{
    /// <summary>
    ///     Prefix: if `clan` is null, skip the original DailyTickClan entirely.
    /// </summary>
    private static bool Prefix(Clan clan)
    {
        try
        {
            return clan != null;
        }
        catch (Exception ex)
        {
            BatterLog.Error($"[DailyTickClanPatch] Unexpected error in Prefix: {ex}");
            return false;
        }
    }

    /// <summary>
    ///     Finalizer: runs if the original DailyTickClan threw an exception.
    ///     Attempts to “fix” or disband a broken clan so that Bannerlord does not crash.
    /// </summary>
    private static void Finalizer(Clan clan, Exception __exception)
    {
        if (__exception == null)
            return;

        BatterLog.Warn($"[DailyTickClanPatch] Suppressed exception in DailyTickClan:\n  {__exception}");

        if (clan == null)
            return;

        BatterLog.Warn($"[DailyTickClanPatch] Attempting to repair or disband broken clan: {clan.Name}");

        try
        {
            // If the clan has a Leader, sanitize that hero first.
            if (clan.Leader != null)
            {
                var leader = clan.Leader;
                // Call the static helper we exposed above
                HeroClanSanitizerBehavior.FixClanAndFaction(leader);

                // After sanitizing, see which clan the leader now belongs to:
                var newClan = leader.Clan ?? clan;

                // Reassign each lord to the new clan
                if (clan.Lords != null && clan.Lords.Count > 0)
                    foreach (var lord in clan.Lords)
                        if (lord != null)
                        {
                            lord.Clan = newClan;
                            BatterLog.Info(
                                $"[DailyTickClanPatch] Reassigned lord '{lord.Name}' -> clan '{newClan.Name}'");
                        }

                // Reassign each hero (non‐lord) to the new clan
                if (clan.Heroes != null && clan.Heroes.Count > 0)
                    foreach (var member in clan.Heroes)
                        if (member != null)
                        {
                            member.Clan = newClan;
                            BatterLog.Info(
                                $"[DailyTickClanPatch] Reassigned hero '{member.Name}' -> clan '{newClan.Name}'");
                        }

                // Clear the old clan’s leader so Bannerlord doesn’t look it up again.
                // We cannot assign to the read‐only `clan.Leader` property—use SetLeader(null) if available,
                // or simply remove all Lords so the clan is effectively empty.
                // Unfortunately Bannerlord’s API does not expose SetLeader(null).
                // Instead, remove everyone to break any dictionary lookups:

                // Remove all lords from the old clan:
                if (clan.Lords != null && clan.Lords.Count > 0)
                {
                    // We must iterate a copy to avoid modifying the collection while enumerating
                    var lordsCopy = new Hero[clan.Lords.Count];
                    clan.Lords.CopyTo(lordsCopy, 0);
                    foreach (var lord in lordsCopy)
                        if (lord != null)
                            lord.Clan = newClan; // Already reassigned above
                }

                // Remove all heroes from the old clan
                if (clan.Heroes != null && clan.Heroes.Count > 0)
                {
                    var heroesCopy = new Hero[clan.Heroes.Count];
                    clan.Heroes.CopyTo(heroesCopy, 0);
                    foreach (var member in heroesCopy)
                        if (member != null)
                            member.Clan = newClan; // Already reassigned above
                }

                BatterLog.Info($"[DailyTickClanPatch] Cleared member lists of broken clan '{clan.Name}'");
            }
            else
            {
                // If there is no Leader but there are lords, promote the first lord to be leader
                if (clan.Lords != null && clan.Lords.Count > 0)
                {
                    var firstLord = clan.Lords[0];
                    if (firstLord != null)
                    {
                        clan.SetLeader(firstLord);
                        BatterLog.Info(
                            $"[DailyTickClanPatch] Promoted '{firstLord.Name}' to Leader of clan '{clan.Name}'");
                    }
                }
            }

            // If the clan now has absolutely no members (no Leader, no Lords, no Heroes),
            // disband it by clearing its Kingdom and letting Bannerlord garbage‐collect it later.
            var hasNoMembers =
                clan.Leader == null
                && (clan.Lords == null || clan.Lords.Count == 0)
                && (clan.Heroes == null || clan.Heroes.Count == 0);

            if (hasNoMembers)
            {
                var n = clan.Name.Value ?? "noname";
                BatterLog.Warn($"[DailyTickClanPatch] Clan '{n}' has no remaining members; clearing its Kingdom.");
                if (clan.Kingdom != null) clan.Kingdom = null;
            }
        }
        catch (Exception repairEx)
        {
            BatterLog.Error($"[DailyTickClanPatch] Error while repairing clan '{clan.Name}': {repairEx}");
        }
    }
}