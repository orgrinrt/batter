using Batter.ItemValuation.Calculation;

namespace Batter.ItemValuation.Modifiers;

public interface IModifier {
    ref IModifierTarget Target { get; }
}

public interface ICalculatedModifier : IModifier, ICalculationTarget { }