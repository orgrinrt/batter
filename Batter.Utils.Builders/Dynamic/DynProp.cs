#region

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Batter.Utils.Builders.Dynamic;

/// <summary>
///     A strictly typed dynamic attribute that implements IProp with TypeErasedKey keys
/// </summary>
/// <typeparam name="TContainer">The container type this attribute belongs to</typeparam>
public abstract class DynProp<TContainer> : IProp<TContainer, DynProp<TContainer>, DynKey> {

    /// <summary>
    ///     Gets or sets the attribute value
    /// </summary>
    public object? Value { get; set; }

    /// <summary>
    ///     Gets the runtime type of the stored value
    /// </summary>
    public Type? ValueType { get; private set; }

    /// <summary>
    ///     Initializes a new dynamic attribute
    /// </summary>
    /// <param name="name">The attribute name</param>
    /// <param name="value">The attribute value</param>
    public DynProp([NotNull] string name, object? value = null) {
        this.Id = name ?? throw new ArgumentNullException(nameof(name));
        this.SetValue(value);
    }

    /// <summary>
    ///     Gets the attribute name
    /// </summary>
    [NotNull]
    public DynKey Id { get; }

    /// <inheritdoc />
    public bool Equals(DynProp<TContainer>? other) {
        return other != null && this.Id.Equals(other.Id) && Equals(this.Value, other.Value);
    }

    /// <inheritdoc />
    public abstract DynProp<TContainer> GetDefault();

    /// <summary>
    ///     Sets the value and updates type information
    /// </summary>
    /// <param name="value">The new value</param>
    /// <returns>This attribute for chaining</returns>
    [return: NotNull]
    public DynProp<TContainer> SetValue(object? value) {
        this.Value     = value;
        this.ValueType = value?.GetType();

        return this;
    }

    /// <summary>
    ///     Gets the value as the specified type
    /// </summary>
    /// <typeparam name="T">The expected type</typeparam>
    /// <returns>The typed value or default</returns>
    public T? GetValue<T>() {
        return this.Value is T result
            ? result
            : default;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) { return obj is DynProp<TContainer> other && this.Equals(other); }

    /// <inheritdoc />
    public override int GetHashCode() { return HashCode.Combine(this.Id, this.Value); }

    /// <inheritdoc />
    public override string ToString() { return $"{this.Id}: {this.Value} ({this.ValueType?.Name ?? "null"})"; }

}