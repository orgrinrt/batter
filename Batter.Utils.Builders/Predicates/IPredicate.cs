namespace Batter.Utils.Builders.Predicates;

/// <summary>
///     Represents a predicate that can evaluate to a boolean result.
/// </summary>
/// <typeparam name="T">The type of object the predicate evaluates.</typeparam>
public interface IPredicate<in T> {
    /// <summary>
    ///     Evaluates the predicate against the specified value.
    /// </summary>
    /// <param name="value">The value to evaluate.</param>
    /// <returns>True if the predicate is satisfied; otherwise, false.</returns>
    bool Evaluate(T value);
}

/// <summary>
///     Non-generic predicate for simple boolean evaluation.
/// </summary>
public interface IPredicate : IPredicate<object> {
    /// <summary>
    ///     Evaluates the predicate without requiring input.
    /// </summary>
    /// <returns>True if the predicate is satisfied; otherwise, false.</returns>
    bool Evaluate();
}