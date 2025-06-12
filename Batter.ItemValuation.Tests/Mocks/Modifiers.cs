#region

using Batter.ItemValuation.Modifiers;

#endregion

namespace Batter.ItemValuation.Tests.Mocks;

/// <summary>
/// A simple additive modifier that adds a flat value to the base.
/// </summary>
public class AdditiveModifier : IModifier {
    private readonly float _value;
    private IModifierTarget? _target;

    public AdditiveModifier(float value) => this._value = value;

    public ref IModifierTarget Target {
        get {
            if (this._target == null)
                throw new InvalidOperationException("Target has not been set");
            return ref this._target!;
        }
    }

    public float Apply(float current, ref float result) {
        return current + this._value;
    }
}

/// <summary>
/// A simple multiplicative modifier that multiplies the current value by a factor.
/// </summary>
public class MultiplicativeModifier : IModifier {
    private readonly float _factor;
    private IModifierTarget? _target;

    public MultiplicativeModifier(float factor) => this._factor = factor;

    public ref IModifierTarget Target {
        get {
            if (this._target == null)
                throw new InvalidOperationException("Target has not been set");
            return ref this._target!;
        }
    }

    public float Apply(float current, ref float result) {
        return current * this._factor;
    }
}