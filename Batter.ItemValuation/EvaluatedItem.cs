#region

using System.Collections.Concurrent;
using TaleWorlds.Core;

#endregion

namespace Batter.ItemValuation;

public class EvaluatedItem {
    private readonly ConcurrentDictionary<string, IEvaluatedProperty> _propsByName;

    public EvaluatedItem(ref ItemObject itemObject) {
        this.ItemObject = itemObject;
        this.Id = itemObject.StringId;
        this._propsByName = new();
        this.EvaluatedProperties = new();
    }

    public ItemObject ItemObject { get; }

    public string Id { get; }

    internal ConcurrentQueue<IEvaluatedProperty> EvaluatedProperties { get; }

    // Updated ItemValuations.cs (relevant methods)
    public void AddProp(IEvaluatedProperty prop) {
        if (prop is null) throw new ArgumentNullException(nameof(prop));

        this.EvaluatedProperties.Enqueue(prop);
        this._propsByName[prop.Name] = prop; // Add to dictionary for faster lookup
    }

    public bool TryGetProp(string name, out IEvaluatedProperty? prop) {
        // Use dictionary for O(1) lookup instead of O(n) search
        return this._propsByName.TryGetValue(name, out prop);
    }

    public IEnumerable<float> Evaluate() {
        return this.EvaluatedProperties.Select(prop => prop.Evaluate());
    }

    public float Evaluate(string name) {
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

        if (!this.TryGetProp(name, out var prop))
            throw new KeyNotFoundException($"Property with name '{name}' not found.");

        return prop?.Evaluate() ?? throw new KeyNotFoundException($"Property with name '{name}' not found.");
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

        // Remove from dictionary for fast lookup
        this._propsByName.TryRemove(prop.Name, out var _);

        // Remove from queue
        // This still requires rebuilding the queue, but it's unavoidable with ConcurrentQueue
        var tempArray = this.EvaluatedProperties.Where(p => !p.Equals(prop)).ToArray();
        this.EvaluatedProperties.Clear();

        foreach (var item in tempArray) this.EvaluatedProperties.Enqueue(item);
    }

    public void ClearProps() {
        this.EvaluatedProperties.Clear();
        this._propsByName.Clear();
    }

    public IEnumerable<IEvaluatedProperty> GetProps() {
        return this.EvaluatedProperties.ToArray();
    }
}