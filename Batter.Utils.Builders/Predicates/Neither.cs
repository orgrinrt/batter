#region

    using System.Diagnostics.CodeAnalysis;

#endregion

    namespace Batter.Utils.Builders.Predicates;

    /// <summary>
    /// </summary>
    public sealed class Neither : IPredicate {

        private readonly Either _inner;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Neither" /> class.
        /// </summary>
        /// <param name="left">The first predicate.</param>
        /// <param name="right">The second predicate.</param>
        public Neither(IPredicate left, IPredicate right) { this._inner = new(left, right); }

        /// <inheritdoc />
        public bool Evaluate() { return !this._inner.Evaluate(); }

        /// <inheritdoc />
        public bool Equals(IPredicate? other) {
            if (other is Neither otherNeither) return this._inner.Equals(otherNeither._inner);

            return false;
        }

        /// <inheritdoc />
        public bool Is([NotNull] IPredicate other) { return this.Equals(other); }

        /// <inheritdoc />
        public IPredicate Clone() { return new Neither(this._inner.Clone(), this._inner.Clone()); }


        /// <inheritdoc />
        IPredicate<object> IInto<IPredicate<object>>.Into() { throw new NotImplementedException(); }

        /// <inheritdoc />
        public IPredicate From(IPredicate<object> other) { throw new NotImplementedException(); }

    }