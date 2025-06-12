#region

using Batter.Utils.Builders;
using Batter.Utils.Builders.Predicates;

#endregion

namespace Batter.ItemValuation.Builders;

/// <summary>
/// Fluent builder for creating evaluated items.
/// </summary>
public class ItemBuilder :
    IBuilder<EvaluatedItem, ItemBuilder>,
    ISelfConditionalBuilder<IPredicate, ItemBuilder> {
    private readonly IItemObject _itemObject;
    private readonly List<IEvaluatedProperty> _properties = new();
    private Func<bool> _currentCondition = Condition.Default;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemBuilder"/> class.
    /// </summary>
    /// <param name="itemObject">The item object to evaluate.</param>
    public ItemBuilder(IItemObject itemObject) => this._itemObject = itemObject;

    /// <summary>
    /// Returns a reference to this builder.
    /// </summary>
    /// <returns>This builder instance.</returns>
    public ItemBuilder Self() {
        return this;
    }

    /// <summary>
    /// Builds and returns the evaluated item.
    /// </summary>
    /// <returns>The built evaluated item.</returns>
    public EvaluatedItem Build() {
        // Create a copy of the item object to use with the ref parameter
        var itemObject = this._itemObject;
        var item = new EvaluatedItem(ref itemObject);

        foreach (var property in this._properties) item.AddProp(property);

        return item;
    }

    /// <summary>
    /// Sets a condition for subsequent operations when a predicate evaluates to true.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate.</param>
    /// <returns>This builder instance for chaining.</returns>
    public ItemBuilder When(IPredicate predicate) {
        this._currentCondition = () => predicate.Evaluate();
        return this;
    }

    /// <summary>
    /// Sets a condition for subsequent operations when a boolean condition is true.
    /// </summary>
    /// <param name="condition">The boolean condition to check.</param>
    /// <returns>This builder instance for chaining.</returns>
    public ItemBuilder If(bool condition) {
        this._currentCondition = () => condition;
        return this;
    }

    /// <summary>
    /// Sets a condition for subsequent operations when a parameterless expression evaluates to true.
    /// </summary>
    /// <param name="expression">A parameterless expression that returns a boolean value.</param>
    /// <returns>This builder instance for chaining.</returns>
    public ItemBuilder When(Func<bool> expression) {
        this._currentCondition = expression;
        return this;
    }

    /// <summary>
    /// Adds a property to the item.
    /// </summary>
    /// <param name="property">The property to add.</param>
    /// <returns>This builder instance.</returns>
    public ItemBuilder WithProperty(IEvaluatedProperty property) {
        // Check if there's a current condition that needs to be evaluated
        if (this._currentCondition != null) {
            if (this._currentCondition()) this._properties.Add(property);
            this._currentCondition = Condition.Default;
            return this;
        }

        this._properties.Add(property);
        return this;
    }

    /// <summary>
    /// Adds a property created using the PropertyBuilder.
    /// </summary>
    /// <param name="configurator">Action to configure the property.</param>
    /// <returns>This builder instance.</returns>
    public ItemBuilder WithProperty(Action<PropertyBuilder> configurator) {
        // Check if there's a current condition that needs to be evaluated
        if (this._currentCondition != null) {
            if (this._currentCondition()) {
                var propertyName = $"Property_{this._properties.Count + 1}";
                var builder = new PropertyBuilder(propertyName);
                configurator(builder);
                this._properties.Add(builder.Build());
            }

            this._currentCondition = Condition.Default;
            return this;
        }

        var pName = $"Property_{this._properties.Count + 1}";
        var pBuilder = new PropertyBuilder(pName);
        configurator(pBuilder);
        return this.WithProperty(pBuilder.Build());
    }

    /// <summary>
    /// Adds a property with the specified name created using the PropertyBuilder.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="configurator">Action to configure the property.</param>
    /// <returns>This builder instance.</returns>
    public ItemBuilder WithProperty(string name, Action<PropertyBuilder> configurator) {
        // Check if there's a current condition that needs to be evaluated
        if (this._currentCondition != null) {
            if (this._currentCondition()) {
                var builder = new PropertyBuilder(name);
                configurator(builder);
                this._properties.Add(builder.Build());
            }

            this._currentCondition = Condition.Default;
            return this;
        }

        var pBuilder = new PropertyBuilder(name);
        configurator(pBuilder);
        return this.WithProperty(pBuilder.Build());
    }

    /// <summary>
    /// Adds a simple value property with the specified name and value.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>This builder instance.</returns>
    public ItemBuilder WithValue(string name, float value) {
        return this.WithProperty(name, b => b.WithBaseValue(value));
    }

    /// <summary>
    /// Applies a transformation when the condition evaluates to true for the given context.
    /// </summary>
    /// <typeparam name="T">The type of the context to evaluate.</typeparam>
    /// <param name="context">The context to evaluate.</param>
    /// <param name="condition">The condition to check against the context.</param>
    /// <returns>This builder instance.</returns>
    public ItemBuilder When<T>(T context, Func<T, bool> condition) {
        var predicate = Predicates.When(context).Matches(condition);
        return this.When(predicate);
    }
}