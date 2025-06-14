#region

using Batter.Utils.Builders.Predicates;

#endregion

namespace Batter.Utils.Builders;

/// <summary>
/// 
/// </summary>
public interface IFluentApiMethods<out TThis> {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <typeparam name="TThis"></typeparam>
    /// <returns></returns>
    TThis When(IPredicate predicate);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <typeparam name="TThis"></typeparam>
    /// <returns></returns>
    TThis If(
        bool condition);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <typeparam name="TThis"></typeparam>
    /// <returns></returns>
    TThis IfNot(
        bool condition);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <typeparam name="TThis"></typeparam>
    /// <returns></returns>
    TThis IfNot(
        IPredicate predicate);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <typeparam name="TThis"></typeparam>
    /// <returns></returns>
    TThis And(
        bool condition);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <typeparam name="TThis"></typeparam>
    /// <returns></returns>
    TThis And(
        IPredicate predicate);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <typeparam name="TThis"></typeparam>
    /// <returns></returns>
    TThis AndNot(
        bool condition);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <typeparam name="TThis"></typeparam>
    /// <returns></returns>
    TThis AndNot(
        IPredicate predicate);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <typeparam name="TThis"></typeparam>
    /// <returns></returns>
    TThis Or(
        bool condition);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <typeparam name="TThis"></typeparam>
    /// <returns></returns>
    TThis Or(
        IPredicate predicate);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="TThis"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    bool Equals<TValue>(
        TValue value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="TThis"></typeparam>
    /// <returns></returns>
    bool Equals(
        object value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="TThis"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    bool DoesntEqual<TValue>(
        TValue value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="TThis"></typeparam>
    /// <returns></returns>
    bool DoesntEqual(
        object value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="op"></param>
    /// <typeparam name="TThis"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    TThis Matches<TValue>(
        Func<TThis, TValue> op);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="op"></param>
    /// <typeparam name="TThis"></typeparam>
    /// <returns></returns>
    TThis Matches(
        Func<TThis, object> op);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actions"></param>
    /// <typeparam name="TThis"></typeparam>
    /// <returns></returns>
    TThis Chain(
        params Func<TThis, object>[] actions);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="action"></param>
    /// <typeparam name="TThis"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    TThis Chain<TValue>(
        params Func<TThis, TValue>[] action);
}