namespace Batter.Utils.Builders;

/// <summary>
///     Represents a step in the building process.
/// </summary>
/// <typeparam name="TBuilder">The concrete builder type.</typeparam>
public interface IBuilderStep<out TBuilder>
    where TBuilder : IBuilder<object, TBuilder, IBuilderStep<TBuilder>> {
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    TBuilder Apply();
}