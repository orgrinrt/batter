#region

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Batter.Utils.Builders;

/// <summary>
///     Base container interface that provides strongly-typed collection management.
///     Provides methods for accessing and managing typed collections of properties.
/// </summary>
/// <typeparam name="TThis">The implementing container type (CRTP pattern)</typeparam>
/// <typeparam name="TStorage">The storage type that manages collections</typeparam>
/// <typeparam name="TCollection">The collection type used to store properties</typeparam>
public interface IContainer<TThis, out TStorage, out TCollection> : IIsContainer
    where TThis : class, IContainer<TThis, TStorage, TCollection>
    where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
    where TCollection : class, IObjCollection {

    internal TStorage    Storage    { get; }
    internal TCollection Collection => this.Storage.Collection;

    internal AdditionHandler<DynKey, object>? OnAddition { get; }

    #region "Collection Management"

    /// <summary>
    ///     Clears all items from the collection.
    /// </summary>
    /// <returns>The container instance for method chaining</returns>
    TThis Clear() {
        this.Collection.Raw().Clear();

        return (TThis)this;
    }

    #endregion

    internal delegate void AdditionHandler<in TKey, in TItem>(TThis container, TKey key, TItem item)
        where TKey : IValidKey;


    #region "Property Accessors"

    /// <summary>
    ///     Gets a property of the specified type using the given key.
    /// </summary>
    /// <param name="key">The key to look up the property</param>
    /// <typeparam name="TProp">The property type</typeparam>
    /// <returns>The property associated with the key</returns>
    TProp GetProp<TProp>(DynKey key)
        where TProp : IProp<TThis, TProp, IValidKey> {
        return (TProp)this.Collection.Raw()[key];
    }

    TProp GetProp<TKey, TProp>(TKey key)
        where TKey : notnull, IValidKey<TKey>
        where TProp : IProp<TThis, TProp, IValidKey<TKey>> {
        return this.GetProp<TProp>(new(key));
    }

    TProp GetProp<TProp, T>(IValidKey<T> key)
        where TProp : IProp<TThis, TProp, IValidKey<T>>
        where T : class, IValidKey<T>, IValidKey {
        return new DynKey(key) is T dynKey
            ? this.GetProp<T, TProp>(dynKey)
            : this.GetProp<T, TProp>(key as T ?? throw new InvalidOperationException());
    }

    /// <summary>
    ///     Attempts to get a property of the specified type using the given key.
    /// </summary>
    /// <param name="key">The key to look up the property</param>
    /// <param name="prop">
    ///     When this method returns, contains the property associated with the specified key,
    ///     if the key is found; otherwise, the default value for the type of the prop parameter.
    /// </param>
    /// <typeparam name="TProp">The property type</typeparam>
    /// <returns>true if the container contains a property with the specified key; otherwise, false.</returns>
    bool TryGetProp<TProp>(DynKey key, out TProp? prop)
        where TProp : IProp<TThis, TProp, IValidKey> {
        if (this.Collection.Raw().TryGetValue(key, out object? value) && value is TProp typedValue) {
            prop = typedValue;

            return true;
        }

        prop = default;

        return false;
    }

    bool TryGetProp<TKey, TProp>(TKey key, out TProp? prop)
        where TKey : notnull, IValidKey<TKey>, IValidKey
        where TProp : IProp<TThis, TProp, IValidKey<TKey>> {
        return this.TryGetProp(new(key), out prop);
    }

    bool TryGetProp<TProp, T>(IValidKey<T> key, out TProp? prop)
        where TProp : IProp<TThis, TProp, IValidKey<T>>
        where T : class, IValidKey<T>, IValidKey {
        return new DynKey(key) is T dynKey
            ? this.TryGetProp(dynKey,                                            out prop)
            : this.TryGetProp(key as T ?? throw new InvalidOperationException(), out prop);
    }

    #endregion

    #region "Item Accessors"

    /// <summary>
    ///     Gets an item of the specified type using the given key.
    /// </summary>
    /// <param name="key">The key to look up the item</param>
    /// <typeparam name="TItem">The item type</typeparam>
    /// <returns>The item associated with the key</returns>
    TItem GetObj<TItem>(DynKey key) { return (TItem)this.Collection.Raw()[key]; }

    TItem GetObj<TKey, TItem>(TKey key)
        where TKey : notnull, IEquatable<TKey>, IValidKey<TKey> {
        return this.GetObj<TItem>(new(key));
    }

    TItem GetObj<TItem>(IValidKey key) { return this.GetObj<TItem>(new(key)); }

    /// <summary>
    ///     Attempts to get an item of the specified type using the given key.
    /// </summary>
    /// <param name="key">The key to look up the item</param>
    /// <param name="item">
    ///     When this method returns, contains the item associated with the specified key,
    ///     if the key is found; otherwise, the default value for the type of the item parameter.
    /// </param>
    /// <typeparam name="TItem">The item type</typeparam>
    /// <returns>true if the container contains an item with the specified key; otherwise, false.</returns>
    bool TryGetObj<TItem>(DynKey key, out TItem? item) {
        if (this.Collection.Raw().TryGetValue(key, out object? value) && value is TItem typedValue) {
            item = typedValue;

            return true;
        }

        item = default;

        return false;
    }

    bool TryGetObj<TKey, TItem>(TKey key, out TItem? item)
        where TKey : notnull, IEquatable<TKey>, IValidKey<TKey> {
        return this.TryGetObj(new(key), out item);
    }

    bool TryGetObj<TItem>(IValidKey key, out TItem? item) { return this.TryGetObj(new(key), out item); }

    #endregion

    #region "Property Setters"

    /// <summary>
    ///     Sets a property with the specified key and value.
    /// </summary>
    /// <param name="key">The key to associate with the property</param>
    /// <param name="value">The property value to set</param>
    /// <typeparam name="TProp">The property type</typeparam>
    /// <returns>The container instance for method chaining</returns>
    TThis WithProp<TProp>(DynKey key, TProp value)
        where TProp : IProp<TThis, TProp, IValidKey> {
        this.Collection.Raw()[key] = value;
        this.OnAddition?.Invoke((TThis)this, key, value);

        return (TThis)this;
    }

    TThis WithProp<TKey, TProp>(TKey key, TProp value)
        where TKey : notnull, IEquatable<TKey>, IValidKey<TKey>
        where TProp : IProp<TThis, TProp, IValidKey<TKey>> {
        // FIXME: likely not efficient
        return this.WithProp(new(key), value);
    }

    TThis WithProp<TProp>(IValidKey key, TProp value)
        where TProp : IProp<TThis, TProp, IValidKey> {
        // FIXME: likely not efficient 
        return this.WithProp(new(key), value);
    }

    /// <summary>
    ///     Sets a property with the specified key to its default value.
    /// </summary>
    /// <param name="key">The key to associate with the property</param>
    /// <typeparam name="TProp">The property type</typeparam>
    /// <returns>The container instance for method chaining</returns>
    TThis WithDefaultProp<TProp>(DynKey key)
        where TProp : IProp<TThis, TProp, IValidKey> {
        return this.WithProp(key, Activator.CreateInstance<TProp>());
    }

    TThis WithDefaultProp<TKey, TProp>(TKey key)
        where TKey : notnull, IValidKey<TKey>
        where TProp : IProp<TThis, TProp, IValidKey<TKey>> {
        return this.WithDefaultProp<TProp>(new(key));
    }

    TThis WithDefaultProp<TProp, T>(IValidKey<T> key)
        where TProp : IProp<TThis, TProp, IValidKey<T>>
        where T : class, IValidKey<T>, IValidKey {
        return new DynKey(key) is T dynKey
            ? this.WithDefaultProp<T, TProp>(dynKey)
            : this.WithDefaultProp<T, TProp>(key as T ?? throw new InvalidOperationException());
    }

    #endregion

    #region "Item Setters"

    /// <summary>
    ///     Sets an item with the specified key and value.
    /// </summary>
    /// <param name="key">The key to associate with the item</param>
    /// <param name="value">The item value to set</param>
    /// <typeparam name="TItem">The item type</typeparam>
    /// <returns>The container instance for method chaining</returns>
    TThis WithObj<TItem>(DynKey key, TItem value) {
        this.Collection.Raw()[key] = value!;
        this.OnAddition?.Invoke((TThis)this, key, value ?? throw new ArgumentNullException(nameof(value)));

        return (TThis)this;
    }

    TThis WithObj<TKey, TItem>(TKey key, TItem value)
        where TKey : notnull, IEquatable<TKey>, IValidKey<TKey> {
        return this.WithObj(new(key), value);
    }

    TThis WithObj<TItem>(IValidKey key, TItem value) { return this.WithObj(new(key), value); }

    /// <summary>
    ///     Sets an item with the specified key to its default value.
    /// </summary>
    /// <param name="key">The key to associate with the item</param>
    /// <typeparam name="TItem">The item type</typeparam>
    /// <returns>The container instance for method chaining</returns>
    TThis WithDefaultObj<TItem>(DynKey key) { return this.WithObj<TItem>(key, default!); }

    TThis WithDefaultObj<TKey, TItem>(TKey key)
        where TKey : notnull, IEquatable<TKey>, IValidKey<TKey> {
        return this.WithDefaultObj<TItem>(new(key));
    }

    TThis WithDefaultObj<TItem>(IValidKey key) { return this.WithDefaultObj<TItem>(new(key)); }

    #endregion

    #region "Removal"

    /// <summary>
    ///     Removes a property with the specified key.
    /// </summary>
    /// <param name="key">The key of the property to remove</param>
    /// <typeparam name="TProp">The property type</typeparam>
    /// <returns>The container instance for method chaining</returns>
    TThis RemoveProp<TProp>(DynKey key)
        where TProp : IProp<TThis, TProp, IValidKey> {
        this.Collection.Raw().Remove(key);

        return (TThis)this;
    }

    TThis RemoveProp<TKey, TProp>(TKey key)
        where TKey : notnull, IValidKey<TKey>
        where TProp : IProp<TThis, TProp, IValidKey<TKey>> {
        // TODO: probably better use instance pooling for DynKeys rather than allocate a new one each time
        return this.RemoveProp<TProp>(new(key));
    }

    TThis RemoveProp<TProp, T>(IValidKey<T> key)
        where TProp : IProp<TThis, TProp, IValidKey<T>>
        where T : class, IValidKey<T>, IValidKey {
        // TODO: probably better use instance pooling for DynKeys rather than allocate a new one each time
        return new DynKey(key) is T dynKey
            ? this.RemoveProp<T, TProp>(dynKey)
            : this.RemoveProp<T, TProp>(key as T ?? throw new InvalidOperationException());
    }

    /// <summary>
    ///     Removes an item with the specified key.
    /// </summary>
    /// <param name="key">The key of the item to remove</param>
    /// <typeparam name="TItem">The item type</typeparam>
    /// <returns>The container instance for method chaining</returns>
    TThis RemoveObj<TItem>(DynKey key) {
        this.Collection.Raw().Remove(key);

        return (TThis)this;
    }

    TThis RemoveObj<TKey, TItem>(TKey key)
        where TKey : notnull, IEquatable<TKey>, IValidKey<TKey> {
        return this.RemoveObj<TItem>(new(key));
    }

    TThis RemoveObj<TItem>(IValidKey key) { return this.RemoveObj<TItem>(new(key)); }

    #endregion

    #region Unique Properties

    TProp GetUniqueProp<TProp>()
        where TProp : IUniqueProp<TThis, TProp> {
        return (TProp)this.Collection.Raw()[(DynKey)typeof(TProp)];
    }

    TObj GetUniqueProp<TObj>(Type type) { return (TObj)this.Collection.Raw()[(DynKey)type]; }


    bool TryGetUniqueProp<TProp>(out TProp? prop)
        where TProp : IUniqueProp<TThis, TProp> {
        if (this.Collection.Raw().TryGetValue((DynKey)typeof(TProp), out object? value) && value is TProp typedValue) {
            prop = typedValue;

            return true;
        }

        prop = default;

        return false;
    }

    TThis WithUniqueProp<TProp>(TProp value)
        where TProp : IUniqueProp<TThis, TProp> {
        var key = new DynKey(typeof(TProp));
        this.Collection.Raw()[key] = value;
        this.OnAddition?.Invoke((TThis)this, key, value ?? throw new ArgumentNullException(nameof(value)));


        return (TThis)this;
    }

    TThis WithUniqueProp<TProp>(Type type, [NotNull] TProp value) {
        var key = new DynKey(type);
        this.Collection.Raw()[key] = value ?? throw new ArgumentNullException(nameof(value));
        this.OnAddition?.Invoke((TThis)this, key, value ?? throw new ArgumentNullException(nameof(value)));

        return (TThis)this;
    }

    TThis RemoveUniqueProp<TProp>()
        where TProp : IUniqueProp<TThis, TProp> {
        this.Collection.Raw().Remove((DynKey)typeof(TProp));

        return (TThis)this;
    }

    #endregion

}

