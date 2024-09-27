using AutoFixture;
using FluentAssertions;
using ToolBX.Eloquentest.AutoFixture;

namespace Eloquentest.AutoFixture.Tests;

[TestClass]
public sealed class ObjectGeneratorProviderTests
{
    [TestMethod]
    public void Always_CreatesDummyWrapper()
    {
        //Arrange
        var fixture = new Fixture();

        //Act
        var result = ObjectGeneratorProvider.Create();

        //Assert
        result.Should().BeOfType<FixtureWrapper>();
    }
}