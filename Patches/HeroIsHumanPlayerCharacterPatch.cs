using HarmonyLib;
using SafeWarLogPatch.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace SafeWarLogPatch.Patches;

[HarmonyPatch(typeof(Hero), nameof(Hero.IsHumanPlayerCharacter), MethodType.Getter)]
public class HeroIsHumanPlayerCharacterPatch
{
    private static void Prefix()
    {
        try
        {
            var hero = Hero.OneToOneConversationHero;

            if (hero == null)
            {
                BatterLog.Info(
                    "[HeroConvSafePatch] Skipped getter for IsHumanPlayerCharacter: OneToOneConversationHero is null.");
                return;
            }

            // Safe to access after null check
            if (hero.Clan == null || hero.MapFaction == null)
            {
                var occupation = hero.CharacterObject?.Occupation;

                if (hero.IsNotable || hero.IsWanderer || occupation == Occupation.Villager)
                    return;

                BatterLog.Info(
                    $"[HeroConvSafePatch] Skipped conversation: missing faction or clan for {hero.Name} (Occupation: {occupation})");
            }
        }
        catch (Exception ex)
        {
            var msg = $"[HeroConvSafePatch] Error during conversation safety check: {ex}";
            InformationManager.DisplayMessage(new InformationMessage(msg));
            BatterLog.Info(msg);
        }
    }
}