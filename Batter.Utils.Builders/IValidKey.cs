#region

using System.Data;
using System.Reflection;

#endregion

namespace Batter.Utils.Builders;

/// <summary>
///     Non-generic marker interface for valid dictionary keys.
///     Provides a simpler API while maintaining key requirements.
/// </summary>
public interface IValidKey : IEquatable<IValidKey>,
                             IEquatable<DynKey>,
                             IComparable<DynKey> {

    /// <summary>
    ///     Gets a string representation of the key, suitable for debugging and logging.
    ///     All valid keys should provide a meaningful string representation.
    /// </summary>
    /// <returns>A string representation of the key</returns>
    string Display();

    /// <summary>
    ///     Gets a hash code for this key.
    /// </summary>
    /// <returns>A hash code for this key</returns>
    int GetHashCode();

    /// <summary>
    ///     Determines whether this key is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with</param>
    /// <returns>true if the objects are equal; otherwise, false</returns>
    bool Equals(object? obj);

}

/// <summary>
///     Generic interface that defines minimum requirements for valid dictionary keys.
///     Keys must be non-null and implement IEquatable for proper comparison behavior.
/// </summary>
/// <typeparam name="TThis">The implementing type (CRTP pattern)</typeparam>
public interface IValidKey<TThis> : IValidKey,
                                    IEquatable<IValidKey<TThis>>,
                                    IEqualityComparer<TThis>
    where TThis : IValidKey, IValidKey<TThis> {

    // Combines the non-generic IValidKey with IEquatable<TSelf>

}

public sealed class Key<TKey> : IValidKey<Key<TKey>>,
                                IEquatable<Key<TKey>> {

    private readonly TKey _value;
    public           TKey Value => this._value;

    public Key(TKey value) { this._value = value; }

    /// <inheritdoc />
    public bool Equals(Key<TKey>? other) { throw new NotImplementedException(); }


    /// <inheritdoc />
    public bool Equals(IValidKey? other) { throw new NotImplementedException(); }

    /// <inheritdoc />
    public bool Equals(DynKey? other) { throw new NotImplementedException(); }

    /// <inheritdoc />
    public int CompareTo(DynKey? other) { throw new NotImplementedException(); }

    public string Display() { return this.ToString() ?? nameof(Key<TKey>); }

    /// <inheritdoc />
    public bool Equals(IValidKey<Key<TKey>>? other) { throw new NotImplementedException(); }

    /// <inheritdoc />
    public bool Equals(Key<TKey>? x, Key<TKey>? y) { throw new NotImplementedException(); }

    /// <inheritdoc />
    public int GetHashCode(Key<TKey> obj) { throw new NotImplementedException(); }

    public static implicit operator Key<TKey>(TKey value) {
        if (value == null) throw new ArgumentNullException(nameof(value), "Key value cannot be null.");

        return new(value);
    }

    public static implicit operator TKey(Key<TKey> key) {
        if (key == null) throw new ArgumentNullException(nameof(key), "Key cannot be null.");

        return key.Value;
    }

    public static implicit operator DynKey(Key<TKey> key) {
        if (key == null) throw new ArgumentNullException(nameof(key), "Key cannot be null.");

        return new(key.Value!);
    }

    public static implicit operator Key<TKey>(DynKey key) {
        if (key == null) throw new ArgumentNullException(nameof(key), "Key cannot be null.");

        return new((TKey)key.Value);
    }

}

/// <summary>
///     Provides a type-safe wrapper for any object to be used as a key.
///     Enforces equality and hash code behavior at runtime.
/// </summary>
public sealed class DynKey : IValidKey<DynKey> {

    public const           string INVALID_NAME = "__Invalid__";
    public static readonly DynKey INVALID      = new(DynKey.INVALID_NAME);

    private readonly object _value;
    private readonly Type   _valueType;

    /// <summary>
    ///     Gets the type of the underlying value.
    /// </summary>
    public Type ValueType => this._valueType;

    /// <summary>
    ///     Gets the underlying value of this key.
    /// </summary>
    public object Value => this._value;

