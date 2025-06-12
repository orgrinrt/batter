#region

using Batter.ItemValuation.Calculation;
using Batter.Utils.Builders;

#endregion

namespace Batter.ItemValuation.Builders;

/// <summary>
/// Fluent builder for creating calculations.
/// </summary>
public class CalculationBuilder : IBuilder<ICalculation, CalculationBuilder> {
    private readonly string _name;
    private Func<float> _calculationFunc;
    private ICalculationTarget? _target;

    /// <summary>
    /// Initializes a new instance of the <see cref="CalculationBuilder"/> class.
    /// </summary>
    /// <param name="name">The name of the calculation.</param>
    public CalculationBuilder(string name) {
        this._name = name;
        this._calculationFunc = () => 0f; // Default function
    }

    /// <summary>
    /// Returns a reference to this builder.
    /// </summary>
    /// <returns>This builder instance.</returns>
    public CalculationBuilder Self() {
        return this;
    }

    /// <summary>
    /// Builds and returns the calculation.
    /// </summary>
    /// <returns>The built calculation.</returns>
    public ICalculation Build() {
        var calculationTarget = this._target;
        return calculationTarget != null
            ? new FunctionalCalculation(this._name, calculationTarget, this._calculationFunc)
            : throw new InvalidOperationException("Calculation target must be set before building.");
    }

    /// <summary>
    /// Sets the target of the calculation.
    /// </summary>
    /// <param name="target">The target to apply the calculation to.</param>
    /// <returns>This builder instance.</returns>
    public CalculationBuilder ForTarget(ICalculationTarget target) {
        this._target = target;
        return this;
    }

    /// <summary>
    /// Sets the calculation function.
    /// </summary>
    /// <param name="calculationFunc">The function to calculate the value.</param>
    /// <returns>This builder instance.</returns>
    public CalculationBuilder WithFormula(Func<float> calculationFunc) {
        this._calculationFunc = calculationFunc;
        return this;
    }

    /// <summary>
    /// Sets a constant value for the calculation.
    /// </summary>
    /// <param name="value">The constant value to return.</param>
    /// <returns>This builder instance.</returns>
    public CalculationBuilder WithConstantValue(float value) {
        this._calculationFunc = () => value;
        return this;
    }

    /// <summary>
    /// Adds a simple additive calculation that adds the specified value.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>This builder instance.</returns>
    public CalculationBuilder Add(float value) {
        var current = this._calculationFunc;
        this._calculationFunc = () => current() + value;
        return this;
    }

    /// <summary>
    /// Adds a simple multiplicative calculation that multiplies by the specified factor.
    /// </summary>
    /// <param name="factor">The factor to multiply by.</param>
    /// <returns>This builder instance.</returns>
    public CalculationBuilder Multiply(float factor) {
        var current = this._calculationFunc;
        this._calculationFunc = () => current() * factor;
        return this;
    }

    /// <summary>
    /// Creates a calculation based on a minimum value.
    /// </summary>
    /// <param name="minValue">The minimum value.</param>
    /// <returns>This builder instance.</returns>
    public CalculationBuilder Min(float minValue) {
        var current = this._calculationFunc;
        this._calculationFunc = () => Math.Max(current(), minValue);
        return this;
    }

    /// <summary>
    /// Creates a calculation based on a maximum value.
    /// </summary>
    /// <param name="maxValue">The maximum value.</param>
    /// <returns>This builder instance.</returns>
    public CalculationBuilder Max(float maxValue) {
        var current = this._calculationFunc;
        this._calculationFunc = () => Math.Min(current(), maxValue);
        return this;
    }

    /// <summary>
    /// A simple implementation of ICalculation using a function.
    /// </summary>
    private class FunctionalCalculation : ICalculation {
        private readonly Func<float> _calculationFunc;
        private ICalculationTarget _target;

        public FunctionalCalculation(string name, ICalculationTarget target, Func<float> calculationFunc) {
            this.Name = name;
            this._target = target;
            this._calculationFunc = calculationFunc;
        }

        public string Name { get; }

        ref ICalculationTarget ICalculation.Target => ref this._target;

        public float Calculate() {
            return this._calculationFunc();
        }
    }
}