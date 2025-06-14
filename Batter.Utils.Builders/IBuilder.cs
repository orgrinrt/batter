namespace Batter.Utils.Builders;

/// <summary>
///     Represents a builder for constructing objects of type <typeparamref name="TResult" />.
/// </summary>
/// <typeparam name="TResult">The type of object being built.</typeparam>
/// <typeparam name="TBuilder">The concrete builder type, enabling fluent method chaining.</typeparam>
/// <typeparam name="TBuilderStep"></typeparam>
public interface IBuilder<out TResult,  out TBuilder,  TBuilderStep> :
    IFluentApiMethods<TBuilder>,
    IContainer<TBuilder, TBuilderStep, string>
    where TResult : class
    where TBuilder :
    IBuilder<TResult, TBuilder, TBuilderStep>, IBuilder<object, TBuilder, IBuilderStep<TBuilder>>
    where TBuilderStep : IBuilderStep<TBuilder> {
    /// <summary>
    ///     Builds and returns the final result.
    /// </summary>
    /// <returns>The constructed object.</returns>
    TResult Build();

    /// <summary>
    ///     Returns the builder instance for method chaining.
    /// </summary>
    /// <returns>The builder instance.</returns>
    TBuilder Self();
}