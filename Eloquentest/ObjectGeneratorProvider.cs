namespace ToolBX.Eloquentest;

public static class ObjectGeneratorProvider
{
    public static IObjectGenerator Create()
    {
        if (ProviderUtils.IsAssemblyLoaded("Eloquentest.AutoFixture"))
        {
            return ProviderUtils.CreateInstance<IObjectGenerator>("Eloquentest.AutoFixture", "FixtureWrapper");
        }
        if (ProviderUtils.IsAssemblyLoaded("Eloquentest.Dummies"))
        {
            return ProviderUtils.CreateInstance<IObjectGenerator>("Eloquentest.Dummies", "DummyWrapper");
        }
        throw new InvalidOperationException("No supported IObjectGenerator implementation found.");
    }
}