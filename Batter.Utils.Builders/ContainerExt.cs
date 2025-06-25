#region

using System.Reflection;

using Batter.Utils.Builders.Dynamic;

#endregion

namespace Batter.Utils.Builders;

/// <summary>
///     The more cohesive, fluid and unified extension methods for IContainer.
/// </summary>
public static partial class ContainerExtensions {

    public static TItem? Get<TItem, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        IValidKey                                     key
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection {
        // TODO: there must be some more idiomatic and efficient way to do this...

        Type[] interfaces = TypeExtensions.GetInterfaces(typeof(TItem));

        foreach (Type i in interfaces) {
            string name = i.FullName ?? "";

            if (name.Contains("IUniqueProp")) return container.GetUniqueProp<TItem>(typeof(TItem));

            if (name.Contains("IProp")) return container.GetObj<TItem>(key);

            return container.TryGetObj(key, out TItem? o)
                ? o
                : default;
        }

        return default;
    }

    public static TThis With<TItem, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        IValidKey                                     key,
        TItem?                                        value = default
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TItem : class, new() {
        // TODO: there must be some more idiomatic and efficient way to do this...

        Type[] interfaces = TypeExtensions.GetInterfaces(typeof(TItem));

        foreach (Type i in interfaces) {
            string name = i.FullName ?? "";

            if (name.Contains("IUniqueProp")) {
                try {
                    return container.WithUniqueProp(typeof(TItem), value ?? Activator.CreateInstance(typeof(TItem)));
                } catch (Exception) {
                    // TODO: log it
                    if (TypeExtensions.IsAssignableFrom(typeof(DynBuilder), container.GetType())) {
                        return (container as DynBuilder ?? throw new InvalidOperationException()).WithCtor<TItem>(
                                   value ?? new TItem()) as TThis ??
                               throw new InvalidOperationException();
                    }
                }
            }

            if (name.Contains("IProp")) {
                return value != null
                    ? container.WithObj(key, value!)
                    : container.WithDefaultObj<TItem>(key);
            }
        }

        return value != null
            ? container.WithObj(key, value!)
            : container.WithDefaultObj<TItem>(key);
    }

    public static TThis With<TItem, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        DynKey                                        key,
        TItem?                                        value
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TItem : class {
        // TODO: there must be some more idiomatic and efficient way to do this...

        Type[] interfaces = TypeExtensions.GetInterfaces(typeof(TItem));

        foreach (Type i in interfaces) {
            string name = i.FullName ?? "";

            if (name.Contains("IUniqueProp")) {
                try {
                    return container.WithUniqueProp(typeof(TItem), value ?? Activator.CreateInstance(typeof(TItem)));
                } catch (Exception) {
                    // TODO: log it
                    if (TypeExtensions.IsAssignableFrom(typeof(DynBuilder), container.GetType()) && value != null) {
                        return (container as DynBuilder ?? throw new InvalidOperationException()).WithCtor<TItem>(
                                   value!) as TThis ??
                               throw new ArgumentException("failed to create instance of " + typeof(TItem).FullName);
                    }
                }
            }

            if (name.Contains("IProp")) {
                return value != null
                    ? container.WithObj(key, value!)
                    : container.WithDefaultObj<TItem>(key);
            }
        }

        return value != null
            ? container.WithObj(key, value!)
            : container.WithDefaultObj<TItem>(key);
    }

}

/// <summary>
///     Extension methods for IContainer to provide overloaded functionality of the interfaces (1:1 or close to)
/// </summary>
public static partial class ContainerExtensions {

    #region "Property Accessors Extensions"

    /// <summary>
    ///     Gets a property of the specified type using the given typed key.
    /// </summary>
    public static TProp GetProp<TKey, TProp, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        TKey                                          key
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TKey : notnull, IEquatable<TKey>, IValidKey<TKey>
        where TProp : IProp<TThis, TProp, IValidKey<TKey>> {
        return container.GetProp<TProp>(new(key));
    }

    /// <summary>
    ///     Gets a property of the specified type using the given key.
    /// </summary>
    public static TProp GetProp<TProp, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        IValidKey                                     key
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TProp : IProp<TThis, TProp, IValidKey> {
        return container.GetProp<TProp>(new(key));
    }

    /// <summary>
    ///     Tries to get a property of the specified type using the given typed key.
    /// </summary>
    public static bool TryGetProp<TKey, TProp, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        TKey                                          key,
        out TProp?                                    value
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TKey : notnull, IEquatable<TKey>, IValidKey<TKey>
        where TProp : IProp<TThis, TProp, IValidKey<TKey>> {
        return container.TryGetProp(new(key), out value);
    }

    /// <summary>
    ///     Tries to get a property of the specified type using the given key.
    /// </summary>
    public static bool TryGetProp<TProp, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        IValidKey                                     key,
        out TProp?                                    value
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TProp : IProp<TThis, TProp, IValidKey> {
        return container.TryGetProp(new(key), out value);
    }

    #endregion

    #region "Item Accessors Extensions"

    /// <summary>
    ///     Gets an item of the specified type using the given typed key.
    /// </summary>
    public static TItem GetObj<TKey, TItem, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        TKey                                          key
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TKey : notnull, IEquatable<TKey>, IValidKey<TKey> {
        return container.GetObj<TItem>(new(key));
    }

    /// <summary>
    ///     Gets an item of the specified type using the given key.
    /// </summary>
    public static TItem GetObj<TItem, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        IValidKey                                     key
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection {
        return container.GetObj<TItem>(new(key));
    }

