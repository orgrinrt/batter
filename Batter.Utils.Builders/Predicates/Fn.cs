namespace Batter.Utils.Builders.Predicates;

/// <summary>
/// A predicate based on a function.
/// </summary>
public sealed partial class Fn : IPredicate {
    private readonly  Func<FnParams, bool> _func;
    private  FnParams _params;

    // NOTE: See `FnExtObj` and `FnExtTyped` for ctor impls

    /// <summary>
    /// Evaluates the predicate.
    /// </summary>
    /// <returns>The result of the function evaluation.</returns>
    public bool Evaluate() {
        if (this._params.Args.Length < this._params.ExpectedArgsCount)
            throw new ArgumentException(
                $"Expected at least {this._params.ExpectedArgsCount} arguments, but got {this._params.Args.Length}.");
        return this._func(this._params);
    }

    /// <inheritdoc />
    public IPredicate When(IPredicate predicate) {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IPredicate If(bool condition) {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IPredicate IfNot(bool condition) {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IPredicate IfNot(IPredicate predicate) {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IPredicate And(bool condition) {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IPredicate And(IPredicate predicate) {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IPredicate AndNot(bool condition) {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IPredicate AndNot(IPredicate predicate) {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IPredicate Or(bool condition) {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IPredicate Or(IPredicate predicate) {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool Equals<TValue>(TValue value) {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool DoesntEqual<TValue>(TValue value) {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool DoesntEqual(object value) {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IPredicate Matches<TValue>(Func<IPredicate, TValue> op) {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IPredicate Matches(Func<IPredicate, object> op) {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IPredicate Chain(params Func<IPredicate, object>[] actions) {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IPredicate Chain<TValue>(params Func<IPredicate, TValue>[] action) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public bool Evaluate(params object[] args) {
        this._params = new(args);
        return this.Evaluate();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public static implicit operator Fn(Func<bool> func) {
        return new(func);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fn"></param>
    /// <returns></returns>
    public static implicit operator Func<bool>(Fn fn) {
        return () => fn._func(fn._params);
    }
}