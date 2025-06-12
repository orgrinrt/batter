#region

using System.Reflection;
using Batter.Core.Utils;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

#endregion

namespace Batter.Core.Patches;

[HarmonyPatch]
public static class TownMarketDataGetPricePatch {
    private static MethodBase TargetMethod() {
        return AccessTools.Method(
            typeof(TownMarketData),
            "GetPrice",
            new[] {
                typeof(EquipmentElement),
                typeof(MobileParty),
                typeof(Boolean),
                typeof(PartyBase),
            }
        );
    }

    // 1) Prefix: if the roster element itself is null, bail out immediately.
    private static Boolean Prefix(EquipmentElement itemRosterElement, MobileParty tradingParty, Boolean isSelling,
        PartyBase merchantParty, ref Int32 __result) {
        if ( /*itemRosterElement.IsEmpty
        || */itemRosterElement.IsInvalid()
             || itemRosterElement.Equals(default(EquipmentElement))
             || (itemRosterElement.Item == null && itemRosterElement.CosmeticItem == null) // no IItemObject
            /*|| itemRosterElement.Item.WeaponComponent == null */ // example of deeper null
           ) {
            BatterLog.Error("[SafePrice] Skipping GetPrice for truly invalid element — dumping props:");
            TownMarketDataGetPricePatch.DumpEquipmentElement(itemRosterElement);
            __result = 0;
            return false;
        }

        return true;
    }

    // 2) Finalizer: catch any NullReferenceException thrown by the original,
    //    then try to set IsMerchantItem = false, and return a safe price of 0.
    private static void Finalizer(ref Int32 __result, Exception __exception) {
        if (__exception == null) return;
        if (__exception is NullReferenceException)
            // return a fallback price of 0 and swallow the exception
            try {
                __result = 0;
                return;
            }
            catch {
                BatterLog.Warn($"[TownMarketDataGetPricePatch] failed to set __reult: \n {__exception} ");
                return;
            }

        // any other exception bubbles up normally
        throw __exception;
    }

    /// <summary>
    ///     Reflectively logs all public properties and their values (or errors) on this element.
    /// </summary>
    private static void DumpEquipmentElement(EquipmentElement item) {
        var t = typeof(EquipmentElement);

        // 1) public properties
        foreach (var prop in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            try {
                var val = prop.GetIndexParameters().Length == 0
                    ? prop.GetValue(item, null)
                    : "[indexed]";
                BatterLog.Error($"  Property {prop.Name}: {val}");
            }
            catch (Exception ex) {
                BatterLog.Error($"  Property {prop.Name} threw {ex.GetType().Name}: {ex.Message}");
            }

        // 2) public fields (if you want them too)
        foreach (var field in t.GetFields(BindingFlags.Instance | BindingFlags.Public))
            try {
                var val = field.GetValue(item);
                BatterLog.Error($"  Field    {field.Name}: {val}");
            }
            catch (Exception ex) {
                BatterLog.Error($"  Field    {field.Name} threw {ex.GetType().Name}: {ex.Message}");
            }
    }
}