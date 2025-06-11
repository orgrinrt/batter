#region

using System.Collections.Concurrent;
using Batter.ItemValuation.TotalEvaluationMode;
using TaleWorlds.Core;

#endregion

namespace Batter.ItemValuation;

public class ItemValuationRegistry {
    private readonly Lazy<ConcurrentDictionary<string, EvaluatedItem>> _valuations = new(() => new());
    private readonly Lazy<ConcurrentDictionary<string, int>> _valueCache = new(() => new());
    private string _lastGameVersion = string.Empty; // Track game version for cache invalidation


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

        // Construct a cache key that includes the evaluation mode
        var cacheKey = $"{itemId}:{mode.GetType().Name}";

        // Check cache first
        if (this._valueCache.Value.TryGetValue(cacheKey, out var cachedValue)) {
            value = cachedValue;
            return true;
        }

        // If not in cache, check valuations
        if (!this.TryGetValuation(itemId, out var valuation)) return false;
        // If valuation exists, calculate the value
        if (valuation != null) value = (int)valuation.EvaluateTotal(mode);
        // Store the calculated value in the cache
        this._valueCache.Value[cacheKey] = value;
        return true;

        // Item not found
    }

    public bool TryGetValuation(string itemId, out EvaluatedItem? valuation) {
        // Initialize the out parameter
        valuation = null;

        if (string.IsNullOrEmpty(itemId))
            throw new ArgumentException("Item ID cannot be null or empty", nameof(itemId));

        // Check if the valuation already exists in the cache
        if (this._valuations.Value.TryGetValue(itemId, out var existingValuation)) {
            valuation = existingValuation;
            return true;
        }

        // Try to create a new valuation
        try {
            var itemObject = new ItemObject(itemId);
            valuation = new(ref itemObject);
            this._valuations.Value[itemId] = valuation;
            return true;
        }
        catch (Exception ex) when (
            ex is ArgumentException ||  // Invalid itemId
            ex is KeyNotFoundException ||  // Item not found
            ex is InvalidOperationException)  // Item creation failed
        {
            // Log specific exceptions for better diagnostics
            // Could add logging here in a production environment
            return false;
        }
        catch (Exception) {
            // Catch all other exceptions as a last resort
            // In production, would log this unexpected error
            return false;
        }
    }

    // Should only call once on startup, or in other specific special circumstances
    // this is expensive, and should not be called frequently
    public void ValidateCache() {
        // Check if cache is valid using game version
        var isCacheValid = this.CheckCacheValidity();

        // If not valid, clear the value cache
        if (!isCacheValid) this._valueCache.Value.Clear();
    }

    private bool CheckCacheValidity() {
        // Get current game version - replace this with actual version retrieval
        var currentGameVersion = this.GetCurrentGameVersion();

        // Compare with last known version
        var isValid = this._lastGameVersion == currentGameVersion;

        // Update the stored version
        this._lastGameVersion = currentGameVersion;

        return isValid;
    }

    private string GetCurrentGameVersion() {
        // Implementation would depend on how to retrieve the game version
        // This is a placeholder - replace with actual implementation
        try {
            // Example: Could get this from a game API, assembly version, etc.
            return "1.0.0"; // Placeholder
        }
        catch {
            // If version retrieval fails, return a default to force cache refresh
            return Guid.NewGuid().ToString();
        }
    }
}