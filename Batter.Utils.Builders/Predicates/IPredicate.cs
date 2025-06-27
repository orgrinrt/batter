#region

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Batter.Utils.Builders.Predicates;

/// <summary>
///     Represents a predicate that can evaluate to a boolean result.
/// </summary>
/// <remarks>
///     This interface is the foundation for the predicate system.
///     For most common usage scenarios, consider using the fluent API
///     available through the <see cref="Predicates" /> class.
///     Example:
///     <code>
/// bool result = Predicates
///     .When(item).Matches(i =&gt; i.Value &gt; 100)
///     .AndAlso()
///     .When(item).Matches(i =&gt; i.Weight &lt; 5.0f)
///     .Evaluate();
/// </code>
/// </remarks>
public interface IPredicate : IDeepCloneable<IPredicate>,
                              IInto<IPredicate<object>>,
                              IFrom<IPredicate<object>, IPredicate> {

    /// <summary>
    ///     Evaluates the predicate.
    /// </summary>
    /// <returns>True if the predicate is satisfied; otherwise, false.</returns>
    bool Evaluate();

}

/// <summary>
/// </summary>
/// <typeparam name="TThis"></typeparam>
public interface IPredicate<out TThis> : IDeepCloneable<IPredicate<TThis>>,
                                         IInto<IPredicate>,
                                         IFrom<IPredicate, TThis> {

    /// <summary>
    ///     Evaluates the predicate.
    /// </summary>
    /// <returns>True if the predicate is satisfied; otherwise, false.</returns>
    bool Evaluate();

}

/// <summary>
/// </summary>
public static class IPredicateExtensions {

    /// <summary>
    ///     Creates a simple predicate from a function for compatibility with IPredicate APIs.
    /// </summary>
    [return: NotNull]
    public static IPredicate ToPredicate([NotNull] this Func<bool> func) {
        if (func == null) throw new ArgumentNullException(nameof(func));

        return new Fn(func);
    }

    /// <summary>
    ///     Creates a predicate from a function with argument for compatibility with IPredicate APIs.
    /// </summary>
    [return: NotNull]
    public static IPredicate ToPredicate<T>([NotNull] this Func<T, bool> func, T arg) {
        if (func == null) throw new ArgumentNullException(nameof(func));

        return new Fn(() => func(arg));
    }

}