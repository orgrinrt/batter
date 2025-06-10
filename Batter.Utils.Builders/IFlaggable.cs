namespace Batter.Utils.Builders;

public interface IFlaggable<TFlags>
    where TFlags : Enum {
    static readonly TFlags EMPTY = default;
    internal TFlags Flags { get; set; }

    TFlags GetFlags() {
        return this.Flags;
    }

    bool HasFlag(TFlags flag) {
        return (Convert.ToInt32(this.Flags) & Convert.ToInt32(flag)) == Convert.ToInt32(flag);
    }

    bool HasFlags(params TFlags[]? flags) {
        if (flags == null || flags.Length == 0)
            return true;

        var mask = flags.Aggregate(0, (current, f) => current | Convert.ToInt32(f));
        return (Convert.ToInt32(this.Flags) & mask) == mask;
    }

    void AddFlag(TFlags flag) {
        if (this.HasFlag(flag))
            return;

        this.Flags = (TFlags)Convert.ChangeType(Convert.ToInt32(this.Flags) | Convert.ToInt32(flag), typeof(TFlags));
    }

    void AddFlags(params TFlags[]? flags) {
        if (flags == null || flags.Length == 0)
            return;

        foreach (var flag in flags) this.AddFlag(flag);
    }


    void RemoveFlag(TFlags flag) {
        if (!this.HasFlag(flag))
            return;

        this.Flags = (TFlags)Convert.ChangeType(Convert.ToInt32(this.Flags) & ~Convert.ToInt32(flag), typeof(TFlags));
    }

    void RemoveFlags(params TFlags[]? flags) {
        if (flags == null || flags.Length == 0)
            return;

        foreach (var flag in flags) this.RemoveFlag(flag);
    }

    void ClearFlags() {
        this.Flags = IFlaggable<TFlags>.EMPTY;
    }
}