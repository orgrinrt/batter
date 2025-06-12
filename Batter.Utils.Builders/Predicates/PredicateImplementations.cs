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
public sealed class ConstantPredicate : IPredicate {
    private readonly bool _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConstantPredicate"/> class.
    /// </summary>
    /// <param name="value">The constant value to return.</param>
    public ConstantPredicate(bool value) => this._value = value;

    /// <summary>
    /// Evaluates the predicate.
    /// </summary>
    /// <returns>The constant value this predicate was initialized with.</returns>
    public bool Evaluate() {
        return this._value;
    }
}

/// <summary>
/// A predicate based on a function.
/// </summary>
public sealed class FuncPredicate : IPredicate {
    private readonly Func<bool> _func;

    /// <summary>
    /// Initializes a new instance of the <see cref="FuncPredicate"/> class.
    /// </summary>
    /// <param name="func">The function to evaluate.</param>
    public FuncPredicate(Func<bool> func) => this._func = func ?? throw new ArgumentNullException(nameof(func));

    /// <summary>
    /// Evaluates the predicate.
    /// </summary>
    /// <returns>The result of the function evaluation.</returns>
    public bool Evaluate() {
        return this._func();
    }
}

/// <summary>
/// A predicate that performs a logical AND on two other predicates.
/// </summary>
public sealed class AndPredicate : IPredicate {
    private readonly IPredicate _left;
    private readonly IPredicate _right;

    /// <summary>
    /// Initializes a new instance of the <see cref="AndPredicate"/> class.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    public AndPredicate(IPredicate left, IPredicate right) {
        this._left = left ?? throw new ArgumentNullException(nameof(left));
        this._right = right ?? throw new ArgumentNullException(nameof(right));
    }

    /// <summary>
    /// Evaluates the predicate.
    /// </summary>
    /// <returns>True if both predicates evaluate to true; otherwise, false.</returns>
    public bool Evaluate() {
        return this._left.Evaluate() && this._right.Evaluate();
    }
}

/// <summary>
/// A predicate that performs a logical OR on two other predicates.
/// </summary>
public sealed class OrPredicate : IPredicate {
    private readonly IPredicate _left;
    private readonly IPredicate _right;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrPredicate"/> class.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    public OrPredicate(IPredicate left, IPredicate right) {
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
}

/// <summary>
/// A predicate that negates another predicate.
/// </summary>
public sealed class NotPredicate : IPredicate {
    private readonly IPredicate _predicate;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotPredicate"/> class.
    /// </summary>
    /// <param name="predicate">The predicate to negate.</param>
    public NotPredicate(IPredicate predicate) =>
        this._predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

    /// <summary>
    /// Evaluates the predicate.
    /// </summary>
    /// <returns>True if the inner predicate evaluates to false; otherwise, false.</returns>
    public bool Evaluate() {
        return !this._predicate.Evaluate();
    }
}