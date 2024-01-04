namespace ToolBX.Eloquentest;

public static class FixtureProvider
{
    public static IReadOnlyList<object> AutoCustomizations => LazyAutoCustomizations.Value;

    private static readonly Lazy<IReadOnlyList<object>> LazyAutoCustomizations = new(() => Types.Where(x => x.HasAttribute<AutoCustomizationAttribute>())
           .OrderBy(x => x.GetCustomAttribute<AutoCustomizationAttribute>()!.Order)
           .Select(Activator.CreateInstance)
           .ToList()!);

    public static IFixture Create()
    {
        var fixture = new Fixture();
        foreach (var autoCustomization in AutoCustomizations)
        {
            if (autoCustomization is ICustomization customization)
                fixture.Customize(customization);
            else if (autoCustomization is ISpecimenBuilder specimenBuilder)
                fixture.Customizations.Add(specimenBuilder);
            else throw new NotSupportedException($"{nameof(AutoCustomizationAttribute)} does not support type '{autoCustomization.GetType()}'");
        }
        return fixture;
    }
}