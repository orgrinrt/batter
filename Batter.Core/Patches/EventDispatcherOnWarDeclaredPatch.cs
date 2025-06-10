using Batter.Core.Utils;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;

namespace Batter.Core.Patches;

[HarmonyPatch(typeof(CampaignEventDispatcher), "OnWarDeclared")]
public class EventDispatcherOnWarDeclaredPatch {
    private static bool Prefix(IFaction faction1, IFaction faction2,
        DeclareWarAction.DeclareWarDetail declareWarDetail) {
        //SafeLog.Info("WarLogSafePatch Prefix called!");

        var replaced = false;

        // if (declaringHero == null || declaringHero.IsDead)
        // {
        //     if (Hero.MainHero == null)
        //     {
        //         const string msg = "[SafeWarLogPatch] Skipped war declaration: no valid hero.";
        //         SafeLog.Info(msg);
        //         return false;
        //     }
        //     declaringHero = Hero.MainHero;
        // }

        if (faction1 == null) {
            IFaction? fallback = null; //declaringHero?.MapFaction;
            if (fallback == null) {
                const string msg =
                    "[WarLogSafePatch] Skipped war declaration: missing faction1 and could not infer from hero.";
                BatterLog.Info(msg);
                return false;
            }

            faction1 = fallback;
            replaced = true;
        }

        if (faction2 == null && faction1 is Kingdom k1) {
            var validTargets = Kingdom.All
                .Where(k => k != k1 && !k.IsEliminated && !k1.IsAtWarWith(k))
                .ToList();

            faction2 = validTargets.Count > 0 ? validTargets[MBRandom.RandomInt(validTargets.Count)] : null;
            replaced = true;
        }

        if (faction1 == null || faction2 == null) {
            const string msg = "[WarLogSafePatch] Skipped war declaration: unrecoverable context.";
            BatterLog.Info(msg);
            return false;
        }

        if (replaced) {
            var msg = $"[WarLogSafePatch] Reconstructed war declaration: {faction1?.Name} vs {faction2?.Name}";
            BatterLog.Info(msg);
        }

        return true;
    }
}