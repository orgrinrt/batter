namespace Batter.Utils.Builders;

/// <summary>
///     Base implementation for types that support flag-based operations.
/// </summary>
/// <typeparam name="TFlags">The enum type used for flags.</typeparam>
public abstract class FlaggableBase<TFlags> : IFlaggable<TFlags>
    where TFlags : Enum {
    private static readonly TFlags _EMPTY = default!; // should be default of enum, i.e integer, not null

    protected FlaggableBase() => this.Flags = FlaggableBase<TFlags>._EMPTY;

    public TFlags Flags { get; set; }

    public TFlags GetFlags() {
        return this.Flags;
    }

    public bool HasFlag(TFlags flag) {
        return (Convert.ToInt32(this.Flags) & Convert.ToInt32(flag)) == Convert.ToInt32(flag);
    }

    public bool HasFlags(params TFlags[]? flags) {
        if (flags == null || flags.Length == 0)
            return true;

        var mask = flags.Aggregate(0, (current, f) => current | Convert.ToInt32(f));
        return (Convert.ToInt32(this.Flags) & mask) == mask;
    }

    public void AddFlag(TFlags flag) {
        if (this.HasFlag(flag))
            return;

        this.Flags = (TFlags)Convert.ChangeType(Convert.ToInt32(this.Flags) | Convert.ToInt32(flag), typeof(TFlags));
    }

    public void AddFlags(params TFlags[]? flags) {
        if (flags == null || flags.Length == 0)
            return;

        foreach (var flag in flags) this.AddFlag(flag);
    }

    public void RemoveFlag(TFlags flag) {
        if (!this.HasFlag(flag))
            return;

        this.Flags = (TFlags)Convert.ChangeType(Convert.ToInt32(this.Flags) & ~Convert.ToInt32(flag), typeof(TFlags));
    }

    public void RemoveFlags(params TFlags[]? flags) {
        if (flags == null || flags.Length == 0)
            return;

        foreach (var flag in flags) this.RemoveFlag(flag);
    }

    public void ClearFlags() {
        this.Flags = FlaggableBase<TFlags>._EMPTY;
    }
}