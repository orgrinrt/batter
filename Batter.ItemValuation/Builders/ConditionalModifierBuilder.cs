#region

using Batter.Utils.Builders;
using Batter.Utils.Builders.Predicates;

#endregion

namespace Batter.ItemValuation.Builders;

/// <summary>
/// A conditional builder that applies modifications only when a predicate evaluates to true.
/// </summary>
public class ConditionalModifierBuilder : IConditionalBuilder<ModifierBuilder, ConditionalModifierBuilder> {
    /// <summary>
    /// Initializes a new instance of the <see cref="ConditionalModifierBuilder"/> class.
    /// </summary>
    /// <param name="modifierBuilder">The parent modifier builder.</param>
    /// <param name="predicate">The predicate to evaluate.</param>
    public ConditionalModifierBuilder(ModifierBuilder modifierBuilder, IPredicate predicate) {
        this.ParentBuilder = modifierBuilder;
        this.Predicate = predicate;
    }

    /// <summary>
    /// Gets the parent modifier builder.
    /// </summary>
    public ModifierBuilder ParentBuilder { get; }

    /// <summary>
    /// Gets the predicate that determines when modifications are applied.
    /// </summary>
    public IPredicate Predicate { get; }

    /// <summary>
    /// Returns the parent builder to continue the chain.
    /// </summary>
    /// <returns>The parent builder.</returns>
    public ModifierBuilder EndCondition() {
        return this.ParentBuilder;
    }
}