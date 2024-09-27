namespace ToolBX.Eloquentest.AutoFixture;

public sealed class FixtureWrapper : ObjectGenerator
{
    public static IReadOnlyList<object> AutoCustomizations => LazyAutoCustomizations.Value;

    private static readonly Lazy<IReadOnlyList<object>> LazyAutoCustomizations = new(() => Types.Where(x => x.HasAttribute<AutoCustomizationAttribute>())
        .OrderBy(x => x.GetCustomAttribute<AutoCustomizationAttribute>()!.Order)
        .Select(Activator.CreateInstance)
        .ToList()!);

    private readonly IFixture _unwrapped;

    public FixtureWrapper() : this(new Fixture())
    {
 
    }

    public FixtureWrapper(IFixture fixture)
    {
        _unwrapped = fixture;
        foreach (var autoCustomization in AutoCustomizations)
        {
            if (autoCustomization is ICustomization customization)
                _unwrapped.Customize(customization);
            else if (autoCustomization is ISpecimenBuilder specimenBuilder)
                _unwrapped.Customizations.Add(specimenBuilder);
            else throw new NotSupportedException($"{nameof(AutoCustomizationAttribute)} does not support type '{autoCustomization.GetType()}'");
        }
    }

    public override T Create<T>() => _unwrapped.Create<T>();

    public override object Create(Type type) => _unwrapped.Create(type);
    public override IEnumerable<T> CreateMany<T>() => _unwrapped.CreateMany<T>();

    public static implicit operator Fixture(FixtureWrapper wrapper) => (Fixture)wrapper._unwrapped;
    public static implicit operator FixtureWrapper(Fixture fixture) => new(fixture);
}