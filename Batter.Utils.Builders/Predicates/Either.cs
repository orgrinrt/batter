#region

    using System.Diagnostics.CodeAnalysis;

#endregion

    namespace Batter.Utils.Builders.Predicates;

    /// <summary>
    ///     A predicate that performs a logical OR on two other predicates.
    /// </summary>
    public sealed class Either : IPredicate {

        private readonly IPredicate _left;
        private readonly IPredicate _right;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Either" /> class.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        public Either(IPredicate left, IPredicate right) {
            this._left  = left  ?? throw new ArgumentNullException(nameof(left));
            this._right = right ?? throw new ArgumentNullException(nameof(right));
        }

        /// <summary>
        ///     Evaluates the predicate.
        /// </summary>
        /// <returns>True if either predicate evaluates to true; otherwise, false.</returns>
        public bool Evaluate() { return this._left.Evaluate() || this._right.Evaluate(); }

        /// <inheritdoc />
        public bool Equals(IPredicate? other) {
            if (other is Either otherEither)
                return this._left.Equals(otherEither._left) && this._right.Equals(otherEither._right);

            return false;
        }

        /// <inheritdoc />
        public bool Is([NotNull] IPredicate other) { return this.Equals(other); }

        /// <inheritdoc />
        public IPredicate Clone() { return new Either(this._left.Clone(), this._right.Clone()); }


        /// <inheritdoc />
        IPredicate<object> IInto<IPredicate<object>>.Into() { throw new NotImplementedException(); }

        /// <inheritdoc />
        public IPredicate From(IPredicate<object> other) { throw new NotImplementedException(); }

    }