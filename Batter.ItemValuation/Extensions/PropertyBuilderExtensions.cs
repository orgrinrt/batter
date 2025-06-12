#region

using Batter.ItemValuation.Builders;

#endregion

namespace Batter.ItemValuation.Extensions;

/// <summary>
/// Extension methods for PropertyBuilder to add common property operations.
/// </summary>
public static class PropertyBuilderExtensions {
    /// <summary>
    /// Adds a simple additive modifier.
    /// </summary>
    /// <param name="builder">The property builder.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>The builder instance.</returns>
    public static PropertyBuilder AddValue(this PropertyBuilder builder, float value) {
        return builder.WithModifier(mod => mod.Custom(current => current + value));
    }

    /// <summary>
    /// Adds a simple multiplicative modifier.
    /// </summary>
    /// <param name="builder">The property builder.</param>
    /// <param name="factor">The factor to multiply by.</param>
    /// <returns>The builder instance.</returns>
    public static PropertyBuilder MultiplyBy(this PropertyBuilder builder, float factor) {
        return builder.WithModifier(mod => mod.Custom(current => current * factor));
    }

    /// <summary>
    /// Adds a simple additive modifier that only applies when the predicate is true.
    /// </summary>
    /// <param name="conditionalBuilder">The conditional property builder.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>The parent builder instance.</returns>
    public static PropertyBuilder Add(this ConditionalPropertyBuilder conditionalBuilder, float value) {
        conditionalBuilder.ParentBuilder.WithModifier(mod =>
            mod.When(conditionalBuilder.Predicate).Custom(current => current + value)
        );
        return conditionalBuilder.ParentBuilder;
    }

    /// <summary>
    /// Adds a simple multiplicative modifier that only applies when the predicate is true.
    /// </summary>
    /// <param name="conditionalBuilder">The conditional property builder.</param>
    /// <param name="factor">The factor to multiply by.</param>
    /// <returns>The parent builder instance.</returns>
    public static PropertyBuilder Multiply(this ConditionalPropertyBuilder conditionalBuilder, float factor) {
        conditionalBuilder.ParentBuilder.WithModifier(mod =>
            mod.When(conditionalBuilder.Predicate).Custom(current => current * factor)
        );
        return conditionalBuilder.ParentBuilder;
    }

    /// <summary>
    /// Adds a custom modifier that only applies when the predicate is true.
    /// </summary>
    /// <param name="conditionalBuilder">The conditional property builder.</param>
    /// <param name="customModifier">Function that takes a value and returns a modified value.</param>
    /// <returns>The parent builder instance.</returns>
    public static PropertyBuilder Custom(this ConditionalPropertyBuilder conditionalBuilder,
        Func<float, float> customModifier) {
        conditionalBuilder.ParentBuilder.WithModifier(mod =>
            mod.When(conditionalBuilder.Predicate).Custom(customModifier)
        );
        return conditionalBuilder.ParentBuilder;
    }

    /// <summary>
    /// Adds an advanced custom modifier that only applies when the predicate is true.
    /// </summary>
    /// <param name="conditionalBuilder">The conditional property builder.</param>
    /// <param name="advancedModifier">Function that takes the current value and result reference and returns the modified value.</param>
    /// <returns>The parent builder instance.</returns>
    public static PropertyBuilder WithAdvancedModifier(this ConditionalPropertyBuilder conditionalBuilder,
        Func<float, float, float> advancedModifier) {
        conditionalBuilder.ParentBuilder.WithModifier(mod =>
            mod.When(conditionalBuilder.Predicate).CustomAdvanced(advancedModifier)
        );
        return conditionalBuilder.ParentBuilder;
    }
}