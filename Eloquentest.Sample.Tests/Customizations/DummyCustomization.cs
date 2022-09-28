using AutoFixture;
using ToolBX.Eloquentest.Customizations;

namespace Eloquentest.Sample.Tests.Customizations;

[AutoCustomization]
public class DummyCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<IDummy>(x => x.FromFactory(fixture.Create<Dummy>));
    }
}