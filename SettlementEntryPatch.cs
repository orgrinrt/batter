using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using System.Linq;
using System;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.LogEntries;
using System.Collections;
using System.Collections.Generic;

namespace SafeWarLogPatch
{
    [HarmonyPatch(typeof(VillagerCampaignBehavior), "OnSettlementEntered")]
    [HarmonyPriority(Priority.High)]
    public static class SafeVillagerSettlementEntryPatch
    {
        static bool Prefix(MobileParty mobileParty, Settlement settlement, Hero hero)
        {
            try
            {
                if (hero == null)
                    SafeLog.Info("[SafeVillagerSettlementEntryPatch] param hero is null. not cause for skip though. Continuing OnSettlementEntered...");

                // 1) If mobileParty or settlement is null, skip original.
                if (mobileParty == null || settlement == null)
                {
                    SafeLog.Warn("[SafeVillagerSettlementEntryPatch] mobileParty or settlement is null. Skipping OnSettlementEntered...");
                    return false;
                }

                 // 2) If this party is NOT a villager, skip original.
                 if (!mobileParty.IsVillager)
                   return true;

                // 3) If they do not have a home‐village, skip original.
                var home = mobileParty.HomeSettlement;
                if (home == null || home.Village == null)
                {
                    SafeLog.Warn("[SafeVillagerSettlementEntryPatch] HomeSettlement or Village is null for mobileParty. Attempting to proceed (if crashes, revert return to false here)");
                    //mobileParty.RemoveParty();
                    //return false;
                }

                // 4) If that village’s TradeBound is null, skip original.
                if (home.Village.TradeBound == null)
                {
                    SafeLog.Warn("[SafeVillagerSettlementEntryPatch] Village's TradeBound is null. Attempting to proceed (if crashes, revert return to false here)");
                    //mobileParty.RemoveParty();
                    //return false;
                }

                // Example check for Settlement's Parties collection
                if (settlement?.Parties == null || settlement.Parties.Count < 1)
                {
                    SafeLog.Warning($"No parties found in settlement {settlement?.Name?.ToString()}. Attempting to proceed (if crashes, revert return to false here)");
                    //return false;
                }

                if (settlement?.ClaimedBy == null)
                {
                    SafeLog.Info($"Settlement {settlement?.Name?.ToString()} is not claimed by anyone! it's rebellious perhaps?  Attempting to proceed (if crashes, revert return to false here)");
                    // return false;
                }

                if (settlement != null && settlement.IsUnderRebellionAttack()) {
                    SafeLog.Info($"Settlement {settlement?.Name?.ToString()} is under rebellion attack! it's rebellious.  Attempting to proceed (if crashes, revert return to false here)");
                    // return false;
                }

                // Example check for HeroesWithoutParty
                if (settlement?.HeroesWithoutParty == null || settlement.HeroesWithoutParty.Count < 1)
                {
                    SafeLog.Warning($"No heroes found without party in settlement {settlement?.Name?.ToString()}.  Attempting to proceed (if crashes, revert return to false here)");
                    // return false;
                }

                var villagerParties = MobileParty.AllVillagerParties;
                if (villagerParties == null || villagerParties.Count < 1)
                {
                    SafeLog.Warning("No villager parties found.  Attempting to proceed (if crashes, revert return to false here)");
                    // mobileParty.RemoveParty();
                    // return false;
                }

                // Check for MobileParty's attachment
                /*if (mobileParty?.AttachedTo == null)
                {
                    SafeLog.Info("[SafeVillagerSettlementEntryPatch] MobileParty is not attached to anything.");
                } */// NOTE: this doesn't seem consequential

                // Check for missing HomeSettlement or Village
                if (mobileParty?.HomeSettlement?.Village == null)
                {
                    SafeLog.Warning("[SafeVillagerSettlementEntryPatch] MobileParty's home settlement or village is missing. Attempting to proceed (if crashes, revert return to false here)");
                    //mobileParty.RemoveParty();
                    //return false;  // Prevent further execution
                }

                try
                {
                    return true; // Run original if no issues
                }
                catch (KeyNotFoundException knf)
                {
                    SafeLog.Warn($"[SafeVillagerSettlementEntryPatch] KeyNotFoundException for {mobileParty?.Name}: {knf}");

                    if (mobileParty != null)
                    {
                        SafeLog.Warn($"[SafeVillagerSettlementEntryPatch] Destroying broken party: {mobileParty.Name}");
                        mobileParty.RemoveParty();
                    }

                    return false; // Skip original
                }
                catch (Exception ex)
                {
                    SafeLog.Error($"[SafeVillagerHourlyTickPatch] Unexpected error: {ex}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                string msg = $"[SafeVillagerSettlementEntryPatch] Error during SafeVillagerSettlementEntryPatch: {ex}";
                SafeLog.Error(msg);
                // Reflection: Print properties and fields for MobileParty
                LogObjectPropertiesAndFields("MobileParty", mobileParty);

                // Reflection: Print properties and fields for Settlement
                LogObjectPropertiesAndFields("Settlement", settlement);

                mobileParty.RemoveParty();

                return false;
            }
        }

        // Harmony finalizer method to catch exceptions and log them instead of letting them crash the game.
        static void Finalizer(MobileParty mobileParty, Exception __exception)
        {
            try
            {
                if (__exception != null)
                {
                    SafeLog.Warn($"[SafeVillagerSettlementEntryPatch] Suppressed exception in VillagerCampaignBehavior.OnSettlementEntered:\n  {__exception}");
                    // We intentionally do not rethrow. This prevents a KeyNotFoundException (or anything else)
                    // from aborting the tick loop. The log above helps diagnose what exactly happened if it ever does.
                    mobileParty.RemoveParty();
                }
            }
            catch (Exception ex)
            {
                // If an error occurs within the finalizer itself, log it.
                SafeLog.Error($"[SafeVillagerSettlementEntryPatch] Error in finalizer: {ex}");

            }
        }

        static void LogObjectPropertiesAndFields(string objectName, object obj)
        {
            if (obj == null)
            {
                SafeLog.Warn($"{objectName} is null.");
                return;
            }

            Type type = obj.GetType();
            SafeLog.Info($"[Reflection] {objectName} Properties and Fields:");

            // Log properties
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                try
                {
                    var value = property.GetValue(obj);
                    SafeLog.Info($"  Property: {property.Name}, Value: {value?.ToString() ?? "null"}");
                }
                catch (Exception ex)
                {
                    SafeLog.Error($"  Failed to read property {property.Name}: {ex.Message}");
                }
            }

            // Log fields
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                try
                {
                    var value = field.GetValue(obj);
                    SafeLog.Info($"  Field: {field.Name}, Value: {value?.ToString() ?? "null"}");
                }
                catch (Exception ex)
                {
                    SafeLog.Error($"  Failed to read field {field.Name}: {ex.Message}");
                }
            }
        }
    }


