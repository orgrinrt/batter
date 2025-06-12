namespace Batter.Utils.Builders;

/// <summary>
///     Base implementation for types that support flag-based operations.
/// </summary>
/// <typeparam name="TFlags">The enum type used for flags.</typeparam>
public abstract class FlaggableBase<TFlags> : IFlaggable<TFlags>
    where TFlags : Enum {
    private static readonly TFlags _EMPTY = default!; // should be default of enum, i.e integer, not null

    /// <summary>
    /// Initializes a new instance of the <see cref="FlaggableBase{TFlags}"/> class.
    /// </summary>
    protected FlaggableBase() => this.Flags = FlaggableBase<TFlags>._EMPTY;

    /// <summary>
    /// Gets or sets the flags currently applied to this instance.
    /// </summary>
    public TFlags Flags { get; set; }

    /// <summary>
    /// Gets the current flags applied to this instance.
    /// </summary>
    /// <returns>The current flags.</returns>
    public TFlags GetFlags() {
        return this.Flags;
    }

    /// <summary>
    /// Determines whether this instance has the specified flag.
    /// </summary>
    /// <param name="flag">The flag to check for.</param>
    /// <returns><c>true</c> if the instance has the flag; otherwise, <c>false</c>.</returns>
    public bool HasFlag(TFlags flag) {
        return (Convert.ToInt32(this.Flags) & Convert.ToInt32(flag)) == Convert.ToInt32(flag);
    }

    /// <summary>
    /// Determines whether this instance has all the specified flags.
    /// </summary>
    /// <param name="flags">The flags to check for.</param>
    /// <returns><c>true</c> if the instance has all the specified flags; otherwise, <c>false</c>.</returns>
    public bool HasFlags(params TFlags[]? flags) {
        if (flags == null || flags.Length == 0)
            return true;

        var mask = flags.Aggregate(0, (current, f) => current | Convert.ToInt32(f));
        return (Convert.ToInt32(this.Flags) & mask) == mask;
    }

    /// <summary>
    /// Adds the specified flag to this instance.
    /// </summary>
    /// <param name="flag">The flag to add.</param>
    public void AddFlag(TFlags flag) {
        if (this.HasFlag(flag))
            return;

        this.Flags = (TFlags)Convert.ChangeType(Convert.ToInt32(this.Flags) | Convert.ToInt32(flag), typeof(TFlags));
    }

    /// <summary>
    /// Adds the specified flags to this instance.
    /// </summary>
    /// <param name="flags">The flags to add.</param>
    public void AddFlags(params TFlags[]? flags) {
        if (flags == null || flags.Length == 0)
            return;

        foreach (var flag in flags) this.AddFlag(flag);
    }

    /// <summary>
    /// Removes the specified flag from this instance.
    /// </summary>
    /// <param name="flag">The flag to remove.</param>
    public void RemoveFlag(TFlags flag) {
        if (!this.HasFlag(flag))
            return;

        this.Flags = (TFlags)Convert.ChangeType(Convert.ToInt32(this.Flags) & ~Convert.ToInt32(flag), typeof(TFlags));
    }

    /// <summary>
    /// Removes the specified flags from this instance.
    /// </summary>
    /// <param name="flags">The flags to remove.</param>
    public void RemoveFlags(params TFlags[]? flags) {
        if (flags == null || flags.Length == 0)
            return;

        foreach (var flag in flags) this.RemoveFlag(flag);
    }

    /// <summary>
    /// Clears all flags from this instance.
    /// </summary>
    public void ClearFlags() {
        this.Flags = FlaggableBase<TFlags>._EMPTY;
    }
}