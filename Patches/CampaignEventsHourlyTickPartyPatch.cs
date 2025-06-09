using HarmonyLib;
using SafeWarLogPatch.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;

namespace SafeWarLogPatch.Patches;

[HarmonyPatch(typeof(CampaignEvents), "HourlyTickParty")]
public static class CampaignEventsHourlyTickPartyPatch
{
    private static bool Prefix(MobileParty mobileParty)
    {
        try
        {
            // 1) If mobileParty or settlement is null, skip original.
            if (mobileParty == null)
            {
                BatterLog.Warn("[SafeVillagerHourlyTickPatchLate] mobileParty or settlement is null.");
                return false;
            }

            // 2) If this party is NOT a villager, skip original.
            if (!mobileParty.IsVillager)
                return true;

            // 3) If they do not have a home‐village, skip original.
            var home = mobileParty.HomeSettlement;
            if (home == null || home.Village == null)
                BatterLog.Warn(
                    "[SafeVillagerHourlyTickPatchLate] HomeSettlement or Village is null for mobileParty. Attempting to proceed (if crashes, revert return to false here)");
            // mobileParty.RemoveParty();
            // return false;
            // 4) If that village’s TradeBound is null, skip original.
            if (home.Village.TradeBound == null)
                BatterLog.Warn(
                    "[SafeVillagerHourlyTickPatchLate] Village's TradeBound is null. Attempting to proceed (if crashes, revert return to false here)");
            // mobileParty.RemoveParty();
            // return false;
            // Check for MobileParty's attachment
            if (mobileParty?.AttachedTo == null)
                BatterLog.Info(
                    "[SafeVillagerHourlyTickPatchLate] MobileParty is not attached to anything. Continuing...");

            // Check for missing HomeSettlement or Village
            if (mobileParty?.HomeSettlement?.Village == null)
                BatterLog.Warning(
                    "[SafeVillagerHourlyTickPatchLate] MobileParty's home settlement or village is missing. Attempting to proceed (if crashes, revert return to false here)");
            // mobileParty.RemoveParty();
            // return false;  // Prevent further execution
            try
            {
                return true; // Run original if no issues
            }
            catch (KeyNotFoundException knf)
            {
                BatterLog.Warn($"[SafeVillagerHourlyTickPatch] KeyNotFoundException for {mobileParty?.Name}: {knf}");

                if (mobileParty != null)
                {
                    BatterLog.Warn($"[SafeVillagerHourlyTickPatch] Destroying broken party: {mobileParty.Name}");
                    mobileParty.RemoveParty();
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
            BatterLog.Warn($"[SafeVillagerHourlyTickPatchLate] KeyNotFoundException for {mobileParty?.Name}: {knf}");

            if (mobileParty != null)
            {
                BatterLog.Warn($"[SafeVillagerHourlyTickPatchLate] Destroying broken party: {mobileParty.Name}");
                mobileParty.RemoveParty();
            }

            return false; // Skip original
        }
        catch (Exception ex)
        {
            BatterLog.Error($"[SafeVillagerHourlyTickPatchLate] Unexpected error: {ex}");
            return false;
        }
    }

    private static void Finalizer(MobileParty mobileParty, Exception __exception)
    {
        if (__exception != null)
        {
            BatterLog.Warn($"[SafeVillagerHourlyTickPatchLate] Exception: {__exception}");

            if (mobileParty != null)
            {
                BatterLog.Warn($"Destroying broken party: {mobileParty.Name}");
                mobileParty.RemoveParty();
            }
        }
    }
}