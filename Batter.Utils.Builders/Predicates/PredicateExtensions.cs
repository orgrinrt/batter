namespace Batter.Utils.Builders.Predicates;

/// <summary>
/// Provides extension methods for working with predicates.
/// </summary>
/// <remarks>
/// These extension methods provide a convenient way to create and combine predicates.
/// For more complex scenarios, consider using the fluent API (Predicates.When()...).
/// </remarks>
public static class PredicateExtensions {
    /// <summary>
    /// Creates a predicate from a boolean value.
    /// </summary>
    /// <param name="value">The value to create a predicate for.</param>
    /// <returns>A predicate that evaluates to the specified value.</returns>
    public static IPredicate AsPredicate(this bool value) {
        return new ConstantPredicate(value);
    }

    /// <summary>
    /// Creates a predicate from a function.
    /// </summary>
    /// <param name="func">The function to evaluate.</param>
    /// <returns>A predicate that evaluates the function.</returns>
    public static IPredicate AsPredicate(this Func<bool> func) {
        return new FuncPredicate(func);
    }

    /// <summary>
    /// Creates a predicate from a function that requires no context.
    /// </summary>
    /// <param name="func">The function to evaluate.</param>
    /// <returns>A predicate based on the function.</returns>
    public static IPredicate From(Func<bool> func) {
        return new FuncPredicate(func);
    }

    /// <summary>
    /// Combines two predicates with a logical AND.
    /// </summary>
    /// <param name="left">The left predicate.</param>
    /// <param name="right">The right predicate.</param>
    /// <returns>A predicate that evaluates to true only if both predicates evaluate to true.</returns>
    public static IPredicate And(this IPredicate left, IPredicate right) {
        return new AndPredicate(left, right);
    }

    /// <summary>
    /// Combines two predicates with a logical OR.
    /// </summary>
    /// <param name="left">The left predicate.</param>
    /// <param name="right">The right predicate.</param>
    /// <returns>A predicate that evaluates to true if either predicate evaluates to true.</returns>
    public static IPredicate Or(this IPredicate left, IPredicate right) {
        return new OrPredicate(left, right);
    }

    /// <summary>
    /// Negates a predicate.
    /// </summary>
    /// <param name="predicate">The predicate to negate.</param>
    /// <returns>A predicate that evaluates to the opposite of the original predicate.</returns>
    public static IPredicate Not(this IPredicate predicate) {
        return new NotPredicate(predicate);
    }
}