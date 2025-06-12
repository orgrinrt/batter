#region

using Batter.ItemValuation.Modifiers;
using Batter.Utils.Builders;
using Batter.Utils.Builders.Predicates;

#endregion

namespace Batter.ItemValuation.Builders;

/// <summary>
/// Fluent builder for creating modifiers.
/// </summary>
public class ModifierBuilder :
    IBuilder<ModifierBuilder.ModifierTemplate, ModifierBuilder>,
    IConditionalBuilderFactory<IPredicate, ModifierBuilder, ConditionalModifierBuilder> {
    internal Func<float, float, float> _modifierFunc;
    private IModifierTarget? _target;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModifierBuilder"/> class.
    /// </summary>
    public ModifierBuilder() {
        this._modifierFunc = (current, _) => current;
    }

    /// <summary>
    /// Returns a reference to this builder.
    /// </summary>
    /// <returns>This builder instance.</returns>
    public ModifierBuilder Self() {
        return this;
    }

    /// <summary>
    /// Builds and returns a modifier template that can be converted to a modifier when a target is provided.
    /// </summary>
    /// <returns>A modifier template.</returns>
    public ModifierTemplate Build() {
        return new(this._modifierFunc, this._target);
    }

    /// <summary>
    /// Creates a conditional builder for applying modifiers when a predicate evaluates to true.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate.</param>
    /// <returns>A conditional builder that applies modifiers only when the predicate is true.</returns>
    public ConditionalModifierBuilder When(IPredicate predicate) {
        return new(this, predicate);
    }

    /// <summary>
    /// Creates a conditional builder for applying modifiers when a boolean condition is true.
    /// A more concise alternative to When(bool) for direct boolean expressions.
    /// </summary>
    /// <param name="condition">The boolean condition to check.</param>
    /// <returns>A conditional builder that applies modifiers only when the condition is true.</returns>
    public ConditionalModifierBuilder If(bool condition) {
        return this.When(condition.AsPredicate());
    }

    /// <summary>
    /// Creates a conditional builder for applying modifiers when a parameterless expression evaluates to true.
    /// This is a more semantically accurate alternative to When(() => expression).
    /// </summary>
    /// <param name="expression">A parameterless expression that returns a boolean value.</param>
    /// <returns>A conditional builder that applies modifiers only when the expression evaluates to true.</returns>
    public ConditionalModifierBuilder If(Func<bool> expression) {
        return this.When(new FuncPredicate(expression));
    }

    /// <summary>
    /// Builds and returns a modifier with the specified target.
    /// </summary>
    /// <param name="target">The target for the modifier.</param>
    /// <returns>A modifier with the specified target.</returns>
    public IModifier BuildWithTarget(IModifierTarget target) {
        return new FunctionalModifier(target, this._modifierFunc);
    }

    /// <summary>
    /// Sets the target of the modifier.
    /// </summary>
    /// <param name="target">The target to apply the modifier to.</param>
    /// <returns>This builder instance.</returns>
    public ModifierBuilder ForTarget(IModifierTarget target) {
        this._target = target;
        return this;
    }

    /// <summary>
    /// Adds a custom modifier function.
    /// </summary>
    /// <param name="modifier">A function that takes the current value and returns the modified value.</param>
    /// <returns>This builder instance.</returns>
    public ModifierBuilder Custom(Func<float, float> modifier) {
        var currentFunc = this._modifierFunc;
        this._modifierFunc = (current, result) => modifier(currentFunc(current, result));
        return this;
    }

    /// <summary>
    /// Adds a more complex custom modifier function that can use both the current value and the result reference.
    /// </summary>
    /// <param name="modifier">A function that takes the current value and result reference and returns the modified value.</param>
    /// <returns>This builder instance.</returns>
    public ModifierBuilder CustomAdvanced(Func<float, float, float> modifier) {
        var currentFunc = this._modifierFunc;
        this._modifierFunc = (current, result) => modifier(currentFunc(current, result), result);
        return this;
    }


    /// <summary>
    /// A template for creating modifiers that can have their target set later.
    /// </summary>
    public class ModifierTemplate : ITargetable<IModifier, IModifierTarget> {
        private readonly Func<float, float, float> _modifierFunc;
        private readonly IModifierTarget? _target;

        internal ModifierTemplate(Func<float, float, float> modifierFunc, IModifierTarget? target) {
            this._modifierFunc = modifierFunc;
            this._target = target;
        }

        /// <summary>
        /// Creates a modifier with the specified target.
        /// </summary>
        /// <param name="target">The target for the modifier.</param>
        /// <returns>A modifier with the specified target.</returns>
        public IModifier WithTarget(IModifierTarget target) {
            return new FunctionalModifier(target, this._modifierFunc);
        }

        /// <summary>
        /// Creates a modifier with the target that was set during building, if any.
        /// </summary>
        /// <returns>A modifier with the target that was set during building.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no target was set during building.</exception>
        public IModifier Build() {
            if (this._target == null)
                throw new InvalidOperationException(
                    "No target was set for this modifier. Use WithTarget to specify a target.");
            return new FunctionalModifier(this._target, this._modifierFunc);
        }
    }

    /// <summary>
    /// A simple implementation of IModifier using a function.
    /// </summary>
    private class FunctionalModifier : IModifier {
        private readonly Func<float, float, float> _modifierFunc;
        private IModifierTarget _target;

        public FunctionalModifier(IModifierTarget target, Func<float, float, float> modifierFunc) {
            this._target = target ?? throw new ArgumentNullException(nameof(target), "Modifier target cannot be null");
            this._modifierFunc = modifierFunc;
        }

        public ref IModifierTarget Target => ref this._target;

        public float Apply(float current, ref float result) {
            return this._modifierFunc(current, result);
        }
    }
}