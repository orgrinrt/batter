#region

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Batter.Utils.Builders.Attributes;
using Batter.Utils.Builders.Dynamic;
using Batter.Utils.Builders.Predicates;

#endregion

namespace Batter.Utils.Builders;

public interface IInternallyMutableBuilderTarget { }

public interface IInternallyMutableBuilder<TBuilder> : IInternallyMutableBuilderTarget
    where TBuilder : class, IInternallyMutableBuilder<TBuilder> {

    protected IInternallyMutableBuilderTarget LatestTarget { get; set; }

    [MethodLikeIndexer]
    TBuilder this[Func<IInternallyMutableBuilderTarget, IInternallyMutableBuilderTarget> op] {
        get {
            if (op == null) throw new ArgumentNullException(nameof(op));

            return new Operation<Func<IInternallyMutableBuilderTarget, IInternallyMutableBuilderTarget>>(
                (TBuilder)this,
                op).Invoke<IInternallyMutableBuilderTarget>();
        }
    }

    [MethodLikeIndexer]
    TBuilder this[Action<IInternallyMutableBuilderTarget> op] {
        get {
            if (op == null) throw new ArgumentNullException(nameof(op));

            return new Operation<Action<IInternallyMutableBuilderTarget>>((TBuilder)this, op)
               .Invoke<IInternallyMutableBuilderTarget>();
        }
    }

    public sealed class Operation<TFunc> {

        public TBuilder Builder { get; }
        public TFunc    Func    { get; }

        public Operation(TBuilder builder, TFunc op) {
            this.Builder = builder;
            this.Func    = op;
        }

        public TBuilder Invoke<TTarget>()
            where TTarget : IInternallyMutableBuilderTarget {
            switch (this.Func) {
                case Func<TBuilder, TTarget> func: {
                    TTarget target = func(this.Builder);
                    this.Builder.LatestTarget = target;

                    return this.Builder;
                }
                case Func<TBuilder, IInternallyMutableBuilderTarget> func2: {
                    IInternallyMutableBuilderTarget target = func2(this.Builder);
                    this.Builder.LatestTarget = target;

                    return this.Builder;
                }
                default: {
                    switch (this.Func) {
                        case Func<IInternallyMutableBuilderTarget, IInternallyMutableBuilderTarget> func3: {
                            IInternallyMutableBuilderTarget target = func3(this.Builder.LatestTarget);
                            this.Builder.LatestTarget = target;

                            if (target is TTarget tTarget) return this.Builder;

                            throw new InvalidOperationException(
                                $"Operation '{this.Func.GetType().Name}' is not compatible with the expected type '{typeof(TTarget).Name}'.");
                        }
                        case Func<TBuilder, object> func4 when typeof(TTarget) == typeof(object): {
                            object target = func4(this.Builder);
                            this.Builder.LatestTarget = (IInternallyMutableBuilderTarget)target;

                            return this.Builder;
                        }
                        case Func<TBuilder, IInternallyMutableBuilderTarget> func5
                            when typeof(TTarget) == typeof(IInternallyMutableBuilderTarget): {
                            IInternallyMutableBuilderTarget target = func5(this.Builder);
                            this.Builder.LatestTarget = target;

                            return this.Builder;
                        }
                        default:
                            throw new InvalidOperationException(
                                $"Operation '{this.Func?.GetType().Name}' is not compatible with the expected type '{typeof(TTarget).Name}'.");
                    }
                }
            }
        }

    }

}

/// <summary>
///     Non-generic marker interface for all builders.
/// </summary>
public interface IObjBuilder {

    /// <summary>
    ///     Builds and returns the final result.
    /// </summary>
    /// <returns>The constructed object.</returns>
    object? Build(object? nonDefaultStartingPoint = null);

    /// <summary>
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    TResult? Build<TResult>(TResult? nonDefaultStartingPoint = null)
        where TResult : class, IBuildableObj;

}

