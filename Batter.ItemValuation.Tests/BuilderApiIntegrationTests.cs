#region

using Batter.ItemValuation.Builders;
using Batter.ItemValuation.Extensions;
using Batter.ItemValuation.Tests.Mocks;
using Batter.Utils.Builders.Predicates;
using NUnit.Framework;

#endregion

namespace Batter.ItemValuation.Tests;

[TestFixture]
[Category("BuilderAPI")]
[Description("Tests the full builder API with complex scenarios")]
public class BuilderApiIntegrationTests {
    [SetUp]
    public void Setup() {
        this._mockItem = new("test_weapon_01");
    }

    private MockItemObject _mockItem;

    // Helper predicates for testing
    private static bool AlwaysTrue() {
        return true;
    }

    private static bool AlwaysFalse() {
        return false;
    }

    // Using the new predicate system - simple predicates
    private static readonly IPredicate TruePredicate = Predicates.Always();
    private static readonly IPredicate FalsePredicate = Predicates.Never();

    [Test]
    [Description("Tests a complex weapon evaluation with multiple properties and modifiers")]
    public void ComplexWeaponEvaluation_WithMultiplePropertiesAndModifiers_ReturnsExpectedValues() {
        // Arrange
        // Simulate game conditions that would affect item evaluation
        var isTwoHanded = true;
        var isCrafted = true;
        var hasLegendaryPrefix = true;
        var playerSmithingSkill = 250;
        var qualityMultiplier = 1.35f;
        var rarityBonus = 1.2f;

        // Create a complex evaluated item using the improved fluent builder API
        var evaluatedItem = new ItemBuilder(this._mockItem)
            // Basic weapon properties with fluent conditional modifiers
            .WithProperty("BaseValue", prop => prop
                .WithBaseValue(1000f)
                .If(() => isCrafted).Multiply(1.15f)
                .If(() => hasLegendaryPrefix).Multiply(rarityBonus))

            // Damage property with conditional modifiers
            .WithProperty("Damage", prop => prop
                .WithBaseValue(45f)
                .If(isTwoHanded).Multiply(1.5f)
                .MultiplyBy(qualityMultiplier))

            // Weight with additive and multiplicative modifiers
            .WithProperty("Weight", prop => prop
                .WithBaseValue(2.2f)
                .AddValue(0.3f)
                .If(isTwoHanded).Add(0.8f))

            // Handle Length with custom modifier
            .WithProperty("HandleLength", prop => prop
                .WithBaseValue(55f)
                .WithModifier(mod => mod
                    .If(isTwoHanded).Custom(value => value + 15f)))

            // Crafting bonus that only applies to crafted weapons using conditional builder
            .If(isCrafted).WithProperty("CraftingBonus", prop => prop
                .WithBaseValue(playerSmithingSkill / 2f)
                .WithModifier(mod => mod.Custom(value =>
                    value * (1f + (playerSmithingSkill > 200 ? 0.25f : 0.1f)))))

            // Conditional negative property for balance
            .If(isTwoHanded && isCrafted).WithProperty("HandlingPenalty", prop => prop
                .WithBaseValue(-15f)
                .MultiplyBy(1.2f))
            .Build();

        // Act
        var baseValue = this.GetPropertyValue(evaluatedItem, "BaseValue");
        var damage = this.GetPropertyValue(evaluatedItem, "Damage");
        var weight = this.GetPropertyValue(evaluatedItem, "Weight");
        var handleLength = this.GetPropertyValue(evaluatedItem, "HandleLength");
        var craftingBonus = this.GetPropertyValue(evaluatedItem, "CraftingBonus");
        var handlingPenalty = this.GetPropertyValue(evaluatedItem, "HandlingPenalty");

        // Calculate expected values manually
        var expectedBaseValue = 1000f * 1.15f * 1.2f; // Base * crafted * legendary
        var expectedDamage = 45f * 1.5f * 1.35f; // Base * two-handed * quality
        var expectedWeight = 2.2f + 0.3f + 0.8f; // Base + additive + two-handed bonus
        var expectedHandleLength = 55f + 15f; // Base + two-handed bonus
        var expectedCraftingBonus = (250f / 2f) * (1f + 0.25f); // (Skill/2) * (1 + skill bonus)
        var expectedHandlingPenalty = -15f * 1.2f; // Base * multiplier

        // Calculate total value
        var totalValue = baseValue + damage + weight + handleLength + craftingBonus + handlingPenalty;
        var expectedTotal = expectedBaseValue + expectedDamage + expectedWeight +
                            expectedHandleLength + expectedCraftingBonus + expectedHandlingPenalty;

        // Assert
        Assert.That(baseValue, Is.EqualTo(expectedBaseValue).Within(0.001f), "BaseValue calculation incorrect");
        Assert.That(damage, Is.EqualTo(expectedDamage).Within(0.001f), "Damage calculation incorrect");
        Assert.That(weight, Is.EqualTo(expectedWeight).Within(0.001f), "Weight calculation incorrect");
        Assert.That(handleLength, Is.EqualTo(expectedHandleLength).Within(0.001f),
            "HandleLength calculation incorrect");
        Assert.That(craftingBonus, Is.EqualTo(expectedCraftingBonus).Within(0.001f),
            "CraftingBonus calculation incorrect");
        Assert.That(handlingPenalty, Is.EqualTo(expectedHandlingPenalty).Within(0.001f),
            "HandlingPenalty calculation incorrect");
        Assert.That(totalValue, Is.EqualTo(expectedTotal).Within(0.001f), "Total value calculation incorrect");
    }