    [HarmonyPatch(typeof(CampaignEvents), "OnSettlementEntered")]
    public static class SafeVillagerSettlementEntryPatchLate
    {

        static bool Prefix(MobileParty party, Settlement settlement, Hero hero)
        {
            try
            {
                if (hero == null)
                    SafeLog.Info("[SafeVillagerSettlementEntryPatch] hero is null. should not be a problem, but can be useful to know.");

                // 1) If mobileParty or settlement is null, skip original.
                if (party == null || settlement == null)
                {
                    SafeLog.Warn("[SafeVillagerSettlementEntryPatch] party or settlement is null. Skipping...");
                    return false;
                }

                // 2) If this party is NOT a villager, skip original.
                if (!party.IsVillager)
                    return true;

                // 3) If they do not have a home‐village, skip original.
                var home = party.HomeSettlement;
                if (home == null || home.Village == null)
                {
                    SafeLog.Warn("[SafeVillagerSettlementEntryPatch] HomeSettlement or Village is null for party. Attempting to proceed (if crashes, revert return to false here)");
                    //party.RemoveParty();
                    //return false;
                }

                // 4) If that village’s TradeBound is null, skip original.
                if (home.Village.TradeBound == null)
                {
                    SafeLog.Warn("[SafeVillagerSettlementEntryPatch] Village's TradeBound is null. Attempting to proceed (if crashes, revert return to false here)");
                    //party.RemoveParty();
                    //return false;
                }

                // Example check for Settlement's Parties collection
                if (settlement?.Parties == null || settlement.Parties.Count < 1)
                {
                    SafeLog.Warning($"No parties found in settlement {settlement?.Name?.ToString()}.  Attempting to proceed (if crashes, revert return to false here)");
                    // return false;
                }

                if (settlement?.ClaimedBy == null)
                {
                    SafeLog.Info($"Settlement {settlement?.Name?.ToString()} is not claimed by anyone! it's rebellious.  Attempting to proceed (if crashes, revert return to false here)");
                    // return false;
                }

                if (settlement != null && settlement.IsUnderRebellionAttack()) {
                    SafeLog.Info($"Settlement {settlement?.Name?.ToString()} is under rebellion attack! it's rebellious.  Attempting to proceed (if crashes, revert return to false here)");
                    // return false;
                }

                // Example check for HeroesWithoutParty
                if (settlement?.HeroesWithoutParty == null || settlement.HeroesWithoutParty.Count < 1)
                {
                    SafeLog.Warning($"No heroes found without party in settlement {settlement?.Name?.ToString()}.  Attempting to proceed (if crashes, revert return to false here)");
                    // return false;
                }

                var villagerParties = MobileParty.AllVillagerParties;
                if (villagerParties == null || villagerParties.Count < 1)
                {
                    SafeLog.Warning("No villager parties found.  Attempting to proceed (if crashes, revert return to false here)");
                    //party.RemoveParty();
                    //return false;
                }

                // Check for MobileParty's attachment
                // if (party?.AttachedTo == null)
                // {
                //     SafeLog.Info("[SafeVillagerSettlementEntryPatch] MobileParty is not attached to anything.");
                // }// NOTE: this doesn't seem consequential

                // Check for missing HomeSettlement or Village
                if (party?.HomeSettlement?.Village == null)
                {
                    SafeLog.Warning("[SafeVillagerSettlementEntryPatch] MobileParty's home settlement or village is missing. Attempting to proceed (if crashes, revert return to false here)");
                    //party.RemoveParty();
                    //return false;  // Prevent further execution
                }

                try
                {
                    return true; // Run original if no issues
                }
                catch (KeyNotFoundException knf)
                {
                    SafeLog.Warn($"[SafeVillagerSettlementEntryPatch] KeyNotFoundException for {party?.Name}: {knf}");

                    if (party != null)
                    {
                        SafeLog.Warn($"[SafeVillagerSettlementEntryPatch] Destroying broken party: {party.Name}");
                        party.RemoveParty();
                    }

                    return false; // Skip original
                }
                catch (Exception ex)
                {
                    SafeLog.Error($"[SafeVillagerHourlyTickPatch] Unexpected error: {ex}");
                    return false;
                }

            }
            catch (Exception ex)
            {
                string msg = $"[SafeVillagerSettlementEntryPatch] Error during SafeVillagerSettlementEntryPatch: {ex}";
                SafeLog.Error(msg);

                party.RemoveParty();
                return false;
            }
        }

