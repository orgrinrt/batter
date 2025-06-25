namespace Batter.Utils.Builders.Dynamic;

public interface IBuildableWithCreatableBuilder<TThis>
    where TThis : class {

    public static TThis Create<TBuilder>()
        where TBuilder : TThis, new() {
        return (TThis)(Activator.CreateInstance(typeof(TBuilder)) ??
                       throw new InvalidOperationException(
                           "Failed to create instance of type " + typeof(TThis).FullName));
    }

}

public abstract class DynBuildable<TResult, TBuilder> : DynBuildable<TResult>,
                                                        IBuildableWithCreatableBuilder<TBuilder>
    where TResult : IBuildableObj
    where TBuilder : class, IObjBuilder, new() {

    public static TBuilder Create() { return IBuildableWithCreatableBuilder<TBuilder>.Create<TBuilder>(); }

}

public sealed class DynBuildable : IBuildable<DynBuildable, DynBuilder, DynStorage<DynBuilder>> {

    private readonly object? _inner;
    public           object  Inner => this._inner ?? throw new InvalidOperationException("Inner object is not set.");
    public DynBuildable(object? inner) { this._inner = inner; }

    public object BuildWith(object builder) {
        return builder switch {
            DynBuilder dynBuilderObj => dynBuilderObj.Build(this.AsObj()) ?? throw new InvalidOperationException(),
            _ => throw new InvalidOperationException("Builder must be of type DynBuilder<TResult> or DynBuilder."),
        };
    }

    public TResult As<TResult>()
        where TResult : DynBuildable<TResult> {
        return (TResult)this.Inner;
    }

    public IBuildableObj AsObj() {
        if (this.Inner is IBuildableObj buildableObj) return buildableObj;

        throw new InvalidOperationException("Inner object does not implement IBuildableObj.");
    }

    public static implicit operator DynBuildable(DynBuildable<IBuildableObj> buildable) { return new(buildable); }

    public static explicit operator DynBuildable<IBuildableObj>(DynBuildable buildable) {
        return (DynBuildable<IBuildableObj>)buildable.AsObj();
    }

}

/// <summary>
///     A base class for objects that can be dynamically built from a property bag.
/// </summary>
public abstract class DynBuildable<TResult> : IBuildable<DynBuildable, DynBuilder, DynStorage<DynBuilder>>
    where TResult : IBuildableObj {

    public static TBuilder From<TBuilder>()
        where TBuilder : DynBuilder<TResult>, new() {
        return IBuildable<DynBuildable, DynBuilder, DynStorage<DynBuilder>>.From<TBuilder>();
    }


    public object BuildWith(object builder) {
        return builder switch {
            DynBuilder<TResult> dynBuilder => dynBuilder.Build(this) ?? throw new InvalidOperationException(),
            DynBuilder dynBuilderObj => dynBuilderObj.Build(this) ?? throw new InvalidOperationException(),
            _ => throw new InvalidOperationException("Builder must be of type DynBuilder<TResult> or DynBuilder."),
        };
    }


    /// <inheritdoc />
    public TCast BuildWith<TCast>(DynBuilder builder)
        where TCast : class, IBuildableObj {
        return this.BuildWith(builder) as TCast ?? throw new InvalidOperationException();
    }

    public static explicit operator DynBuildable(DynBuildable<TResult> buildable) { return new(buildable); }

    public static explicit operator DynBuildable<TResult>(DynBuildable buildable) {
        return (DynBuildable<TResult>)buildable.AsObj();
    }

}