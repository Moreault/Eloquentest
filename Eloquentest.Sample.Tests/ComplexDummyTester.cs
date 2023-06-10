namespace Eloquentest.Sample.Tests;

[TestClass]
public class ComplexDummyTester
{
    [TestClass]
    public class AutomaticCreation : Tester<ComplexDummy>
    {
        [TestMethod]
        public void Always_AutomaticallyGenerateProperties()
        {
            //Arrange

            //Act
            var result = Instance;

            //Assert
            result.Child.Should().NotBeNull();
            result.Child.Children.Should().NotBeEmpty();
            result.Child.Interfaces.Should().NotBeEmpty();
        }
    }
}