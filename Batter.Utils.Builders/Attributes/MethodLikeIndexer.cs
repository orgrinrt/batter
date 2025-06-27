namespace Batter.Utils.Builders.Attributes;

/// <summary>
///     Indicates that an indexer should be treated more like a method call in terms of code style and formatting.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class MethodLikeIndexerAttribute : Attribute { }