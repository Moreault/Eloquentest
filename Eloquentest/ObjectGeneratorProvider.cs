namespace ToolBX.Eloquentest;

public static class ObjectGeneratorProvider
{
    public static IObjectGenerator Create()
    {
        if (IsAssemblyLoaded("Eloquentest.AutoFixture"))
        {
            return CreateInstance("Eloquentest.AutoFixture", "FixtureWrapper");
        }
        if (IsAssemblyLoaded("Eloquentest.Dummies"))
        {
            return CreateInstance("Eloquentest.Dummies", "DummyWrapper");
        }
        throw new InvalidOperationException("No supported IObjectGenerator implementation found.");
    }

    private static IObjectGenerator CreateInstance(string assemblyName, string typeName)
    {
        var assembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name.Equals(assemblyName, StringComparison.OrdinalIgnoreCase));

        if (assembly == null)
        {
            throw new InvalidOperationException($"Assembly {assemblyName} not found.");
        }

        var type = assembly.GetType($"ToolBX.{assemblyName}.{typeName}");
        if (type == null)
        {
            throw new InvalidOperationException($"Type {typeName} not found in assembly {assemblyName}.");
        }

        return (IObjectGenerator)Activator.CreateInstance(type);
    }

    private static bool IsAssemblyLoaded(string assemblyName)
    {
        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        return loadedAssemblies.Any(assembly =>
            assembly.GetName().Name.Equals(assemblyName, StringComparison.OrdinalIgnoreCase));
    }
}