using System;
using Batter.Core.Extensions;
using Batter.Core.Utils;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace Batter.Core.Patches;

[HarmonyPatch(typeof(VillagerCampaignBehavior), "OnSettlementEntered")]
[HarmonyPriority(Priority.High)]
public static class VillagerCampaignBehaviourOnSettlementEnteredPatch {
    private static bool Prefix(MobileParty mobileParty, Settlement settlement, Hero hero) {
        try {
            if (hero == null)
                BatterLog.Info(
                    "[SafeVillagerSettlementEntryPatch] param hero is null. not cause for skip though. Continuing OnSettlementEntered...");

            // 1) If mobileParty or settlement is null, skip original.
            if (mobileParty == null || settlement == null) {
                BatterLog.Warn(
                    "[SafeVillagerSettlementEntryPatch] mobileParty or settlement is null. Skipping OnSettlementEntered...");
                return false;
            }

            // 2) If this party is NOT a villager, skip original.
            if (!mobileParty.IsVillager)
                return true;

            // 3) If they do not have a home‐village, skip original.
            var home = mobileParty.HomeSettlement;
            if (home == null || home.Village == null)
                BatterLog.Warn(
                    "[SafeVillagerSettlementEntryPatch] HomeSettlement or Village is null for mobileParty. Attempting to proceed (if crashes, revert return to false here)");
            //mobileParty.RemoveParty();
            //return false;
            // 4) If that village’s TradeBound is null, skip original.
            if (home.Village.TradeBound == null)
                BatterLog.Warn(
                    "[SafeVillagerSettlementEntryPatch] Village's TradeBound is null. Attempting to proceed (if crashes, revert return to false here)");
            //mobileParty.RemoveParty();
            //return false;
            // Example check for Settlement's Parties collection
            if (settlement?.Parties == null || settlement.Parties.Count < 1)
                BatterLog.Warning(
                    $"No parties found in settlement {settlement?.Name}. Attempting to proceed (if crashes, revert return to false here)");
            //return false;
            if (settlement?.ClaimedBy == null)
                BatterLog.Info(
                    $"Settlement {settlement?.Name} is not claimed by anyone! it's rebellious perhaps?  Attempting to proceed (if crashes, revert return to false here)");
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
            // mobileParty.RemoveParty();
            // return false;
            // Check for MobileParty's attachment
            /*if (mobileParty?.AttachedTo == null)
            {
                SafeLog.Info("[SafeVillagerSettlementEntryPatch] MobileParty is not attached to anything.");
            } */ // NOTE: this doesn't seem consequential

            // Check for missing HomeSettlement or Village
            if (mobileParty?.HomeSettlement?.Village == null)
                BatterLog.Warning(
                    "[SafeVillagerSettlementEntryPatch] MobileParty's home settlement or village is missing. Attempting to proceed (if crashes, revert return to false here)");
            //mobileParty.RemoveParty();
            //return false;  // Prevent further execution
            try {
                return true; // Run original if no issues
            }
            catch (KeyNotFoundException knf) {
                BatterLog.Warn(
                    $"[SafeVillagerSettlementEntryPatch] KeyNotFoundException for {mobileParty?.Name}: {knf}");

                if (mobileParty != null) {
                    BatterLog.Warn($"[SafeVillagerSettlementEntryPatch] Destroying broken party: {mobileParty.Name}");
                    mobileParty.RemoveParty();
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
            // Reflection: Print properties and fields for MobileParty
            mobileParty.LogObjectPropertiesAndFields("MobileParty");

            // Reflection: Print properties and fields for Settlement
            settlement.LogObjectPropertiesAndFields("Settlement");

            mobileParty.RemoveParty();

            return false;
        }
    }

    // Harmony finalizer method to catch exceptions and log them instead of letting them crash the game.
    private static void Finalizer(MobileParty mobileParty, Exception __exception) {
        try {
            if (__exception != null) {
                BatterLog.Warn(
                    $"[SafeVillagerSettlementEntryPatch] Suppressed exception in VillagerCampaignBehavior.OnSettlementEntered:\n  {__exception}");
                // We intentionally do not rethrow. This prevents a KeyNotFoundException (or anything else)
                // from aborting the tick loop. The log above helps diagnose what exactly happened if it ever does.
                mobileParty.RemoveParty();
            }
        }
        catch (Exception ex) {
            // If an error occurs within the finalizer itself, log it.
            BatterLog.Error($"[SafeVillagerSettlementEntryPatch] Error in finalizer: {ex}");
        }
    }
}