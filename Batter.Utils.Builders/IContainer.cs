#region

using Batter.Utils.Builders.Predicates;

#endregion

namespace Batter.Utils.Builders;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TThisContainer">self, the container..</typeparam>
/// <typeparam name="TItem">The contained type.</typeparam>
/// <typeparam name="TItemKeyType"></typeparam>
public interface IContainer<out TThisContainer,  TItem,  TItemKeyType>
    : IContainerFluentApi<TThisContainer, TItem, TItemKeyType>,
        IEnumerable<KeyValuePair<TItemKeyType, TItem>>
    where TThisContainer : IContainer<TThisContainer, TItem, TItemKeyType> { }

/// <summary>
/// 
/// </summary>
/// <typeparam name="TThisContainer"></typeparam>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TItemKeyType"></typeparam>
public interface
    IPropContainer<out TThisContainer,  TItem,  TItemKeyType> : IContainer<TThisContainer, TItem, TItemKeyType>
    where TThisContainer : IPropContainer<TThisContainer, TItem, TItemKeyType>
    where TItem : IProp<TThisContainer, TItem, TItemKeyType>
    where TItemKeyType : IEquatable<TItemKeyType> { }

/// <summary>
/// 
/// </summary>
/// <typeparam name="TThisContainer"></typeparam>
/// <typeparam name="TItemType"></typeparam>
/// <typeparam name="TItemKeyType"></typeparam>
public  interface IContainerFluentApi<out TThisContainer, TItemType,  TItemKeyType>
    where TThisContainer : IContainer<TThisContainer, TItemType, TItemKeyType> {
    /// <summary>
    /// 
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// 
    /// </summary>
    int Count { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerable<TItemType> GetAll();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IEnumerable<TItemType> Get(IPredicate predicate);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    TItemType Get(TItemKeyType key);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    TThisContainer Add(TItemKeyType key, TItemType item);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    TThisContainer Add(KeyValuePair<TItemKeyType, TItemType> item);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    TThisContainer Add(Tuple<TItemKeyType, TItemType> item);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    TThisContainer Add(params Tuple<TItemKeyType, TItemType>[] items);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    TThisContainer Add(params KeyValuePair<TItemKeyType, TItemType>[] items);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    TThisContainer Remove(TItemType item);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    TThisContainer Remove(TItemKeyType key);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    TThisContainer Clear();
}

/// <summary>
/// 
/// </summary>
public static partial class IContainerFluentApiExtensions {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="self"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool Any<TThisContainer, TItemType, TItemKeyType>(
        this IContainerFluentApi<TThisContainer, TItemType, TItemKeyType> self, IPredicate predicate)
        where TThisContainer : IContainer<TThisContainer, TItemType, TItemKeyType> {
        // FIXME: a hack that probably needs revisiting to optimise, this is very much not efficient
        return (self as IEnumerable<KeyValuePair<TItemKeyType, TItemType>> ??
                Array.Empty<KeyValuePair<TItemKeyType, TItemType>>())
            .Where(predicate as Fn as Func<KeyValuePair<TItemKeyType, TItemType>, bool> ??
                   throw new InvalidOperationException()).Any();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="self"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool Any<TThisContainer, TItemType, TItemKeyType>(
        this IContainerFluentApi<TThisContainer, TItemType, TItemKeyType> self,
        Func<TItemKeyType, TItemType, bool> predicate)
        where TThisContainer : IContainer<TThisContainer, TItemType, TItemKeyType> {
        // FIXME: a hack that probably needs revisiting to optimise, this is very much not efficient
        return self.Any(new Fn(predicate as Func<object, object, bool> ?? throw new InvalidOperationException()));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="self"></param>
    /// <param name="predicate"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns></returns>
    public static bool All<TThisContainer, TItemType, TItemKeyType>(
        this IContainerFluentApi<TThisContainer, TItemType, TItemKeyType> self, IPredicate predicate)
        where TThisContainer : IContainer<TThisContainer, TItemType, TItemKeyType> {
        return (self as IEnumerable<KeyValuePair<TItemKeyType, TItemType>> ??
                Array.Empty<KeyValuePair<TItemKeyType, TItemType>>()).All(
            predicate as Fn as Func<KeyValuePair<TItemKeyType, TItemType>, bool> ??
            throw new ArgumentNullException());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="self"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool None<TThisContainer, TItemType, TItemKeyType>(
        this IContainerFluentApi<TThisContainer, TItemType, TItemKeyType> self, IPredicate predicate)
        where TThisContainer : IContainer<TThisContainer, TItemType, TItemKeyType> {
        return !self.Any(predicate);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="self"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public static bool Contains<TThisContainer, TItemType, TItemKeyType>(
        this IContainerFluentApi<TThisContainer, TItemType, TItemKeyType> self, TItemType item)
        where TThisContainer : IContainer<TThisContainer, TItemType, TItemKeyType>
        where TItemType : IEquatable<TItemType> {
        if (self is null)
            throw new ArgumentNullException(nameof(self), "Container cannot be null.");
        if (item is null)
            throw new ArgumentNullException(nameof(item), "Item cannot be null.");
        try {
            var match = self.GetAll().FirstOrDefault(i => i.Equals(item));
            return match != null;
        }
        catch (KeyNotFoundException) {
            // FIXME: there's probably a better exception to catch, since self.GetAll() likely won't throw this
            return false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="self"></param>
    /// <returns></returns>
    public static bool ContainsKey<TThisContainer, TItemType, TItemKeyType>(
        this IContainerFluentApi<TThisContainer, TItemType, TItemKeyType> self, TItemKeyType key)
        where TThisContainer : IContainer<TThisContainer, TItemType, TItemKeyType> {
        if (self is null)
            throw new ArgumentNullException(nameof(self), "Container cannot be null.");
        if (key is null)
            throw new ArgumentNullException(nameof(key), "Key cannot be null.");
        try {
            self.Get(key);
            return true;
        }
        catch (KeyNotFoundException) {
            return false;
        }
    }
}

/// <summary>
/// 
/// </summary>
public static partial class IContainerFluentApiExtensions {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="self"></param>
    /// <returns></returns>
    public static TThisContainer Filter<TThisContainer, TItemType, TItemKeyType>(
        this IContainerFluentApi<TThisContainer, TItemType, TItemKeyType> self, IPredicate predicate)
        where TThisContainer : IContainer<TThisContainer, TItemType, TItemKeyType> {
        // FIXME: not efficient, needs to be optimised
        foreach (var item in self.GetAll())
            if (predicate.Evaluate())
                self.Remove(item);
        return (TThisContainer)self;
    }
}