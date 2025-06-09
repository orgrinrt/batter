using HarmonyLib;
using SafeWarLogPatch.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Localization;

namespace SafeWarLogPatch.Patches;

[HarmonyPatch(
    typeof(DefaultDiplomacyModel),
    nameof(DefaultDiplomacyModel.GetScoreOfDeclaringWar) //,
    // new Type[]{
    //   typeof(IFaction),
    //   typeof(IFaction),
    //   typeof(IFaction),
    //   typeof(TextObject)
    // }
)]
public static class DefaultDiplomacyModelGetScoreOfDeclaringWarPatch
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

    private static bool Prefix(
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
                BatterLog.Info(
                    "[SafeScoreOfDeclaringWarPatch] Skipping GetScoreOfDeclaringWar: null parameter(s) " +
                    $"declares={factionDeclaresWar?.Name?.ToString() ?? "nil"}, " +
                    $"declared={factionDeclaredWar?.Name?.ToString() ?? "nil"}, " +
                    $"evaluating={evaluatingClan?.Name?.ToString() ?? "nil"}"
                );
                __result = 0;
                warReason = TextObject.Empty;
                return false;
            }

            // 2) If the same faction is declaring war on itself, skip original
            if (ReferenceEquals(factionDeclaresWar, factionDeclaredWar))
            {
                BatterLog.Info(
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
            BatterLog.Warn($"[SafeScoreOfDeclaringWarPatch] Exception in GetScoreOfDeclaringWar Prefix: {ex}");
            __result = 0;
            warReason = TextObject.Empty;
            return false;
        }
    }

    // Finalizer: if original threw, we catch here, log, and force safe defaults.
    private static void Finalizer(
        ref TextObject warReason,
        ref float __result,
        Exception __exception)
    {
        try
        {
            if (__exception != null)
            {
                BatterLog.Warn(
                    $"[SafeScoreOfDeclaringWarPatch] Suppressed exception in GetScoreOfDeclaringWar:\n  {__exception}");
                __result = 0;
                warReason = TextObject.Empty;
            }
        }
        catch (Exception ex)
        {
            BatterLog.Error($"[SafeScoreOfDeclaringWarPatch] Suppression failed in GetScoreOfDeclaringWar:\n  {ex}");
        }
    }
}

[HarmonyPatch(
    typeof(DefaultDiplomacyModel),
    "GetScoreOfWarInternal" //,
    // new Type[]{
    //   typeof(IFaction),
    //   typeof(IFaction),
    //   typeof(IFaction),
    //   typeof(bool),
    //   typeof(TextObject)
    // }
)]
public static class DefaultDiplomacyModelGetScoreOfDeclaringWarInternalPatch
{
    private static bool Prefix(
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
                BatterLog.Info(
                    "[SafeGetScoreOfWarInternalPatch] Skipping GetScoreOfWarInternal: null parameter(s) " +
                    $"declares={factionDeclaresWar?.Name?.ToString() ?? "nil"}, " +
                    $"declared={factionDeclaredWar?.Name?.ToString() ?? "nil"}, " +
                    $"evaluating={evaluatingClan?.Name?.ToString() ?? "nil"}"
                );
                __result = 0;
                reason = TextObject.Empty;
                return false;
            }

            // 2) If declaringFaction == declaredFaction, return 0
            if (ReferenceEquals(factionDeclaresWar, factionDeclaredWar))
            {
                BatterLog.Info(
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
            BatterLog.Warn($"[SafeGetScoreOfWarInternalPatch] Exception in GetScoreOfWarInternal Prefix: {ex}");
            __result = 0;
            reason = TextObject.Empty;
            return false;
        }
    }

    // Finalizer: catch any exception thrown by the original GetScoreOfWarInternal
    private static void Finalizer(
        ref TextObject reason,
        ref float __result,
        Exception __exception)
    {
        try
        {
            if (__exception != null)
            {
                BatterLog.Warn(
                    $"[SafeGetScoreOfWarInternalPatch] Suppressed exception in GetScoreOfWarInternal:\n  {__exception}");
                __result = 0;
                reason = TextObject.Empty;
            }
        }
        catch (Exception ex)
        {
            BatterLog.Error($"[SafeScoreOfDeclaringWarPatch] Suppression failed in GetScoreOfDeclaringWar:\n  {ex}");
        }
    }
}