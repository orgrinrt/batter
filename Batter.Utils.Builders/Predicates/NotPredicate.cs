namespace Batter.Utils.Builders.Predicates;

public class NotPredicate<T> : IPredicate<T> {
    private readonly IPredicate<T> _predicate;

    public NotPredicate(IPredicate<T> predicate) => this._predicate = predicate;

    public bool Evaluate(T value) {
        return !this._predicate.Evaluate(value);
    }
}