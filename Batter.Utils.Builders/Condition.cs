#region

using Batter.Utils.Builders.Predicates;

#endregion

namespace Batter.Utils.Builders;

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
    public static implicit operator Condition(FuncPredicate predicate) {
        return new(predicate.Evaluate);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static implicit operator FuncPredicate(Condition condition) {
        return new(condition._condition);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static implicit operator Condition(NotPredicate predicate) {
        return new(predicate.Evaluate);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static implicit operator NotPredicate(Condition condition) {
        // FIXME: this is a little bit of a hack, needs optimising since that's not efficient
        return new(new FuncPredicate(() => !condition._condition()));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static implicit operator Condition(ConstantPredicate predicate) {
        return new(predicate.Evaluate);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static implicit operator ConstantPredicate(Condition condition) {
        return new(condition._condition());
    }
}