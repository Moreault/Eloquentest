namespace ToolBX.Eloquentest.Customizations;

[AutoCustomization(Order = AutoCustomizationAttribute.CustomizationOrder.Early)]
public class BaseCustomizations : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Version>(x => x.FromFactory(() => new Version(fixture.Create<int>(), fixture.Create<int>(), fixture.Create<int>(), fixture.Create<int>())));
        fixture.Customize<IFormatProvider>(x => x.FromFactory(() => CultureInfo.InvariantCulture));
    }
}