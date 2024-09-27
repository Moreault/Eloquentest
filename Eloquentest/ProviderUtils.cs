namespace ToolBX.Eloquentest;

internal static class ProviderUtils
{
    internal static T CreateInstance<T>(string assemblyName, string typeName)
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

        return (T)Activator.CreateInstance(type)!;
    }

    internal static bool IsAssemblyLoaded(string assemblyName)
    {
        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        return loadedAssemblies.Any(assembly =>
            assembly.GetName().Name.Equals(assemblyName, StringComparison.OrdinalIgnoreCase));
    }

}