using ToolBX.Reflection4Humans.ValueEquality;

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

    public sealed record Splitted<T>
    {
        public IReadOnlyList<T> Remaining { get; init; } = Array.Empty<T>();

        public IReadOnlyList<T> Excluded { get; init; } = Array.Empty<T>();

        public bool Equals(Splitted<T>? other) => other != null && Remaining.SequenceEqual(other.Remaining) && Excluded.SequenceEqual(other.Excluded);

        public override int GetHashCode() => this.GetValueHashCode();
    }

    [TestMethod]
    public void WhenValuesAreEqual_ReturnSameHashCode() => Ensure.ConsistentHashCode<Dummy>();

    [TestMethod]
    public void WhenHasReadOnlyLists_ReturnSameHashCode() => Ensure.ConsistentHashCode<Splitted<Dummy>>();
}