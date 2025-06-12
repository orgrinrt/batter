using System.Collections.Generic;
using Batter.ItemValuation.Modifiers;

namespace Batter.ItemValuation.Tests.Mocks;

/// <summary>
/// A simple test property implementation for testing the evaluation system.
/// </summary>
public class TestProperty : EvaluatedPropertyBase<List<IModifier>>
{
    public TestProperty(string name, float baseValue) : base(name, baseValue)
    {
    }
}
