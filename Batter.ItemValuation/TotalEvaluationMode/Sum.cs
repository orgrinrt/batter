using System.Collections.Concurrent;

namespace Batter.ItemValuation.TotalEvaluationMode;

public class Sum : ITotalEvaluationMode {
    public float EvaluateTotal(ConcurrentQueue<IEvaluatedProperty> itemProps, float _default = 0f) {
        if (itemProps is null || itemProps.IsEmpty) return _default;

        return itemProps.Sum(prop => prop.Evaluate());
    }
}