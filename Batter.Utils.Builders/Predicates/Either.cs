namespace Batter.Utils.Builders.Predicates;

/// <summary>
/// A predicate that performs a logical OR on two other predicates.
/// </summary>
public sealed class Either : IPredicate {
    private readonly IPredicate _left;
    private readonly IPredicate _right;

    /// <summary>
    /// Initializes a new instance of the <see cref="Either"/> class.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    public Either(IPredicate left, IPredicate right) {
        this._left = left ?? throw new ArgumentNullException(nameof(left));
        this._right = right ?? throw new ArgumentNullException(nameof(right));
    }

    /// <summary>
    /// Evaluates the predicate.
    /// </summary>
    /// <returns>True if either predicate evaluates to true; otherwise, false.</returns>
    public bool Evaluate() {
        return this._left.Evaluate() || this._right.Evaluate();
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