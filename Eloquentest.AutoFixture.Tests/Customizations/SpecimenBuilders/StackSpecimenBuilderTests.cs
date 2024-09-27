using FluentAssertions;
using ToolBX.Dummies;

namespace Eloquentest.AutoFixture.Tests.Customizations.SpecimenBuilders;

[TestClass]
public sealed class StackSpecimenBuilderTests
{
    private Dummy _dummy = new();

    [TestMethod]
    public void Create_Always_HasItems()
    {
        //Arrange

        //Act
        var result = _dummy.Create<Stack<int>>();

        //Assert
        result.Should().NotBeEmpty();
    }
}