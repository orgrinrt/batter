namespace Batter.Utils.Builders.Predicates;

public class OrPredicate<T> : IPredicate<T> {
    private readonly IPredicate<T> _first;
    private readonly IPredicate<T> _second;

    public OrPredicate(IPredicate<T> first, IPredicate<T> second) =>
        (this._first, this._second) = (first, second);

    public bool Evaluate(T value) {
        return this._first.Evaluate(value) || this._second.Evaluate(value);
    }
}