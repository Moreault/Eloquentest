namespace ToolBX.Eloquentest.Extensions;

public static class MockExtensions
{
    public static IList<T> GetObjects<T>(this IEnumerable<Mock<T>> mocks) where T : class => mocks.Select(x => x.Object).ToList();

    public static IEnumerable<ISetup<TMock, TResult>> Setup<TMock, TResult>(this IEnumerable<Mock<TMock>> mocks, Expression<Func<TMock, TResult>> expression) where TMock : class
    {
        return mocks.Select(x => x.Setup(expression)).ToList();
    }

    public static IEnumerable<IReturnsResult<TMock>> Returns<TMock, TResult>(this IEnumerable<IReturns<TMock, TResult>> r, TResult value) where TMock : class
    {
        return r.Select(x => x.Returns(value)).ToList();
    }

    public static IEnumerable<IReturnsResult<TMock>> Returns<TMock, TResult>(this IEnumerable<IReturns<TMock, TResult>> r, Func<TResult> value) where TMock : class
    {
        return r.Select(x => x.Returns(value)).ToList();
    }

    public static IEnumerable<IReturnsResult<TMock>> Returns<TMock, TResult>(this IEnumerable<IReturns<TMock, TResult>> r, IEnumerable<TResult> value) where TMock : class
    {
        var returns = r.ToList();
        var values = value.ToList();
        if (returns.Count != values.Count)
            throw new Exception($"Returns and values must be equal in size but they had {returns.Count} and {values.Count} respectively.");

        return returns.Select((t, i) => t.Returns(values[i])).ToList();
    }

    public static void VerifySet<TMock>(this IEnumerable<Mock<TMock>> mocks, Action<TMock> expression) where TMock : class
    {
        foreach (var m in mocks)
            m.VerifySet(expression);
    }

    public static void VerifySet<TMock>(this IEnumerable<Mock<TMock>> mocks, Action<TMock> expression, Func<Times> times) where TMock : class
    {
        foreach (var m in mocks)
            m.VerifySet(expression, times);
    }

    public static void Verify<TMock>(this IEnumerable<Mock<TMock>> mocks, Expression<Action<TMock>> expression) where TMock : class
    {
        foreach (var m in mocks)
            m.Verify(expression);
    }

    public static void Verify<TMock>(this IEnumerable<Mock<TMock>> mocks, Expression<Action<TMock>> expression, Func<Times> times) where TMock : class
    {
        foreach (var m in mocks)
            m.Verify(expression, times);
    }
}