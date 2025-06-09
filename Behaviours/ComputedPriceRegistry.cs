using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace Batter.Core.Behaviours;

/// <summary>
///     Bulk‐loads every
///     <ComputedPrice>
///         tag from merged item XML on campaign load.
///         Implements itself as a singleton so we can pass a non‐null owner to the event.
/// </summary>
public class ComputedPriceRegistry : CampaignBehaviorBase {
    private const String TagName = "ComputedPrice";

    // The singleton instance
    private static ComputedPriceRegistry _instance;
    private readonly Dictionary<String, Int32> _map = new();
    private Boolean _initialized;

    // Private ctor: enforce singleton
    private ComputedPriceRegistry() { }

    public static ComputedPriceRegistry Instance
        => ComputedPriceRegistry._instance ??= new();

    public override void RegisterEvents() {
        CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, this.OnGameLoaded);
    }

    public override void SyncData(IDataStore dataStore) {
        // No persistent data beyond the in‐memory _alreadySanitized set.
    }

    private void OnGameLoaded(CampaignGameStarter starter) {
        try {
            var allItems = MBObjectManager.Instance.GetObjectTypeList<ItemObject>();
            foreach (var item in allItems) {
                if (String.IsNullOrEmpty(item.StringId))
                    continue;

                try {
                    var doc = MBObjectManager.GetMergedXmlForManaged(
                        item.StringId,
                        true,
                        true,
                        "ItemObject"
                    );
                    var nodes = doc?.GetElementsByTagName(ComputedPriceRegistry.TagName);
                    if (nodes?.Count > 0 && Int32.TryParse(nodes[0].InnerText.Trim(), out var price))
                        this._map[item.StringId] = price;
                }
                catch {
                    // ignore per‐item errors
                }
            }

            InformationManager.DisplayMessage(
                new($"[ComputedPriceRegistry] Loaded {this._map.Count} computed prices")
            );
        }
        catch (Exception ex) {
            InformationManager.DisplayMessage(
                new($"[ComputedPriceRegistry] OnGameLoaded error: {ex.Message}")
            );
        }
    }

    /// <summary>Get the computed price, or 0 if none defined.</summary>
    public Int32 GetPrice(String itemStringId) {
        return this._map.TryGetValue(itemStringId, out var v) ? v : 0;
    }

    /// <summary>Try get the computed price; returns false if none defined.</summary>
    public Boolean TryGetPrice(String itemStringId, out Int32 price) {
        return this._map.TryGetValue(itemStringId, out price);
    }
}