/// <summary>
///     Represents a builder for constructing objects of type <typeparamref name="TResult" />.
///     Uses unified storage and tree-based BuilderStep container for conditional logic tracking.
///     All fluent API operations register build steps that are executed when Build() is called.
/// </summary>
/// <typeparam name="TResult">The type of object being built.</typeparam>
/// <typeparam name="TBuilder">The concrete builder type, enabling fluent method chaining.</typeparam>
/// <typeparam name="TStorage">The unified storage type</typeparam>
public interface IBuilder<out TResult, TBuilder, out TStorage> : IObjBuilder,
                                                                 IInternallyMutableBuilder<TBuilder>,
                                                                 IContainer<TBuilder, TStorage,
                                                                     DynCollection<DynKey, object>>,
                                                                 IDeepCloneable<TBuilder>
    where TResult : class
    where TBuilder : class, IBuilder<TResult, TBuilder, TStorage>
    where TStorage : class, IContainerStorage<TStorage, TBuilder, DynCollection<DynKey, object>>, new() {

    AdditionHandler<DynKey, object>? IContainer<TBuilder, TStorage, DynCollection<DynKey, object>>.OnAddition =>
        this._OnAddition;

    object IObjBuilder.Build(object? nonDefaultStartingPoint) { return this.Build(nonDefaultStartingPoint); }

    void _OnAddition(TBuilder container, DynKey key, object item) {
        this.LatestTarget = item as IInternallyMutableBuilderTarget ?? container;
        this._OnAddition(key, item);
    }

    void _OnAddition(IValidKey key, object value);

    /// <summary>
    ///     Builds and returns the final result by executing the step tree.
    ///     This method traverses the step tree in execution order and applies each step.
    /// </summary>
    /// <returns>The constructed object.</returns>
    [return: NotNull]
    new TResult Build(object? nonDefaultStartingPoint = null);

    /// <summary>
    ///     Returns the builder instance for method chaining.
    /// </summary>
    /// <returns>The builder instance.</returns>
    [return: NotNull]
    TBuilder Self();

    [return: NotNull]
    TBuilder Init();

}

/// <summary>
///     Provides a set of extension methods for the <see cref="IBuilder{TResult, TBuilder, TStorage}" /> interface.
///     Allows enhanced functionalities for cloning, self-referencing, and building objects using the builder pattern.
/// </summary>
public static class IBuilderExtensions {

    /// <summary>
    ///     Creates a deep clone of the builder, preserving all registered build steps.
    /// </summary>
    /// <param name="builder">The builder to clone.</param>
    /// <typeparam name="TBuilder">The concrete builder type.</typeparam>
    /// <typeparam name="TResult">The type of object being built.</typeparam>
    /// <typeparam name="TStorage">The unified storage type.</typeparam>
    /// <returns>A new builder instance with the same configuration.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the builder does not support cloning.</exception>
    [return: NotNull]
    public static TBuilder Clone<TResult, TBuilder, TStorage>([NotNull] this TBuilder builder)
        where TBuilder : class, IBuilder<object, TBuilder, TStorage>
        where TResult : class
        where TStorage : class, IContainerStorage<TStorage, TBuilder, DynCollection<DynKey, object>>, new() {
        if (builder is IDeepCloneable<TBuilder> cloneable) return cloneable.Clone();

        throw new InvalidOperationException(
            $"Builder of type '{builder.GetType().FullName}' does not support cloning.");
    }

    /// <summary>
    ///     Returns the builder instance for method chaining.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <typeparam name="TBuilder">The concrete builder type.</typeparam>
    /// <typeparam name="TStorage">The unified storage type.</typeparam>
    /// <returns>The builder instance.</returns>
    [return: NotNull]
    public static TBuilder Self<TBuilder, TStorage>([NotNull] this IBuilder<object, TBuilder, TStorage> builder)
        where TBuilder : class, IBuilder<object, TBuilder, TStorage>
        where TStorage : class, IContainerStorage<TStorage, TBuilder, DynCollection<DynKey, object>>, new() {
        return builder.Self();
    }

    /// <summary>
    ///     Builds and returns the final result by executing the step tree.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <typeparam name="TBuilder">The concrete builder type.</typeparam>
    /// <typeparam name="TStorage">The unified storage type.</typeparam>
    /// <returns>The constructed object.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the builder does not support building.</exception>
    [return: NotNull]
    public static object Build<TBuilder, TStorage>([NotNull] this IBuilder<object, TBuilder, TStorage> builder)
        where TBuilder : class, IBuilder<object, TBuilder, TStorage>
        where TStorage : class, IContainerStorage<TStorage, TBuilder, DynCollection<DynKey, object>>, new() {
        return builder.Build();
    }

    [return: NotNull]
    public static TResult Build<TResult>([NotNull] this DynBuilder builder)
        where TResult : DynBuildable<TResult>, new() {
        return (TResult)builder.Build();
    }

