namespace Batter.Utils.Builders.Predicates;

/// <summary>
/// Provides simple predicate implementations.
/// </summary>
/// <remarks>
/// This file contains basic predicate implementations that follow the IPredicate interface.
/// For most use cases, the fluent API (Predicates.When()...) is recommended.
/// </remarks>
/// <summary>
/// A predicate that always returns a constant value.
/// </summary>
public sealed class Const : IPredicate {
    private readonly bool _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Const"/> class.
    /// </summary>
    /// <param name="value">The constant value to return.</param>
    public Const(bool value) => this._value = value;

    /// <summary>
    /// Evaluates the predicate.
    /// </summary>
    /// <returns>The constant value this predicate was initialized with.</returns>
    public bool Evaluate() {
        return this._value;
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
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator Const(bool value) {
        return new(value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static implicit operator bool(Const predicate) {
        return predicate._value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static implicit operator Fn(Const predicate) {
        return new(predicate.Evaluate);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static implicit operator Const(Fn predicate) {
        return new(predicate.Evaluate());
    }
}