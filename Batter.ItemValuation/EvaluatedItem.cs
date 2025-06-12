#region

using System.Collections.Concurrent;
using Batter.ItemValuation.TotalEvaluationMode;

#endregion

namespace Batter.ItemValuation;

/// <summary>
/// Represents an item with evaluated properties that determine its value.
/// </summary>
/// <remarks>
/// This class manages a collection of evaluated properties for an item,
/// providing methods to add, remove, and evaluate properties to calculate the item's total value.
/// </remarks>
/// <example>
/// <code>
/// // Create an item evaluation from a game item object
/// var itemObject = new IItemObject("empire_sword_1_t2");
/// var evaluatedItem = new EvaluatedItem(ref IItemObject);
/// 
/// // Add properties to the item
/// evaluatedItem.AddProp(new WeightProperty("Weight", 1.5f));
/// evaluatedItem.AddProp(new DamageProperty("Damage", 25f));
/// 
/// // Calculate the total value using the Sum evaluation mode
/// float totalValue = evaluatedItem.EvaluateTotal&lt;Sum&gt;();
/// Console.WriteLine($"Total value: {totalValue}");
/// </code>
/// </example>
public class EvaluatedItem {
    private readonly ConcurrentDictionary<string, IEvaluatedProperty> _propsByName;

    /// <summary>
    /// Initializes a new instance of the <see cref="EvaluatedItem"/> class.
    /// </summary>
    /// <param name="itemObject">The game item object to evaluate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="itemObject"/> is null.</exception>
    public EvaluatedItem(ref IItemObject itemObject) {
        this.ItemObject = itemObject;
        this.Id = itemObject.StringId;
        this._propsByName = new();
        this.EvaluatedProperties = new();
    }

    /// <summary>
    /// Gets the game item object that this evaluation is based on.
    /// </summary>
    public IItemObject ItemObject { get; }

    /// <summary>
    /// Gets the unique identifier of the item.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the collection of evaluated properties for this item.
    /// </summary>
    internal ConcurrentQueue<IEvaluatedProperty> EvaluatedProperties { get; }

    /// <summary>
    /// Adds a property to the item evaluation.
    /// </summary>
    /// <param name="prop">The property to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="prop"/> is null.</exception>
    /// <example>
    /// <code>
    /// evaluatedItem.AddProp(new WeightProperty("Weight", 1.5f));
    /// </code>
    /// </example>
    public void AddProp(IEvaluatedProperty prop) {
        if (prop is null) throw new ArgumentNullException(nameof(prop));

        this.EvaluatedProperties.Enqueue(prop);
        this._propsByName[prop.Name] = prop; // Add to dictionary for faster lookup
    }

    /// <summary>
    /// Attempts to get a property by name.
    /// </summary>
    /// <param name="name">The name of the property to retrieve.</param>
    /// <param name="prop">When this method returns, contains the property if found; otherwise, null.</param>
    /// <returns>True if the property was found; otherwise, false.</returns>
    /// <example>
    /// <code>
    /// if (evaluatedItem.TryGetProp("Weight", out var weightProp))
    /// {
    ///     Console.WriteLine($"Weight: {weightProp.Evaluate()}");
    /// }
    /// </code>
    /// </example>
    public bool TryGetProp(string name, out IEvaluatedProperty? prop) {
        // Use dictionary for O(1) lookup instead of O(n) search
        return this._propsByName.TryGetValue(name, out prop);
    }

    /// <summary>
    /// Evaluates all properties and returns their values.
    /// </summary>
    /// <returns>A collection of evaluated property values.</returns>
    /// <example>
    /// <code>
    /// var values = evaluatedItem.Evaluate();
    /// Console.WriteLine($"Property values: {string.Join(", ", values)}");
    /// </code>
    /// </example>
    public IEnumerable<float> Evaluate() {
        return this.EvaluatedProperties.Select(prop => prop.Evaluate());
    }

    /// <summary>
    /// Evaluates a specific property by name.
    /// </summary>
    /// <param name="name">The name of the property to evaluate.</param>
    /// <returns>The evaluated value of the property.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null or empty.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the property with the specified name is not found.</exception>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     float damage = evaluatedItem.Evaluate("Damage");
    ///     Console.WriteLine($"Damage value: {damage}");
    /// }
    /// catch (KeyNotFoundException)
    /// {
    ///     Console.WriteLine("Damage property not found");
    /// }
    /// </code>
    /// </example>
    public float Evaluate(string name) {
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

        if (!this.TryGetProp(name, out var prop))
            throw new KeyNotFoundException($"Property with name '{name}' not found.");

        return prop?.Evaluate() ?? throw new KeyNotFoundException($"Property with name '{name}' not found.");
    }

