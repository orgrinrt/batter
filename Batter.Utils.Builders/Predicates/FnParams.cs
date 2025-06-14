namespace Batter.Utils.Builders.Predicates;

/// <summary>
/// 
/// </summary>
public struct FnParams {
    /// <summary>
    /// 
    /// </summary>
    public object[] Args { get; }

    /// <summary>
    /// 
    /// </summary>
    public ushort ExpectedArgsCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public FnParams(params object[] args) {
        this.ExpectedArgsCount = (ushort)args.Length;
        this.Args = args ?? throw new ArgumentNullException(nameof(args));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="expectedArgsCount"></param>
    public FnParams(ushort expectedArgsCount) {
        this.ExpectedArgsCount = expectedArgsCount;
        this.Args = Array.Empty<object>();
    }

    /// <summary>
    /// 
    /// </summary>
    public FnParams() => this = new(0);
}