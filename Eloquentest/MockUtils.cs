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
}