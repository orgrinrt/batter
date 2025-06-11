namespace Batter.ItemValuation.Calculation;

/// <summary>
/// Defines an interface for calculations that can be performed and applied to targets.
/// </summary>
/// <remarks>
/// Calculations represent operations that can be performed to generate values dynamically,
/// typically used by modifiers to apply complex transformations.
/// </remarks>
/// <example>
/// <code>
/// public class ScalingCalculation : ICalculation
/// {
///     private readonly float _baseValue;
///     private readonly float _scaleFactor;
///     private ICalculationTarget _target;
///     
///     public ScalingCalculation(float baseValue, float scaleFactor)
///     {
///         _baseValue = baseValue;
///         _scaleFactor = scaleFactor;
///     }
///     
///     internal ref ICalculationTarget Target => ref _target;
///     
///     public float Calculate()
///     {
///         return _baseValue * _scaleFactor;
///     }
/// }
/// 
/// // Usage:
/// var calculation = new ScalingCalculation(10.0f, 1.5f);
/// var modifier = new CalculatedModifier(calculation);
/// property.AddModifier(modifier);
/// </code>
/// </example>
public interface ICalculation {
    /// <summary>
    /// Gets a reference to the target this calculation is applied to.
    /// </summary>
    internal ref ICalculationTarget Target { get; }
    
    /// <summary>
    /// Performs the calculation and returns the result.
    /// </summary>
    /// <returns>The result of the calculation.</returns>
    /// <example>
    /// <code>
    /// var calculation = new ScalingCalculation(10.0f, 1.5f);
    /// float result = calculation.Calculate(); // Returns 15.0f
    /// </code>
    /// </example>
    float Calculate();
}