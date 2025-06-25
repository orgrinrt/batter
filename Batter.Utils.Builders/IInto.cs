#region

    using System.Diagnostics.CodeAnalysis;

#endregion

    namespace Batter.Utils.Builders;

    /// <summary>
    ///     Provides a mechanism for converting the current instance into another type.
    ///     This interface is commonly used in builder patterns to enable type transformations
    ///     and fluent method chaining across different builder types.
    /// </summary>
    /// <typeparam name="TInto">The target type to convert into</typeparam>
    public interface IInto<out TInto> {

        /// <summary>
        ///     Converts the current instance to the specified target type.
        ///     This method enables type transformations in builder patterns,
        ///     allowing builders to be cast or converted to related types.
        /// </summary>
        /// <returns>The converted instance of type <typeparamref name="TInto" /></returns>
        [return: NotNull]
        TInto Into();

    }

    /// <summary>
    ///     Provides a mechanism for converting the current instance from another type.
    ///     This interface serves as the inverse of <see cref="IInto{TInto}" /> and is used
    ///     when you need to extract or convert back from a transformed type.
    /// </summary>
    /// <typeparam name="TFrom">The source type to convert from</typeparam>
    /// <typeparam name="TThis"></typeparam>
    public interface IFrom<in TFrom, out TThis> {

        /// <summary>
        ///     Converts the current instance from the specified source type.
        ///     This method enables reverse type transformations in builder patterns,
        ///     allowing extraction of the original or underlying type.
        /// </summary>
        /// <returns>The converted instance of type <typeparamref name="TFrom" /></returns>
        [return: NotNull]
        TThis From(TFrom other);

    }