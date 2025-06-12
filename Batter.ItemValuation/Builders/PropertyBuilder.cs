#region

using Batter.ItemValuation.Modifiers;
using Batter.Utils.Builders;
using Batter.Utils.Builders.Predicates;

#endregion

namespace Batter.ItemValuation.Builders;

/// <summary>
/// Fluent builder for creating evaluated properties.
/// </summary>
public class PropertyBuilder :
    IBuilder<IEvaluatedProperty, PropertyBuilder>,
    IConditionalBuilderFactory<IPredicate, PropertyBuilder, ConditionalPropertyBuilder> {
    internal readonly List<IModifier> _MODIFIERS = new();
    private readonly string _name;
    private float _baseValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyBuilder"/> class.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="baseValue">The base value of the property.</param>
    public PropertyBuilder(string name, float baseValue = 0f) {
        this._name = name;
        this._baseValue = baseValue;
    }

    /// <summary>
    /// Returns a reference to this builder.
    /// </summary>
    /// <returns>This builder instance.</returns>
    public PropertyBuilder Self() {
        return this;
    }

    /// <summary>
    /// Builds and returns the evaluated property.
    /// </summary>
    /// <returns>The built evaluated property.</returns>
    public IEvaluatedProperty Build() {
        var property = new SimpleEvaluatedProperty(this._name, this._baseValue);

        foreach (var modifier in this._MODIFIERS) property.AddModifier(modifier);

        return property;
    }

    /// <summary>
    /// Creates a conditional builder for applying modifiers when a predicate evaluates to true.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate.</param>
    /// <returns>A conditional builder that applies modifiers only when the predicate is true.</returns>
    public ConditionalPropertyBuilder When(IPredicate predicate) {
        return new(this, predicate);
    }

    /// <summary>
    /// Creates a conditional builder for applying modifiers when a boolean condition is true.
    /// A more concise alternative to When(bool) for direct boolean expressions.
    /// </summary>
    /// <param name="condition">The boolean condition to check.</param>
    /// <returns>A conditional builder that applies modifiers only when the condition is true.</returns>
    public ConditionalPropertyBuilder If(bool condition) {
        return this.When(condition.AsPredicate());
    }

    /// <summary>
    /// Creates a conditional builder for applying modifiers when a parameterless expression evaluates to true.
    /// This is a more semantically accurate alternative to When(() => expression).
    /// </summary>
    /// <param name="expression">A parameterless expression that returns a boolean value.</param>
    /// <returns>A conditional builder that applies modifiers only when the expression evaluates to true.</returns>
    public ConditionalPropertyBuilder If(Func<bool> expression) {
        return this.When(new FuncPredicate(expression));
    }

    /// <summary>
    /// Sets the base value of the property.
    /// </summary>
    /// <param name="value">The base value to set.</param>
    /// <returns>This builder instance.</returns>
    public PropertyBuilder WithBaseValue(float value) {
        this._baseValue = value;
        return this;
    }

    /// <summary>
    /// Adds a modifier to the property.
    /// </summary>
    /// <param name="modifier">The modifier to add.</param>
    /// <returns>This builder instance.</returns>
    public PropertyBuilder WithModifier(IModifier modifier) {
        this._MODIFIERS.Add(modifier);
        return this;
    }

    /// <summary>
    /// Adds a modifier created using the ModifierBuilder.
    /// </summary>
    /// <param name="configurator">Action to configure the modifier.</param>
    /// <returns>This builder instance.</returns>
    public PropertyBuilder WithModifier(Action<ModifierBuilder> configurator) {
        var builder = new ModifierBuilder();
        configurator(builder);

        // Create the property if needed, to use as target
        var property = new PropertyTarget(this);

        // Use the new BuildWithTarget method to directly create a modifier with target
        var modifier = builder.BuildWithTarget(property);
        return this.WithModifier(modifier);
    }

    /// <summary>
    /// Creates a conditional builder for applying modifiers when a boolean condition is true.
    /// </summary>
    /// <param name="condition">The boolean condition to check.</param>
    /// <returns>A conditional builder that applies modifiers only when the condition is true.</returns>
    public ConditionalPropertyBuilder When(bool condition) {
        return this.When(condition.AsPredicate());
    }

    /// <summary>
    /// Creates a conditional builder for applying modifiers when a function returns true.
    /// </summary>
    /// <param name="condition">Function that returns a boolean condition.</param>
    /// <returns>A conditional builder that applies modifiers only when the function returns true.</returns>
    public ConditionalPropertyBuilder When(Func<bool> condition) {
        return this.When(new FuncPredicate(condition));
    }


    /// <summary>
    /// An adapter class that allows a PropertyBuilder to be used as an IModifierTarget.
    /// </summary>
    internal class PropertyTarget : IModifierTarget {
        private readonly PropertyBuilder _builder;

        public PropertyTarget(PropertyBuilder builder) => this._builder = builder;

        // The Name property is used internally by the modifier system
        public string Name => this._builder._name;

        // Implement the Modifiers property required by IModifierTarget
        IEnumerable<IModifier> IModifierTarget.Modifiers => this._builder._MODIFIERS;
    }

    /// <summary>
    /// A simple implementation of IEvaluatedProperty.
    /// </summary>
    private class SimpleEvaluatedProperty : EvaluatedPropertyBase<List<IModifier>> {
        public SimpleEvaluatedProperty(string name, float baseValue)
            : base(name, baseValue) { }
    }
}