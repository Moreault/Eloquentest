namespace Eloquentest.Tests;

[TestClass]
public sealed class EnsureHasConsistentHashCodeTest : RecordTester<EnsureHasConsistentHashCodeTest.Dummy>
{
    public sealed record Dummy
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
        public string Description { get; init; } = null!;
    }

    [TestMethod]
    public void GetHashCode_WhenValuesAreEqual_ReturnSameHashCode() => Ensure.ConsistentHashCode<Dummy>();
}