/// <summary>
///     Interface for container storage that manages collections of properties.
///     This provides a way to store and retrieve properties for a specific container type.
/// </summary>
/// <typeparam name="TThis">The implementing storage type (CRTP pattern)</typeparam>
/// <typeparam name="TContainer">The container type this storage serves</typeparam>
/// <typeparam name="TCollection">The collection type used to store properties</typeparam>
public interface IContainerStorage<TThis, TContainer, out TCollection>
    where TContainer : class, IContainer<TContainer, TThis, TCollection>
    where TCollection : class, IObjCollection
    where TThis : class, IContainerStorage<TThis, TContainer, TCollection>, new() {

    /// <summary>
    ///     Gets the collection associated with this storage.
    /// </summary>
    TCollection Collection { get; }

}

/// <summary>
///     Marker interface for all container types.
///     This allows for basic type checking and identification of container objects.
/// </summary>
public interface IIsContainer { }

/// <summary>
///     Interface for collections that can provide access to their raw objects.
/// </summary>
public interface IObjCollection {

    /// <summary>
    ///     Gets the raw objects in this collection as a dictionary.
    /// </summary>
    /// <returns>A dictionary containing all objects in this collection</returns>
    Dictionary<DynKey, object> Raw() { return this.Raw<DynKey, object>(); }

    /// <summary>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    Dictionary<TKey, TValue> Raw<TKey, TValue>()
        where TKey : notnull;

}