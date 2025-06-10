namespace Batter.ItemValuation.Calculation;

public interface ICalculationTarget {
    internal IEnumerable<ICalculation> Calculations { get; }
}