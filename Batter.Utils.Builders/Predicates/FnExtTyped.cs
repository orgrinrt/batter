namespace Batter.Utils.Builders.Predicates;

/// <summary>
/// 
/// </summary>
public abstract class TypedFn : IPredicate {
    /// <summary>
    /// 
    /// </summary>
    protected  Fn? _inner;

    /// <summary>
    /// 
    /// </summary>
    protected TypedFn() => this._inner = null;

    /// <inheritdoc />
    public bool Evaluate() {
        return (this._inner ?? throw new NullReferenceException()).Evaluate();
    }

    /// <inheritdoc />
    public IPredicate When(IPredicate predicate) {
        return this._inner?.When(predicate) ?? throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public IPredicate If(bool condition) {
        return this._inner?.If(condition) ?? throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public IPredicate IfNot(bool condition) {
        return this._inner?.IfNot(condition) ?? throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public IPredicate IfNot(IPredicate predicate) {
        return this._inner?.IfNot(predicate) ?? throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public IPredicate And(bool condition) {
        return this._inner?.And(condition) ?? throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public IPredicate And(IPredicate predicate) {
        return this._inner?.And(predicate) ?? throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public IPredicate AndNot(bool condition) {
        return this._inner?.AndNot(condition) ?? throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public IPredicate AndNot(IPredicate predicate) {
        return this._inner?.AndNot(predicate) ?? throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public IPredicate Or(bool condition) {
        return this._inner?.Or(condition) ?? throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public IPredicate Or(IPredicate predicate) {
        return this._inner?.Or(predicate) ?? throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public bool Equals<TValue>(TValue value) {
        return this._inner?.Equals(value) ?? throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public bool DoesntEqual<TValue>(TValue value) {
        return this._inner?.DoesntEqual(value) ?? throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public bool DoesntEqual(object value) {
        return this._inner?.DoesntEqual(value) ?? throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public IPredicate Matches<TValue>(Func<IPredicate, TValue> op) {
        return this._inner?.Matches(op) ?? throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public IPredicate Matches(Func<IPredicate, object> op) {
        return this._inner?.Matches(op) ?? throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public IPredicate Chain(params Func<IPredicate, object>[] actions) {
        return this._inner?.Chain(actions) ?? throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public IPredicate Chain<TValue>(params Func<IPredicate, TValue>[] action) {
        return this._inner?.Chain(action) ?? throw new InvalidOperationException();
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed  class Fn<T> : TypedFn {
    /// <summary>
    /// Initializes a new instance of the <see cref="Fn"/> class.
    /// </summary>
    /// <param name="func">The function to evaluate.</param>
    public Fn(Func<T, bool> func) =>
        this._inner = new(func as Func<object, bool> ?? throw new InvalidOperationException());
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
public sealed class Fn<T1, T2> : TypedFn {
    /// <summary>
    /// Initializes a new instance of the <see cref="Fn"/> class.
    /// </summary>
    /// <param name="func">The function to evaluate.</param>
    public Fn(Func<T1, T2, bool> func) =>
        this._inner = new(func as Func<object, object, bool> ?? throw new InvalidOperationException());
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
public sealed class Fn<T1, T2, T3> : TypedFn {
    /// <summary>
    /// Initializes a new instance of the <see cref="Fn"/> class.
    /// </summary>
    /// <param name="func">The function to evaluate.</param>
    public Fn(Func<T1, T2, T3, bool> func) =>
        this._inner = new(func as Func<object, object, object, bool> ?? throw new InvalidOperationException());
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
public sealed class Fn<T1, T2, T3, T4> : TypedFn {
    /// <summary>
    /// Initializes a new instance of the <see cref="Fn"/> class.
    /// </summary>
    /// <param name="func">The function to evaluate.</param>
    public Fn(Func<T1, T2, T3, T4, bool> func) =>
        this._inner = new(func as Func<object, object, object, object, bool> ?? throw new InvalidOperationException());
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
public sealed class Fn<T1, T2, T3, T4, T5> : TypedFn {
    /// <summary>
    /// Initializes a new instance of the <see cref="Fn"/> class.
    /// </summary>
    /// <param name="func">The function to evaluate.</param>
    public Fn(Func<T1, T2, T3, T4, T5, bool> func) =>
        this._inner = new(func as Func<object, object, object, object, object, bool> ??
                          throw new InvalidOperationException());
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
/// <typeparam name="T6"></typeparam>
public sealed class Fn<T1, T2, T3, T4, T5, T6> : TypedFn {
    /// <summary>
    /// Initializes a new instance of the <see cref="Fn"/> class.
    /// </summary>
    /// <param name="func">The function to evaluate.</param>
    public Fn(Func<T1, T2, T3, T4, T5, T6, bool> func) =>
        this._inner = new(func as Func<object, object, object, object, object, object, bool> ??
                          throw new InvalidOperationException());
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
/// <typeparam name="T6"></typeparam>
/// <typeparam name="T7"></typeparam>
public sealed class Fn<T1, T2, T3, T4, T5, T6, T7> : TypedFn {
    /// <summary>
    /// Initializes a new instance of the <see cref="Fn"/> class.
    /// </summary>
    /// <param name="func">The function to evaluate.</param>
    public Fn(Func<T1, T2, T3, T4, T5, T6, T7, bool> func) =>
        this._inner = new(func as Func<object, object, object, object, object, object, object, bool> ??
                          throw new InvalidOperationException());
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
/// <typeparam name="T6"></typeparam>
/// <typeparam name="T7"></typeparam>
/// <typeparam name="T8"></typeparam>
public sealed class Fn<T1, T2, T3, T4, T5, T6, T7, T8> : TypedFn {
    /// <summary>
    /// Initializes a new instance of the <see cref="Fn"/> class.
    /// </summary>
    /// <param name="func">The function to evaluate.</param>
    public Fn(Func<T1, T2, T3, T4, T5, T6, T7, T8, bool> func) =>
        this._inner = new(func as Func<object, object, object, object, object, object, object, object, bool> ??
                          throw new InvalidOperationException());
}