        // Harmony finalizer method to catch exceptions and log them instead of letting them crash the game.
        static void Finalizer(MobileParty party, Exception __exception)
        {
            try
            {
                if (__exception != null)
                {
                    SafeLog.Warn($"[SafeVillagerSettlementEntryPatchLate] Suppressed exception in VillagerCampaignBehavior.OnSettlementEntered:\n  {__exception}");
                    // We intentionally do not rethrow. This prevents a KeyNotFoundException (or anything else)
                    // from aborting the tick loop. The log above helps diagnose what exactly happened if it ever does.
                    party.RemoveParty();
                }
            }
            catch (Exception ex)
            {
                // If an error occurs within the finalizer itself, log it.
                SafeLog.Error($"[SafeVillagerSettlementEntryPatchLate] Error in finalizer: {ex}");

            }
        }
    }

    [HarmonyPatch(typeof(VillagerCampaignBehavior), "HourlyTickParty")]
    public static class SafeVillagerHourlyTickPatch
    {
        static bool Prefix(MobileParty villagerParty)
        {
            try
            {

                // 1) If mobileParty or settlement is null, skip original.
                if (villagerParty == null)
                {
                    SafeLog.Warn("[SafeVillagerHourlyTickPatch] villagerParty or settlement is null.");
                    return false;
                }

                // 2) If this party is NOT a villager, skip original.
                if (!villagerParty.IsVillager)
                    return true;

                // 3) If they do not have a home‐village, skip original.
                var home = villagerParty.HomeSettlement;
                if (home == null || home.Village == null)
                {
                    SafeLog.Warn("[SafeVillagerHourlyTickPatch] HomeSettlement or Village is null for villagerParty. Attempting to proceed (if crashes, revert return to false here)");
                    //villagerParty.RemoveParty();
                    //return false;
                }

                // 4) If that village’s TradeBound is null, skip original.
                if (home.Village.TradeBound == null)
                {
                    SafeLog.Warn("[SafeVillagerHourlyTickPatch] Village's TradeBound is null. Attempting to proceed (if crashes, revert return to false here)");
                    //villagerParty.RemoveParty();
                    //return false;
                }
                // Check for MobileParty's attachment
                if (villagerParty?.AttachedTo == null)
                {
                    SafeLog.Info("[SafeVillagerHourlyTickPatch] MobileParty is not attached to anything. Continuing...");
                }

                // Check for missing HomeSettlement or Village
                if (villagerParty?.HomeSettlement?.Village == null)
                {
                    SafeLog.Warning("[SafeVillagerHourlyTickPatch] MobileParty's home settlement or village is missing. Attempting to proceed (if crashes, revert return to false here)");
                    //villagerParty.RemoveParty();
                   //return false;  // Prevent further execution
                }

                try
                {
                    return true; // Run original if no issues
                }
                catch (KeyNotFoundException knf)
                {
                    SafeLog.Warn($"[SafeVillagerHourlyTickPatch] KeyNotFoundException for {villagerParty?.Name}: {knf}");

                    if (villagerParty != null)
                    {
                        SafeLog.Warn($"[SafeVillagerHourlyTickPatch] Destroying broken party: {villagerParty.Name}");
                        villagerParty.RemoveParty();
                    }

                    return false; // Skip original
                }
                catch (Exception ex)
                {
                    SafeLog.Error($"[SafeVillagerHourlyTickPatch] Unexpected error: {ex}");
                    return false;
                }
            }
            catch (KeyNotFoundException knf)
            {
                SafeLog.Warn($"[SafeVillagerHourlyTickPatch] KeyNotFoundException for {villagerParty?.Name}: {knf}");

                if (villagerParty != null)
                {
                    SafeLog.Warn($"[SafeVillagerHourlyTickPatch] Destroying broken party: {villagerParty.Name}");
                    villagerParty.RemoveParty();
                }

                return false; // Skip original
            }
            catch (Exception ex)
            {
                SafeLog.Error($"[SafeVillagerHourlyTickPatch] Unexpected error: {ex}");
                return false;
            }
        }

