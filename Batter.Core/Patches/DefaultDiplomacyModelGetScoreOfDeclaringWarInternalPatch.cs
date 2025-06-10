using Batter.Core.Utils;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Localization;

namespace Batter.Core.Patches;

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
public static class DefaultDiplomacyModelGetScoreOfDeclaringWarInternalPatch {
    private static Boolean Prefix(
        IFaction factionDeclaresWar,
        IFaction factionDeclaredWar,
        IFaction evaluatingClan,
        Boolean evaluatingPeace,
        ref TextObject reason,
        ref Single __result) {
        try {
            // 1) If any faction is null, return 0 immediately
            if (factionDeclaresWar == null
                || factionDeclaredWar == null
                || evaluatingClan == null) {
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
            if (ReferenceEquals(factionDeclaresWar, factionDeclaredWar)) {
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
        catch (Exception ex) {
            BatterLog.Warn($"[SafeGetScoreOfWarInternalPatch] Exception in GetScoreOfWarInternal Prefix: {ex}");
            __result = 0;
            reason = TextObject.Empty;
            return false;
        }
    }

    // Finalizer: catch any exception thrown by the original GetScoreOfWarInternal
    private static void Finalizer(
        ref TextObject reason,
        ref Single __result,
        Exception __exception) {
        try {
            if (__exception != null) {
                BatterLog.Warn(
                    $"[SafeGetScoreOfWarInternalPatch] Suppressed exception in GetScoreOfWarInternal:\n  {__exception}");
                __result = 0;
                reason = TextObject.Empty;
            }
        }
        catch (Exception ex) {
            BatterLog.Error($"[SafeScoreOfDeclaringWarPatch] Suppression failed in GetScoreOfDeclaringWar:\n  {ex}");
        }
    }
}