# Predicate System

The predicate system provides a way to create and combine boolean conditions in a readable, fluent syntax.

## Key Features

- Natural, human-readable syntax
- Strong typing
- Composable predicates
- Flexible evaluation

## Basic Usage

The most common way to use predicates is through the fluent API:

```csharp
bool result = Predicates
    .When(item).Matches(i => i.Value > 100)
    .AndAlso()
    .When(item).Matches(i => i.Weight < 5.0f)
    .Evaluate();
```

This approach clearly shows:

- What objects are being evaluated
- What conditions are being checked
- How the conditions are combined

## Advanced Usage

For more advanced scenarios, you can create domain-specific extension methods:

```csharp
// Define extension methods for your specific types
public static class ItemPredicateExtensions
{
    public static PredicateResult HasHighValue(this PredicateContext<Item> context)
    {
        return context.Matches(item => item.Value > 1000);
    }
    
    public static PredicateResult IsLightWeight(this PredicateContext<Item> context)
    {
        return context.Matches(item => item.Weight < 1.0f);
    }
}

// Use them in your code
bool isPremiumItem = Predicates
    .When(item).HasHighValue()
    .AndAlso()
    .When(item).IsLightWeight()
    .Evaluate();
```

## Legacy Support

If you prefer a more traditional approach, you can use the basic predicates directly:

```csharp
IPredicate predicate1 = new FuncPredicate(() => item.Value > 100);
IPredicate predicate2 = new FuncPredicate(() => item.Weight < 5.0f);
IPredicate combined = new AndPredicate(predicate1, predicate2);
bool result = combined.Evaluate();
```

Or with extension methods:

```csharp
IPredicate combined = (() => item.Value > 100).AsPredicate()
    .And((() => item.Weight < 5.0f).AsPredicate());
bool result = combined.Evaluate();
```

However, the fluent API is recommended for most scenarios as it's more readable and less error-prone.
