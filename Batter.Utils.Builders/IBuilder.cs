namespace Batter.Utils.Builders;

/// <summary>
///     Represents a builder for constructing objects of type <typeparamref name="TResult" />.
/// </summary>
/// <typeparam name="TResult">The type of object being built.</typeparam>
/// <typeparam name="TBuilder">The concrete builder type, enabling fluent method chaining.</typeparam>
public interface IBuilder<out TResult, out TBuilder>
    where TBuilder : IBuilder<TResult, TBuilder> {
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

/// <summary>
///     Extends the base builder with standard configuration methods.
/// </summary>
public interface IFluentBuilder<TResult, TBuilder> : IBuilder<TResult, TBuilder>
    where TBuilder : IFluentBuilder<TResult, TBuilder> {
    /// <summary>
    ///     Configures the builder with a property value.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="propertyValue">The property value to set.</param>
    /// <returns>The builder for method chaining.</returns>
    TBuilder With<TProperty>(TProperty propertyValue);

    /// <summary>
    ///     Configures the builder using a builder step.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="step">The builder step to apply.</param>
    /// <returns>The builder for method chaining.</returns>
    TBuilder With<TProperty>(IBuilderStep<TProperty, TResult, TBuilder> step);

    /// <summary>
    ///     Configures the builder with a property value from a factory function.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="valueFactory">The function that produces the property value.</param>
    /// <returns>The builder for method chaining.</returns>
    TBuilder WithFactory<TProperty>(Func<TProperty> valueFactory);

    /// <summary>
    ///     Applies a transformation to the builder.
    /// </summary>
    /// <param name="transformation">The transformation function.</param>
    /// <returns>The builder for method chaining.</returns>
    TBuilder Transform(Func<TBuilder, TBuilder> transformation);

    /// <summary>
    ///     Conditionally applies a transformation.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="transformation">The transformation to apply if condition is true.</param>
    /// <returns>The builder for method chaining.</returns>
    TBuilder When(bool condition, Func<TBuilder, TBuilder> transformation);
}