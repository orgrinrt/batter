#region

using System.Collections.Concurrent;
using Batter.ItemValuation.Modifiers;

#endregion

namespace Batter.ItemValuation;

/// <summary>
/// Base implementation for evaluated properties.
/// </summary>
/// <typeparam name="TColl">The type of collection to use for storing modifiers.</typeparam>
/// <remarks>
/// This abstract class provides a standard implementation of the <see cref="IEvaluatedProperty"/> interface,
/// handling modifier management and value evaluation.
/// </remarks>
/// <example>
/// <code>
/// public class DamageProperty : EvaluatedPropertyBase&lt;List&lt;IModifier&gt;&gt;
/// {
///     public DamageProperty(string name, float baseValue) : base(name, baseValue) { }
/// }
/// 
/// var damage = new DamageProperty("Damage", 25.0f);
/// damage.AddModifier(new MultiplicativeModifier(1.5f));
/// float evaluatedDamage = damage.Evaluate(); // Returns 37.5f
/// </code>
/// </example>
public abstract class EvaluatedPropertyBase<TColl> : IEvaluatedProperty
    where TColl : IEnumerable<IModifier>, new() {
    /// <summary>
    /// Initializes a new instance of the <see cref="EvaluatedPropertyBase{TColl}"/> class.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="baseValue">The base value of the property before modifiers.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null or empty.</exception>
    protected EvaluatedPropertyBase(string name, float baseValue) {
        this.Name = name;
        this.BaseValue = baseValue;
        this.Modifiers = new TColl();
    }

    /// <summary>
    /// Gets the collection of modifiers applied to this property.
    /// </summary>
    public IEnumerable<IModifier> Modifiers { get; private set; }

    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the base value of the property before modifiers.
    /// </summary>
    public float BaseValue { get; }

    /// <summary>
    /// Evaluates the property value with all modifiers applied.
    /// </summary>
    /// <returns>The final evaluated value.</returns>
    /// <example>
    /// <code>
    /// var damage = new DamageProperty("Damage", 25.0f);
    /// damage.AddModifier(new MultiplicativeModifier(1.5f));
    /// float evaluatedDamage = damage.Evaluate(); // Returns 37.5f
    /// </code>
    /// </example>
    public float Evaluate() {
        var result = this.BaseValue;

        return this.Modifiers.Aggregate(result, (current, modifier) => modifier.Apply(current, ref result));
    }

    /// <summary>
    /// Adds a modifier to the property.
    /// </summary>
    /// <param name="modifier">The modifier to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="modifier"/> is null.</exception>
    /// <example>
    /// <code>
    /// var damage = new DamageProperty("Damage", 25.0f);
    /// damage.AddModifier(new MultiplicativeModifier(1.5f));
    /// </code>
    /// </example>
    public void AddModifier(IModifier modifier) {
        if (modifier is null) throw new ArgumentNullException(nameof(modifier));
        if (this.Modifiers is IList<IModifier> modifiers) {
            modifiers.Add(modifier);
        }
        else {
            var newModifiers = new TColl();
            newModifiers = this.Modifiers.Aggregate(newModifiers,
                (current, existingModifier) => (TColl)current.Append(existingModifier));
            newModifiers = (TColl)newModifiers.Append(modifier);
            this.Modifiers = newModifiers;
        }
    }

    /// <summary>
    /// Removes a modifier from the property.
    /// </summary>
    /// <param name="modifier">The modifier to remove.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="modifier"/> is null.</exception>
    /// <example>
    /// <code>
    /// var damage = new DamageProperty("Damage", 25.0f);
    /// var mod = new MultiplicativeModifier(1.5f);
    /// damage.AddModifier(mod);
    /// // Later when the modifier is no longer needed
    /// damage.RemoveModifier(mod);
    /// </code>
    /// </example>
    public void RemoveModifier(IModifier modifier) {
        if (modifier is null) throw new ArgumentNullException(nameof(modifier));
        if (!this.Modifiers.Contains(modifier)) return;
        if (this.Modifiers is IList<IModifier> modifiers)
            modifiers.Remove(modifier);
        else
            this.Modifiers = this.Modifiers.Where(m => m != modifier);
    }

    /// <summary>
    /// Clears all modifiers from the property.
    /// </summary>
    /// <example>
    /// <code>
    /// var damage = new DamageProperty("Damage", 25.0f);
    /// damage.AddModifier(new MultiplicativeModifier(1.5f));
    /// damage.AddModifier(new AdditiveModifier(5.0f));
    /// // Remove all modifiers
    /// damage.ClearModifiers();
    /// </code>
    /// </example>
    public void ClearModifiers() {
        if (this.Modifiers is IList<IModifier> modifiers)
            modifiers.Clear();
        else
            this.Modifiers = new ConcurrentQueue<IModifier>();
    }
}