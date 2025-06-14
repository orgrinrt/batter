namespace Batter.Utils.Builders.Predicates;

/// <summary>
/// 
/// </summary>
public sealed class Neither : IPredicate {
    private readonly Either _inner;

    /// <summary>
    /// Initializes a new instance of the <see cref="Neither"/> class.
    /// </summary>
    /// <param name="left">The first predicate.</param>
    /// <param name="right">The second predicate.</param>
    public Neither(IPredicate left, IPredicate right) => this._inner = new(left, right);

    /// <inheritdoc />
    public bool Evaluate() {
        return !this._inner.Evaluate();
    }

    /// <inheritdoc />
    public IPredicate When(IPredicate predicate) {
        return new Not(this._inner.When(predicate));
    }

    /// <inheritdoc />
    public IPredicate If(bool condition) {
        return new Not(this._inner.If(condition));
    }

    /// <inheritdoc />
    public IPredicate IfNot(bool condition) {
        return new Not(this._inner.IfNot(condition));
    }

    /// <inheritdoc />
    public IPredicate IfNot(IPredicate predicate) {
        return new Not(this._inner.IfNot(predicate));
    }

    /// <inheritdoc />
    public IPredicate And(bool condition) {
        return new Not(this._inner.And(condition));
    }

    /// <inheritdoc />
    public IPredicate And(IPredicate predicate) {
        return new Not(this._inner.And(predicate));
    }

    /// <inheritdoc />
    public IPredicate AndNot(bool condition) {
        return new Not(this._inner.AndNot(condition));
    }

    /// <inheritdoc />
    public IPredicate AndNot(IPredicate predicate) {
        return new Not(this._inner.AndNot(predicate));
    }

    /// <inheritdoc />
    public IPredicate Or(bool condition) {
        return new Not(this._inner.Or(condition));
    }

    /// <inheritdoc />
    public IPredicate Or(IPredicate predicate) {
        return new Not(this._inner.Or(predicate));
    }

    /// <inheritdoc />
    public bool Equals<TValue>(TValue value) {
        return new Not(this._inner).Equals(value);
    }

    /// <inheritdoc />
    public bool DoesntEqual<TValue>(TValue value) {
        return new Not(this._inner).DoesntEqual(value);
    }

    /// <inheritdoc />
    public bool DoesntEqual(object value) {
        return new Not(this._inner).DoesntEqual(value);
    }

    /// <inheritdoc />
    public IPredicate Matches<TValue>(Func<IPredicate, TValue> op) {
        return new Not(this._inner.Matches(op));
    }

    /// <inheritdoc />
    public IPredicate Matches(Func<IPredicate, object> op) {
        return new Not(this._inner.Matches(op));
    }

    /// <inheritdoc />
    public IPredicate Chain(params Func<IPredicate, object>[] actions) {
        return new Not(this._inner.Chain(actions));
    }

    /// <inheritdoc />
    public IPredicate Chain<TValue>(params Func<IPredicate, TValue>[] action) {
        return new Not(this._inner.Chain(action));
    }
}