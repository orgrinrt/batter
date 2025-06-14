namespace Batter.Utils.Builders.Predicates;

/// <summary>
/// A predicate that negates another predicate.
/// </summary>
public sealed class Not : IPredicate {
    private readonly IPredicate _predicate;

    /// <summary>
    /// Initializes a new instance of the <see cref="Not"/> class.
    /// </summary>
    /// <param name="predicate">The predicate to negate.</param>
    public Not(IPredicate predicate) =>
        this._predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

    /// <summary>
    /// Evaluates the predicate.
    /// </summary>
    /// <returns>True if the inner predicate evaluates to false; otherwise, false.</returns>
    public bool Evaluate() {
        return !this._predicate.Evaluate();
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
}