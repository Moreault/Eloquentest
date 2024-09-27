namespace ToolBX.Eloquentest.AutoFixture;

public sealed class FixtureProvider
{
    public static IReadOnlyList<object> AutoCustomizations => LazyAutoCustomizations.Value;

    private static readonly Lazy<IReadOnlyList<object>> LazyAutoCustomizations = new(() => Types.Where(x => x.HasAttribute<AutoCustomizationAttribute>())
        .OrderBy(x => x.GetCustomAttribute<AutoCustomizationAttribute>()!.Order)
        .Select(Activator.CreateInstance)
        .ToList()!);


}