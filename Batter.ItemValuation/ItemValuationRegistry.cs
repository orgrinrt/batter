using System.Collections.Concurrent;
using Batter.ItemValuation.TotalEvaluationMode;
using TaleWorlds.Core;

namespace Batter.ItemValuation;

public class ItemValuationRegistry {
    private static readonly Lazy<ItemValuationRegistry> _instance = new(() => new());

    private readonly Lazy<ConcurrentDictionary<string, EvaluatedItem>> __valuations = new(() => new());
    private readonly Lazy<ConcurrentDictionary<string, int>> __valueCache = new(() => new());

    public static ItemValuationRegistry Instance => ItemValuationRegistry._instance.Value;

    public bool TryGetValue(string itemId, out int value) {
        return this.TryGetValue(itemId, out value, new Sum());
    }

    public bool TryGetValue<TTotalEvalMode>(string itemId, out int value)
        where TTotalEvalMode : ITotalEvaluationMode, new() {
        return this.TryGetValue(itemId, out value, new TTotalEvalMode());
    }

    public bool TryGetValue<TTotalEvalMode>(string itemId, out int value, TTotalEvalMode mode)
        where TTotalEvalMode : ITotalEvaluationMode, new() {
        // Initialize the out parameter with a default value
        value = 0;

        if (string.IsNullOrEmpty(itemId))
            throw new ArgumentException("Item ID cannot be null or empty", nameof(itemId));

        // Check cache first
        if (this.__valueCache.Value.TryGetValue(itemId, out var cachedValue)) {
            value = cachedValue;
            return true;
        }

        // If not in cache, check valuations
        if (this.TryGetValuation(itemId, out var valuation)) {
            // If valuation exists, calculate the value
            value = (int)valuation.EvaluateTotal(mode);
            // Store the calculated value in the cache
            this.__valueCache.Value[itemId] = value;
            return true;
        }

        // Item not found
        return false;
    }

    public bool TryGetValuation(string itemId, out EvaluatedItem? valuation) {
        // Initialize the out parameter
        valuation = null;

        if (string.IsNullOrEmpty(itemId))
            throw new ArgumentException("Item ID cannot be null or empty", nameof(itemId));

        // Check if the valuation already exists in the cache
        if (this.__valuations.Value.TryGetValue(itemId, out var existingValuation)) {
            valuation = existingValuation;
            return true;
        }

        // Try to create a new valuation
        try {
            var itemObject = new ItemObject(itemId);
            valuation = new(ref itemObject);
            this.__valuations.Value[itemId] = valuation;
            return true;
        }
        catch {
            // If item creation fails for any reason
            return false;
        }
    }

    // Should only call once on startup, or in other specific special circumstances
    // this is expensive, and should not be called frequently
    public void ValidateCache() {
        // Check if cache is valid
        var isCacheValid = this.CheckCacheValidity();

        // If not valid, clear the value cache
        if (!isCacheValid) this.__valueCache.Value.Clear();
    }

    private bool CheckCacheValidity() {
        // Implement logic to check if cache is valid
        // For example, check if game version has changed, or if item definitions have updated
        // For now, return true as a placeholder
        return true;
    }
}