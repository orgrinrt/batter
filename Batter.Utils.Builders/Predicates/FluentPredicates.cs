namespace Batter.Utils.Builders.Predicates;

/// <summary>
/// Provides a natural, readable way to build and combine predicates.
/// </summary>
/// <remarks>
/// This class is the main entry point for the fluent predicate API.
/// It allows for building complex conditions in a way that reads almost like natural language.
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
public static class Predicates {
    /// <summary>
    /// Creates a predicate context for the specified object.
    /// </summary>
    /// <param name="context">The object to evaluate predicates against.</param>
    /// <returns>A predicate context that can be used to build conditions.</returns>
    public static PredicateContext<T> When<T>(T context) {
        return new(context);
    }

    /// <summary>
    /// Creates a predicate that always evaluates to true.
    /// </summary>
    /// <returns>A predicate that always evaluates to true.</returns>
    public static PredicateResult Always() {
        return new(true);
    }

    /// <summary>
    /// Creates a predicate that always evaluates to false.
    /// </summary>
    /// <returns>A predicate that always evaluates to false.</returns>
    public static PredicateResult Never() {
        return new(false);
    }

    /// <summary>
    /// Creates a predicate from a boolean value.
    /// </summary>
    /// <param name="value">The boolean value.</param>
    /// <returns>A predicate that evaluates to the given boolean value.</returns>
    public static PredicateResult FromBool(bool value) {
        return new(value);
    }

    /// <summary>
    /// Creates a predicate from a function that returns a boolean value.
    /// </summary>
    /// <param name="func">The function that returns a boolean value.</param>
    /// <returns>A predicate that evaluates to the result of the function.</returns>
    public static PredicateResult FromFunc(Func<bool> func) {
        return new(func());
    }
}

/// <summary>
/// Represents a context for building predicates against a specific object.
/// </summary>
/// <typeparam name="T">The type of object to evaluate predicates against.</typeparam>
public class PredicateContext<T> {
    private readonly T _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="PredicateContext{T}"/> class.
    /// </summary>
    /// <param name="context">The object to evaluate predicates against.</param>
    public PredicateContext(T context) => this._context = context;

    /// <summary>
    /// Creates a predicate that evaluates a custom condition against the context.
    /// </summary>
    /// <param name="predicate">The function that defines the condition.</param>
    /// <returns>A predicate result that can be combined with other predicates.</returns>
    public PredicateResult Matches(Func<T, bool> predicate) {
        return new(predicate(this._context));
    }
}

/// <summary>
/// Represents the result of a predicate evaluation, which can be combined with other predicates.
/// </summary>
public class PredicateResult : IPredicate {
    private readonly bool _result;

    /// <summary>
    /// Initializes a new instance of the <see cref="PredicateResult"/> class.
    /// </summary>
    /// <param name="result">The result of the predicate evaluation.</param>
    public PredicateResult(bool result) => this._result = result;

    /// <summary>
    /// Evaluates the current predicate result.
    /// </summary>
    /// <returns>The boolean result of the predicate evaluation.</returns>
    public bool Evaluate() {
        return this._result;
    }

    /// <summary>
    /// Combines this predicate result with another using a logical AND.
    /// </summary>
    /// <returns>A predicate combiner that can be used to specify the next predicate.</returns>
    public PredicateCombiner AndAlso() {
        return new(this, CombineOperator.And);
    }

    /// <summary>
    /// Combines this predicate result with another using a logical OR.
    /// </summary>
    /// <returns>A predicate combiner that can be used to specify the next predicate.</returns>
    public PredicateCombiner OrElse() {
        return new(this, CombineOperator.Or);
    }

    /// <summary>
    /// Negates this predicate result.
    /// </summary>
    /// <returns>A predicate result that is the negation of the current result.</returns>
    public PredicateResult Not() {
        return new(!this._result);
    }

    /// <summary>
    /// Implicitly converts a predicate result to a boolean.
    /// </summary>
    /// <param name="result">The predicate result to convert.</param>
    public static implicit operator bool(PredicateResult result) {
        return result._result;
    }
}

/// <summary>
/// Represents a combiner that connects two predicates with a logical operator.
/// </summary>
public class PredicateCombiner {
    private readonly PredicateResult _left;
    private readonly CombineOperator _operator;

    /// <summary>
    /// Initializes a new instance of the <see cref="PredicateCombiner"/> class.
    /// </summary>
    /// <param name="left">The left side of the predicate combination.</param>
    /// <param name="operator">The operator to use for combining predicates.</param>
    public PredicateCombiner(PredicateResult left, CombineOperator @operator) {
        this._left = left;
        this._operator = @operator;
    }

    /// <summary>
    /// Combines the result of this predicate with the result of another predicate.
    /// </summary>
    /// <param name="right">The right side of the predicate combination.</param>
    /// <returns>A new predicate result representing the combined result.</returns>
    public PredicateResult Apply(PredicateResult right) {
        return this._operator switch {
            CombineOperator.And => new(this._left && right),
            CombineOperator.Or => new(this._left || right),
            var _ => throw new ArgumentOutOfRangeException(),
        };
    }

    /// <summary>
    /// Specifies the context for the next predicate in the chain.
    /// </summary>
    /// <param name="context">The object to evaluate the next predicate against.</param>
    /// <returns>A predicate context that can be used to build the next condition.</returns>
    /// <remarks>
    /// This method enables the fluent syntax:
    /// <code>
    /// Predicates.When(item).Matches(i =&gt; i.Value &gt; 100).AndAlso().When(item).Matches(i =&gt; i.Weight &lt; 5.0f)
    /// </code>
    /// </remarks>
    public PredicateChainContext<T> When<T>(T context) {
        return new(this, context);
    }
}

/// <summary>
/// A special context for chaining predicates in a fluent syntax.
/// </summary>
/// <typeparam name="T">The type of object to evaluate predicates against.</typeparam>
public class PredicateChainContext<T> {
    private readonly PredicateCombiner _combiner;
    private readonly T _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="PredicateChainContext{T}"/> class.
    /// </summary>
    /// <param name="combiner">The combiner that connects this predicate with the previous one.</param>
    /// <param name="context">The object to evaluate predicates against.</param>
    public PredicateChainContext(PredicateCombiner combiner, T context) {
        this._combiner = combiner;
        this._context = context;
    }

    /// <summary>
    /// Creates a predicate that evaluates a custom condition against the context.
    /// </summary>
    /// <param name="predicate">The function that defines the condition.</param>
    /// <returns>A predicate result that represents the combined result of all predicates so far.</returns>
    public PredicateResult Matches(Func<T, bool> predicate) {
        return this._combiner.Apply(new(predicate(this._context)));
    }
}

/// <summary>
/// Specifies the operator to use when combining predicates.
/// </summary>
public enum CombineOperator {
    /// <summary>
    /// Combines predicates with a logical AND operation.
    /// </summary>
    And,

    /// <summary>
    /// Combines predicates with a logical OR operation.
    /// </summary>
    Or,
}