#region

using System.Collections.Concurrent;

#endregion

namespace Batter.ItemValuation.TotalEvaluationMode;

/// <summary>
/// Defines an interface for total evaluation modes that calculate a final value from a collection of properties.
/// </summary>
/// <remarks>
/// Implementations of this interface provide different strategies for combining multiple evaluated property values
/// into a single total value, such as summation, averaging, or weighted calculations.
/// </remarks>
/// <example>
/// <code>
/// // Custom implementation of the ITotalEvaluationMode interface
/// public class WeightedAverage : ITotalEvaluationMode
/// {
///     public float Weight { get; set; } = 1.0f;
///     
///     public float EvaluateTotal(ConcurrentQueue&lt;IEvaluatedProperty&gt; itemValuations, float _default = 0f)
///     {
///         if (itemValuations == null || !itemValuations.Any())
///             return _default;
///             
///         float total = 0f;
///         int count = 0;
///         
///         foreach (var property in itemValuations)
///         {
///             total += property.Evaluate() * Weight;
///             count++;
///         }
///         
///         return count > 0 ? total / count : _default;
///     }
/// }
/// </code>
/// </example>
public interface ITotalEvaluationMode {
    /// <summary>
    /// Evaluates the total value from a collection of evaluated properties.
    /// </summary>
    /// <param name="itemValuations">The collection of evaluated properties to combine.</param>
    /// <param name="_default">The default value to return if the collection is empty.</param>
    /// <returns>The total evaluated value.</returns>
    /// <example>
    /// <code>
    /// var properties = new ConcurrentQueue&lt;IEvaluatedProperty&gt;();
    /// properties.Enqueue(new DamageProperty("Damage", 25.0f));
    /// properties.Enqueue(new WeightProperty("Weight", 1.5f));
    /// 
    /// var sumMode = new Sum();
    /// float totalValue = sumMode.EvaluateTotal(properties); // Returns 26.5f
    /// </code>
    /// </example>
    float EvaluateTotal(ConcurrentQueue<IEvaluatedProperty> itemValuations, float _default = 0f);
}