#region

using System.Collections.Concurrent;
using Batter.ItemValuation.Modifiers;

#endregion

namespace Batter.ItemValuation;

/// <summary>
///     Base implementation for evaluated properties.
/// </summary>
public abstract class EvaluatedPropertyBase<TColl> : IEvaluatedProperty
    where TColl : IEnumerable<IModifier>, new() {
    protected EvaluatedPropertyBase(string name, float baseValue) {
        this.Name = name;
        this.BaseValue = baseValue;
        this.Modifiers = new TColl();
    }

    public IEnumerable<IModifier> Modifiers { get; private set; }

    public string Name { get; }
    public float BaseValue { get; }

    public float Evaluate() {
        var result = this.BaseValue;

        return this.Modifiers.Aggregate(result, (current, modifier) => modifier.Apply(current, ref result));
    }

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

    public void RemoveModifier(IModifier modifier) {
        if (modifier is null) throw new ArgumentNullException(nameof(modifier));
        if (!this.Modifiers.Contains(modifier)) return;
        if (this.Modifiers is IList<IModifier> modifiers)
            modifiers.Remove(modifier);
        else
            this.Modifiers = this.Modifiers.Where(m => m != modifier);
    }

    public void ClearModifiers() {
        if (this.Modifiers is IList<IModifier> modifiers)
            modifiers.Clear();
        else
            this.Modifiers = new ConcurrentQueue<IModifier>();
    }
}