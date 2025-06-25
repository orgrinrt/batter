#region

    using System.Diagnostics.CodeAnalysis;

#endregion

    namespace Batter.Utils.Builders.Predicates;

    /// <summary>
    ///     A predicate that negates another predicate.
    /// </summary>
    public sealed class Not : IPredicate {

        private readonly IPredicate _predicate;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Not" /> class.
        /// </summary>
        /// <param name="predicate">The predicate to negate.</param>
        public Not(IPredicate predicate) {
            this._predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        /// <summary>
        ///     Evaluates the predicate.
        /// </summary>
        /// <returns>True if the inner predicate evaluates to false; otherwise, false.</returns>
        public bool Evaluate() { return !this._predicate.Evaluate(); }

        /// <inheritdoc />
        public bool Equals(IPredicate? other) {
            if (other is Not otherNot) return this._predicate.Equals(otherNot._predicate);

            return false;
        }

        /// <inheritdoc />
        public bool Is([NotNull] IPredicate other) { return this.Equals(other); }

        /// <inheritdoc />
        public IPredicate Clone() { return new Not(this._predicate.Clone()); }


        /// <inheritdoc />
        IPredicate<object> IInto<IPredicate<object>>.Into() { throw new NotImplementedException(); }

        /// <inheritdoc />
        public IPredicate From(IPredicate<object> other) { throw new NotImplementedException(); }

    }