namespace ToolBX.Eloquentest;

public static class ObjectGeneratorProvider
{
    public static ObjectGenerator Create()
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