#region

using System;
using System.Reflection;
using Batter.Core.Utils;
using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;

#endregion

// for EquipmentElement
// for Workshop

namespace Batter.Core.Patches;

[HarmonyPatch]
public static class WorkshopsCampaignBehaviorProduceAnOutputToTownPatch {
    private static MethodBase TargetMethod() {
        return AccessTools.Method(
            typeof(WorkshopsCampaignBehavior),
            "ProduceAnOutputToTown",
            new[] {
                typeof(EquipmentElement),
                typeof(Workshop),
                typeof(bool),
            });
    }

    private static bool Prefix(EquipmentElement outputItem) {
        if ( /*itemRosterElement.IsEmpty
            || */outputItem.IsInvalid()
                 || outputItem.Equals(default(EquipmentElement))
                 || (outputItem.Item == null && outputItem.CosmeticItem == null) // no IItemObject
            /*|| itemRosterElement.Item.WeaponComponent == null */ // example of deeper null
           ) {
            BatterLog.Error(
                $"[WorkshopsCampaignBehaviorProduce] Skipping ProduceAnOutputToTown for invalid element: {outputItem}");
            return false;
        }

        return true;
    }

    private static Exception Finalizer(Exception __exception) {
        if (__exception != null) {
            BatterLog.Error(
                $"[WorkshopsCampaignBehaviorProduce] ProduceAnOutputToTown threw {__exception.GetType().Name}: {__exception.Message}");
            return null;
        }

        return __exception;
    }
}