    [Test]
    [Description("Tests various predicate combinations in property evaluation")]
    public void PropertyEvaluation_WithComplexPredicateCombinations_AppliesModifiersCorrectly() {
        // Arrange - using the improved fluent predicate API
        var complexItem = new ItemBuilder(this._mockItem)
            // AND predicate test
            .WithProperty("AndTest", prop => prop
                .WithBaseValue(100f)
                // true AND true = true
                .When(Predicates.Always()).AndAlso(Predicates.Always()).Add(50f)
                // true AND false = false
                .When(Predicates.Always()).AndAlso(Predicates.Never()).Add(25f))

            // OR predicate test
            .WithProperty("OrTest", prop => prop
                .WithBaseValue(100f)
                // false OR true = true
                .When(Predicates.Never()).OrElse(Predicates.Always()).Add(50f)
                // false OR false = false
                .When(Predicates.Never()).OrElse(Predicates.Never()).Add(25f))

            // NOT predicate test
            .WithProperty("NotTest", prop => prop
                .WithBaseValue(100f)
                // NOT true = false
                .When(Predicates.Always().Not()).Add(50f)
                // NOT false = true
                .When(Predicates.Never().Not()).Add(25f))

            // Nested predicates with improved fluent API
            .WithProperty("NestedTest", prop => prop
                .WithBaseValue(100f)
                // (true AND false) OR true = true
                .When(Predicates.Always().AndAlso(Predicates.Never())
                    .OrElse(Predicates.Always())).Add(50f)
                // false OR (true AND true) = true
                .When(Predicates.Never().OrElse(Predicates.Always()
                    .AndAlso(Predicates.Always()))).Add(25f))
            .Build();

        // Act
        var andTestValue = this.GetPropertyValue(complexItem, "AndTest");
        var orTestValue = this.GetPropertyValue(complexItem, "OrTest");
        var notTestValue = this.GetPropertyValue(complexItem, "NotTest");
        var nestedTestValue = this.GetPropertyValue(complexItem, "NestedTest");

        // Expected values:
        // AndTest: 100 + 50 (true AND true) = 150
        // OrTest: 100 + 50 (false OR true) = 150
        // NotTest: 100 + 25 (NOT false) = 125
        // NestedTest: 100 + 50 (true AND (false OR true)) + 25 (false OR (true AND true)) = 175

        // Assert
        Assert.That(andTestValue, Is.EqualTo(150f).Within(0.001f), "AND predicate test failed");
        Assert.That(orTestValue, Is.EqualTo(150f).Within(0.001f), "OR predicate test failed");
        Assert.That(notTestValue, Is.EqualTo(125f).Within(0.001f), "NOT predicate test failed");
        Assert.That(nestedTestValue, Is.EqualTo(175f).Within(0.001f), "Nested predicates test failed");
    }

