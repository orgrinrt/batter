#region

using Batter.ItemValuation.Calculation;

#endregion

namespace Batter.ItemValuation.Modifiers;

public interface IModifier {
    ref IModifierTarget Target { get; }
    float Apply(float current, ref float result);
}

public interface ICalculatedModifier : IModifier, ICalculationTarget { }