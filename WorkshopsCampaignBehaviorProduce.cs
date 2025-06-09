using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;                // for EquipmentElement
using TaleWorlds.CampaignSystem.Settlements; // for Workshop
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
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.LogEntries;
using System.Collections;
using System.Collections.Generic;

namespace SafeWarLogPatch
{
    [HarmonyPatch]
    public static class WorkshopsCampaignBehaviorProduce
    {
        static MethodBase TargetMethod() => AccessTools.Method(
            typeof(WorkshopsCampaignBehavior),
                                                               "ProduceAnOutputToTown",
                                                               new Type[] {
                                                                   typeof(EquipmentElement),
                                                               typeof(Workshop),
                                                               typeof(bool)
                                                               });

        static bool Prefix(EquipmentElement outputItem)
        {
            if (  /*itemRosterElement.IsEmpty
                || */outputItem.IsInvalid()
                || outputItem.Equals(default(EquipmentElement))
                || (outputItem.Item == null && outputItem.CosmeticItem == null)           // no ItemObject
                /*|| itemRosterElement.Item.WeaponComponent == null */// example of deeper null
            ) {
                SafeLog.Error($"[WorkshopsCampaignBehaviorProduce] Skipping ProduceAnOutputToTown for invalid element: {outputItem}");
                return false;
            }
            return true;
        }

        static Exception Finalizer(Exception __exception)
        {
            if (__exception != null)
            {
                SafeLog.Error($"[WorkshopsCampaignBehaviorProduce] ProduceAnOutputToTown threw {__exception.GetType().Name}: {__exception.Message}");
                return null;
            }
            return __exception;
        }
        }
}
