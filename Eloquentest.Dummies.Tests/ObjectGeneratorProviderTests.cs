namespace Eloquentest.Dummies.Tests;

[TestClass]
public sealed class ObjectGeneratorProviderTests
{
    [TestMethod]
    public void Always_CreatesDummyWrapper()
    {
        //Arrange

        //Act
        var result = ObjectGeneratorProvider.Create();

        //Assert
        result.Should().BeOfType<DummyWrapper>();
    }
}