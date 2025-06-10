using System.Collections.Concurrent;

namespace Batter.ItemValuation;

public interface ITotalEvaluationMode {
    float EvaluateTotal(ConcurrentQueue<IEvaluatedProperty> itemValuations, float _default = 0f);
}