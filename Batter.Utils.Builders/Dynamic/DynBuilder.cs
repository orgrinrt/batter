namespace Batter.Utils.Builders.Dynamic;

public abstract class DynBuilder<TResult, TThis> : DynBuilder<TResult>
    where TResult : IBuildableObj
    where TThis : DynBuilder<TResult, TThis>, new() {

    /// <summary>
    ///     Initializes a new instance of the DynBuilder.
    /// </summary>
    public DynBuilder() { }

    public static TThis New() { return new(); }

    // public static TThis Create() {
    //     return (TThis)(Activator.CreateInstance(typeof(TThis)) ??
    //                    throw new InvalidOperationException(
    //                        "Failed to create instance of type " + typeof(TThis).FullName));
    // }

}

public abstract class DynBuilder<TResult> : DynBuilder
    where TResult : IBuildableObj {

    public abstract TResult Build(TResult? nonDefaultStartingPoint = default);

    public override T? Build<T>(T? nonDefaultStartingPoint = default)
        where T : class {
        if (nonDefaultStartingPoint is TResult dynBuildable) return this.Build(dynBuildable) as T;

        if (typeof(T) == typeof(TResult)) return this.Build() as T;

        throw new InvalidOperationException($"Cannot build {typeof(TResult).Name}");
    }

}

/// <summary>
///     Ultra-simplified builder base class that eliminates generic complexity
///     and provides excellent developer experience for common scenarios.
///     Now implements IBuilder for full compatibility with the builder ecosystem.
/// </summary>
/// <typeparam name="TResult">The type being built</typeparam>
public abstract class DynBuilder : IBuilder<DynBuildable, DynBuilder, DynStorage<DynBuilder>> {

    protected readonly DynBuildable?          _buildable = null;
    protected readonly DynStorage<DynBuilder> _storage   = new();
    private            bool                   _isInitialized;

    /// <summary>
    ///     Initializes a new instance of the DynBuilder.
    /// </summary>
    public DynBuilder() {
        if (!this._isInitialized) this.Init();
    }


    /// <inheritdoc />
    public DynBuilder Self() { return this; }

    /// <inheritdoc />
    public DynBuilder Init() {
        // nothing to do currently
        this._isInitialized = true;

        return this;
    }

    /// <inheritdoc />
    public DynBuilder Clone() {
        return this; // TODO: actually clone
    }

    /// <inheritdoc />
    public DynStorage<DynBuilder> Storage => this._storage;

    /// <inheritdoc />
    public virtual TResult? Build<TResult>(TResult? nonDefaultStartingPoint = null)
        where TResult : class, IBuildableObj {
        if (this is DynBuilder<TResult> dynBuilder)
            return dynBuilder.Build(nonDefaultStartingPoint as DynBuildable ?? this._buildable) as TResult;

        return this.Build((object?)nonDefaultStartingPoint ?? this._buildable ?? null) as TResult;
    }

    /// <inheritdoc />
    public virtual DynBuildable Build(object? nonDefaultStartingPoint = null) {
        // NOTE (original todo):
        //       0. The very first thing is check if *we* are a DynBuilder<TResult, TThis> type, and if so, we can just cast to TThis and call Build<TResult> on it directly, done and done!
        //       1. if either not, or the above one fails; check if nonDefaultStartingPoint is not null and it's a type that implements IBuildableObj
        //       2. if so, cast and use the DynBuilder<TResult> version to build it (delegate to the actual end-user impl)
        //       3. on catch, if above fails, fall back and continue further:
        //       4. see if we have a valid type that implements IBuildableObj in this._buildable
        //       5. if so, use it to build the result with the DynBuilder<TResult> version, again
        //       6. if that fails, throw an InvalidOperationException
        // 0. Check if we are a DynBuilder<TResult, TThis> type
        Type? resultType = null;
        Type  thisType   = this.GetType();

        foreach (Type interfaceType in this.GetType().GetInterfaces()) {
            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(DynBuilder<,>)) {
                resultType = interfaceType.GetGenericArguments()[0];
                thisType   = interfaceType.GetGenericArguments()[1];

                break;
            }

            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(DynBuilder<>)) {
                resultType = interfaceType.GetGenericArguments()[0];

                break;
            }
        }

        if (resultType == null) {
            // Console.WriteLine($"DynBuilder: No matching interface found for {thisType.FullName}. Trying base type");
            // If we didn't find a matching interface, try the base type
            Type? baseType = thisType.BaseType;
            if (baseType == null) Console.WriteLine($"DynBuilder: No basetype found for {thisType.FullName}");
            // Console.WriteLine($"DynBuilder: Base type is {baseType?.FullName}");

            if (baseType!.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(DynBuilder<,>)) {
                resultType = baseType.GetGenericArguments()[0];
                thisType   = baseType.GetGenericArguments()[1];
            }

            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(DynBuilder<>))
                resultType = baseType.GetGenericArguments()[0];
        }

        if (resultType == null) {
            // If we still don't have a result type, throw an exception
            throw new InvalidOperationException(
                "DynBuilder: No valid result type could be inferred from interfaces or basetype for " +
                thisType.FullName);
        }

        {
            // We are a DynBuilder<TResult, TThis>, call Build<TResult> directly
            object? result = thisType.GetMethod("Build", new[] { resultType })
                                    ?.Invoke(this, new[] { nonDefaultStartingPoint ?? this._buildable });


            switch (result) {
                case DynBuildable dynResult: return dynResult;
                case null:
                    throw new InvalidOperationException(
                        $"DynBuilder: No suitable buildable found for {thisType.FullName}");
            }

            if (resultType.IsInstanceOfType(result)) {
                // First try to cast to DynBuildable<resultType>
                try { return (DynBuildable)Convert.ChangeType(result, resultType); } catch {
                    try { return new(result); } catch {
                        // Both casts failed, will continue to fallback methods
                    }
                }

                Type dynBuildableGenericType = typeof(DynBuildable<>).MakeGenericType(resultType);

                // First try to cast directly to resultType, then to DynBuildable<resultType>
                if (dynBuildableGenericType.IsInstanceOfType(result)) {
                    // Result is already of the right type, try to convert to DynBuildable
                    try { return (DynBuildable)Convert.ChangeType(result, dynBuildableGenericType); } catch {
                        try { return new(result); } catch {
                            // Both casts failed, will continue to fallback methods
                        }
                    }
                }
            }
        }

        try {
            DynBuildable? result = this.Build((nonDefaultStartingPoint ?? this._buildable) as DynBuildable);

            if (result != null) return result;
        } catch {
            // Fall through to next step
        }

        // 6. If all else fails, throw an exception
        throw new InvalidOperationException("Could not build a result. No valid builder implementation found.");
    }


    /// <summary>
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static implicit operator DynStorage<DynBuilder>(DynBuilder builder) { return builder.Storage; }

}