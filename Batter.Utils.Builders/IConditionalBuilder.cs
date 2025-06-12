namespace Batter.Utils.Builders;

/// <summary>
/// Interface for conditional builder that applies operations only when a condition is met.
/// </summary>
/// <typeparam name="TBuilder">The parent builder type.</typeparam>
/// <typeparam name="TConditionalBuilder">The conditional builder type.</typeparam>
public interface IConditionalBuilder<out TBuilder, out TConditionalBuilder>
    where TBuilder : IBuilder<object, TBuilder>
    where TConditionalBuilder : IConditionalBuilder<TBuilder, TConditionalBuilder> {
    /// <summary>
    /// Returns the parent builder to continue the chain.
    /// </summary>
    /// <returns>The parent builder.</returns>
    TBuilder EndCondition();
}

/// <summary>
/// Interface for objects that can be built with a target.
/// </summary>
/// <typeparam name="TResult">The result type.</typeparam>
/// <typeparam name="TTarget">The target type.</typeparam>
public interface ITargetable<out TResult, in TTarget> {
    /// <summary>
    /// Builds the result with the specified target.
    /// </summary>
    /// <param name="target">The target to use.</param>
    /// <returns>The built result.</returns>
    TResult WithTarget(TTarget target);
}

/// <summary>
/// Interface for a builder that can create conditional builders.
/// </summary>
/// <typeparam name="TPredicate">The predicate type.</typeparam>
/// <typeparam name="TBuilder">The builder type.</typeparam>
/// <typeparam name="TConditionalBuilder">The conditional builder type.</typeparam>
public interface IConditionalBuilderFactory<in TPredicate, out TBuilder, out TConditionalBuilder>
    where TBuilder : IBuilder<object, TBuilder>
    where TConditionalBuilder : IConditionalBuilder<TBuilder, TConditionalBuilder> {
    /// <summary>
    /// Creates a conditional builder for applying operations when a predicate evaluates to true.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate.</param>
    /// <returns>A conditional builder that applies operations only when the predicate is true.</returns>
    TConditionalBuilder When(TPredicate predicate);

    /// <summary>
    /// Creates a conditional builder for applying operations when a boolean condition is true.
    /// </summary>
    /// <param name="condition">The boolean condition to check.</param>
    /// <returns>A conditional builder that applies operations only when the condition is true.</returns>
    TConditionalBuilder If(bool condition);
}

/// <summary>
/// Interface for a self-conditional builder that can apply conditions to itself.
/// </summary>
/// <typeparam name="TPredicate">The predicate type.</typeparam>
/// <typeparam name="TBuilder">The builder type.</typeparam>
public interface ISelfConditionalBuilder<in TPredicate, out TBuilder>
    where TBuilder : IBuilder<object, TBuilder> {
    /// <summary>
    /// Sets a condition for subsequent operations.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate.</param>
    /// <returns>This builder instance for chaining.</returns>
    TBuilder When(TPredicate predicate);

    /// <summary>
    /// Sets a condition for subsequent operations.
    /// </summary>
    /// <param name="condition">The boolean condition to check.</param>
    /// <returns>This builder instance for chaining.</returns>
    TBuilder If(bool condition);

    /// <summary>
    /// Sets a condition for subsequent operations.
    /// </summary>
    /// <param name="expression">A parameterless expression that returns a boolean value.</param>
    /// <returns>This builder instance for chaining.</returns>
    TBuilder When(Func<bool> expression);
}