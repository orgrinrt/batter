namespace Batter.Utils.Builders;

/// <summary>
///     Represents a step in the building process.
/// </summary>
/// <typeparam name="TProperty">The property type this step processes.</typeparam>
/// <typeparam name="TResult">The type of object being built.</typeparam>
/// <typeparam name="TBuilder">The concrete builder type.</typeparam>
public interface IBuilderStep<in TProperty, TResult, TBuilder>
    where TBuilder : IBuilder<TResult, TBuilder> {
    /// <summary>
    ///     Applies this step to the builder using the specified property value.
    /// </summary>
    /// <param name="builder">The builder to modify.</param>
    /// <param name="propertyValue">The property value to process.</param>
    /// <returns>The modified builder.</returns>
    TBuilder Apply(TBuilder builder, TProperty propertyValue);
}