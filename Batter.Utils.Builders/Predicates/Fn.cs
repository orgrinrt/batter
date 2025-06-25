#region

    using System.Diagnostics.CodeAnalysis;

#endregion

    namespace Batter.Utils.Builders.Predicates;

    /// <summary>
    ///     A predicate based on a function.
    /// </summary>
    public sealed partial class Fn : IPredicate {

        private readonly Func<FnParams, bool> _func;
        private          FnParams             _params;

        private Fn(Func<FnParams, bool> func) {
            this._func   = func ?? throw new ArgumentNullException(nameof(func));
            this._params = new();
        }

        // NOTE: See `FnExtObj` and `FnExtTyped` for ctor impls

        /// <summary>
        ///     Evaluates the predicate.
        /// </summary>
        /// <returns>The result of the function evaluation.</returns>
        public bool Evaluate() {
            if (this._params.Args.Length < this._params.ExpectedArgsCount) {
                throw new ArgumentException(
                    $"Expected at least {this._params.ExpectedArgsCount} arguments, but got {this._params.Args.Length}.");
            }

            return this._func(this._params);
        }

        /// <inheritdoc />
        public bool Equals(IPredicate? other) {
            if (other is Fn otherFn)
                return object.ReferenceEquals(this._func, otherFn._func) && this._params.Equals(otherFn._params);

            return false;
        }

        /// <inheritdoc />
        public bool Is([NotNull] IPredicate other) { return this.Equals(other); }

        /// <inheritdoc />
        public IPredicate Clone() { return new Fn((Func<FnParams, bool>)this._func.Clone()) { _params = this._params }; }

        /// <inheritdoc />
        IPredicate<object> IInto<IPredicate<object>>.Into() { throw new NotImplementedException(); }

        /// <inheritdoc />
        public IPredicate From(IPredicate<object> other) { throw new NotImplementedException(); }


        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool Evaluate(params object[] args) {
            this._params = new(args);

            return this.Evaluate();
        }

        /// <summary>
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static implicit operator Fn(Func<bool> func) { return new(func); }

        /// <summary>
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static implicit operator Func<bool>(Fn fn) { return () => fn._func(fn._params); }

    }