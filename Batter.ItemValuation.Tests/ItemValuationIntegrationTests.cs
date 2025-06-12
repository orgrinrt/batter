#region

using Batter.ItemValuation.Tests.Mocks;
using NUnit.Framework;

#endregion

namespace Batter.ItemValuation.Tests;

/// <summary>
/// Integration tests for the Batter.ItemValuation system.
/// </summary>
[TestFixture]
public class ItemValuationIntegrationTests {
    [OneTimeSetUp]
    public void OneTimeSetUp() {
        // Initialization that happens once before all tests
        TestContext.Progress.WriteLine("Starting ItemValuation integration tests");
    }

    [SetUp]
    public void SetUp() {
        // Create a mock item and an evaluated item wrapper before each test
        this._mockItem = new("test_item_01");
        this._evaluatedItem = new(this._mockItem);
    }

    [TearDown]
    public void TearDown() {
        // Clean up after each test if needed
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() {
        // Clean up after all tests have run
        TestContext.Progress.WriteLine("Completed ItemValuation integration tests");
    }

    private MockItemObject _mockItem;
    private TestEvaluatedItem _evaluatedItem;

    [Test]
    [Category("Property")]
    [Description("Tests that a property without modifiers returns its base value")]
    public void Property_WithoutModifiers_ReturnsBaseValue() {
        // Arrange
        var baseValue = 100f;
        var property = new TestProperty("TestProperty", baseValue);

        // Act
        this._evaluatedItem.AddProp(property);
        var result = property.Evaluate();

        // Assert
        Assert.That(result, Is.EqualTo(baseValue).Within(0.001f),
            "Property without modifiers should return base value");
    }

    [Test]
    [Category("Property")]
    [Description("Tests that a property with an additive modifier correctly adds the value")]
    public void Property_WithAdditiveModifier_AddsValueToBase() {
        // Arrange
        var baseValue = 100f;
        var addValue = 50f;
        var property = new TestProperty("TestProperty", baseValue);
        property.AddModifier(new AdditiveModifier(addValue));

        // Act
        this._evaluatedItem.AddProp(property);
        var result = property.Evaluate();

        // Assert
        Assert.That(result, Is.EqualTo(baseValue + addValue).Within(0.001f),
            "Property with additive modifier should add value to base");
    }

    [Test]
    [Category("Property")]
    [Description("Tests that a property with a multiplicative modifier correctly multiplies the value")]
    public void Property_WithMultiplicativeModifier_MultipliesBaseValue() {
        // Arrange
        var baseValue = 100f;
        var factor = 1.5f;
        var property = new TestProperty("TestProperty", baseValue);
        property.AddModifier(new MultiplicativeModifier(factor));

        // Act
        this._evaluatedItem.AddProp(property);
        var result = property.Evaluate();

        // Assert
        Assert.That(result, Is.EqualTo(baseValue * factor).Within(0.001f),
            "Property with multiplicative modifier should multiply base value");
    }

    [Test]
    [Category("Property")]
    [Description("Tests that a property with multiple modifiers applies them in sequence")]
    public void Property_WithMultipleModifiers_AppliesThemInSequence() {
        // Arrange
        var baseValue = 100f;
        var addValue = 50f;
        var factor = 1.5f;
        var property = new TestProperty("TestProperty", baseValue);

        // Add modifiers in sequence (first add, then multiply)
        property.AddModifier(new AdditiveModifier(addValue));
        property.AddModifier(new MultiplicativeModifier(factor));

        // Act
        this._evaluatedItem.AddProp(property);
        var result = property.Evaluate();

        // Assert
        var expected = (baseValue + addValue) * factor;
        Assert.That(result, Is.EqualTo(expected).Within(0.001f),
            "Property with multiple modifiers should apply them in sequence");
    }

    [Test]
    [Category("TotalValue")]
    [Description("Tests that the total value calculation sums all evaluated properties")]
    public void EvaluateTotal_WithMultipleProperties_SumsAllEvaluatedProperties() {
        // Arrange
        var property1 = new TestProperty("Property1", 100f);
        property1.AddModifier(new AdditiveModifier(50f));

        var property2 = new TestProperty("Property2", 200f);
        property2.AddModifier(new MultiplicativeModifier(1.5f));

        this._evaluatedItem.AddProp(property1);
        this._evaluatedItem.AddProp(property2);

        // Act
        var totalValue = this._evaluatedItem.EvaluateTotal();

        // Assert
        var expected = 150f + 300f; // (100 + 50) + (200 * 1.5)
        Assert.That(totalValue, Is.EqualTo(expected).Within(0.001f),
            "Total value should be the sum of all evaluated properties");
    }

    [Test]
    [Category("Lookup")]
    [Description("Tests that properties can be looked up by name")]
    public void TryGetProp_WithExistingPropertyName_ReturnsProperty() {
        // Arrange
        var property = new TestProperty("UniqueProperty", 100f);
        this._evaluatedItem.AddProp(property);

        // Act
        var found = this._evaluatedItem.TryGetProp("UniqueProperty", out var retrievedProp);

        // Assert
        Assert.That(found, Is.True, "Should find property by name");
        Assert.That(retrievedProp, Is.SameAs(property), "Retrieved property should match the added property");
    }

    [Test]
    [Category("Lookup")]
    [Description("Tests that looking up non-existent property returns false")]
    public void TryGetProp_WithNonExistentPropertyName_ReturnsFalse() {
        // Act
        var found = this._evaluatedItem.TryGetProp("NonExistentProperty", out var retrievedProp);

        // Assert
        Assert.That(found, Is.False, "Should not find non-existent property");
        Assert.That(retrievedProp, Is.Null, "Retrieved property should be null for non-existent property");
    }
}