    /// <summary>
    /// Evaluates the total value using the specified evaluation mode type.
    /// </summary>
    /// <typeparam name="TMode">The type of evaluation mode to use.</typeparam>
    /// <returns>The total evaluated value.</returns>
    /// <example>
    /// <code>
    /// float sumValue = evaluatedItem.EvaluateTotal&lt;Sum&gt;();
    /// float weightedValue = evaluatedItem.EvaluateTotal&lt;WeightedAverage&gt;();
    /// Console.WriteLine($"Sum: {sumValue}, Weighted: {weightedValue}");
    /// </code>
    /// </example>
    public float EvaluateTotal<TMode>() where TMode : ITotalEvaluationMode, new() {
        var mode = new TMode();
        return mode.EvaluateTotal(this.EvaluatedProperties);
    }

    /// <summary>
    /// Evaluates the total value using the provided evaluation mode instance.
    /// </summary>
    /// <typeparam name="TMode">The type of evaluation mode to use.</typeparam>
    /// <param name="mode">The evaluation mode instance to use.</param>
    /// <returns>The total evaluated value.</returns>
    /// <example>
    /// <code>
    /// var customMode = new WeightedAverage { Weight = 1.5f };
    /// float value = evaluatedItem.EvaluateTotal(customMode);
    /// Console.WriteLine($"Custom weighted value: {value}");
    /// </code>
    /// </example>
    public float EvaluateTotal<TMode>(TMode mode) where TMode : ITotalEvaluationMode {
        return mode.EvaluateTotal(this.EvaluatedProperties);
    }

    /// <summary>
    /// Evaluates the total value using a custom evaluation function.
    /// </summary>
    /// <param name="fn">The function to apply to each property for evaluation.</param>
    /// <returns>The sum of the function applied to all properties.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="fn"/> is null.</exception>
    /// <example>
    /// <code>
    /// float customTotal = evaluatedItem.EvaluateTotal(prop => prop.BaseValue * 2);
    /// Console.WriteLine($"Custom total: {customTotal}");
    /// </code>
    /// </example>
    public float EvaluateTotal(Func<IEvaluatedProperty, float> fn) {
        return fn is null
            ? throw new ArgumentNullException(nameof(fn))
            : this.EvaluatedProperties.Sum(fn);
    }

    /// <summary>
    /// Removes a property from the item evaluation.
    /// </summary>
    /// <param name="prop">The property to remove.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="prop"/> is null.</exception>
    /// <example>
    /// <code>
    /// if (evaluatedItem.TryGetProp("Weight", out var weightProp))
    /// {
    ///     evaluatedItem.RemoveProp(weightProp);
    ///     Console.WriteLine("Weight property removed");
    /// }
    /// </code>
    /// </example>
    public void RemoveProp(IEvaluatedProperty prop) {
        if (prop is null) throw new ArgumentNullException(nameof(prop));

        // Remove from dictionary for fast lookup
        this._propsByName.TryRemove(prop.Name, out var _);

        // Remove from queue
        // This still requires rebuilding the queue, but it's unavoidable with ConcurrentQueue
        var tempArray = this.EvaluatedProperties.Where(p => !p.Equals(prop)).ToArray();
        this.EvaluatedProperties.Clear();

        foreach (var item in tempArray) this.EvaluatedProperties.Enqueue(item);
    }

    /// <summary>
    /// Clears all properties from the item evaluation.
    /// </summary>
    /// <example>
    /// <code>
    /// evaluatedItem.ClearProps();
    /// Console.WriteLine("All properties cleared");
    /// </code>
    /// </example>
    public void ClearProps() {
        this.EvaluatedProperties.Clear();
        this._propsByName.Clear();
    }

    /// <summary>
    /// Gets all properties in the item evaluation.
    /// </summary>
    /// <returns>A collection of all evaluated properties.</returns>
    /// <example>
    /// <code>
    /// var properties = evaluatedItem.GetProps();
    /// foreach (var prop in properties)
    /// {
    ///     Console.WriteLine($"{prop.Name}: {prop.Evaluate()}");
    /// }
    /// </code>
    /// </example>
    public IEnumerable<IEvaluatedProperty> GetProps() {
        return this.EvaluatedProperties.ToArray();
    }
}