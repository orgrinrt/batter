#region

    using System.Diagnostics.CodeAnalysis;

#endregion

    namespace Batter.Utils.Builders.Predicates;

    /// <summary>
    ///     A predicate that performs a logical AND on two other predicates.
    /// </summary>
    public sealed class Combine : IPredicate {

        [NotNull] private readonly IPredicate _left;
        [NotNull] private readonly IPredicate _right;

        /// <summary>
        /// </summary>
        /// <param name="predicates"></param>
        /// <exception cref="ArgumentException"></exception>
        public Combine(params IPredicate[] predicates) {
            if (predicates is null || predicates.Length < 2)
                throw new ArgumentException("At least two predicates are required.", nameof(predicates));

            if (predicates.Any(p => p is null))
                throw new ArgumentException("Predicates cannot be null.", nameof(predicates));
            this._left  = predicates[0];
            this._right = predicates[1];
            var rest = predicates.Skip(2).ToArray();

            foreach (var predicate in rest) {
                if (predicate is null) throw new ArgumentException("Predicates cannot be null.", nameof(predicates));
                this._right = new Combine(this._right, predicate);
            }
        }

        /// <summary>
        ///     Evaluates the predicate.
        /// </summary>
        /// <returns>True if both predicates evaluate to true; otherwise, false.</returns>
        public bool Evaluate() { return this._left.Evaluate() && this._right.Evaluate(); }

        /// <inheritdoc />
        public bool Equals(IPredicate? other) {
            if (other is Combine otherCombine)
                return this._left.Equals(otherCombine._left) && this._right.Equals(otherCombine._right);

            return false;
        }

        /// <inheritdoc />
        public bool Is([NotNull] IPredicate other) { return this.Equals(other); }

        /// <inheritdoc />
        public IPredicate Clone() { return new Combine(this._left.Clone(), this._right.Clone()); }

        /// <inheritdoc />
        IPredicate<object> IInto<IPredicate<object>>.Into() { throw new NotImplementedException(); }

        /// <inheritdoc />
        public IPredicate From(IPredicate<object> other) { throw new NotImplementedException(); }


        /// <summary>
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static implicit operator Combine((IPredicate left, IPredicate right) tuple) {
            return new(tuple.left, tuple.right);
        }

        /// <summary>
        /// </summary>
        /// <param name="combination"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static implicit operator (IPredicate left, IPredicate right)(Combine combination) {
            return combination is null
                ? throw new ArgumentNullException(nameof(combination), "Combination cannot be null.")
                : (combination._left, combination._right);
        }

        /// <summary>
        /// </summary>
        /// <param name="combination"></param>
        /// <returns></returns>
        public static implicit operator Fn(Combine combination) {
            return combination is null
                ? throw new ArgumentNullException(nameof(combination), "Combination cannot be null.")
                : new Fn(combination.Evaluate);
        }

        /// <summary>
        /// </summary>
        /// <param name="combination"></param>
        /// <returns></returns>
        public static implicit operator Const(Combine combination) {
            return combination is null
                ? throw new ArgumentNullException(nameof(combination), "Combination cannot be null.")
                : new Const(combination.Evaluate());
        }

    }

    /// <summary>
    /// </summary>
    public static class CombinationExtensions {

        /// <summary>
        ///     Combines two predicates using logical AND.
        /// </summary>
        /// <param name="left">The left predicate.</param>
        /// <param name="right">The right predicate.</param>
        /// <returns>A new predicate that combines the two.</returns>
        public static IPredicate And(this IPredicate left, IPredicate right) { return new Combine(left, right); }

        /// <summary>
        ///     Combines multiple predicates using logical AND.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="rest">An array of predicates to combine.</param>
        /// <returns>A new predicate that combines all provided predicates.</returns>
        public static IPredicate And(this IPredicate first, IEnumerable<IPredicate> rest) {
            var p = rest.Prepend(first);

            return p.Aggregate((current, next) => current.And(next));
        }

        /// <summary>
        /// </summary>
        /// <param name="first"></param>
        /// <param name="rest"></param>
        /// <returns></returns>
        public static IPredicate And(this IPredicate first, params IPredicate[] rest) {
            return first.And(rest.AsEnumerable());
        }

    }