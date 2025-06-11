namespace Batter.ItemValuation.Modifiers;

/// <summary>
/// Defines an interface for objects that can have modifiers applied to them.
/// </summary>
/// <remarks>
/// This interface is implemented by objects that can be modified by <see cref="IModifier"/> instances,
/// typically properties that need to have their values transformed during evaluation.
/// </remarks>
/// <example>
/// <code>
/// public class DamageProperty : EvaluatedPropertyBase&lt;List&lt;IModifier&gt;&gt;, IModifierTarget
/// {
///     public DamageProperty(string name, float baseValue) : base(name, baseValue) { }
///     
///     // IModifierTarget implementation is provided by EvaluatedPropertyBase
/// }
/// 
/// // Usage:
/// var damage = new DamageProperty("Damage", 25.0f);
/// damage.AddModifier(new MultiplicativeModifier(1.5f));
/// </code>
/// </example>
public interface IModifierTarget {
    /// <summary>
    /// Gets the collection of modifiers applied to this target.
    /// </summary>
    internal IEnumerable<IModifier> Modifiers { get; }
}