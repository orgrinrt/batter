namespace Batter.Utils.Builders;

/// <summary>
///     Represents a type that supports flag-based operations.
/// </summary>
/// <typeparam name="TFlags">The enum type used for flags.</typeparam>
public interface IFlaggable<TFlags>
    where TFlags : Enum {
    /// <summary>
    ///     Gets or sets the flags associated with this instance.
    /// </summary>
    TFlags Flags { get; set; }

    /// <summary>
    ///     Gets the current flags.
    /// </summary>
    /// <returns>The current flags.</returns>
    TFlags GetFlags();

    /// <summary>
    ///     Determines whether this instance has the specified flag.
    /// </summary>
    /// <param name="flag">The flag to check.</param>
    /// <returns>True if the flag is set; otherwise, false.</returns>
    bool HasFlag(TFlags flag);

    /// <summary>
    ///     Determines whether this instance has all the specified flags.
    /// </summary>
    /// <param name="flags">The flags to check.</param>
    /// <returns>True if all flags are set; otherwise, false.</returns>
    bool HasFlags(params TFlags[]? flags);

    /// <summary>
    ///     Adds the specified flag to this instance.
    /// </summary>
    /// <param name="flag">The flag to add.</param>
    void AddFlag(TFlags flag);

    /// <summary>
    ///     Adds the specified flags to this instance.
    /// </summary>
    /// <param name="flags">The flags to add.</param>
    void AddFlags(params TFlags[]? flags);

    /// <summary>
    ///     Removes the specified flag from this instance.
    /// </summary>
    /// <param name="flag">The flag to remove.</param>
    void RemoveFlag(TFlags flag);

    /// <summary>
    ///     Removes the specified flags from this instance.
    /// </summary>
    /// <param name="flags">The flags to remove.</param>
    void RemoveFlags(params TFlags[]? flags);

    /// <summary>
    ///     Clears all flags from this instance.
    /// </summary>
    void ClearFlags();
}