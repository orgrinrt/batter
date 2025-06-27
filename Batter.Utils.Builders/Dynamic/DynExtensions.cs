#region

using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Batter.Utils.Builders.Predicates;

#endregion

namespace Batter.Utils.Builders.Dynamic;

/// <summary>
///     The more cohesive, fluid and unified extension methods for IContainer.
/// </summary>
public static class DynExtensions {

    internal static bool IsPrimitiveTypeAndNotClass(this Type type) {
        return !type.IsClass && (type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal));
    }

    internal static bool IsPrimitiveTypeAndNotClass(this DynKey key) {
        return key.GetType().IsPrimitiveTypeAndNotClass();
    }

    public static TItem? Get<TItem>(this DynBuilder builder, DynKey key) {
        return builder.Get<TItem, DynBuilder, DynStorage<DynBuilder>, DynCollection<DynKey, object>>(key);
    }

    // NOTE: the below With<TItem> is a good example of what we'd want these helper extensions to do
    //        basically just make the api less strict and verbose, but still type-safe by conforming
    //        to the underlying builder api interfaces and their constraints (e.g ensure a cast is valid before
    //        doing it dynamically! but still allow to do it dynamically and having very lax constraints on this method itself)
    // TODO: we want to extend this same idea to the other integral methods, such as Remove, Get, Has, etc.


    public static DynBuilder With<TItem>(this DynBuilder builder, Func<DynBuilder, TItem> factory)
        where TItem : class {
        TItem item = factory(builder);

        return builder.With<TItem>(DynKey.INVALID, item);
    }

    public static TItem? Add<TItem>(this DynBuilder builder, DynKey key, params object[] ctorArgs)
        where TItem : class {
        // FIXME: obviously this should be the other way around; i.e this should implement what the below With<TItem> does,
        //        and delegate to this, but since that one was written first and this is more of an afterthought, let's
        //        do it like this for now for prototyping. NEEDS FIXING THOUGH! not efficient.

        DynBuilder _ = builder.With<TItem>(key, ctorArgs);

        return builder.Get<TItem>(key);
    }

    public static DynBuilder With<TItem>(this DynBuilder builder, DynKey key, params object[] ctorArgs)
        where TItem : class {
        return builder.With(new DynCtorCallArgs<TItem>((key, ctorArgs)).WithBuilder(builder));
    }


    public static DynBuilder With<TItem>(this DynBuilder builder, DynCtorCallArgs<TItem> args_)
        where TItem : class {
        DynKey?  key      = args_.Key;
        object[] ctorArgs = args_.CtorArgs;

        if (ctorArgs.Length == 1                                             &&
            key             == DynKey.INVALID                                &&
            typeof(TItem).GetInterfaces().Any(i => i.Name.Contains("IProp")) &&
            ctorArgs[0] is TItem ctorInstance) {
            // now we know it implements IProp, so we can call builder.WithProp directly
            // BUT since the TItem constraints aren't correct, we need to do this via reflection:
            // Get the method info for WithProp
            MethodInfo? method = typeof(DynBuilder).GetMethod(nameof(ContainerExtensions.WithProp),
                                                              new[] { typeof(DynKey), typeof(TItem) });

            if (method == null) {
                throw new InvalidOperationException(
                    $"Method {nameof(ContainerExtensions.WithProp)} not found for {nameof(DynBuilder)}");
            }

            // Create a generic method with the TItem type
            MethodInfo genericMethod = method.MakeGenericMethod(typeof(TItem));

            // Invoke the method with the key and the item
            return (DynBuilder)genericMethod.Invoke(builder, new object?[] { ctorInstance })!;
        }

        if (key == DynKey.INVALID) throw new ArgumentNullException(nameof(key), "Key cannot be invalid.");

        if (key == null) throw new ArgumentNullException(nameof(key), "Key cannot be null.");

        if (ctorArgs.Length == 1 && ctorArgs[0] is TItem item) return builder.With(key, item);


        if (ctorArgs.Length == 0) {
            Type             keyType = key.GetType();
            ConstructorInfo? c       = typeof(TItem).GetConstructor(new[] { keyType });

            if (c != null) return builder.With(key, Activator.CreateInstance(typeof(TItem), key) as TItem);

            // Try with the key's value type if the key itself isn't accepted
            c = typeof(TItem).GetConstructor(new[] { key.ValueType });

            if (c != null) return builder.With(key, Activator.CreateInstance(typeof(TItem), key.Value) as TItem);

            // Look for constructors that have the key's type or value type as first parameter 
            // and where all other parameters have default values
            ConstructorInfo[] constructors = typeof(TItem).GetConstructors();

            foreach (ConstructorInfo constructor in constructors) {
                ParameterInfo[] parameters = constructor.GetParameters();

                // Skip if no parameters
                if (parameters.Length == 0) continue;

                // Check if first parameter matches key type or key.ValueType
                bool firstParamMatches = parameters[0].ParameterType == keyType ||
                                         parameters[0].ParameterType == key.ValueType;

                if (!firstParamMatches) continue;

                // Check if all remaining parameters have default values
                var allOtherParamsHaveDefaults = true;

                for (var i = 1; i < parameters.Length; i++) {
                    if (!parameters[i].HasDefaultValue) {
                        allOtherParamsHaveDefaults = false;

                        break;
                    }
                }

                if (allOtherParamsHaveDefaults) {
                    // Create parameter array with key as first param and defaults for the rest
                    var constructorParams = new object[parameters.Length];

                    // First parameter is either key or key.Value
                    constructorParams[0] = parameters[0].ParameterType == keyType
                        ? key
                        : key.Value;

                    // Set default values for remaining parameters
                    for (var i = 1; i < parameters.Length; i++)
                        constructorParams[i] = parameters[i].DefaultValue ?? throw new InvalidOperationException();

                    // Create the instance
                    return builder.With(key, Activator.CreateInstance(typeof(TItem), constructorParams) as TItem);
                }
            }

            // Fall back to the default constructor if no suitable constructor with default parameters was found
            return builder.With(key, Activator.CreateInstance(typeof(TItem)) as TItem);

            // return builder.With(key,
            //                     Activator.CreateInstance(typeof(TItem),
            //                                              Activator.CreateInstance(typeof(TItem), key.Value)) as TItem);
        }

        bool isUnique = typeof(TItem).GetInterfaces().Any(t => t.Name.Contains("IUniqueProp"));

        // First, try to find a constructor that matches the provided arguments.
        Type[]           argTypes  = ctorArgs.Select(a => a.GetType()).ToArray();
        ConstructorInfo? ctor      = typeof(TItem).GetConstructor(argTypes);
        object[]?        finalArgs = ctorArgs;

        switch (ctor) {
            case not null when ctorArgs.Length > 0: {
                // If a constructor was found, invoke it. Otherwise, the instance will be null.
                var instance = ctor.Invoke(finalArgs) as TItem;

                return builder.With(key, instance);
            }
            // If a constructor was found, but no arguments were provided, try to find a default constructor.
            case not null when ctorArgs.Length == 0: {
                // Try to find a default constructor
                ctor = typeof(TItem).GetConstructor(Type.EmptyTypes);

                if (ctor != null) {
                    // If a default constructor was found, invoke it.
                    var instance = ctor.Invoke(null) as TItem;

                    return builder.With(key, instance);
                }

                break;
            }
            // If no matching constructor is found, try again by prepending the key to the arguments.
            case null when !isUnique: {
                finalArgs = new object[] { key }.Concat(ctorArgs).ToArray();
                Type[] newArgTypes = finalArgs.Select(a => a.GetType()).ToArray();
                ctor = typeof(TItem).GetConstructor(newArgTypes);

                // If a constructor was found, invoke it. Otherwise, the instance will be null.
                var instance = ctor?.Invoke(finalArgs) as TItem;

                if (instance == null) {
                    finalArgs   = new[] { key.Value }.Concat(ctorArgs).ToArray();
                    newArgTypes = finalArgs.Select(a => a.GetType()).ToArray();
                    ctor        = typeof(TItem).GetConstructor(newArgTypes);
                    // If a constructor was found, invoke it. Otherwise, the instance will be null.
                    instance = ctor?.Invoke(finalArgs) as TItem;
                }

                return builder.With(key, instance);
            }
            case null when isUnique: {
                // Try to find an appropriate constructor based on the key's actual type
                object           instanceCtorParam     = key.Value;
                Type             instanceCtorParamType = key.ValueType;
                ConstructorInfo? instanceCtor          = null;

                // First try with DynKey parameter
                instanceCtor = typeof(TItem).GetConstructor(new[] { typeof(DynKey) });

                // If not found, try with the actual value type
                if (instanceCtor == null) instanceCtor = typeof(TItem).GetConstructor(new[] { instanceCtorParamType });

                // If still not found, try with common type conversions
                if (instanceCtor == null && instanceCtorParamType != typeof(string)) {
                    instanceCtor = typeof(TItem).GetConstructor(new[] { typeof(string) });

                    if (instanceCtor != null) {
                        // We found a string constructor, convert the key to string
                        instanceCtorParam = instanceCtorParam.ToString() ??
                                            throw new InvalidCastException(
                                                $"Key value of type {instanceCtorParamType.Name} cannot be converted to string");
                    }
                }

                if (instanceCtor == null) {
                    throw new MissingMethodException(
                        $"Type {typeof(TItem).Name} does not have a compatible constructor for key of type {instanceCtorParamType.Name}");
                }

                // Create the instance using the appropriate value
                var i = (TItem)instanceCtor.Invoke(new[] { instanceCtorParam });
                Console.WriteLine($"typeof(TItem): {typeof(TItem).Name}, key: {key}, instance: {i}");

                return builder.WithUniqueProp(typeof(TItem), i);
            }
        }

        throw new MissingMethodException(
            $"Type {typeof(TItem).Name} does not have a compatible constructor for key {key}");
    }

    public static DynBuilder With<TItem>(this DynBuilder builder, TItem instance)
        where TItem : class, IProp<DynBuilder, TItem, IValidKey> {
        // return builder.WithProp(instance.Id, instance ?? throw new NoNullAllowedException());
        if (typeof(TItem).GetInterfaces().Any(i => i.Name.Contains("IUniqueProp"))) {
            // If TItem is a unique property, we can use the WithUniqueProp method
            return (builder as IContainer<DynBuilder, DynStorage<DynBuilder>, DynCollection<DynKey, object>>)
               .WithUniqueProp(typeof(TItem), instance ?? throw new NoNullAllowedException());
        }

        if (typeof(TItem).GetInterfaces().Any(i => i.Name.Contains("IProp"))) {
            // If TItem is a regular property, we can use the WithProp method
            return (builder as IContainer<DynBuilder, DynStorage<DynBuilder>, DynCollection<DynKey, object>>).WithProp(
                instance.Id,
                instance ?? throw new NoNullAllowedException());
        }

        return builder.With<TItem>(new object[] { instance });
    }
    //
    // public static DynBuilder With<TItem>(this DynBuilder builder, params object[] ctorArgs)
    //     where TItem : class, IUniqueProp<DynBuilder, TItem> {
    //     // First, try to find a constructor that matches the provided arguments.
    //     Type[]           argTypes  = ctorArgs.Select(a => a.GetType()).ToArray();
    //     ConstructorInfo? ctor      = typeof(TItem).GetConstructor(argTypes);
    //     object[]?        finalArgs = ctorArgs;
    //
    //     // If a constructor was found, invoke it. Otherwise, the instance will be null.
    //     var instance = ctor?.Invoke(finalArgs) as TItem;
    //
    //     return builder.WithUniqueProp(instance ?? throw new NoNullAllowedException());
    // }

    public static DynBuilder WithCtor<TItem>(this DynBuilder builder, params object[] ctorArgs)
        where TItem : class {
        // First, try to find a constructor that matches the provided arguments.
        Type[]           argTypes  = ctorArgs.Select(a => a.GetType()).ToArray();
        ConstructorInfo? ctor      = typeof(TItem).GetConstructor(argTypes);
        object[]?        finalArgs = ctorArgs;

        // If a constructor was found, invoke it. Otherwise, the instance will be null.
        var instance = ctor?.Invoke(finalArgs) as TItem;

        return (builder as IContainer<DynBuilder, DynStorage<DynBuilder>, DynCollection<DynKey, object>>)
           .WithUniqueProp(typeof(TItem), instance ?? throw new NoNullAllowedException());
    }

    [return: NotNull]
    internal static IfResult<TItem> If<TItem>(
        [NotNullWhen(true)] this TItem?     prop,
        [NotNullWhen(true)]      IPredicate predicate
    )
        where TItem : IValidProperty, IBuildableObj {
        if (prop is null) return IfResult<TItem>.ItemNull();

        return predicate.Evaluate()
            ? IfResult<TItem>.True(prop)
            : IfResult<TItem>.False(prop);
    }

    [return: NotNull]
    internal static IfResult<DynKey> If(
        [NotNullWhen(true)] this DynKey?    key,
        [NotNullWhen(true)]      IPredicate predicate
    ) {
        if (key is null) return IfResult<DynKey>.ItemNull();

        return predicate.Evaluate()
            ? IfResult<DynKey>.True(key)
            : IfResult<DynKey>.False(key);
    }

    [return: NotNull]
    internal static IfResult<Enum> If([NotNullWhen(true)] this Enum? key, [NotNullWhen(true)] IPredicate predicate) {
        if (key is null) return IfResult<Enum>.ItemNull();

        return predicate.Evaluate()
            ? IfResult<Enum>.True(key)
            : IfResult<Enum>.False(key);
    }

}

