namespace Batter.ItemValuation.Modifiers;

/// <summary>
///     Builder interface for creating ItemModifierSet instances.
/// </summary>
public interface IModifierTarget {
    internal IEnumerable<IModifier> Modifiers { get; }
}