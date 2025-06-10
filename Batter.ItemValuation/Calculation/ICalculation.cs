namespace Batter.ItemValuation.Calculation;

public interface ICalculation {
    internal ref ICalculationTarget Target { get; }
}