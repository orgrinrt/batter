namespace Batter.Utils.Builders.Predicates;

public static class PredicateExtensions {
    /// <summary>
    ///     Creates a predicate from a boolean value.
    /// </summary>
    public static IPredicate From(bool value) {
        return new ConstantPredicate(value);
    }

    /// <summary>
    ///     Creates a predicate from a function that evaluates to a boolean.
    /// </summary>
    public static IPredicate From(Func<bool> predicate) {
        return new FuncPredicate(predicate);
    }

    /// <summary>
    ///     Creates a typed predicate from a function that takes an input and returns a boolean.
    /// </summary>
    public static IPredicate<T> From<T>(Func<T, bool> predicate) {
        return new FuncPredicate<T>(predicate);
    }

    /// <summary>
    ///     Combines two predicates with a logical AND.
    /// </summary>
    public static IPredicate<T> And<T>(this IPredicate<T> first, IPredicate<T> second) {
        return new AndPredicate<T>(first, second);
    }

    /// <summary>
    ///     Combines two predicates with a logical OR.
    /// </summary>
    public static IPredicate<T> Or<T>(this IPredicate<T> first, IPredicate<T> second) {
        return new OrPredicate<T>(first, second);
    }

    /// <summary>
    ///     Negates a predicate.
    /// </summary>
    public static IPredicate<T> Not<T>(this IPredicate<T> predicate) {
        return new NotPredicate<T>(predicate);
    }
}