namespace Batter.Utils.Builders.Predicates;

/// <summary>
/// 
/// </summary>
public class Condition {
    private readonly Func<bool> _condition;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    public Condition(Func<bool> condition) => this._condition = condition;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    public Condition(Fn predicate) => this._condition = predicate;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    public Condition(Const predicate) => this._condition = () => predicate;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static Func<bool> Default => () => false; // Default condition always returns false (no condition).

    /// <summary>
    /// 
    /// </summary>
    public static Func<bool> True => () => true; // Default condition always returns true (no condition).

    /// <summary>
    /// 
    /// </summary>
    public static Func<bool> False => () => false; // Default condition always returns false (no condition).

    /// <summary>
    /// 
    /// </summary>
    public static Func<Func<bool>, bool> If => condition => condition();

    /// <summary>
    /// 
    /// </summary>
    public static Func<Func<bool>, bool> Unless => condition => !condition();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static implicit operator Func<bool>(Condition condition) {
        return condition._condition;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static implicit operator Condition(Func<bool> condition) {
        return new(condition);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static implicit operator Condition(bool condition) {
        return new(() => condition);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static implicit operator Condition(Fn predicate) {
        return new(predicate.Evaluate);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static implicit operator Fn(Condition condition) {
        return new(condition._condition);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static implicit operator Condition(Not predicate) {
        return new(predicate.Evaluate);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static implicit operator Not(Condition condition) {
        // FIXME: this is a little bit of a hack, needs optimising since that's not efficient
        // TODO: also this might not actually work as expected if condition is already a Not condition
        return new(new Fn(() => !condition._condition()));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static implicit operator Condition(Const predicate) {
        return new(predicate.Evaluate);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static implicit operator Const(Condition condition) {
        return new(condition._condition());
    }
}