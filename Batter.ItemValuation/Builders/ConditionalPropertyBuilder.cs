#region

using Batter.Utils.Builders;
using Batter.Utils.Builders.Predicates;

#endregion

namespace Batter.ItemValuation.Builders;

/// <summary>
/// A conditional builder that applies modifiers only when a predicate evaluates to true.
/// </summary>
public class ConditionalPropertyBuilder : IConditionalBuilder<PropertyBuilder, ConditionalPropertyBuilder> {
    /// <summary>
    /// Initializes a new instance of the <see cref="ConditionalPropertyBuilder"/> class.
    /// </summary>
    /// <param name="propertyBuilder">The parent property builder.</param>
    /// <param name="predicate">The predicate to evaluate.</param>
    public ConditionalPropertyBuilder(PropertyBuilder propertyBuilder, IPredicate predicate) {
        this.ParentBuilder = propertyBuilder;
        this.Predicate = predicate;
    }

    /// <summary>
    /// Gets the parent property builder.
    /// </summary>
    public PropertyBuilder ParentBuilder { get; }

    /// <summary>
    /// Gets the predicate that determines when modifiers are applied.
    /// </summary>
    public IPredicate Predicate { get; }

    /// <summary>
    /// Returns the parent builder to continue the chain.
    /// </summary>
    /// <returns>The parent property builder to continue the chain.</returns>
    public PropertyBuilder EndCondition() {
        return this.ParentBuilder;
    }
}