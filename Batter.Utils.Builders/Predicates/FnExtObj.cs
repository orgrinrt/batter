namespace Batter.Utils.Builders.Predicates;

public sealed partial class Fn {

    /// <summary>
    ///     Initializes a new instance of the <see cref="Fn" /> class.
    /// </summary>
    /// <param name="func">The function to evaluate.</param>
    public Fn(Func<bool> func) {
        this._params = new();
        this._func   = _ => func();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Fn" /> class.
    /// </summary>
    /// <param name="func">The function to evaluate.</param>
    public Fn(Func<object, bool> func) {
        this._params = new(1);
        this._func   = arg => func(arg);
    }

    /// <summary>
    /// </summary>
    /// <param name="func"></param>
    public Fn(Func<object, object, bool> func) {
        this._params = new(2);
        this._func   = args => func(args.Args[0], args.Args[1]);
    }

    /// <summary>
    /// </summary>
    /// <param name="func"></param>
    public Fn(Func<object, object, object, bool> func) {
        this._params = new(3);
        this._func   = args => func(args.Args[0], args.Args[1], args.Args[2]);
    }

    /// <summary>
    /// </summary>
    /// <param name="func"></param>
    public Fn(Func<object, object, object, object, bool> func) {
        this._params = new(4);
        this._func   = args => func(args.Args[0], args.Args[1], args.Args[2], args.Args[3]);
    }

    /// <summary>
    /// </summary>
    /// <param name="func"></param>
    public Fn(Func<object, object, object, object, object, bool> func) {
        this._params = new(5);
        this._func   = args => func(args.Args[0], args.Args[1], args.Args[2], args.Args[3], args.Args[4]);
    }

    /// <summary>
    /// </summary>
    /// <param name="func"></param>
    public Fn(Func<object, object, object, object, object, object, bool> func) {
        this._params = new(6);
        this._func   = args => func(args.Args[0], args.Args[1], args.Args[2], args.Args[3], args.Args[4], args.Args[5]);
    }

    /// <summary>
    /// </summary>
    /// <param name="func"></param>
    public Fn(Func<object, object, object, object, object, object, object, bool> func) {
        this._params = new(7);

        this._func = args => func(args.Args[0],
                                  args.Args[1],
                                  args.Args[2],
                                  args.Args[3],
                                  args.Args[4],
                                  args.Args[5],
                                  args.Args[6]);
    }

    /// <summary>
    /// </summary>
    /// <param name="func"></param>
    public Fn(Func<object, object, object, object, object, object, object, object, bool> func) {
        this._params = new(8);

        this._func = args => func(args.Args[0],
                                  args.Args[1],
                                  args.Args[2],
                                  args.Args[3],
                                  args.Args[4],
                                  args.Args[5],
                                  args.Args[6],
                                  args.Args[7]);
    }

}