    [Test]
    [Description("Tests sequential application of modifiers in correct order")]
    public void ModifierSequencing_WithMultipleModifiers_AppliesModifiersInCorrectOrder() {
        // Arrange
        var sequencedItem = new ItemBuilder(this._mockItem)
            // Test the order of operations with the extension methods
            .WithProperty("SequenceTest", prop => prop
                .WithBaseValue(100f)
                .AddValue(50f)         // Now 150
                .MultiplyBy(2f)        // Now 300
                .AddValue(25f))        // Now 325

            // Test with more explicit custom modifiers
            .WithProperty("CustomSequenceTest", prop => prop
                .WithBaseValue(100f)
                .WithModifier(mod => mod.Custom(v => v + 50f))    // Now 150
                .WithModifier(mod => mod.Custom(v => v * 2))      // Now 300
                .WithModifier(mod => mod.Custom(v => v + 25f)))   // Now 325
            .Build();

        // Act
        var sequenceTestValue = this.GetPropertyValue(sequencedItem, "SequenceTest");
        var customSequenceTestValue = this.GetPropertyValue(sequencedItem, "CustomSequenceTest");

        // Assert
        Assert.That(sequenceTestValue, Is.EqualTo(325f).Within(0.001f),
            "Sequence of modifiers not applied correctly");
        Assert.That(customSequenceTestValue, Is.EqualTo(325f).Within(0.001f),
            "Sequence of custom modifiers not applied correctly");
        Assert.That(sequenceTestValue, Is.EqualTo(customSequenceTestValue).Within(0.001f),
            "Standard and custom modifier sequences should yield the same result");
    }

    [Test]
    [Description("Tests the most complex use case with advanced custom modifiers")]
    public void AdvancedModifiers_WithCustomLogic_CalculatesCorrectValues() {
        // Define a complex modifier function that uses the result reference
        Func<float, float, float> complexModifier = (current, result) => {
            // This represents a complex scenario where the modifier
            // depends on both the current value and running result
            if (current > 100f) return current * 1.5f + (result * 0.1f);

            return current * 1.2f + (result * 0.05f);
        };

        // Arrange
        var advancedItem = new ItemBuilder(this._mockItem)
            // Advanced custom modifier using the CustomAdvanced extension method
            .WithProperty("AdvancedTest", prop => prop
                .WithBaseValue(120f)
                .WithModifier(mod => mod.CustomAdvanced(complexModifier)))

            // Chained conditional property modification with the new fluent API
            .WithProperty("ChainedTest", prop => prop
                .WithBaseValue(50f)
                .If(true).Add(25f)
                .If(true).Multiply(1.5f))
            .Build();

        // Act
        var advancedTestValue = this.GetPropertyValue(advancedItem, "AdvancedTest");
        var chainedTestValue = this.GetPropertyValue(advancedItem, "ChainedTest");

        // Calculate expected values:
        // AdvancedTest: For base=120, the modifier should yield: 120 * 1.5 + (120 * 0.1) = 192
        var expectedAdvanced = 120f * 1.5f + (120f * 0.1f);

        // ChainedTest: 50 + 25 = 75, then 75 * 1.5 = 112.5
        var expectedChained = (50f + 25f) * 1.5f;

        // Assert
        Assert.That(advancedTestValue, Is.EqualTo(expectedAdvanced).Within(0.001f),
            "Advanced custom modifier calculation incorrect");
        Assert.That(chainedTestValue, Is.EqualTo(expectedChained).Within(0.001f),
            "Chained conditional modification incorrect");
    }

