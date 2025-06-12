#region

using Batter.ItemValuation.Calculation;

#endregion

namespace Batter.ItemValuation.Modifiers;

/// <summary>
/// Represents a modifier that can be applied to a value.
/// </summary>
public interface IModifier {
    /// <summary>
    /// Gets a reference to the target of this modifier.
    /// </summary>
    ref IModifierTarget Target { get; }

    /// <summary>
    /// Applies the modifier to the given value.
    /// </summary>
    /// <param name="current">The current value before applying the modifier.</param>
    /// <param name="result">A reference to the result value being built up.</param>
    /// <returns>The value after applying the modifier.</returns>
    float Apply(float current, ref float result);
}

/// <summary>
/// Represents a modifier that can be calculated using a series of calculations.
/// </summary>
public interface ICalculatedModifier : IModifier, ICalculationTarget { }