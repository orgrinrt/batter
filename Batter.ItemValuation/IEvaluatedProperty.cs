#region

using Batter.ItemValuation.Modifiers;

#endregion

namespace Batter.ItemValuation;

/// <summary>
///     Represents a property that can be evaluated with modifiers applied.
/// </summary>
public interface IEvaluatedProperty : IModifierTarget {
    /// <summary>
    ///     Gets the name of the property.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets the base value of the property before modifiers.
    /// </summary>
    float BaseValue { get; }

    /// <summary>
    ///     Evaluates the property value with all modifiers applied.
    /// </summary>
    /// <returns>The final evaluated value.</returns>
    float Evaluate();
}