    [Test]
    [Description("Tests the new If() API compared to the When() API")]
    public void IfApi_ComparedToWhenApi_BehavesIdentically() {
        // Arrange
        var isEnchanted = true;
        var isDamaged = false;
        var playerLevel = 30;

        var evaluatedItem = new ItemBuilder(this._mockItem)
            // Testing If(bool) vs When(bool)
            .WithProperty("BooleanConditionTest", prop => prop
                .WithBaseValue(100f)
                .If(isEnchanted).Add(50f)     // Using If with direct boolean - should add 50
                .When(isEnchanted).Add(50f)   // Using When with the same boolean - should add another 50
                .If(isDamaged).Add(25f)       // Using If with false boolean - should not add
                .When(isDamaged).Add(25f))    // Using When with the same boolean - should not add

            // Testing If(Func<bool>) vs When(Func<bool>)
            .WithProperty("LambdaExpressionTest", prop => prop
                .WithBaseValue(100f)
                .If(() => playerLevel > 20).Add(50f)     // Using If with lambda - should add 50
                .When(() => playerLevel > 20).Add(50f)   // Using When with the same lambda - should add another 50
                .If(() => playerLevel > 50).Add(25f)     // Using If with false lambda - should not add
                .When(() => playerLevel > 50).Add(25f))  // Using When with the same lambda - should not add

            // Testing If() with complex expressions
            .WithProperty("ComplexExpressionTest", prop => prop
                .WithBaseValue(100f)
                .If(isEnchanted && playerLevel > 20).Add(50f)     // True AND True = True
                .If(isEnchanted && isDamaged).Add(25f)            // True AND False = False
                .If(isDamaged || playerLevel > 20).Add(35f))      // False OR True = True

            // Testing If() on the ItemBuilder itself
            .If(isEnchanted).WithProperty("EnchantmentBonus", prop => prop
                .WithBaseValue(100f)
                .If(playerLevel > 25).Add(50f))

            // This property should not be created
            .If(isDamaged).WithProperty("DamageProperty", prop => prop
                .WithBaseValue(50f))
            .Build();

        // Act
        var booleanConditionTestValue = this.GetPropertyValue(evaluatedItem, "BooleanConditionTest");
        var lambdaExpressionTestValue = this.GetPropertyValue(evaluatedItem, "LambdaExpressionTest");
        var complexExpressionTestValue = this.GetPropertyValue(evaluatedItem, "ComplexExpressionTest");
        var enchantmentBonusValue = this.GetPropertyValue(evaluatedItem, "EnchantmentBonus");
        var hasDamageProperty = evaluatedItem.TryGetProp("DamageProperty", out var _);

        // Expected values
        // BooleanConditionTest: 100 + 50 (If true) + 50 (When true) = 200
        // LambdaExpressionTest: 100 + 50 (If lambda true) + 50 (When lambda true) = 200
        // ComplexExpressionTest: 100 + 50 (true && true) + 0 (true && false) + 35 (false || true) = 185
        // EnchantmentBonus: 100 + 50 (level > 25) = 150

        // Assert
        Assert.That(booleanConditionTestValue, Is.EqualTo(200f).Within(0.001f), "Boolean condition test failed");
        Assert.That(lambdaExpressionTestValue, Is.EqualTo(200f).Within(0.001f), "Lambda expression test failed");
        Assert.That(complexExpressionTestValue, Is.EqualTo(185f).Within(0.001f), "Complex expression test failed");
        Assert.That(enchantmentBonusValue, Is.EqualTo(150f).Within(0.001f), "Enchantment bonus test failed");
        Assert.That(hasDamageProperty, Is.False, "Damage property should not exist");
    }

    // Helper method to get property value from an evaluated item
    private float GetPropertyValue(EvaluatedItem item, string propertyName) {
        if (item.TryGetProp(propertyName, out var property)) return property.Evaluate();
        throw new ArgumentException($"Property '{propertyName}' not found on evaluated item");
    }
}