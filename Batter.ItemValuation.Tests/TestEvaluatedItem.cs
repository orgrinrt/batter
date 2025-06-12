#region

using System.Collections.Concurrent;

#endregion

namespace Batter.ItemValuation.Tests;

/// <summary>
/// A test implementation of EvaluatedItem that works with MockIItemObject
/// </summary>
public class TestEvaluatedItem {
    private readonly ConcurrentDictionary<string, IEvaluatedProperty> _propsByName;

    public TestEvaluatedItem(IItemObject mockIItemObject) {
        this.ItemObject = mockIItemObject;
        this.Id = mockIItemObject.StringId;
        this._propsByName = new();
        this.EvaluatedProperties = new();
    }

    public IItemObject ItemObject { get; }
    public string Id { get; }
    internal ConcurrentQueue<IEvaluatedProperty> EvaluatedProperties { get; }

    public void AddProp(IEvaluatedProperty prop) {
        if (prop is null) throw new ArgumentNullException(nameof(prop));

        this.EvaluatedProperties.Enqueue(prop);
        this._propsByName[prop.Name] = prop; // Add to dictionary for faster lookup
    }

    public bool TryGetProp(string name, out IEvaluatedProperty? prop) {
        return this._propsByName.TryGetValue(name, out prop);
    }

    public IEnumerable<float> EvaluateAll() {
        return this.EvaluatedProperties.Select(prop => prop.Evaluate());
    }

    public float EvaluateTotal() {
        // Simple sum implementation for testing
        return this.EvaluatedProperties.Sum(prop => prop.Evaluate());
    }
}