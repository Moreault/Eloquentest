namespace Eloquentest.Tests;

[TestClass]
public sealed class EnsureEqualityTests : Tester
{
    public sealed record Dummy
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
        public string Description { get; init; } = null!;
    }

    [TestMethod]
    public void Always_EnsureEqualityBetweenEquivalentObjects()
    {
        //Arrange
        var dummy1 = Fixture.Create<Dummy>();
        var dummy2 = dummy1 with { };

        //Act
        //Assert
        Ensure.Equality(dummy1, dummy2);
    }

    [TestMethod]
    public void Always_EnsureInequalityBetweenDifferentObjects()
    {
        //Arrange
        var dummy1 = Fixture.Create<Dummy>();
        var dummy2 = Fixture.Create<Dummy>();

        //Act
        //Assert
        Ensure.Inequality(dummy1, dummy2);
    }
}