    /// <summary>
    ///     Creates a new TypeErasedKey with the specified value.
    /// </summary>
    /// <param name="value">The value to wrap</param>
    /// <exception cref="ArgumentNullException">Thrown when value is null</exception>
    /// <exception cref="ArgumentException">Thrown when value does not implement proper equality behavior</exception>
    public DynKey(object value) {
        if (value == null) throw new ArgumentNullException(nameof(value), "Key value cannot be null.");

        this._value     = value;
        this._valueType = value.GetType();

        if (value is Type) {
            // types are special and do not require IEquatable<T> (guaranteed to be unique and equatable)
            return;
        }

        // Enums are implicitly equatable and make valid keys.
        // For other types, we require IEquatable<T> for type-safe equality.
        if (!this._valueType.IsEnum) {
            Type equatableType = typeof(IEquatable<>).MakeGenericType(this._valueType);

            if (!equatableType.IsAssignableFrom(this._valueType)) {
                throw new ArgumentException(
                    $"Type '{this._valueType.FullName}' cannot be used as a key because it does not implement IEquatable<{this._valueType.Name}>.",
                    nameof(value));
            }
        }
    }

    /// <inheritdoc />
    public bool Equals(IValidKey? other) { throw new NotImplementedException(); }

    /// <inheritdoc />
    public int CompareTo(DynKey? other) { throw new NotImplementedException(); }

    /// <inheritdoc />
    public bool Equals(IValidKey<DynKey>? other) { throw new NotImplementedException(); }


    /// <summary>
    ///     Determines whether this key is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with</param>
    /// <returns>true if the objects are equal; otherwise, false</returns>
    public override bool Equals(object? obj) {
        return ReferenceEquals(this, obj) || (obj is DynKey other && this.Equals(other));
    }

    /// <summary>
    ///     Determines whether this key is equal to another TypeErasedKey.
    /// </summary>
    /// <param name="other">The key to compare with</param>
    /// <returns>true if the keys are equal; otherwise, false</returns>
    public bool Equals(DynKey? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        // Two keys are equal if they have the same type and their values are equal.
        // This works correctly for enums and types implementing IEquatable<T>.
        return this._valueType == other._valueType && this._value.Equals(other._value);
    }

    /// <inheritdoc />
    public string Display() { return this._value.ToString() ?? string.Empty; }

    /// <summary>
    ///     Gets a hash code for this key.
    /// </summary>
    /// <returns>A hash code for this key</returns>
    public override int GetHashCode() {
        // Hash code should be based on the value and type for consistency with Equals.
        return HashCode.Combine(this._value, this._valueType);
    }

    /// <inheritdoc />
    public bool Equals(DynKey? x, DynKey? y) { throw new NotImplementedException(); }

    /// <inheritdoc />
    public int GetHashCode(DynKey obj) { throw new NotImplementedException(); }

    /// <summary>
    ///     Determines whether two DynKey instances are equal.
    /// </summary>
    /// <param name="left">The first DynKey to compare</param>
    /// <param name="right">The second DynKey to compare</param>
    /// <returns>true if the keys are equal; otherwise, false</returns>
    public static bool operator ==(DynKey? left, DynKey? right) {
        if (left is null) return right is null;

        return left.Equals(right);
    }

    /// <summary>
    ///     Determines whether two DynKey instances are not equal.
    /// </summary>
    /// <param name="left">The first DynKey to compare</param>
    /// <param name="right">The second DynKey to compare</param>
    /// <returns>true if the keys are not equal; otherwise, false</returns>
    public static bool operator !=(DynKey? left, DynKey? right) { return !(left == right); }

    /// <summary>
    ///     Gets a string representation of this key.
    /// </summary>
    /// <returns>A string representation of this key</returns>
    public override string ToString() { return this._value.ToString() ?? string.Empty; }

    /// <summary>
    ///     Gets the underlying value as the specified type.
    /// </summary>
    /// <typeparam name="T">The type to convert to</typeparam>
    /// <returns>The value as the specified type</returns>
    /// <exception cref="InvalidCastException">Thrown when the value cannot be cast to the specified type</exception>
    public T As<T>() { return (T)this.As(typeof(T)); }

    private object As(Type targetType) {
        // Case 1: The value is already of the target type.
        if (targetType.IsInstanceOfType(this._value)) return this._value;

