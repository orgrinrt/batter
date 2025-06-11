using System.Collections.Concurrent;

namespace Batter.ItemValuation.TotalEvaluationMode;

/// <summary>
/// Provides a total evaluation mode that calculates the sum of all property values.
/// </summary>
/// <remarks>
/// This is the simplest evaluation mode, adding together all property values without any weighting or other transformations.
/// </remarks>
/// <example>
/// <code>
/// // Create a collection of properties
/// var properties = new ConcurrentQueue&lt;IEvaluatedProperty&gt;();
/// properties.Enqueue(new DamageProperty("Damage", 25.0f));
/// properties.Enqueue(new WeightProperty("Weight", 1.5f));
/// 
/// // Sum all property values
/// var sumMode = new Sum();
/// float totalValue = sumMode.EvaluateTotal(properties); // Returns 26.5f
/// </code>
/// </example>
public class Sum : ITotalEvaluationMode {
    /// <summary>
    /// Evaluates the total value by summing all property values.
    /// </summary>
    /// <param name="itemProps">The collection of evaluated properties to sum.</param>
    /// <param name="_default">The default value to return if the collection is empty.</param>
    /// <returns>The sum of all evaluated property values, or the default value if the collection is empty.</returns>
    /// <example>
    /// <code>
    /// var sumMode = new Sum();
    /// float totalValue = sumMode.EvaluateTotal(itemProperties);
    /// Console.WriteLine($"Total sum: {totalValue}");
    /// </code>
    /// </example>
    public float EvaluateTotal(ConcurrentQueue<IEvaluatedProperty> itemProps, float _default = 0f) {
        if (itemProps is null || itemProps.IsEmpty) return _default;

        return itemProps.Sum(prop => prop.Evaluate());
    }
}