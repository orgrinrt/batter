namespace Batter.Utils.Builders.Predicates;

public class ConstantPredicate : IPredicate {
    private readonly bool _value;

    public ConstantPredicate(bool value) => this._value = value;

    public bool Evaluate() {
        return this._value;
    }

    public bool Evaluate(object value) {
        return this._value;
    }
}