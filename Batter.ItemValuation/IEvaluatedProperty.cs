using Batter.ItemValuation.Modifiers;

namespace Batter.ItemValuation;

public interface IEvaluatedProperty : IModifierTarget {
    string Name { get; }
    float BaseValue { get; }

    float Evaluate() {
        var result = this.BaseValue;
        foreach (var modifier in this.Modifiers) {
            // Apply modifiers to result
        }

        return result;
    }
}