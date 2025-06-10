using System.Collections.Concurrent;
using TaleWorlds.Core;

namespace Batter.ItemValuation;

public class EvaluatedItem {
    public EvaluatedItem(ref ItemObject itemObject) {
        this.ItemObject = itemObject;
        this.Id = itemObject.StringId;
    }

    public ItemObject ItemObject { get; }

    public string Id { get; }

    internal ConcurrentQueue<IEvaluatedProperty> EvaluatedProperties { get; } = new();

    // Updated ItemValuations.cs (relevant methods)
    public void AddProp(IEvaluatedProperty prop) {
        if (prop is null) throw new ArgumentNullException(nameof(prop));
        this.EvaluatedProperties.Enqueue(prop);
    }

    public bool TryGetProp(string name, out IEvaluatedProperty? prop) {
        foreach (var itemProp in this.EvaluatedProperties)
            if (itemProp.Name == name) {
                prop = itemProp;
                return true;
            }

        prop = null;
        return false;
    }

    public IEnumerable<float> Evaluate() {
        return this.EvaluatedProperties.Select(prop => prop.Evaluate());
    }

    public float Evaluate(string name) {
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

        if (this.TryGetProp(name, out var prop)) return prop.Evaluate();

        throw new KeyNotFoundException($"Property with name '{name}' not found.");
    }

    public float EvaluateTotal<TMode>() where TMode : ITotalEvaluationMode, new() {
        var mode = new TMode();
        return mode.EvaluateTotal(this.EvaluatedProperties);
    }

    public float EvaluateTotal<TMode>(TMode mode) where TMode : ITotalEvaluationMode {
        return mode.EvaluateTotal(this.EvaluatedProperties);
    }

    public float EvaluateTotal(Func<IEvaluatedProperty, float> fn) {
        return fn is null
            ? throw new ArgumentNullException(nameof(fn))
            : this.EvaluatedProperties.Sum(fn);
    }

    public void RemoveProp(IEvaluatedProperty prop) {
        if (prop is null) throw new ArgumentNullException(nameof(prop));

        // FIXME: this is not efficient, but goes for now
        var newProps = new ConcurrentQueue<IEvaluatedProperty>();
        foreach (var itemProp in this.EvaluatedProperties)
            if (!itemProp.Equals(prop))
                newProps.Enqueue(itemProp);

        this.EvaluatedProperties.Clear();
        foreach (var itemProp in newProps) this.EvaluatedProperties.Enqueue(itemProp);
    }

    public void ClearProps() {
        this.EvaluatedProperties.Clear();
    }

    public IEnumerable<IEvaluatedProperty> GetProps() {
        return this.EvaluatedProperties.ToArray();
    }
}