    /// <summary>
    ///     Tries to get an item of the specified type using the given typed key.
    /// </summary>
    public static bool TryGetObj<TKey, TItem, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        TKey                                          key,
        out TItem?                                    value
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TKey : notnull, IEquatable<TKey>, IValidKey<TKey> {
        return container.TryGetObj(new(key), out value);
    }

    /// <summary>
    ///     Tries to get an item of the specified type using the given key.
    /// </summary>
    public static bool TryGetObj<TItem, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        IValidKey                                     key,
        out TItem?                                    value
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection {
        return container.TryGetObj(new(key), out value);
    }

    #endregion

    #region "Property Setters Extensions"

    /// <summary>
    ///     Sets a property with the specified typed key and value.
    /// </summary>
    public static TThis WithProp<TKey, TProp, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        TKey                                          key,
        TProp                                         value
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TKey : notnull, IEquatable<TKey>, IValidKey<TKey>
        where TProp : IProp<TThis, TProp, IValidKey<TKey>> {
        return container.WithProp(new(key), value);
    }

    /// <summary>
    ///     Sets a property with the specified key and value.
    /// </summary>
    public static TThis WithProp<TProp, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        IValidKey                                     key,
        TProp                                         value
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TProp : IProp<TThis, TProp, IValidKey> {
        return container.WithProp(new(key), value);
    }

    /// <summary>
    ///     Sets a property with the specified typed key to its default value.
    /// </summary>
    public static TThis WithDefaultProp<TKey, TProp, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        TKey                                          key
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TKey : notnull, IEquatable<TKey>, IValidKey<TKey>
        where TProp : IProp<TThis, TProp, IValidKey<TKey>> {
        return container.WithDefaultProp<TProp>(new(key));
    }

    /// <summary>
    ///     Sets a property with the specified key to its default value.
    /// </summary>
    public static TThis WithDefaultProp<TProp, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        IValidKey                                     key
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TProp : IProp<TThis, TProp, DynKey> {
        return container.WithDefaultProp<TProp>(new(key));
    }

    #endregion

    #region "Item Setters Extensions"

    /// <summary>
    ///     Sets an item with the specified typed key and value.
    /// </summary>
    public static TThis WithObj<TKey, TItem, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        TKey                                          key,
        TItem                                         value
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TKey : notnull, IEquatable<TKey>, IValidKey<TKey> {
        return container.WithObj(key, value);
    }

    /// <summary>
    ///     Sets an item with the specified key and value.
    /// </summary>
    public static TThis WithObj<TItem, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        IValidKey                                     key,
        TItem                                         value
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection {
        return container.WithObj(key, value);
    }

    /// <summary>
    ///     Sets an item with the specified typed key to its default value.
    /// </summary>
    public static TThis WithDefaultObj<TKey, TItem, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        TKey                                          key
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TKey : notnull, IEquatable<TKey>, IValidKey<TKey> {
        return container.WithDefaultObj<TItem>(new(key));
    }

    /// <summary>
    ///     Sets an item with the specified key to its default value.
    /// </summary>
    public static TThis WithDefaultObj<TItem, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        IValidKey                                     key
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection {
        return container.WithDefaultObj<TItem>(new(key));
    }

    #endregion

    #region "Removal Extensions"

    /// <summary>
    ///     Removes a property with the specified typed key.
    /// </summary>
    public static TThis RemoveProp<TKey, TProp, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        TKey                                          key
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TKey : notnull, IEquatable<TKey>, IValidKey<TKey>
        where TProp : IProp<TThis, TProp, IValidKey<TKey>> {
        return container.RemoveProp<TProp>(new(key));
    }

    /// <summary>
    ///     Removes a property with the specified key.
    /// </summary>
    public static TThis RemoveProp<TProp, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        IValidKey                                     key
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TProp : IProp<TThis, TProp, DynKey> {
        return container.RemoveProp<TProp>(new(key));
    }

    /// <summary>
    ///     Removes an item with the specified typed key.
    /// </summary>
    public static TThis RemoveObj<TKey, TItem, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        TKey                                          key
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TKey : notnull, IEquatable<TKey>, IValidKey<TKey> {
        return container.RemoveObj<TItem>(new(key));
    }

    /// <summary>
    ///     Removes an item with the specified key.
    /// </summary>
    public static TThis RemoveObj<TItem, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        IValidKey                                     key
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection {
        return container.RemoveObj<TItem>(new(key));
    }

    #endregion

    #region Unique Properties

    public static TThis WithUniqueProp<TProp, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        TProp                                         value
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TProp : IUniqueProp<TThis, TProp> {
        return container.WithUniqueProp(value);
    }

    public static TThis WithUniqueProp<TProp, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        Type                                          type,
        TProp                                         value
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TProp : class {
        return container.WithUniqueProp(type, value);
    }

    public static TProp? GetUniqueProp<TProp, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TProp : IUniqueProp<TThis, TProp> {
        return container.GetUniqueProp<TProp>();
    }

    public static bool TryGetUniqueProp<TProp, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container,
        out  TProp?                                   value
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TProp : IUniqueProp<TThis, TProp> {
        return container.TryGetUniqueProp(out value);
    }

    public static TThis RemoveUniqueProp<TProp, TThis, TStorage, TCollection>(
        this IContainer<TThis, TStorage, TCollection> container
    )
        where TThis : class, IContainer<TThis, TStorage, TCollection>
        where TStorage : class, IContainerStorage<TStorage, TThis, TCollection>, new()
        where TCollection : class, IObjCollection
        where TProp : IUniqueProp<TThis, TProp> {
        return container.RemoveUniqueProp<TProp>();
    }

    #endregion

}