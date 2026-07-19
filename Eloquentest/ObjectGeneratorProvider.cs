namespace ToolBX.Eloquentest;

public static class ObjectGeneratorProvider
{
    private static readonly Lazy<ObjectGenerator> CachedGenerator = new(ResolveGenerator);

    public static ObjectGenerator Create() => CachedGenerator.Value;

    private static ObjectGenerator ResolveGenerator()
    {
        if (ProviderUtils.IsAssemblyLoaded("Eloquentest.AutoFixture"))
        {
            return ProviderUtils.CreateInstance<ObjectGenerator>("Eloquentest.AutoFixture", "FixtureWrapper");
        }
        if (ProviderUtils.IsAssemblyLoaded("Eloquentest.Dummies"))
        {
            return ProviderUtils.CreateInstance<ObjectGenerator>("Eloquentest.Dummies", "DummyWrapper");
        }
        throw new InvalidOperationException("No supported IObjectGenerator implementation found.");
    }
}