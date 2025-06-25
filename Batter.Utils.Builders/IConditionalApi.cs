#region

using System.Diagnostics.CodeAnalysis;

using Batter.Utils.Builders.Predicates;

#endregion

namespace Batter.Utils.Builders;

/// <summary>
///     Interface that provides fluent API methods for conditional operations
/// </summary>
/// <typeparam name="TThis">The implementing type, enabling method chaining</typeparam>
public interface IConditionalApi<TThis>
    where TThis : IConditionalApi<TThis> {

    /// <summary>
    ///     Creates a conditional fork in the fluent API based on a predicate.
    ///     This method allows branching logic in builder patterns, enabling
    ///     different configuration paths based on runtime conditions.
    /// </summary>
    /// <param name="predicate">The condition to evaluate for the fork</param>
    /// <returns>A fork instance that can handle Then/Else scenarios</returns>
    [return: NotNull]
    IForkApi<TThis> If([NotNull] IPredicate predicate) { return IForkApi<TThis>.Create(this, predicate); }

}

/// <summary>
///     Extension methods for the IFluentApiMethods interface
/// </summary>
public static class FluentApiMethodsExtensions { }