        // Case 2: The value is a Key<TInner>, so unwrap it and convert its inner value.
        if (this._valueType.IsGenericType && this._valueType.GetGenericTypeDefinition() == typeof(Key<>)) {
            PropertyInfo? valueProperty = this._valueType.GetProperty("Value");
            object?       keyValue      = valueProperty?.GetValue(this._value);

            if (keyValue == null) throw new InvalidOperationException("Failed to extract value from Key<> instance.");

            // Recursively call As on a new DynKey with the unwrapped value.
            return new DynKey(keyValue).As(targetType);
        }

        // Case 3: The target is Key<TInner>, so wrap the current value.
        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Key<>)) {
            Type innerType = targetType.GetGenericArguments()[0];

            try {
                // Convert the current value to the inner type of the target Key.
                object innerValue = this.As(innerType);

                return Activator.CreateInstance(targetType, innerValue)!;
            } catch (Exception ex) {
                throw new InvalidCastException(
                    $"Cannot convert value of type '{this._valueType.Name}' to create Key<{innerType.Name}>.",
                    ex);
            }
        }

        // Case 4: The target type is an enum.
        if (targetType.IsEnum) {
            // If the value is a string, parse it.
            if (this._value is string s) return Enum.Parse(targetType, s, true);

            // For other types (like int), convert to the enum's underlying type first for robustness.
            try {
                object underlyingValue = Convert.ChangeType(this._value, Enum.GetUnderlyingType(targetType));

                return Enum.ToObject(targetType, underlyingValue);
            } catch (Exception) {
                // Fall through to the general conversion below.
            }
        }

        // Case 5: General-purpose conversion, which also handles enum-to-integer.
        try { return Convert.ChangeType(this._value, targetType); } catch (Exception ex) {
            throw new InvalidCastException(
                $"Cannot convert underlying key value of type '{this._valueType.Name}' to the requested type '{targetType.FullName}'.",
                ex);
        }
    }

    /// <summary>
    ///     Implicitly converts a string to a TypeErasedKey.
    /// </summary>
    /// <param name="value">The string value to convert</param>
    public static implicit operator DynKey(string value) { return new(value); }

    /// <summary>
    ///     Implicitly converts an integer to a TypeErasedKey.
    /// </summary>
    /// <param name="value">The integer value to convert</param>
    public static implicit operator DynKey(int value) { return new(value); }

    /// <summary>
    ///     Implicitly converts a long to a TypeErasedKey.
    /// </summary>
    /// <param name="value">The long value to convert</param>
    public static implicit operator DynKey(long value) { return new(value); }

    /// <summary>
    ///     Implicitly converts a GUID to a TypeErasedKey.
    /// </summary>
    /// <param name="value">The GUID value to convert</param>
    public static implicit operator DynKey(Guid value) { return new(value); }

    /// <summary>
    ///     Implicitly converts an enum value to a TypeErasedKey.
    /// </summary>
    /// <param name="value">The enum value to convert</param>
    public static implicit operator DynKey(Enum value) { return new(value); }

    public static explicit operator DynKey(Type value) { return new(value); }

    public static implicit operator DynKey(Key<IValidKey> value) {
        if (value == null) throw new ArgumentNullException(nameof(value), "Key cannot be null.");

        return new(value.Value);
    }

    /// <summary>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator string(DynKey value) {
        return value != DynKey.INVALID
            ? value.ToString()
            : throw new InvalidExpressionException();
    }

    /// <summary>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static explicit operator int(DynKey value) {
        return value != DynKey.INVALID
            ? value.As<int>()
            : throw new InvalidExpressionException();
    }

    /// <summary>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static explicit operator long(DynKey value) {
        return value != DynKey.INVALID
            ? value.As<long>()
            : throw new InvalidExpressionException();
    }

    /// <summary>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static explicit operator Guid(DynKey value) {
        return value != DynKey.INVALID
            ? value.As<Guid>()
            : throw new InvalidExpressionException();
    }

    /// <summary>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static explicit operator Enum(DynKey value) {
        return value != DynKey.INVALID
            ? value.As<Enum>()
            : throw new InvalidExpressionException();
    }

    public static explicit operator Type(DynKey value) {
        return value != DynKey.INVALID
            ? value.As<Type>()
            : throw new InvalidExpressionException();
    }


    public static implicit operator Key<IValidKey>(DynKey value) {
        return value != DynKey.INVALID
            ? new(value)
            : throw new InvalidExpressionException();
    }

}