using Batter.Utils.Builders;

namespace Batter.ItemValuation.Calculation;

/// <summary>
///     Builder interface for creating PriceCalculation instances.
/// </summary>
public interface ICalculationBuilder<TTarget, TCalculationFlags> :
    IFluentBuilder<TTarget, ICalculationBuilder<TTarget, TCalculationFlags>>,
    IFlaggable<TCalculationFlags>
    where TTarget : ICalculationTarget
    where TCalculationFlags : Enum { }