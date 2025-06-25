#region

using System.Diagnostics.CodeAnalysis;

using Batter.Utils.Builders.Dynamic;

#endregion

namespace Batter.Utils.Builders;

public interface IBuildableObj {

    object BuildWith(object builder);

}

/// <summary>
///     Interface for objects that can be built using the builder pattern with custom unified storage.
/// </summary>
/// <typeparam name="TThis">The type being built</typeparam>
/// <typeparam name="TBuilder">The builder type</typeparam>
/// <typeparam name="TStorage">The unified storage type</typeparam>
public interface IBuildable<out TThis, TBuilder, TStorage> : IBuildableObj
    where TThis : class, IBuildable<TThis, TBuilder, TStorage>
    where TBuilder : class, IBuilder<TThis, TBuilder, TStorage>
    where TStorage : class, IContainerStorage<TStorage, TBuilder, DynCollection<DynKey, object>>, new() {

    /// <summary>
    ///     Creates a new builder instance for this buildable type.
    /// </summary>
    /// <returns>A new builder instance</returns>
    static TB From<TB>()
        where TB : TBuilder, new() {
        var builder = new TB();

        return (TB)builder.Init();
    }

    TBuildable BuildWith<TBuildable>(TBuilder builder)
        where TBuildable : class, IBuildableObj {
        return builder.Build<TBuildable>(this as TBuildable) ?? throw new InvalidOperationException();
    }

    new object BuildWith(object builder) { return this.BuildWith<TThis>((TBuilder)builder); }

}

/// <summary>
///     Provides extension methods for working with IBuildable types.
/// </summary>
public static class IBuildableExtensions {

    /// <summary>
    ///     Creates a new builder, configures it, and builds an instance of the buildable type.
    /// </summary>
    /// <typeparam name="TThis">The buildable type</typeparam>
    /// <typeparam name="TBuilder">The builder type</typeparam>
    /// <typeparam name="TStorage">The storage type</typeparam>
    /// <param name="buildable">The buildable instance</param>
    /// <param name="configure">Configuration action to apply to the builder</param>
    /// <returns>A built instance with the configuration applied</returns>
    [return: NotNull]
    public static TThis BuildWithConfigurator<TThis, TBuilder, TStorage>(
        this IBuildable<TThis, TBuilder, TStorage> buildable,
        Action<TBuilder>                           configure
    )
        where TThis : class, IBuildable<TThis, TBuilder, TStorage>
        where TBuilder : class, IBuilder<TThis, TBuilder, TStorage>, new()
        where TStorage : class, IContainerStorage<TStorage, TBuilder, DynCollection<DynKey, object>>, new() {
        var builder = IBuildable<TThis, TBuilder, TStorage>.From<TBuilder>();
        configure(builder);

        return (TThis)buildable.BuildWith(builder);
    }

    /// <summary>
    ///     Creates a new builder and returns it for further configuration.
    /// </summary>
    /// <typeparam name="TThis">The buildable type</typeparam>
    /// <typeparam name="TBuilder">The builder type</typeparam>
    /// <typeparam name="TStorage">The storage type</typeparam>
    /// <param name="buildable">The buildable instance</param>
    /// <returns>A new builder instance</returns>
    [return: NotNull]
    public static TBuilder CreateBuilder<TThis, TBuilder, TStorage>(
        this IBuildable<TThis, TBuilder, TStorage> buildable
    )
        where TThis : class, IBuildable<TThis, TBuilder, TStorage>
        where TBuilder : class, IBuilder<TThis, TBuilder, TStorage>, new()
        where TStorage : class, IContainerStorage<TStorage, TBuilder, DynCollection<DynKey, object>>, new() {
        return IBuildable<TThis, TBuilder, TStorage>.From<TBuilder>();
    }

}