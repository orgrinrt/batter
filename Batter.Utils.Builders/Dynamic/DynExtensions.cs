#region

using System.Data;
using System.Reflection;

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

    public static DynBuilder With<TItem>(this DynBuilder builder, DynKey key, params object[] ctorArgs)
        where TItem : class {
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
        return builder.WithProp(instance.Id, instance ?? throw new NoNullAllowedException());
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

}