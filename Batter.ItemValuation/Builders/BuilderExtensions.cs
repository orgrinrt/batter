#region

using Batter.Utils.Builders.Predicates;

#endregion

namespace Batter.ItemValuation.Builders;

/// <summary>
/// Extension methods for the item valuation builder API.
/// </summary>
public static class BuilderExtensions {
    /// <summary>
    /// Creates a new item builder for the specified item object.
    /// </summary>
    /// <param name="itemObject">The item object to evaluate.</param>
    /// <returns>A new item builder.</returns>
    public static ItemBuilder Evaluate(this IItemObject itemObject) {
        return new(itemObject);
    }

    /// <summary>
    /// Creates a new modifier builder.
    /// </summary>
    /// <returns>A new modifier builder.</returns>
    public static ModifierBuilder CreateModifier() {
        return new();
    }

    /// <summary>
    /// Creates a new property builder with the specified name.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="baseValue">The base value of the property.</param>
    /// <returns>A new property builder.</returns>
    public static PropertyBuilder CreateProperty(string name, float baseValue = 0f) {
        return new(name, baseValue);
    }

    /// <summary>
    /// Creates a new calculation builder with the specified name.
    /// </summary>
    /// <param name="name">The name of the calculation.</param>
    /// <returns>A new calculation builder.</returns>
    public static CalculationBuilder CreateCalculation(string name) {
        return new(name);
    }

    /// <summary>
    /// Creates a predicate from a function that evaluates to a boolean.
    /// </summary>
    /// <param name="registry">self</param>
    /// <param name="func">The function to evaluate.</param>
    /// <returns>A new predicate.</returns>
    public static IPredicate When(this ItemValuationRegistry registry, Func<bool> func) {
        // Using the new PredicateResult directly instead of the old PredicateExtensions
        return new PredicateResult(func());
    }

    /// <summary>
    /// Creates a predicate from a boolean value.
    /// </summary>
    /// <param name="registry">self</param>
    /// <param name="value">The boolean value.</param>
    /// <returns>A new predicate.</returns>
    public static IPredicate When(this ItemValuationRegistry registry, bool value) {
        // Using the new PredicateResult directly
        return new PredicateResult(value);
    }

    /// <summary>
    /// Creates a predicate that evaluates a function against a context.
    /// </summary>
    /// <typeparam name="T">The type of the context.</typeparam>
    /// <param name="registry">self</param>
    /// <param name="context">The context to evaluate against.</param>
    /// <param name="func">The function to evaluate.</param>
    /// <returns>A new predicate result.</returns>
    public static PredicateResult When<T>(this ItemValuationRegistry registry, T context, Func<T, bool> func) {
        // Using the new fluent API pattern
        return Predicates.When(context).Matches(func);
    }
}