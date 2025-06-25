namespace Batter.Utils.Builders;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TThis"></typeparam>
public interface IDeepCloneable<out TThis>
    where TThis : IDeepCloneable<TThis> {
    /// <summary>
    ///     Creates a deep clone of the current instance.
    /// </summary>
    /// <returns>A new instance that is a deep clone of the current instance.</returns>
    TThis Clone();
}