    [return: NotNull]
    public static TResult Build<TResult>(
        [NotNull] this IBuilder<DynBuildable, DynBuilder, DynStorage<DynBuilder>> builder
    )
        where TResult : DynBuildable<TResult>, new() {
        return (TResult)builder.Build();
    }
    //
    // [return: NotNull]
    // public static TResult Build<TResult, TBuilder>([NotNull] this TBuilder builder)
    //     where TBuilder : class, IBuilder<TResult, TBuilder, DynStorage<TBuilder>>
    //     where TResult : DynBuildable<TResult>, new() {
    //     return builder.Build();
    // }

    /// <summary>
    ///     Builds an object if the builder supports it, returning null otherwise.
    /// </summary>
    /// <typeparam name="T">The builder type.</typeparam>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The built object or null if building is not possible.</returns>
    public static object? TryBuild<T>(this T builder)
        where T : class {
        MethodInfo? buildMethod = typeof(T).GetMethod("Build", Type.EmptyTypes);

        return buildMethod?.Invoke(builder, null);
    }

    /// <summary>
    ///     Applies multiple transformations in sequence.
    /// </summary>
    [return: NotNull]
    public static TBuilder WithAll<TBuilder>(this TBuilder builder, params Func<TBuilder, TBuilder>[] actions)
        where TBuilder : class {
        return actions.Aggregate(builder, (current, action) => action(current));
    }

    /// <summary>
    ///     Conditionally applies a transformation based on a predicate.
    /// </summary>
    [return: NotNull]
    public static TBuilder When<TBuilder>(
        this TBuilder            builder,
        Func<TBuilder, bool>     predicate,
        Func<TBuilder, TBuilder> action
    )
        where TBuilder : class {
        return predicate(builder)
            ? action(builder)
            : builder;
    }

    /// <summary>
    ///     Conditionally applies a transformation based on an IPredicate.
    /// </summary>
    [return: NotNull]
    public static TBuilder When<TBuilder>(
        this      TBuilder       builder,
        [NotNull] IPredicate     predicate,
        Func<TBuilder, TBuilder> action
    )
        where TBuilder : class {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return predicate.Evaluate()
            ? action(builder)
            : builder;
    }

    /// <summary>
    ///     Applies an action only if the predicate evaluates to false.
    /// </summary>
    [return: NotNull]
    public static TBuilder Unless<TBuilder>(
        this      TBuilder                 builder,
        [NotNull] IPredicate               predicate,
        [NotNull] Func<TBuilder, TBuilder> action
    )
        where TBuilder : class {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        if (action    == null) throw new ArgumentNullException(nameof(action));

        return !predicate.Evaluate()
            ? action(builder)
            : builder;
    }


    /// <summary>
    ///     Applies a side-effect action without changing the builder.
    /// </summary>
    [return: NotNull]
    public static TBuilder Tap<TBuilder>(this TBuilder builder, Action<TBuilder> action)
        where TBuilder : class {
        action(builder);

        return builder;
    }

    /// <summary>
    ///     Transforms the builder and returns a different type.
    /// </summary>
    [return: NotNull]
    public static TResult Transform<TBuilder, TResult>(this TBuilder builder, Func<TBuilder, TResult> transform)
        where TBuilder : class {
        return transform(builder) ?? throw new InvalidOperationException("Transform function returned null.");
    }

    /// <summary>
    ///     Safely applies an action only if the builder is not null.
    /// </summary>
    public static TBuilder? IfNotNull<TBuilder>(this TBuilder? builder, Func<TBuilder, TBuilder> action)
        where TBuilder : class {
        return builder != null
            ? action(builder)
            : null;
    }

    /// <summary>
    ///     Tries to apply an action and continues gracefully on failure.
    /// </summary>
    [return: NotNull]
    public static TBuilder TryWith<TBuilder>(
        this TBuilder            builder,
        Func<TBuilder, TBuilder> action,
        Action<Exception>?       onError = null
    )
        where TBuilder : class {
        try { return action(builder); } catch (Exception ex) {
            onError?.Invoke(ex);

            return builder;
        }
    }

    /// <summary>
    ///     Repeats an action multiple times.
    /// </summary>
    [return: NotNull]
    public static TBuilder Repeat<TBuilder>(this TBuilder builder, int count, Func<TBuilder, TBuilder> action)
        where TBuilder : class {
        TBuilder result                        = builder;
        for (var i = 0; i < count; i++) result = action(result);

        return result;
    }

    /// <summary>
    ///     Applies an action for each item in a collection.
    /// </summary>
    [return: NotNull]
    public static TBuilder ForEach<TBuilder, T>(
        this TBuilder               builder,
        IEnumerable<T>              items,
        Func<TBuilder, T, TBuilder> action
    )
        where TBuilder : class {
        return items.Aggregate(builder, action);
    }

}