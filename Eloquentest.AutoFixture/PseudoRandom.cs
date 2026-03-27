namespace ToolBX.Eloquentest.AutoFixture;

internal static class PseudoRandom
{
    public static T Next<T>(T max) where T : INumber<T> => Next(T.Zero, max);

    public static T Next<T>(T min, T max) where T : INumber<T> => T.CreateChecked(Random.Shared.NextInt64(Convert.ToInt64(min), Convert.ToInt64(max)));

    /// <summary>
    /// Returns a random number between 0.0 and 1.0
    /// </summary>
    public static T NextFloating<T>() where T : IFloatingPoint<T> => T.CreateChecked(Random.Shared.NextDouble());

    public static T Next<T>() where T : INumber<T>, IMinMaxValue<T> => T.CreateChecked(Random.Shared.NextInt64(Convert.ToInt64(T.MinValue), Convert.ToInt64(T.MaxValue)));
}
