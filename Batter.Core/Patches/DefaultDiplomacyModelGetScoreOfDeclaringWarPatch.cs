using Batter.Core.Utils;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Localization;

namespace Batter.Core.Patches;

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
public static class DefaultDiplomacyModelGetScoreOfDeclaringWarPatch {
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

    private static Boolean Prefix(
        IFaction factionDeclaresWar,
        IFaction factionDeclaredWar,
        IFaction evaluatingClan,
        ref TextObject warReason,
        ref Single __result) {
        try {
            // 1) If any of the three factions is null, skip original & return 0
            if (factionDeclaresWar == null
                || factionDeclaredWar == null
                || evaluatingClan == null) {
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
            if (object.ReferenceEquals(factionDeclaresWar, factionDeclaredWar)) {
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
        catch (Exception ex) {
            BatterLog.Warn($"[SafeScoreOfDeclaringWarPatch] Exception in GetScoreOfDeclaringWar Prefix: {ex}");
            __result = 0;
            warReason = TextObject.Empty;
            return false;
        }
    }

    // Finalizer: if original threw, we catch here, log, and force safe defaults.
    private static void Finalizer(
        ref TextObject warReason,
        ref Single __result,
        Exception __exception) {
        try {
            if (__exception != null) {
                BatterLog.Warn(
                    $"[SafeScoreOfDeclaringWarPatch] Suppressed exception in GetScoreOfDeclaringWar:\n  {__exception}");
                __result = 0;
                warReason = TextObject.Empty;
            }
        }
        catch (Exception ex) {
            BatterLog.Error($"[SafeScoreOfDeclaringWarPatch] Suppression failed in GetScoreOfDeclaringWar:\n  {ex}");
        }
    }
}