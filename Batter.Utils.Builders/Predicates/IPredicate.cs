namespace Batter.Utils.Builders.Predicates;

/// <summary>
/// Represents a predicate that can evaluate to a boolean result.
/// </summary>
/// <remarks>
/// This interface is the foundation for the predicate system.
/// For most common usage scenarios, consider using the fluent API
/// available through the <see cref="Predicates"/> class.
/// 
/// Example:
/// <code>
/// bool result = Predicates
///     .When(item).Matches(i =&gt; i.Value &gt; 100)
///     .AndAlso()
///     .When(item).Matches(i =&gt; i.Weight &lt; 5.0f)
///     .Evaluate();
/// </code>
/// </remarks>
public interface IPredicate {
    /// <summary>
    /// Evaluates the predicate.
    /// </summary>
    /// <returns>True if the predicate is satisfied; otherwise, false.</returns>
    bool Evaluate();
}