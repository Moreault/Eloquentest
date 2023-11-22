﻿namespace Eloquentest.Sample.Tests;

[TestClass]
public class ValueEqualityMethodTest : Tester
{
    public class Dummy : IEquatable<Dummy>
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
    public void Equals_WhenValuesAreEqual_ReturnTrue() => Cases.TestValueEquality<Dummy>();

    [TestMethod]
    public void Equals_WhenObjectsAreRecords_ReturnTrue() => Cases.TestValueEquality<DummyRecord>();
}