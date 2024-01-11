namespace ToolBX.Eloquentest;

public static class MockUtils
{
    public static IList<Mock<T>> CreateMany<T>(int count = 3) where T : class
    {
        var output = new List<Mock<T>>();
        for (var i = 0; i < count; i++)
            output.Add(new Mock<T>());
        return output;
    }

    /// <summary>
    /// Instantiates a generic Mock object of the specified type.
    /// </summary>
    public static Mock CreateFrom(Type type)
    {
        var typeArgs = new[] { type };
        var mockType = typeof(Mock<>);
        var constructed = mockType.MakeGenericType(typeArgs);
        return (Activator.CreateInstance(constructed) as Mock)!;
    }
}