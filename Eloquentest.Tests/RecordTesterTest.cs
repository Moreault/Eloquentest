namespace Eloquentest.Tests;

[TestClass]
public sealed class RecordTesterTest : RecordTester<RecordTesterTest.Dummy>
{
    public sealed record Dummy
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
    }

    [TestMethod]
    public void Always_IdGetsAndSets() => Ensure.HasBasicGetSetFunctionality<Dummy>(Fixture);
}