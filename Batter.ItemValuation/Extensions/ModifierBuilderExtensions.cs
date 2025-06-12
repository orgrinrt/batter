#region

using Batter.ItemValuation.Builders;

#endregion

namespace Batter.ItemValuation.Extensions;

/// <summary>
/// Extension methods for ModifierBuilder to add common modifier operations.
/// </summary>
public static class ModifierBuilderExtensions {
    /// <summary>
    /// Adds a simple additive modifier that adds the specified value.
    /// </summary>
    /// <param name="builder">The modifier builder.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>The builder instance.</returns>
    public static ModifierBuilder Add(this ModifierBuilder builder, float value) {
        return builder.Custom(current => current + value);
    }

    /// <summary>
    /// Adds a simple multiplicative modifier that multiplies by the specified factor.
    /// </summary>
    /// <param name="builder">The modifier builder.</param>
    /// <param name="factor">The factor to multiply by.</param>
    /// <returns>The builder instance.</returns>
    public static ModifierBuilder Multiply(this ModifierBuilder builder, float factor) {
        return builder.Custom(current => current * factor);
    }

    /// <summary>
    /// Adds a simple additive modifier that only applies when the predicate is true.
    /// </summary>
    /// <param name="conditionalBuilder">The conditional modifier builder.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>The parent builder instance.</returns>
    public static ModifierBuilder Add(this ConditionalModifierBuilder conditionalBuilder, float value) {
        var parentBuilder = conditionalBuilder.ParentBuilder;
        var predicate = conditionalBuilder.Predicate;

        parentBuilder.Custom(current =>
            predicate.Evaluate() ? current + value : current
        );

        return parentBuilder;
    }

    /// <summary>
    /// Adds a simple multiplicative modifier that only applies when the predicate is true.
    /// </summary>
    /// <param name="conditionalBuilder">The conditional modifier builder.</param>
    /// <param name="factor">The factor to multiply by.</param>
    /// <returns>The parent builder instance.</returns>
    public static ModifierBuilder Multiply(this ConditionalModifierBuilder conditionalBuilder, float factor) {
        var parentBuilder = conditionalBuilder.ParentBuilder;
        var predicate = conditionalBuilder.Predicate;

        parentBuilder.Custom(current =>
            predicate.Evaluate() ? current * factor : current
        );

        return parentBuilder;
    }

    /// <summary>
    /// Adds a custom modifier that only applies when the predicate is true.
    /// </summary>
    /// <param name="conditionalBuilder">The conditional modifier builder.</param>
    /// <param name="customModifier">Function that takes a value and returns a modified value.</param>
    /// <returns>The parent builder instance.</returns>
    public static ModifierBuilder Custom(this ConditionalModifierBuilder conditionalBuilder,
        Func<float, float> customModifier) {
        var parentBuilder = conditionalBuilder.ParentBuilder;
        var predicate = conditionalBuilder.Predicate;

        parentBuilder.Custom(current =>
            predicate.Evaluate() ? customModifier(current) : current
        );

        return parentBuilder;
    }

    /// <summary>
    /// Adds an advanced custom modifier that only applies when the predicate is true.
    /// </summary>
    /// <param name="conditionalBuilder">The conditional modifier builder.</param>
    /// <param name="advancedModifier">Function that takes the current value and result reference and returns the modified value.</param>
    /// <returns>The parent builder instance.</returns>
    public static ModifierBuilder CustomAdvanced(this ConditionalModifierBuilder conditionalBuilder,
        Func<float, float, float> advancedModifier) {
        var parentBuilder = conditionalBuilder.ParentBuilder;
        var predicate = conditionalBuilder.Predicate;

        parentBuilder.CustomAdvanced((current, result) =>
            predicate.Evaluate() ? advancedModifier(current, result) : current
        );

        return parentBuilder;
    }
}