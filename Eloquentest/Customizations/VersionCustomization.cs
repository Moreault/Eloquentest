namespace ToolBX.Eloquentest.Customizations;

[AutoCustomization]
public class VersionCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Version>(x => x.FromFactory(() => new Version(fixture.Create<int>(), fixture.Create<int>(), fixture.Create<int>(), fixture.Create<int>())));
    }
}