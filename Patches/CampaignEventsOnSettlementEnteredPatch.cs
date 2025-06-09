using Batter.Core.Utils;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace Batter.Core.Patches;

[HarmonyPatch(typeof(CampaignEvents), "OnSettlementEntered")]
public static class CampaignEventsOnSettlementEnteredPatch {
    private static Boolean Prefix(MobileParty party, Settlement settlement, Hero hero) {
        try {
            if (hero == null)
                BatterLog.Info(
                    "[SafeVillagerSettlementEntryPatch] hero is null. should not be a problem, but can be useful to know.");

            // 1) If mobileParty or settlement is null, skip original.
            if (party == null || settlement == null) {
                BatterLog.Warn("[SafeVillagerSettlementEntryPatch] party or settlement is null. Skipping...");
                return false;
            }

            // 2) If this party is NOT a villager, skip original.
            if (!party.IsVillager)
                return true;

            // 3) If they do not have a home‐village, skip original.
            var home = party.HomeSettlement;
            if (home == null || home.Village == null)
                BatterLog.Warn(
                    "[SafeVillagerSettlementEntryPatch] HomeSettlement or Village is null for party. Attempting to proceed (if crashes, revert return to false here)");
            //party.RemoveParty();
            //return false;
            // 4) If that village’s TradeBound is null, skip original.
            if (home.Village.TradeBound == null)
                BatterLog.Warn(
                    "[SafeVillagerSettlementEntryPatch] Village's TradeBound is null. Attempting to proceed (if crashes, revert return to false here)");
            //party.RemoveParty();
            //return false;
            // Example check for Settlement's Parties collection
            if (settlement?.Parties == null || settlement.Parties.Count < 1)
                BatterLog.Warning(
                    $"No parties found in settlement {settlement?.Name}.  Attempting to proceed (if crashes, revert return to false here)");
            // return false;
            if (settlement?.ClaimedBy == null)
                BatterLog.Info(
                    $"Settlement {settlement?.Name} is not claimed by anyone! it's rebellious.  Attempting to proceed (if crashes, revert return to false here)");
            // return false;
            if (settlement != null && settlement.IsUnderRebellionAttack())
                BatterLog.Info(
                    $"Settlement {settlement?.Name} is under rebellion attack! it's rebellious.  Attempting to proceed (if crashes, revert return to false here)");
            // return false;
            // Example check for HeroesWithoutParty
            if (settlement?.HeroesWithoutParty == null || settlement.HeroesWithoutParty.Count < 1)
                BatterLog.Warning(
                    $"No heroes found without party in settlement {settlement?.Name}.  Attempting to proceed (if crashes, revert return to false here)");
            // return false;
            var villagerParties = MobileParty.AllVillagerParties;
            if (villagerParties == null || villagerParties.Count < 1)
                BatterLog.Warning(
                    "No villager parties found.  Attempting to proceed (if crashes, revert return to false here)");
            //party.RemoveParty();
            //return false;
            // Check for MobileParty's attachment
            // if (party?.AttachedTo == null)
            // {
            //     SafeLog.Info("[SafeVillagerSettlementEntryPatch] MobileParty is not attached to anything.");
            // }// NOTE: this doesn't seem consequential

            // Check for missing HomeSettlement or Village
            if (party?.HomeSettlement?.Village == null)
                BatterLog.Warning(
                    "[SafeVillagerSettlementEntryPatch] MobileParty's home settlement or village is missing. Attempting to proceed (if crashes, revert return to false here)");
            //party.RemoveParty();
            //return false;  // Prevent further execution
            try {
                return true; // Run original if no issues
            }
            catch (KeyNotFoundException knf) {
                BatterLog.Warn($"[SafeVillagerSettlementEntryPatch] KeyNotFoundException for {party?.Name}: {knf}");

                if (party != null) {
                    BatterLog.Warn($"[SafeVillagerSettlementEntryPatch] Destroying broken party: {party.Name}");
                    party.RemoveParty();
                }

                return false; // Skip original
            }
            catch (Exception ex) {
                BatterLog.Error($"[SafeVillagerHourlyTickPatch] Unexpected error: {ex}");
                return false;
            }
        }
        catch (Exception ex) {
            var msg = $"[SafeVillagerSettlementEntryPatch] Error during SafeVillagerSettlementEntryPatch: {ex}";
            BatterLog.Error(msg);

            party.RemoveParty();
            return false;
        }
    }

    // Harmony finalizer method to catch exceptions and log them instead of letting them crash the game.
    private static void Finalizer(MobileParty party, Exception __exception) {
        try {
            if (__exception != null) {
                BatterLog.Warn(
                    $"[SafeVillagerSettlementEntryPatchLate] Suppressed exception in VillagerCampaignBehavior.OnSettlementEntered:\n  {__exception}");
                // We intentionally do not rethrow. This prevents a KeyNotFoundException (or anything else)
                // from aborting the tick loop. The log above helps diagnose what exactly happened if it ever does.
                party.RemoveParty();
            }
        }
        catch (Exception ex) {
            // If an error occurs within the finalizer itself, log it.
            BatterLog.Error($"[SafeVillagerSettlementEntryPatchLate] Error in finalizer: {ex}");
        }
    }
}