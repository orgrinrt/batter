using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace SafeWarLogPatch.Behaviours;

/// <summary>
///     Bulk‐loads every
///     <ComputedPrice>
///         tag from merged item XML on campaign load.
///         Implements itself as a singleton so we can pass a non‐null owner to the event.
/// </summary>
public class ComputedPriceRegistry : CampaignBehaviorBase
{
    private const string TagName = "ComputedPrice";

    // The singleton instance
    private static ComputedPriceRegistry _instance;
    private readonly Dictionary<string, int> _map = new();
    private bool _initialized;

    // Private ctor: enforce singleton
    private ComputedPriceRegistry()
    {
    }

    public static ComputedPriceRegistry Instance
        => _instance ??= new ComputedPriceRegistry();

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
                        true,
                        true,
                        "ItemObject"
                    );
                    var nodes = doc?.GetElementsByTagName(TagName);
                    if (nodes?.Count > 0 && int.TryParse(nodes[0].InnerText.Trim(), out var price))
                        _map[item.StringId] = price;
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
    {
        return _map.TryGetValue(itemStringId, out var v) ? v : 0;
    }

    /// <summary>Try get the computed price; returns false if none defined.</summary>
    public bool TryGetPrice(string itemStringId, out int price)
    {
        return _map.TryGetValue(itemStringId, out price);
    }
}