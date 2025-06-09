using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using System.Linq;
using System;
using System.IO;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.LogEntries;
using System.Reflection;
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

    /// <summary>
    /// Bulk‐loads every <ComputedPrice> tag from merged item XML on campaign load.
    /// Implements itself as a singleton so we can pass a non‐null owner to the event.
    /// </summary>
    public class ComputedPriceRegistry : CampaignBehaviorBase
    {
        private const string TagName = "ComputedPrice";
        private readonly Dictionary<string, int> _map = new();
        private bool _initialized;

        // The singleton instance
        private static ComputedPriceRegistry _instance;
        public static ComputedPriceRegistry Instance
        => _instance ??= new ComputedPriceRegistry();

        // Private ctor: enforce singleton
        private ComputedPriceRegistry() { }

        public override void RegisterEvents()
        {
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, OnGameLoaded);
        }

        public override void SyncData(IDataStore dataStore)
        {
            // No persistent data beyond the in‐memory _alreadySanitized set.
        }

        private void OnGameLoaded(CampaignGameStarter starter)
        {
            try
            {
                var allItems = MBObjectManager.Instance.GetObjectTypeList<ItemObject>();
                foreach (var item in allItems)
                {
                    if (string.IsNullOrEmpty(item.StringId))
                        continue;

                    try
                    {
                        var doc = MBObjectManager.GetMergedXmlForManaged(
                            item.StringId,
                            skipValidation: true,
                            ignoreGameTypeInclusionCheck: true,
                            gameType: "ItemObject"
                        );
                        var nodes = doc?.GetElementsByTagName(TagName);
                        if (nodes?.Count > 0 && int.TryParse(nodes[0].InnerText.Trim(), out var price))
                        {
                            _map[item.StringId] = price;
                        }
                    }
                    catch
                    {
                        // ignore per‐item errors
                    }
                }

                InformationManager.DisplayMessage(
                    new InformationMessage($"[ComputedPriceRegistry] Loaded {_map.Count} computed prices")
                );
            }
            catch (Exception ex)
            {
                InformationManager.DisplayMessage(
                    new InformationMessage($"[ComputedPriceRegistry] OnGameLoaded error: {ex.Message}")
                );
            }
        }

        /// <summary>Get the computed price, or 0 if none defined.</summary>
        public int GetPrice(string itemStringId)
        => _map.TryGetValue(itemStringId, out var v) ? v : 0;

        /// <summary>Try get the computed price; returns false if none defined.</summary>
        public bool TryGetPrice(string itemStringId, out int price)
        => _map.TryGetValue(itemStringId, out price);
    }
}