public sealed class DynCtorCallArgs<TItem>
    where TItem : class {

    public DynKey   Key      { get; }
    public object[] CtorArgs { get; }

    public TItem? Item { get; private set; }

    internal DynBuilder? Builder { get; set; }

    public DynCtorCallArgs(DynKey key, params object[] ctorArgs) {
        this.Key      = key;
        this.CtorArgs = ctorArgs;
    }

    public DynCtorCallArgs((DynKey, object[]) args) : this(args.Item1, args.Item2) { }

    public DynCtorCallArgs(params object[] args) : this(DynKey.INVALID, args) { }

    public DynCtorCallArgs(TItem           item) : this(DynKey.INVALID) { this.Item = item; }
    public DynCtorCallArgs(DynKey          key, TItem item) : this(key) { this.Item = item; }
    public DynCtorCallArgs((DynKey, TItem) args) : this(args.Item1) { this.Item = args.Item2; }

    public DynCtorCallArgs<TItem> WithBuilder(DynBuilder builder) {
        this.Builder = builder ?? throw new ArgumentNullException(nameof(builder), "Builder cannot be null.");

        return this;
    }

    public DynCtorCallArgs<TItem>? AddTo(DynBuilder builder) {
        if (builder == null) throw new ArgumentNullException(nameof(builder), "Builder cannot be null.");

        // Use the builder to create an instance of TResult using the provided key and constructor arguments.
        if (this.Key != DynKey.INVALID && !builder.Storage.Collection.ContainsKey(this.Key))
            this.Item = builder.Add<TItem>(this.Key, this.CtorArgs);

        return this;
    }

    public DynCtorCallArgs<TItem>? Add(DynBuilder? builder = null) {
        DynBuilder b = builder ?? this.Builder ?? throw new ArgumentNullException(nameof(builder));

        // Use the builder to create an instance of TResult using the provided key and constructor arguments.
        return this.AddTo(b);
    }

    public DynCtorCallArgs<TItem> Configure(Func<TItem, TItem> configure) {
        if (configure == null) throw new ArgumentNullException(nameof(configure), "Configure function cannot be null.");

        // If the item is null, throw an exception.
        if (this.Item == null)
            throw new InvalidOperationException("Item has to be created before configuration can be applied.");

        // Apply the configuration function to the item.
        this.Item = configure(this.Item);

        return this;
    }

    public DynCtorCallArgs<TItem> Configure(DynBuilder builder, Func<DynBuilder, TItem> configure) {
        if (configure == null) throw new ArgumentNullException(nameof(configure), "Configure function cannot be null.");

        this.Item = configure(builder.With<TItem>(this.Key, this.CtorArgs));

        return this;
    }

    public DynCtorCallArgs<TItem> Configure(Func<DynBuilder, TItem> configure) {
        return this.Configure(this.Builder!, configure);
    }

    public static implicit operator (DynKey, object[])(DynCtorCallArgs<TItem> args) {
        return (args.Key,
                args.CtorArgs.Length > 0     ? args.CtorArgs :
                args.Item            != null ? new object[] { args.Item } : new object[] { });
    }

    public static explicit operator (DynKey, TItem)(DynCtorCallArgs<TItem> args) {
        return (args.Item is IValidProperty prop
                    ? prop.GetKey()
                    : args.Key, args.Item ?? throw new InvalidOperationException("Item cannot be null."));
    }

    public static implicit operator DynCtorCallArgs<TItem>((DynKey, object[]) args) { return new(args); }
    public static implicit operator DynCtorCallArgs<TItem>((DynKey, TItem)    args) { return new(args); }

    public static implicit operator DynCtorCallArgs<TItem>(object[] args) { return new(args); }

    public static implicit operator DynCtorCallArgs<TItem>(DynKey key) { return new(key); }

    public static implicit operator DynCtorCallArgs<TItem>(TItem item) { return new(item); }

}