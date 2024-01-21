using System.Collections;
using ToolBX.Reflection4Humans.ValueEquality;

namespace Eloquentest.UnitTesting;

public abstract class EnsureTester
{
    protected IObjectGenerator Generator { get; private set; }

    [TestInitialize]
    public void TestInitializeBase()
    {
        Generator = ObjectGeneratorProvider.Create();
    }

    public sealed class Dummy : IEquatable<Dummy>
    {
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }

        public int Id { get; init; }
        public string Name { get; init; } = null!;

        public bool Equals(Dummy? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Name == other.Name;
        }

        public override bool Equals(object? obj) => Equals(obj as Dummy);

        public static bool operator ==(Dummy? a, Dummy? b) => a is null && b is null || a is not null && a.Equals(b);

        public static bool operator !=(Dummy? a, Dummy? b) => !(a == b);
    }

    public sealed record DummyRecord
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
        public IFormatProvider FormatProvider { get; init; } = null!;
    }

    [TestMethod]
    public void ValueEquality_WhenValuesAreEqual_ReturnTrue() => Ensure.ValueEquality<Dummy>(Generator);

    [TestMethod]
    public void ValueEquality_WhenObjectsAreRecords_ReturnTrue() => Ensure.ValueEquality<DummyRecord>(Generator);

    [TestMethod]
    public void ValueEquality_WhenObjectIsComplexCollection_ReturnTrue() => Ensure.ValueEquality<ComplexCollection<DummyRecord>>(Generator);

    public sealed class DummyEnumerable<T> : IEnumerable<T>
    {
        private readonly List<T> _items = new();

        public DummyEnumerable() { }

        public DummyEnumerable(IEnumerable<T> items) => _items.AddRange(items);

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [TestMethod]
    public void Ensure_EnumeratesAllItems() => Ensure.EnumeratesAllItems<DummyEnumerable<int>, int>(Generator);

    [TestMethod]
    public void Always_EnsureIdHasBasicGetSetFunctionality() => Ensure.HasBasicGetSetFunctionality<DummyRecord>(Generator, nameof(Dummy.Id));

    [TestMethod]
    public void Always_EnsureNameHasBasicGetSetFunctionality() => Ensure.HasBasicGetSetFunctionality<DummyRecord>(Generator, nameof(Dummy.Name));

    [TestMethod]
    public void Always_EnsureEqualityBetweenEquivalentObjects()
    {
        //Arrange
        var dummy1 = Generator.Create<Dummy>();
        var dummy2 = dummy1.Clone();

        //Act
        //Assert
        Ensure.Equality(dummy1, dummy2);
    }

    [TestMethod]
    public void Always_EnsureInequalityBetweenDifferentObjects()
    {
        //Arrange
        var dummy1 = Generator.Create<Dummy>();
        var dummy2 = Generator.Create<Dummy>();

        //Act
        //Assert
        Ensure.Inequality(dummy1, dummy2);
    }

    [TestMethod]
    public void WhenIsReferenceDummy_ShouldBeJsonSerializable() => Ensure.IsJsonSerializable<Dummy>(Generator);

    public sealed record Splitted<T>
    {
        public IReadOnlyList<T> Remaining { get; init; } = Array.Empty<T>();

        public IReadOnlyList<T> Excluded { get; init; } = Array.Empty<T>();

        public bool Equals(Splitted<T>? other) => other != null && Remaining.SequenceEqual(other.Remaining) && Excluded.SequenceEqual(other.Excluded);

        public override int GetHashCode() => this.GetValueHashCode();
    }

    [TestMethod]
    public void WhenValuesAreEqual_ReturnSameHashCode() => Ensure.ValueHashCode<Dummy>();

    [TestMethod]
    public void WhenHasReadOnlyLists_ReturnSameHashCode() => Ensure.ValueHashCode<Splitted<Dummy>>();
}