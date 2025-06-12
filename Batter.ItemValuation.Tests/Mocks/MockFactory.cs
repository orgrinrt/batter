#region

using Moq;

#endregion

namespace Batter.ItemValuation.Tests.Mocks;

/// <summary>
/// Provides utility methods for creating mock objects for testing.
/// </summary>
public static class MockFactory {
    /// <summary>
    /// Creates a mock IItemObject with the specified string ID.
    /// </summary>
    /// <param name="stringId">The string ID to assign to the mock IItemObject.</param>
    /// <returns>A mock IItemObject instance.</returns>
    public static IItemObject CreateMockIItemObject(string stringId) {
        var mockIItemObject = new Mock<IItemObject>();
        mockIItemObject.Setup(item => item.StringId).Returns(stringId);

        return mockIItemObject.Object;
    }
}

public class MockItemObject : IItemObject {
    public MockItemObject(string stringId) => this.StringId = stringId;

    public string StringId { get; }
}