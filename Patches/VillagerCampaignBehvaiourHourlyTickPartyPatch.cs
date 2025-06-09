using HarmonyLib;
using SafeWarLogPatch.Utils;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;

namespace SafeWarLogPatch.Patches;

[HarmonyPatch(typeof(VillagerCampaignBehavior), "HourlyTickParty")]
public static class VillagerCampaignBehvaiourHourlyTickPartyPatch
{
    private static bool Prefix(MobileParty villagerParty)
    {
        try
        {
            // 1) If mobileParty or settlement is null, skip original.
            if (villagerParty == null)
            {
                BatterLog.Warn("[SafeVillagerHourlyTickPatch] villagerParty or settlement is null.");
                return false;
            }

            // 2) If this party is NOT a villager, skip original.
            if (!villagerParty.IsVillager)
                return true;

            // 3) If they do not have a home‐village, skip original.
            var home = villagerParty.HomeSettlement;
            if (home == null || home.Village == null)
                BatterLog.Warn(
                    "[SafeVillagerHourlyTickPatch] HomeSettlement or Village is null for villagerParty. Attempting to proceed (if crashes, revert return to false here)");
            //villagerParty.RemoveParty();
            //return false;
            // 4) If that village’s TradeBound is null, skip original.
            if (home.Village.TradeBound == null)
                BatterLog.Warn(
                    "[SafeVillagerHourlyTickPatch] Village's TradeBound is null. Attempting to proceed (if crashes, revert return to false here)");
            //villagerParty.RemoveParty();
            //return false;
            // Check for MobileParty's attachment
            if (villagerParty?.AttachedTo == null)
                BatterLog.Info("[SafeVillagerHourlyTickPatch] MobileParty is not attached to anything. Continuing...");

            // Check for missing HomeSettlement or Village
            if (villagerParty?.HomeSettlement?.Village == null)
                BatterLog.Warning(
                    "[SafeVillagerHourlyTickPatch] MobileParty's home settlement or village is missing. Attempting to proceed (if crashes, revert return to false here)");
            //villagerParty.RemoveParty();
            //return false;  // Prevent further execution
            try
            {
                return true; // Run original if no issues
            }
            catch (KeyNotFoundException knf)
            {
                BatterLog.Warn($"[SafeVillagerHourlyTickPatch] KeyNotFoundException for {villagerParty?.Name}: {knf}");

                if (villagerParty != null)
                {
                    BatterLog.Warn($"[SafeVillagerHourlyTickPatch] Destroying broken party: {villagerParty.Name}");
                    villagerParty.RemoveParty();
                }

                return false; // Skip original
            }
            catch (Exception ex)
            {
                BatterLog.Error($"[SafeVillagerHourlyTickPatch] Unexpected error: {ex}");
                return false;
            }
        }
        catch (KeyNotFoundException knf)
        {
            BatterLog.Warn($"[SafeVillagerHourlyTickPatch] KeyNotFoundException for {villagerParty?.Name}: {knf}");

            if (villagerParty != null)
            {
                BatterLog.Warn($"[SafeVillagerHourlyTickPatch] Destroying broken party: {villagerParty.Name}");
                villagerParty.RemoveParty();
            }

            return false; // Skip original
        }
        catch (Exception ex)
        {
            BatterLog.Error($"[SafeVillagerHourlyTickPatch] Unexpected error: {ex}");
            return false;
        }
    }

    private static void Finalizer(MobileParty villagerParty, Exception __exception)
    {
        if (__exception != null)
        {
            BatterLog.Warn($"[SafeVillagerHourlyTickPatch] Exception: {__exception}");

            if (villagerParty != null)
            {
                BatterLog.Warn($"Destroying broken party: {villagerParty.Name}");
                villagerParty.RemoveParty();
            }
        }
    }
}