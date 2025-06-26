#region

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Batter.Utils.Builders.Predicates;

public static class PredicateExtensions {

    [return: NotNull]
    internal static IfResult<TItem> If<TItem>(
        [NotNullWhen(true)] this TItem?     prop,
        [NotNullWhen(true)]      IPredicate predicate
    )
        where TItem : IValidProperty {
        if (prop is null) return IfResult<TItem>.ItemNull();

        return predicate.Evaluate()
            ? IfResult<TItem>.True(prop)
            : IfResult<TItem>.False(prop);
    }

    [return: NotNull]
    internal static IfResult<IValidKey> If<TContainer>(
        [NotNull] this      DynKey     key,
        [NotNullWhen(true)] IPredicate predicate
    ) {
        return predicate.Evaluate()
            ? IfResult<IValidKey>.True(key)
            : IfResult<IValidKey>.False(key);
    }

}

public sealed class IfResult<TItem> : IInto<TItem> {

    private readonly TItem?     _origItem;
    private readonly IPredicate _predicate;

    public IfResult([MaybeNull] TItem? origItem, IPredicate predicate) {
        this._origItem  = origItem;
        this._predicate = predicate;
    }

    [return: NotNull]
    public TItem Into() {
        return (this._predicate.Evaluate()
                   ? this._origItem
                   : default!) ??
               throw new NullReferenceException("The original item is null and the predicate evaluated to false.");
    }

    public static IfResult<TItem> False(TItem item) { return new(item, new Const(false)); }

    public static IfResult<TItem> True(TItem item) { return new(item, new Const(true)); }

    public static IfResult<TItem> ItemNull() { return new(default, new Const(false)); }

    public TItem? Else(TItem item) {
        return this._predicate.Evaluate()
            ? this._origItem
            : item;
    }

    public IfResult<TItem> ElseIf(IPredicate predicate) {
        return predicate.Evaluate()
            ? new(this._origItem, new Combine(this._predicate, predicate))
            : this;
    }

    public static implicit operator TItem(IfResult<TItem> predicate) { return predicate.Into(); }

    public static implicit operator IfResult<TItem>(TItem item) { return new(item, new Const(true)); }

}