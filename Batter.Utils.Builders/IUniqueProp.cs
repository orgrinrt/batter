namespace Batter.Utils.Builders;

/// <summary>
///     Represents a unique property that can be identified and managed by its type.
///     Unlike IProp which uses explicit keys, IUniqueProp uses the type itself as the key,
///     making it suitable for singleton-style properties within a builder.
/// </summary>
/// <typeparam name="TContainer">The container type that manages this property</typeparam>
/// <typeparam name="TThis">The implementing property type (for self-referencing)</typeparam>
public interface IUniqueProp<out TContainer, TThis> : IEquatable<TThis>
    where TThis : IUniqueProp<TContainer, TThis> {

    // No additional members required - type serves as the key

}