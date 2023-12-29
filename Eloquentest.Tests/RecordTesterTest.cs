namespace Eloquentest.Tests;

[TestClass]
public sealed class SealedRecordTesterTest : RecordTester<SealedRecordTesterTest.Dummy>
{
    public sealed record Dummy
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
    }
}

[TestClass]
public class NonSealedRecordTesterTest : RecordTester<NonSealedRecordTesterTest.Dummy>
{
    public record Dummy
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
    }
}