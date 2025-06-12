namespace Batter.ItemValuation.Calculation;

/// <summary>
/// Represents an object that can be the target of calculations.
/// </summary>
public interface ICalculationTarget {
    /// <summary>
    /// Gets the collection of calculations associated with this target.
    /// </summary>
    internal IEnumerable<ICalculation> Calculations { get; }
}