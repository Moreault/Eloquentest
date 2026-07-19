using System;
using System.Collections.Generic;
using System.Text;

namespace Eloquentest.AutoFixture.Tests;

public abstract class TestBase
{
    protected Fixture Fixture { get; }

    public TestBase()
    {
        Fixture = new Fixture();
        foreach (var customization in FixtureProvider.AutoCustomizations)
        {
            if (customization is ICustomization fixtureCustomization)
                Fixture.Customize(fixtureCustomization);
            else if (customization is ISpecimenBuilder specimenBuilder)
                Fixture.Customizations.Add(specimenBuilder);
            else throw new NotSupportedException($"{nameof(AutoCustomizationAttribute)} does not support type '{customization.GetType()}'");
        }
    }
}