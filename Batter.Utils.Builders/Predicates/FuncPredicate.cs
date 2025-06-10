namespace Batter.Utils.Builders.Predicates;

public class FuncPredicate : IPredicate {
    private readonly Func<bool> _predicate;

    public FuncPredicate(Func<bool> predicate) => this._predicate = predicate;

    public bool Evaluate() {
        return this._predicate();
    }

    public bool Evaluate(object value) {
        return this._predicate();
    }
}

public class FuncPredicate<T> : IPredicate<T> {
    private readonly Func<T, bool> _predicate;

    public FuncPredicate(Func<T, bool> predicate) => this._predicate = predicate;

    public bool Evaluate(T value) {
        return this._predicate(value);
    }
}