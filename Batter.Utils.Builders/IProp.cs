#region

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Batter.Utils.Builders;

public interface IValidProperty : IInternallyMutableBuilderTarget {

    DynKey GetKey();

}

/// <summary>
///     Represents a property that can be managed within a property container.
///     This interface defines the contract for objects that can be stored in
///     builders using a key-based system, enabling flexible property management.
///     Uses unified storage pattern for consistency.
/// </summary>
/// <typeparam name="TThis">The implementing property type (for self-referencing)</typeparam>
/// <typeparam name="TKey">The type of key used to identify this property</typeparam>
/// <typeparam name="TContainer"></typeparam>
public interface IProp<out TContainer, TThis, out TKey> : IEquatable<TThis>,
                                                          IValidProperty
    where TThis : IProp<TContainer, TThis, IValidKey>
    where TKey : notnull, IValidKey {

    /// <summary>
    ///     Gets the unique identifier of a property.
    ///     The identifier is utilized to distinguish and manage individual properties uniquely in a container structure.
    /// </summary>
    [NotNull]
    TKey Id { get; }

    DynKey IValidProperty.GetKey() {
        return this.Id as DynKey ?? throw new InvalidOperationException("Id must be a DynKey");
    }

}