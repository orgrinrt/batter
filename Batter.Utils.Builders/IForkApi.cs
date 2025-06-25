#region

    using System.Diagnostics.CodeAnalysis;

    using Batter.Utils.Builders.Predicates;

#endregion

    namespace Batter.Utils.Builders;

    /// <summary>
    ///     Represents a conditional fork in a fluent API that allows branching logic
    ///     based on predicate evaluation. This interface enables Then/Else patterns
    ///     in builder configurations.
    /// </summary>
    /// <typeparam name="TInstance">The type of instance being conditionally modified</typeparam>
    public interface IForkApi<TInstance> {

        /// <summary>
        ///     The original instance before any conditional modifications.
        ///     This serves as the fallback when conditions are not met.
        /// </summary>
        [NotNull]
        protected TInstance _originalInstance { get; }

        /// <summary>
        ///     The instance that will be modified based on conditional logic.
        ///     This represents the branch that gets transformed when conditions are met.
        /// </summary>
        [NotNull]
        protected TInstance _branchInstance { get; set; }

        /// <summary>
        ///     The predicate that determines which branch of logic to execute.
        ///     This condition is evaluated to decide between Then and Else paths.
        /// </summary>
        protected internal IPredicate _predicate { get; }

        /// <summary>
        ///     Executes the provided action on the branch instance if the predicate evaluates to true.
        ///     This method implements the "Then" part of conditional logic in fluent APIs.
        /// </summary>
        /// <param name="action">The transformation to apply when the condition is true</param>
        /// <returns>The fork instance for continued chaining</returns>
        [return: NotNull]
        IForkApi<TInstance> Then([NotNull] Func<TInstance, TInstance> action) {
            if (this._predicate.Evaluate()) this._branchInstance = action.Invoke(this._branchInstance);

            return this;
        }

        /// <summary>
        ///     Executes the provided action on the branch instance if the predicate evaluates to false.
        ///     This method implements the "Else" part of conditional logic in fluent APIs.
        /// </summary>
        /// <param name="action">The transformation to apply when the condition is false</param>
        /// <returns>The fork instance for continued chaining</returns>
        /// <exception cref="InvalidOperationException">May be thrown by the action if it encounters an invalid state</exception>
        [return: NotNull]
        IForkApi<TInstance> Else([NotNull] Func<TInstance, TInstance> action) {
            if (!this._predicate.Evaluate()) this._branchInstance = action.Invoke(this._branchInstance);

            return this;
        }

        /// <summary>
        ///     Returns the appropriate instance based on the predicate evaluation.
        ///     If the predicate is true, returns the modified branch instance;
        ///     otherwise, returns the original unchanged instance.
        /// </summary>
        /// <returns>The resulting instance after conditional processing</returns>
        [return: NotNull]
        TInstance Return() {
            return this._predicate.Evaluate()
                ? this._branchInstance
                : this._originalInstance;
        }

        /// <summary>
        /// </summary>
        /// <param name="conditionalApi"></param>
        /// <param name="predicate"></param>
        /// <typeparam name="TThis"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [return: NotNull]
        static IForkApi<TThis> Create<TThis>(
            [NotNull] IConditionalApi<TThis> conditionalApi,
            [NotNull] IPredicate             predicate
        ) where TThis : IConditionalApi<TThis> {
            throw new NotImplementedException();
        }

    }