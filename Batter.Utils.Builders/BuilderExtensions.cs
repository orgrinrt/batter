#region

using Batter.Utils.Builders.Predicates;

#endregion

namespace Batter.Utils.Builders;

/// <summary>
/// Provides extension methods for builder classes.
/// </summary>
public static class BuilderExtensions {
    /// <summary>
    /// Applies a transformation when the predicate evaluates to true.
    /// </summary>
    /// <typeparam name="TResult">The type of the result built by the builder.</typeparam>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    /// <param name="builder">The builder to extend.</param>
    /// <param name="predicate">The predicate to evaluate.</param>
    /// <param name="transformation">The transformation to apply when the predicate is true.</param>
    /// <returns>The transformed builder if the predicate is true; otherwise, the original builder.</returns>
    public static TBuilder When<TResult, TBuilder>(
        this TBuilder builder,
        IPredicate predicate,
        Func<TBuilder, TBuilder> transformation)
        where TBuilder : IBuilder<TResult, TBuilder> {
        return predicate.Evaluate() ? transformation(builder) : builder.Self();
    }

    /// <summary>
    /// Applies a transformation when the specified condition is true for the given context.
    /// </summary>
    /// <typeparam name="TResult">The type of the result built by the builder.</typeparam>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    /// <typeparam name="T">The type of the context to evaluate.</typeparam>
    /// <param name="builder">The builder to extend.</param>
    /// <param name="context">The context to evaluate.</param>
    /// <param name="condition">The condition to check against the context.</param>
    /// <param name="transformation">The transformation to apply when the condition is true.</param>
    /// <returns>The transformed builder if the condition is true; otherwise, the original builder.</returns>
    public static TBuilder WhenMatches<TResult, TBuilder, T>(
        this TBuilder builder,
        T context,
        Func<T, bool> condition,
        Func<TBuilder, TBuilder> transformation)
        where TBuilder : IBuilder<TResult, TBuilder> {
        return condition(context) ? transformation(builder) : builder.Self();
    }

    /// <summary>
    /// Applies a transformation when the predicate evaluates to false.
    /// </summary>
    /// <typeparam name="TResult">The type of the result built by the builder.</typeparam>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    /// <param name="builder">The builder to extend.</param>
    /// <param name="predicate">The predicate to evaluate.</param>
    /// <param name="transformation">The transformation to apply when the predicate is false.</param>
    /// <returns>The transformed builder if the predicate is false; otherwise, the original builder.</returns>
    public static TBuilder WhenNot<TResult, TBuilder>(
        this TBuilder builder,
        IPredicate predicate,
        Func<TBuilder, TBuilder> transformation)
        where TBuilder : IBuilder<TResult, TBuilder> {
        return !predicate.Evaluate() ? transformation(builder) : builder.Self();
    }

    /// <summary>
    /// Applies a transformation when the specified condition is false for the given context.
    /// </summary>
    /// <typeparam name="TResult">The type of the result built by the builder.</typeparam>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    /// <typeparam name="T">The type of the context to evaluate.</typeparam>
    /// <param name="builder">The builder to extend.</param>
    /// <param name="context">The context to evaluate.</param>
    /// <param name="condition">The condition to check against the context.</param>
    /// <param name="transformation">The transformation to apply when the condition is false.</param>
    /// <returns>The transformed builder if the condition is false; otherwise, the original builder.</returns>
    public static TBuilder WhenDoesntMatch<TResult, TBuilder, T>(
        this TBuilder builder,
        T context,
        Func<T, bool> condition,
        Func<TBuilder, TBuilder> transformation)
        where TBuilder : IBuilder<TResult, TBuilder> {
        return !condition(context) ? transformation(builder) : builder.Self();
    }

    /// <summary>
    /// Creates a predicate that evaluates using the fluent API.
    /// </summary>
    /// <typeparam name="T">The type of the context to evaluate.</typeparam>
    /// <param name="context">The context to evaluate.</param>
    /// <param name="condition">The condition to check against the context.</param>
    /// <returns>A predicate that can be used with the When and WhenNot methods.</returns>
    public static IPredicate CreatePredicate<T>(T context, Func<T, bool> condition) {
        return new PredicateResult(condition(context));
    }
}