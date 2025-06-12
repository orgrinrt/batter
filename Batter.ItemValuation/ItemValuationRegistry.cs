#region

using System.Collections.Concurrent;
using Batter.ItemValuation.TotalEvaluationMode;

#endregion

namespace Batter.ItemValuation;

/// <summary>
/// Provides a registry for managing item valuations and their cached values.
/// </summary>
/// <remarks>
/// This registry maintains a collection of evaluated items and their calculated values,
/// providing efficient access and caching mechanisms to improve performance.
/// </remarks>
/// <example>
/// <code>
/// // Create an instance of the registry
/// var registry = new ItemValuationRegistry();
/// 
/// // Get an item value using the default Sum evaluation mode
/// if (registry.TryGetValue("empire_sword_1_t2", out int value))
/// {
///     Console.WriteLine($"Item value: {value}");
/// }
/// 
/// // Get an item value using a custom evaluation mode
/// if (registry.TryGetValue("empire_sword_1_t2", out int customValue, new WeightedAverage()))
/// {
///     Console.WriteLine($"Item weighted value: {customValue}");
/// }
/// </code>
/// </example>
public class ItemValuationRegistry {
    private readonly Lazy<ConcurrentDictionary<string, EvaluatedItem>> _valuations = new(() => new());
    private readonly Lazy<ConcurrentDictionary<string, int>> _valueCache = new(() => new());
    private string _lastGameVersion = string.Empty; // Track game version for cache invalidation

    /// <summary>
    /// Attempts to get the value of an item using the default Sum evaluation mode.
    /// </summary>
    /// <param name="itemId">The unique identifier for the item.</param>
    /// <param name="value">When this method returns, contains the calculated value if found; otherwise, 0.</param>
    /// <returns>True if the item was found and its value calculated; otherwise, false.</returns>
    /// <example>
    /// <code>
    /// if (registry.TryGetValue("empire_sword_1_t2", out int value))
    /// {
    ///     Console.WriteLine($"Item value: {value}");
    /// }
    /// </code>
    /// </example>
    public bool TryGetValue(string itemId, out int value) {
        return this.TryGetValue(itemId, out value, new Sum());
    }

    /// <summary>
    /// Attempts to get the value of an item using a specific evaluation mode type.
    /// </summary>
    /// <typeparam name="TTotalEvalMode">The type of evaluation mode to use.</typeparam>
    /// <param name="itemId">The unique identifier for the item.</param>
    /// <param name="value">When this method returns, contains the calculated value if found; otherwise, 0.</param>
    /// <returns>True if the item was found and its value calculated; otherwise, false.</returns>
    /// <example>
    /// <code>
    /// if (registry.TryGetValue&lt;WeightedAverage&gt;("empire_sword_1_t2", out int value))
    /// {
    ///     Console.WriteLine($"Item weighted value: {value}");
    /// }
    /// </code>
    /// </example>
    public bool TryGetValue<TTotalEvalMode>(string itemId, out int value)
        where TTotalEvalMode : ITotalEvaluationMode, new() {
        return this.TryGetValue(itemId, out value, new TTotalEvalMode());
    }

    /// <summary>
    /// Attempts to get the value of an item using a provided evaluation mode instance.
    /// </summary>
    /// <typeparam name="TTotalEvalMode">The type of evaluation mode to use.</typeparam>
    /// <param name="itemId">The unique identifier for the item.</param>
    /// <param name="value">When this method returns, contains the calculated value if found; otherwise, 0.</param>
    /// <param name="mode">The evaluation mode instance to use for calculations.</param>
    /// <returns>True if the item was found and its value calculated; otherwise, false.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="itemId"/> is null or empty.</exception>
    /// <example>
    /// <code>
    /// var customMode = new WeightedAverage { Weight = 1.5f };
    /// if (registry.TryGetValue("empire_sword_1_t2", out int value, customMode))
    /// {
    ///     Console.WriteLine($"Item weighted value: {value}");
    /// }
    /// </code>
    /// </example>
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

    /// <summary>
    /// Attempts to get an evaluated item from the registry.
    /// </summary>
    /// <param name="itemId">The unique identifier for the item.</param>
    /// <param name="valuation">When this method returns, contains the evaluated item if found; otherwise, null.</param>
    /// <returns>True if the item was found; otherwise, false.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="itemId"/> is null or empty.</exception>
    /// <example>
    /// <code>
    /// if (registry.TryGetValuation("empire_sword_1_t2", out var evaluation))
    /// {
    ///     Console.WriteLine($"Item has {evaluation.GetProps().Count()} evaluated properties");
    /// }
    /// </code>
    /// </example>
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
            // var itemObject = new TItemObject(itemId);
            // valuation = new(ref itemObject);
            // this._valuations.Value[itemId] = valuation;
            // return true;
            return false; // FIXME: a way to do the above intent in practice
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

    /// <summary>
    /// Validates the cache and clears it if necessary.
    /// </summary>
    /// <remarks>
    /// Should only be called once on startup, or in other specific special circumstances.
    /// This operation is expensive and should not be called frequently.
    /// </remarks>
    /// <example>
    /// <code>
    /// // At game startup
    /// var registry = new ItemValuationRegistry();
    /// registry.ValidateCache();
    /// </code>
    /// </example>
    public void ValidateCache() {
        // Check if cache is valid using game version
        var isCacheValid = this.CheckCacheValidity();

        // If not valid, clear the value cache
        if (!isCacheValid) this._valueCache.Value.Clear();
    }

    /// <summary>
    /// Checks if the cache is valid based on the current game version.
    /// </summary>
    /// <returns>True if the cache is valid; otherwise, false.</returns>
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