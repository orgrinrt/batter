using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using System;
using System.Linq;
using System.Reflection;

namespace SafeWarLogPatch
{
    [HarmonyPatch(typeof(CampaignEventDispatcher), "OnWarDeclared")]
    public class WarLogSafePatch
    {
        static bool Prefix(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail 	declareWarDetail)
        {
            //SafeLog.Info("WarLogSafePatch Prefix called!");

            bool replaced = false;

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

            if (faction1 == null)
            {
                IFaction? fallback = null;//declaringHero?.MapFaction;
                if (fallback == null)
                {
                    const string msg = "[WarLogSafePatch] Skipped war declaration: missing faction1 and could not infer from hero.";
                    SafeLog.Info(msg);
                    return false;
                }

                faction1 = fallback;
                replaced = true;
            }

            if (faction2 == null && faction1 is Kingdom k1)
            {
                var validTargets = Kingdom.All
                .Where(k => k != k1 && !k.IsEliminated && !k1.IsAtWarWith(k))
                .ToList();

                faction2 = validTargets.Count > 0 ? validTargets[MBRandom.RandomInt(validTargets.Count)] : null;
                replaced = true;
            }

            if (faction1 == null || faction2 == null)
            {
                const string msg = "[WarLogSafePatch] Skipped war declaration: unrecoverable context.";
                SafeLog.Info(msg);
                return false;
            }

            if (replaced)
            {
                string msg = $"[WarLogSafePatch] Reconstructed war declaration: {faction1?.Name} vs {faction2?.Name}";
                SafeLog.Info(msg);
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Hero), nameof(Hero.IsHumanPlayerCharacter), MethodType.Getter)]
    public class HeroConvSafePatch
    {
        static void Prefix()
        {
            try
            {
                var hero = Hero.OneToOneConversationHero;

                if (hero == null)
                {
                    SafeLog.Info("[HeroConvSafePatch] Skipped getter for IsHumanPlayerCharacter: OneToOneConversationHero is null.");
                    return;
                }

                // Safe to access after null check
                if (hero.Clan == null || hero.MapFaction == null)
                {
                    var occupation = hero.CharacterObject?.Occupation;

                    if (hero.IsNotable || hero.IsWanderer || occupation == Occupation.Villager)
                        return;

                    SafeLog.Info($"[HeroConvSafePatch] Skipped conversation: missing faction or clan for {hero.Name} (Occupation: {occupation})");
                    return;
                }
            }
            catch (Exception ex)
            {
                string msg = $"[HeroConvSafePatch] Error during conversation safety check: {ex}";
                InformationManager.DisplayMessage(new InformationMessage(msg));
                SafeLog.Info(msg);
            }
        }
    }

}
