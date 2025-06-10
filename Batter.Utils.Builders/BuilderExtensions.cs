using Batter.Utils.Builders.Predicates;

namespace Batter.Utils.Builders;

public static class BuilderExtensions {
    /// <summary>
    ///     Applies a transformation when the predicate evaluates to true.
    /// </summary>
    public static TBuilder When<TResult, TBuilder>(
        this TBuilder builder,
        IPredicate predicate,
        Func<TBuilder, TBuilder> transformation)
        where TBuilder : IBuilder<TResult, TBuilder> {
        return predicate.Evaluate() ? transformation(builder) : builder.Self();
    }

    /// <summary>
    ///     Applies a transformation when the predicate evaluates to true for the specified value.
    /// </summary>
    public static TBuilder WhenMatches<TResult, TBuilder, T>(
        this TBuilder builder,
        T value,
        IPredicate<T> predicate,
        Func<TBuilder, TBuilder> transformation)
        where TBuilder : IBuilder<TResult, TBuilder> {
        return predicate.Evaluate(value) ? transformation(builder) : builder.Self();
    }

    /// <summary>
    ///     Applies a transformation when the predicate evaluates to true.
    /// </summary>
    public static TBuilder WhenNot<TResult, TBuilder>(
        this TBuilder builder,
        IPredicate predicate,
        Func<TBuilder, TBuilder> transformation)
        where TBuilder : IBuilder<TResult, TBuilder> {
        return !predicate.Evaluate() ? transformation(builder) : builder.Self();
    }

    /// <summary>
    ///     Applies a transformation when the predicate evaluates to true for the specified value.
    /// </summary>
    public static TBuilder WhenDoesntMatch<TResult, TBuilder, T>(
        this TBuilder builder,
        T value,
        IPredicate<T> predicate,
        Func<TBuilder, TBuilder> transformation)
        where TBuilder : IBuilder<TResult, TBuilder> {
        return !predicate.Evaluate(value) ? transformation(builder) : builder.Self();
    }
}