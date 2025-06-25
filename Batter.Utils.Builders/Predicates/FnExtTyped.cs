#region

    using System.Diagnostics.CodeAnalysis;

#endregion

    namespace Batter.Utils.Builders.Predicates;

    /// <summary>
    /// </summary>
    public abstract class TypedFn : IPredicate {

        /// <summary>
        /// </summary>
        protected Fn? _inner;

        /// <summary>
        /// </summary>
        protected TypedFn() { this._inner = null; }

        /// <inheritdoc />
        public bool Evaluate() { return (this._inner ?? throw new NullReferenceException()).Evaluate(); }

        /// <inheritdoc />
        public bool Equals(IPredicate? other) { throw new NotImplementedException(); }

        /// <inheritdoc />
        public bool Is([NotNull] IPredicate other) { throw new NotImplementedException(); }

        /// <inheritdoc />
        public IPredicate Clone() { throw new NotImplementedException(); }


        /// <inheritdoc />
        IPredicate<object> IInto<IPredicate<object>>.Into() { throw new NotImplementedException(); }

        /// <inheritdoc />
        public IPredicate From(IPredicate<object> other) { throw new NotImplementedException(); }

    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Fn<T> : TypedFn {

        /// <summary>
        ///     Initializes a new instance of the <see cref="Fn" /> class.
        /// </summary>
        /// <param name="func">The function to evaluate.</param>
        public Fn(Func<T, bool> func) {
            this._inner = new(func as Func<object, bool> ?? throw new InvalidOperationException());
        }

    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public sealed class Fn<T1, T2> : TypedFn {

        /// <summary>
        ///     Initializes a new instance of the <see cref="Fn" /> class.
        /// </summary>
        /// <param name="func">The function to evaluate.</param>
        public Fn(Func<T1, T2, bool> func) {
            this._inner = new(func as Func<object, object, bool> ?? throw new InvalidOperationException());
        }

    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public sealed class Fn<T1, T2, T3> : TypedFn {

        /// <summary>
        ///     Initializes a new instance of the <see cref="Fn" /> class.
        /// </summary>
        /// <param name="func">The function to evaluate.</param>
        public Fn(Func<T1, T2, T3, bool> func) {
            this._inner = new(func as Func<object, object, object, bool> ?? throw new InvalidOperationException());
        }

    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    public sealed class Fn<T1, T2, T3, T4> : TypedFn {

        /// <summary>
        ///     Initializes a new instance of the <see cref="Fn" /> class.
        /// </summary>
        /// <param name="func">The function to evaluate.</param>
        public Fn(Func<T1, T2, T3, T4, bool> func) {
            this._inner = new(func as Func<object, object, object, object, bool> ?? throw new InvalidOperationException());
        }

    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    public sealed class Fn<T1, T2, T3, T4, T5> : TypedFn {

        /// <summary>
        ///     Initializes a new instance of the <see cref="Fn" /> class.
        /// </summary>
        /// <param name="func">The function to evaluate.</param>
        public Fn(Func<T1, T2, T3, T4, T5, bool> func) {
            this._inner = new(func as Func<object, object, object, object, object, bool> ??
                              throw new InvalidOperationException());
        }

    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    public sealed class Fn<T1, T2, T3, T4, T5, T6> : TypedFn {

        /// <summary>
        ///     Initializes a new instance of the <see cref="Fn" /> class.
        /// </summary>
        /// <param name="func">The function to evaluate.</param>
        public Fn(Func<T1, T2, T3, T4, T5, T6, bool> func) {
            this._inner = new(func as Func<object, object, object, object, object, object, bool> ??
                              throw new InvalidOperationException());
        }

    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    public sealed class Fn<T1, T2, T3, T4, T5, T6, T7> : TypedFn {

        /// <summary>
        ///     Initializes a new instance of the <see cref="Fn" /> class.
        /// </summary>
        /// <param name="func">The function to evaluate.</param>
        public Fn(Func<T1, T2, T3, T4, T5, T6, T7, bool> func) {
            this._inner = new(func as Func<object, object, object, object, object, object, object, bool> ??
                              throw new InvalidOperationException());
        }

    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    public sealed class Fn<T1, T2, T3, T4, T5, T6, T7, T8> : TypedFn {

        /// <summary>
        ///     Initializes a new instance of the <see cref="Fn" /> class.
        /// </summary>
        /// <param name="func">The function to evaluate.</param>
        public Fn(Func<T1, T2, T3, T4, T5, T6, T7, T8, bool> func) {
            this._inner = new(func as Func<object, object, object, object, object, object, object, object, bool> ??
                              throw new InvalidOperationException());
        }

    }