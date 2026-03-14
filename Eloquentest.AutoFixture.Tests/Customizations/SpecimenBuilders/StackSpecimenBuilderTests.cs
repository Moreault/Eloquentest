namespace Eloquentest.AutoFixture.Tests.Customizations.SpecimenBuilders;

[TestClass]
public sealed class StackSpecimenBuilderTests : TestBase
{
    [TestMethod]
    public void Create_Always_HasItems()
    {
        //Arrange

        //Act
        var result = Fixture.Create<Stack<int>>();

        //Assert
        result.Should().NotBeEmpty();
    }
}