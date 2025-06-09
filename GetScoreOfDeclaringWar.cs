using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SafeWarLogPatch
{
    [HarmonyPatch(
        typeof(DefaultDiplomacyModel),
                  nameof(DefaultDiplomacyModel.GetScoreOfDeclaringWar)//,
                  // new Type[]{
                  //   typeof(IFaction),
                  //   typeof(IFaction),
                  //   typeof(IFaction),
                  //   typeof(TextObject)
                  // }
    )]
    public static class SafeScoreOfDeclaringWarPatch
    {
        // static MethodInfo TargetMethod()
        // {
        //     return typeof(DefaultDiplomacyModel).GetMethod(
        //         "GetScoreOfDeclaringWar",
        //         BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
        //         null,
        //         new Type[] {
        //             typeof(IFaction),
        //                                                    typeof(IFaction),
        //                                                    typeof(IFaction),
        //                                                    typeof(TextObject).MakeByRefType() // Works here!
        //         },
        //         null
        //     );
        // }

        static bool Prefix(
            IFaction factionDeclaresWar,
            IFaction factionDeclaredWar,
            IFaction evaluatingClan,
            ref TextObject warReason,
            ref float __result)
        {
            try
            {
                // 1) If any of the three factions is null, skip original & return 0
                if (factionDeclaresWar == null
                    || factionDeclaredWar == null
                    || evaluatingClan == null)
                {
                    SafeLog.Info(
                        "[SafeScoreOfDeclaringWarPatch] Skipping GetScoreOfDeclaringWar: null parameter(s) " +
                        $"declares={(factionDeclaresWar?.Name?.ToString() ?? "nil")}, " +
                        $"declared={(factionDeclaredWar?.Name?.ToString() ?? "nil")}, " +
                        $"evaluating={(evaluatingClan?.Name?.ToString() ?? "nil")}"
                    );
                    __result = 0;
                    warReason = TextObject.Empty;
                    return false;
                }

                // 2) If the same faction is declaring war on itself, skip original
                if (ReferenceEquals(factionDeclaresWar, factionDeclaredWar))
                {
                    SafeLog.Info(
                        "[SafeScoreOfDeclaringWarPatch] Skipping GetScoreOfDeclaringWar: declaringFaction == declaredFaction"
                    );
                    __result = 0;
                    warReason = TextObject.Empty;
                    return false;
                }

                // Otherwise, let the original method run:
                return true;
            }
            catch (Exception ex)
            {
                SafeLog.Warn($"[SafeScoreOfDeclaringWarPatch] Exception in GetScoreOfDeclaringWar Prefix: {ex}");
                __result = 0;
                warReason = TextObject.Empty;
                return false;
            }
        }

        // Finalizer: if original threw, we catch here, log, and force safe defaults.
        static void Finalizer(
            ref TextObject warReason,
            ref float __result,
            Exception __exception)
        {
            try {
                if (__exception != null)
                {
                    SafeLog.Warn($"[SafeScoreOfDeclaringWarPatch] Suppressed exception in GetScoreOfDeclaringWar:\n  {__exception}");
                    __result = 0;
                    warReason = TextObject.Empty;
                }
            } catch(Exception ex) {
                SafeLog.Error($"[SafeScoreOfDeclaringWarPatch] Suppression failed in GetScoreOfDeclaringWar:\n  {ex}");
            }

        }
    }


    [HarmonyPatch(
        typeof(DefaultDiplomacyModel),
                  "GetScoreOfWarInternal"//,
                  // new Type[]{
                  //   typeof(IFaction),
                  //   typeof(IFaction),
                  //   typeof(IFaction),
                  //   typeof(bool),
                  //   typeof(TextObject)
                  // }
    )]
    public static class SafeGetScoreOfWarInternalPatch
    {
        static bool Prefix(
            IFaction factionDeclaresWar,
            IFaction factionDeclaredWar,
            IFaction evaluatingClan,
            bool evaluatingPeace,
            ref TextObject reason,
            ref float __result)
        {
            try
            {
                // 1) If any faction is null, return 0 immediately
                if (factionDeclaresWar == null
                    || factionDeclaredWar == null
                    || evaluatingClan == null)
                {
                    SafeLog.Info(
                        "[SafeGetScoreOfWarInternalPatch] Skipping GetScoreOfWarInternal: null parameter(s) " +
                        $"declares={(factionDeclaresWar?.Name?.ToString() ?? "nil")}, " +
                        $"declared={(factionDeclaredWar?.Name?.ToString() ?? "nil")}, " +
                        $"evaluating={(evaluatingClan?.Name?.ToString() ?? "nil")}"
                    );
                    __result = 0;
                    reason = TextObject.Empty;
                    return false;
                }

                // 2) If declaringFaction == declaredFaction, return 0
                if (ReferenceEquals(factionDeclaresWar, factionDeclaredWar))
                {
                    SafeLog.Info(
                        "[SafeGetScoreOfWarInternalPatch] Skipping GetScoreOfWarInternal: declaringFaction == declaredFaction"
                    );
                    __result = 0;
                    reason = TextObject.Empty;
                    return false;
                }

                // Otherwise, let the original method run
                return true;
            }
            catch (Exception ex)
            {
                SafeLog.Warn($"[SafeGetScoreOfWarInternalPatch] Exception in GetScoreOfWarInternal Prefix: {ex}");
                __result = 0;
                reason = TextObject.Empty;
                return false;
            }
        }

        // Finalizer: catch any exception thrown by the original GetScoreOfWarInternal
        static void Finalizer(
            ref TextObject reason,
            ref float __result,
            Exception __exception)
        {
            try {
                if (__exception != null)
                {
                    SafeLog.Warn($"[SafeGetScoreOfWarInternalPatch] Suppressed exception in GetScoreOfWarInternal:\n  {__exception}");
                    __result = 0;
                    reason = TextObject.Empty;
                }

            } catch(Exception ex) {
                SafeLog.Error($"[SafeScoreOfDeclaringWarPatch] Suppression failed in GetScoreOfDeclaringWar:\n  {ex}");
            }
        }
    }

    [HarmonyPatch(typeof(CampaignEvents), "DailyTickClan")]
    public static class DailyTickClanPatch
    {
        /// <summary>
        /// Prefix: if `clan` is null, skip the original DailyTickClan entirely.
        /// </summary>
        static bool Prefix(Clan clan)
        {
            try
            {
                return clan != null;
            }
            catch (Exception ex)
            {
                SafeLog.Error($"[DailyTickClanPatch] Unexpected error in Prefix: {ex}");
                return false;
            }
        }

        /// <summary>
        /// Finalizer: runs if the original DailyTickClan threw an exception.
        /// Attempts to “fix” or disband a broken clan so that Bannerlord does not crash.
        /// </summary>
        static void Finalizer(Clan clan, Exception __exception)
        {
            if (__exception == null)
                return;

            SafeLog.Warn($"[DailyTickClanPatch] Suppressed exception in DailyTickClan:\n  {__exception}");

            if (clan == null)
                return;

            SafeLog.Warn($"[DailyTickClanPatch] Attempting to repair or disband broken clan: {clan.Name}");

            try
            {
                // If the clan has a Leader, sanitize that hero first.
                if (clan.Leader != null)
                {
                    var leader = clan.Leader;
                    // Call the static helper we exposed above
                    HeroClanSanitizerBehavior.FixClanAndFaction(leader);

                    // After sanitizing, see which clan the leader now belongs to:
                    Clan newClan = leader.Clan ?? clan;

                    // Reassign each lord to the new clan
                    if (clan.Lords != null && clan.Lords.Count > 0)
                    {
                        foreach (var lord in clan.Lords)
                        {
                            if (lord != null)
                            {
                                lord.Clan = newClan;
                                SafeLog.Info($"[DailyTickClanPatch] Reassigned lord '{lord.Name}' -> clan '{newClan.Name}'");
                            }
                        }
                    }

                    // Reassign each hero (non‐lord) to the new clan
                    if (clan.Heroes != null && clan.Heroes.Count > 0)
                    {
                        foreach (var member in clan.Heroes)
                        {
                            if (member != null)
                            {
                                member.Clan = newClan;
                                SafeLog.Info($"[DailyTickClanPatch] Reassigned hero '{member.Name}' -> clan '{newClan.Name}'");
                            }
                        }
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
                        {
                            if (lord != null)
                            {
                                lord.Clan = newClan; // Already reassigned above
                            }
                        }
                    }
                    // Remove all heroes from the old clan
                    if (clan.Heroes != null && clan.Heroes.Count > 0)
                    {
                        var heroesCopy = new Hero[clan.Heroes.Count];
                        clan.Heroes.CopyTo(heroesCopy, 0);
                        foreach (var member in heroesCopy)
                        {
                            if (member != null)
                            {
                                member.Clan = newClan; // Already reassigned above
                            }
                        }
                    }

                    SafeLog.Info($"[DailyTickClanPatch] Cleared member lists of broken clan '{clan.Name}'");
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
                            SafeLog.Info($"[DailyTickClanPatch] Promoted '{firstLord.Name}' to Leader of clan '{clan.Name}'");
                        }
                    }
                }

                // If the clan now has absolutely no members (no Leader, no Lords, no Heroes),
                // disband it by clearing its Kingdom and letting Bannerlord garbage‐collect it later.
                bool hasNoMembers =
                clan.Leader == null
                && (clan.Lords == null || clan.Lords.Count == 0)
                && (clan.Heroes == null || clan.Heroes.Count == 0);

                if (hasNoMembers)
                {
                    var n = clan.Name.Value ?? "noname";
                    SafeLog.Warn($"[DailyTickClanPatch] Clan '{n}' has no remaining members; clearing its Kingdom.");
                    if (clan.Kingdom != null)
                    {
                        clan.Kingdom = null;
                    }
                }
            }
            catch (Exception repairEx)
            {
                SafeLog.Error($"[DailyTickClanPatch] Error while repairing clan '{clan.Name}': {repairEx}");
            }
        }
    }
}
