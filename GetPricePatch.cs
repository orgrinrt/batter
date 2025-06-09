using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;
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

    [HarmonyPatch]
    public static class TownMarketDataGetPricePatch
    {

        static MethodBase TargetMethod()
        {
            return AccessTools.Method(
                typeof(TownMarketData),
                                      "GetPrice",
                                      new Type[] {
                                          typeof(EquipmentElement),
                                      typeof(MobileParty),
                                      typeof(bool),
                                      typeof(PartyBase)
                                      }
            );
        }

        // 1) Prefix: if the roster element itself is null, bail out immediately.
        static bool Prefix(EquipmentElement itemRosterElement, MobileParty tradingParty, Boolean isSelling, PartyBase merchantParty, ref int __result)
        {
            if (  /*itemRosterElement.IsEmpty
            || */itemRosterElement.IsInvalid()
            || itemRosterElement.Equals(default(EquipmentElement))
            || (itemRosterElement.Item == null && itemRosterElement.CosmeticItem == null)            // no ItemObject
            /*|| itemRosterElement.Item.WeaponComponent == null */// example of deeper null
        ) {
                SafeLog.Error("[SafePrice] Skipping GetPrice for truly invalid element — dumping props:");
                DumpEquipmentElement(itemRosterElement);
                __result = 0;
            return false;
        }
        return true;
        }

        // 2) Finalizer: catch any NullReferenceException thrown by the original,
        //    then try to set IsMerchantItem = false, and return a safe price of 0.
        static void Finalizer(ref int __result, Exception __exception)
        {
            if (__exception == null) return;
            if (__exception is NullReferenceException)
            {
                // return a fallback price of 0 and swallow the exception
                try {
                    __result = 0;
                    return;
                }
                catch
                {
                    SafeLog.Warn($"[TownMarketDataGetPricePatch] failed to set __reult: \n {__exception} ");
                    return;
                }

            }

            // any other exception bubbles up normally
            throw __exception;
        }

        /// <summary>
        /// Reflectively logs all public properties and their values (or errors) on this element.
        /// </summary>
        static void DumpEquipmentElement(EquipmentElement item)
        {
            var t = typeof(EquipmentElement);

            // 1) public properties
            foreach (var prop in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                try
                {
                    var val = prop.GetIndexParameters().Length == 0
                    ? prop.GetValue(item, null)
                    : "[indexed]";
                    SafeLog.Error($"  Property {prop.Name}: {val}");
                }
                catch (Exception ex)
                {
                    SafeLog.Error($"  Property {prop.Name} threw {ex.GetType().Name}: {ex.Message}");
                }
            }

            // 2) public fields (if you want them too)
            foreach (var field in t.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                try
                {
                    var val = field.GetValue(item);
                    SafeLog.Error($"  Field    {field.Name}: {val}");
                }
                catch (Exception ex)
                {
                    SafeLog.Error($"  Field    {field.Name} threw {ex.GetType().Name}: {ex.Message}");
                }
            }
        }
    }

    public static class ItemRosterSafeGetters
    {
        /// <summary>
        /// Installs safe guards on every ItemRoster getter/indexer.
        /// Each patch is invoked via the provided patchRunner so you get
        /// uniform logging and error‐handling.
        /// </summary>
        public static void PatchAll(
            Harmony     harmony,
            Action<string, Action> patchRunner)
        {
            // all the int properties
            foreach (var prop in new[]
            {
                "Count", "VersionNo", "TotalFood", "FoodVariety",
                "TotalValue", "TradeGoodsTotalValue",
                "NumberOfPackAnimals", "NumberOfLivestockAnimals", "NumberOfMounts"
            })
            {
                patchRunner(
                    $"ItemRoster.{prop}",
                    () => PatchProperty<int>(harmony, prop)
                );
            }

            // float TotalWeight
            patchRunner(
                "ItemRoster.TotalWeight",
                () => PatchProperty<float>(harmony, "TotalWeight")
            );

            // indexer this[int]
            patchRunner(
                "ItemRoster.get_Item(int)",
                        () => PatchIndexer<ItemRosterElement>(harmony)
            );
        }

        static void PatchProperty<T>(Harmony harmony, string propName)
        {
            var getter      = AccessTools.PropertyGetter(typeof(ItemRoster), propName);
            var patchMethod = typeof(GenericFinalizerPatch<>)
            .MakeGenericType(typeof(T))
            .GetMethod(nameof(GenericFinalizerPatch<T>.Finalizer),
                       BindingFlags.Public | BindingFlags.Static);

            // prefix = null, postfix = null, transpiler = null, finalizer = patchMethod
            harmony.Patch(
                getter,
                prefix:    null,
                postfix:   null,
                transpiler:null,
                finalizer: new HarmonyMethod(patchMethod)
            );
        }

        static void PatchIndexer<T>(Harmony harmony)
        {
            var getter      = AccessTools.DeclaredMethod(typeof(ItemRoster), "get_Item", new[] { typeof(int) });
            var patchMethod = typeof(GenericFinalizerPatch<>)
            .MakeGenericType(typeof(T))
            .GetMethod(nameof(GenericFinalizerPatch<T>.Finalizer),
                       BindingFlags.Public | BindingFlags.Static);

            harmony.Patch(
                getter,
                prefix:    null,
                postfix:   null,
                transpiler:null,
                finalizer: new HarmonyMethod(patchMethod)
            );
        }

        /// <summary>
        /// Catches any exception from the original getter, logs it,
        /// defaults the result, and swallows the exception.
        /// </summary>
        public static class GenericFinalizerPatch<T>
        {
            public static void Finalizer(Exception __exception)
            {
                if (__exception != null)
                {

                    try{
                        // figure out which getter blew up
                        var frame      = new System.Diagnostics.StackTrace().GetFrame(1);
                        var methodName = frame?.GetMethod()?.Name ?? "unknown";
                        SafeLog.Error($"[ItemRosterSafeGetters] {methodName} threw {__exception.GetType().Name}: {__exception.Message}");
                        //__result = default;
                    }
                    catch (Exception ex)
                    {
                        SafeLog.Error($"[ItemRosterSafeGetters] failed to default result: {ex}");
                    }
                    // swallow
                }
            }
        }


    }

}
