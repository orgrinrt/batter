#region

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Batter.Utils.Builders;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TThis"></typeparam>
/// <typeparam name="TContainer"></typeparam>
/// <typeparam name="TKeyType"></typeparam>
public interface IProp<out TContainer, in TThis, out TKeyType>
    where TThis : IProp<TContainer, TThis, TKeyType>
    where TContainer : IPropContainer<TContainer, TThis, TKeyType>
    where TKeyType : notnull, IEquatable<TKeyType> {
    /// <summary>
    /// 
    /// </summary>
    [NotNull]
    TKeyType Id { get; }
}

/// <summary>
/// 
/// </summary>
public static class PropExtensions {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="prop"></param>
    /// <typeparam name="TThisContainer"></typeparam>
    /// <typeparam name="TProp"></typeparam>
    /// <typeparam name="TPropKey"></typeparam>
    /// <returns></returns>
    public static TThisContainer With<TThisContainer, TProp, TPropKey>(
        this TThisContainer builder,
        TProp prop)
        where TProp : IProp<TThisContainer, TProp, TPropKey>
        where TThisContainer : IPropContainer<TThisContainer, TProp, TPropKey>
        where TPropKey : IEquatable<TPropKey> {
        if (builder is not TThisContainer container)
            throw new InvalidOperationException(
                $"Builder of type {typeof(TThisContainer).Name} does not implement IBuilderWithProps<{typeof(TThisContainer).Name}>.");
        if (container.GetAll().Any(p => {
                if (p is TProp thisProp)
                    return thisProp.Id.Equals(prop.Id);
                throw new InvalidOperationException(
                    $"Property with ID '{prop.Id}' is not of type {typeof(TProp).Name}.");
            }))
            throw new InvalidOperationException(
                $"Property with ID '{prop.Id}' already exists in the builder of type {typeof(TThisContainer).Name}.");

        container.Add(prop.Id, prop);
        return container;
    }
}