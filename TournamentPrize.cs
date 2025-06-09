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
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.CampaignSystem.LogEntries;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.Core;
using System;
using System.IO;
using System.Xml.Serialization;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;
using System.Collections;
using System.Collections.Generic;
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

[HarmonyPatch(typeof(FightTournamentGame), "CachePossibleEliteRewardItems")]
class TournamentPrizeCrash
{

    static void Prefix()
    {
        var items = MBObjectManager.Instance.GetObjectTypeList<ItemObject>();
        foreach (var item in items)
        {
            if (item == null)
                SafeLog.Error("[Tournament] Null ItemObject found!");
            else if (item.Name == null || item.StringId == null)
                SafeLog.Error($"[Tournament] Incomplete item: {item?.ToString() ?? "null"}");
        }
    }

    static void Postfix(FightTournamentGame __instance)
    {
        // find the first private instance field of type List<ItemObject>
        var listField = typeof(FightTournamentGame)
        .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
        .FirstOrDefault(f => f.FieldType == typeof(List<ItemObject>));

        if (listField is null)
        {
            // if you want, log a warning once
            return;
        }

        var prizeList = (List<ItemObject>)listField.GetValue(__instance);
        if (prizeList == null)
            return;

        // Remove null entries so the comparer never sees them
        prizeList.RemoveAll(i => i == null);
    }
}
}
