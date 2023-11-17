namespace Eloquentest.Sample.Tests;

public abstract class SerializationTester<T> : Tester
{
    [TestMethod]
    public void WhenIsReferenceDummy_ShouldBeJsonSerializable() => Cases.IsJsonSerializable<T>(Fixture);
}

[TestClass]
public sealed class ReferenceSerializationTest : SerializationTester<ReferenceSerializationTest.Dummy>
{
    public sealed class Dummy
    {
        public int Id { get; init; }
        public string Name { get; init; }
    }
}

[TestClass]
public sealed class ValueSerializationTest : SerializationTester<ValueSerializationTest.Dummy>
{
    protected override void InitializeTest()
    {
        base.InitializeTest();
        Fixture.Customize<Dummy>(x => x.FromFactory(() => new Dummy
        {
            Id = Fixture.Create<int>(),
            Name = Fixture.Create<string>()
        }).OmitAutoProperties());
    }

    public readonly record struct Dummy
    {
        public int Id { get; init; }
        public string Name { get; init; }
    }
}

[TestClass]
public sealed class GenericValueSerializationTest : SerializationTester<GenericValueSerializationTest.Dummy<int>>
{
    protected override void InitializeTest()
    {
        base.InitializeTest();
        Fixture.Customize<Dummy<int>>(x => x.FromFactory(() => new Dummy<int>
        {
            Id = Fixture.Create<int>(),
            Name = Fixture.Create<string>()
        }).OmitAutoProperties());
    }

    public readonly record struct Dummy<T>
    {
        public T Id { get; init; }
        public string Name { get; init; }
    }
}