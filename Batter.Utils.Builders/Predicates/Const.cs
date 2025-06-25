#region

    using System.Diagnostics.CodeAnalysis;

#endregion

    namespace Batter.Utils.Builders.Predicates;

    /// <summary>
    ///     Provides simple predicate implementations.
    /// </summary>
    /// <remarks>
    ///     This file contains basic predicate implementations that follow the IPredicate interface.
    ///     For most use cases, the fluent API (Predicates.When()...) is recommended.
    /// </remarks>
    /// <summary>
    ///     A predicate that always returns a constant value.
    /// </summary>
    public sealed class Const : IPredicate {

        private readonly bool _value;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Const" /> class.
        /// </summary>
        /// <param name="value">The constant value to return.</param>
        public Const(bool value) { this._value = value; }

        /// <summary>
        ///     Evaluates the predicate.
        /// </summary>
        /// <returns>The constant value this predicate was initialized with.</returns>
        public bool Evaluate() { return this._value; }

        /// <inheritdoc />
        public bool Equals(IPredicate? other) {
            if (other is Const otherConst) return this._value == otherConst._value;

            return false;
        }

        /// <inheritdoc />
        public bool Is([NotNull] IPredicate other) { return this.Equals(other); }

        /// <inheritdoc />
        public IPredicate Clone() { return new Const(this._value); }

        /// <inheritdoc />
        IPredicate<object> IInto<IPredicate<object>>.Into() { throw new NotImplementedException(); }

        /// <inheritdoc />
        public IPredicate From(IPredicate<object> other) { throw new NotImplementedException(); }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Const(bool value) { return new(value); }

        /// <summary>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static implicit operator bool(Const predicate) { return predicate._value; }

        /// <summary>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static implicit operator Fn(Const predicate) { return new(predicate.Evaluate); }

        /// <summary>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static implicit operator Const(Fn predicate) { return new(predicate.Evaluate()); }

    }