        static void Finalizer(MobileParty villagerParty, Exception __exception)
        {
            if (__exception != null)
            {
                SafeLog.Warn($"[SafeVillagerHourlyTickPatch] Exception: {__exception}");

                if (villagerParty != null)
                {
                    SafeLog.Warn($"Destroying broken party: {villagerParty.Name}");
                    villagerParty.RemoveParty();
                }
            }
        }

    }

    [HarmonyPatch(typeof(CampaignEvents), "HourlyTickParty")]
    public static class SafeVillagerHourlyTickPatchLate
    {

        static bool Prefix(MobileParty mobileParty)
        {
            try
            {

                // 1) If mobileParty or settlement is null, skip original.
                if (mobileParty == null)
                {
                    SafeLog.Warn("[SafeVillagerHourlyTickPatchLate] mobileParty or settlement is null.");
                    return false;
                }

                // 2) If this party is NOT a villager, skip original.
                if (!mobileParty.IsVillager)
                    return true;

                // 3) If they do not have a home‐village, skip original.
                var home = mobileParty.HomeSettlement;
                if (home == null || home.Village == null)
                {
                    SafeLog.Warn("[SafeVillagerHourlyTickPatchLate] HomeSettlement or Village is null for mobileParty. Attempting to proceed (if crashes, revert return to false here)");
                    // mobileParty.RemoveParty();
                    // return false;
                }

                // 4) If that village’s TradeBound is null, skip original.
                if (home.Village.TradeBound == null)
                {
                    SafeLog.Warn("[SafeVillagerHourlyTickPatchLate] Village's TradeBound is null. Attempting to proceed (if crashes, revert return to false here)");
                    // mobileParty.RemoveParty();
                    // return false;
                }
                // Check for MobileParty's attachment
                if (mobileParty?.AttachedTo == null)
                {
                    SafeLog.Info("[SafeVillagerHourlyTickPatchLate] MobileParty is not attached to anything. Continuing...");
                }

                // Check for missing HomeSettlement or Village
                if (mobileParty?.HomeSettlement?.Village == null)
                {
                    SafeLog.Warning("[SafeVillagerHourlyTickPatchLate] MobileParty's home settlement or village is missing. Attempting to proceed (if crashes, revert return to false here)");
                    // mobileParty.RemoveParty();
                    // return false;  // Prevent further execution
                }

                try
                {
                    return true; // Run original if no issues
                }
                catch (KeyNotFoundException knf)
                {
                    SafeLog.Warn($"[SafeVillagerHourlyTickPatch] KeyNotFoundException for {mobileParty?.Name}: {knf}");

                    if (mobileParty != null)
                    {
                        SafeLog.Warn($"[SafeVillagerHourlyTickPatch] Destroying broken party: {mobileParty.Name}");
                        mobileParty.RemoveParty();
                    }

                    return false; // Skip original
                }
                catch (Exception ex)
                {
                    SafeLog.Error($"[SafeVillagerHourlyTickPatch] Unexpected error: {ex}");
                    return false;
                }
            }
            catch (KeyNotFoundException knf)
            {
                SafeLog.Warn($"[SafeVillagerHourlyTickPatchLate] KeyNotFoundException for {mobileParty?.Name}: {knf}");

                if (mobileParty != null)
                {
                    SafeLog.Warn($"[SafeVillagerHourlyTickPatchLate] Destroying broken party: {mobileParty.Name}");
                    mobileParty.RemoveParty();
                }

                return false; // Skip original
            }
            catch (Exception ex)
            {
                SafeLog.Error($"[SafeVillagerHourlyTickPatchLate] Unexpected error: {ex}");
                return false;
            }
        }

        static void Finalizer(MobileParty mobileParty, Exception __exception)
        {
            if (__exception != null)
            {
                SafeLog.Warn($"[SafeVillagerHourlyTickPatchLate] Exception: {__exception}");

                if (mobileParty != null)
                {
                    SafeLog.Warn($"Destroying broken party: {mobileParty.Name}");
                    mobileParty.RemoveParty();
                }
            }
        }


    }
}
