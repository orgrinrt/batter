#region

using System.Collections;

#endregion

namespace Batter.Utils.Builders.Dynamic;

/// <summary>
///     A strongly-typed collection that stores items with keys.
///     This class replaces the previous type-erased collection system.
/// </summary>
/// <typeparam name="TKey">The type of keys used in the collection</typeparam>
/// <typeparam name="TValue">The type of values stored in the collection</typeparam>
public class DynCollection<TKey, TValue> : IObjCollection,
                                           IEnumerable<KeyValuePair<TKey, TValue>>
    where TKey : notnull {

    private readonly Dictionary<TKey, TValue> _items = new();

    /// <summary>
    ///     Gets the collection of values.
    /// </summary>
    public ICollection<TValue> Values => this._items.Values;

    /// <summary>
    ///     Gets the collection of keys.
    /// </summary>
    public ICollection<TKey> Keys => this._items.Keys;

    /// <summary>
    ///     Gets or sets the value with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get or set</param>
    /// <returns>The value associated with the specified key</returns>
    /// <exception cref="KeyNotFoundException">The key does not exist in the collection</exception>
    public TValue this[TKey key] {
        get => this._items[key];
        set => this._items[key] = value;
    }

    /// <summary>
    ///     Gets the count of items in the collection.
    /// </summary>
    public int Count => this._items.Count;

    /// <summary>
    ///     Creates a new typed collection.
    /// </summary>
    public DynCollection() { }

    /// <summary>
    ///     Creates a new typed collection with the specified items.
    /// </summary>
    /// <param name="items">The initial items to add to the collection</param>
    public DynCollection(Dictionary<TKey, TValue> items) {
        if (items != null) {
            foreach (KeyValuePair<TKey, TValue> kvp in items) this._items[kvp.Key] = kvp.Value;
        }
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() { return this._items.GetEnumerator(); }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

    /// <inheritdoc />
    public Dictionary<TKey1, TValue1> Raw<TKey1, TValue1>()
        where TKey1 : notnull {
        if (typeof(TKey1) != typeof(TKey) || typeof(TValue1) != typeof(TValue)) {
            throw new InvalidOperationException(
                $"Cannot convert DynCollection<{typeof(TKey).Name}, {typeof(TValue).Name}> to DynCollection<{typeof(TKey1).Name}, {typeof(TValue1).Name}>.");
        }

        return this._items as Dictionary<TKey1, TValue1> ??
               throw new InvalidOperationException(
                   $"Cannot cast DynCollection<{typeof(TKey).Name}, {typeof(TValue).Name}> to Dictionary<{typeof(TKey1).Name}, {typeof(TValue1).Name}>.");
    }

    /// <summary>
    ///     Adds a key/value pair to the collection.
    /// </summary>
    /// <param name="key">The key of the element to add</param>
    /// <param name="value">The value to add</param>
    /// <exception cref="ArgumentException">An element with the same key already exists</exception>
    public void Add(TKey key, TValue value) { this._items.Add(key, value); }

    /// <summary>
    ///     Removes the element with the specified key from the collection.
    /// </summary>
    /// <param name="key">The key of the element to remove</param>
    /// <returns>true if the element is successfully found and removed; otherwise, false</returns>
    public bool Remove(TKey key) { return this._items.Remove(key); }

    /// <summary>
    ///     Determines whether the collection contains an element with the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the collection</param>
    /// <returns>true if the collection contains an element with the key; otherwise, false</returns>
    public bool ContainsKey(TKey key) { return this._items.ContainsKey(key); }

    /// <summary>
    ///     Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key whose value to get</param>
    /// <param name="value">The value associated with the specified key, if found</param>
    /// <returns>true if the collection contains an element with the specified key; otherwise, false</returns>
    public bool TryGetValue(TKey key, out TValue? value) { return this._items.TryGetValue(key, out value); }

    /// <summary>
    ///     Removes all keys and values from the collection.
    /// </summary>
    public void Clear() { this._items.Clear(); }

}