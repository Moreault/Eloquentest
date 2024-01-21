using System.Collections;

namespace Eloquentest.UnitTesting.Extensions;

[TestClass]
public abstract class ObjectExtensionsTester : Tester
{
    public record SimpleDummy
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
    }

    [TestMethod]
    public void Clone_WhenValueIsNull_Throw()
    {
        //Arrange
        object source = null!;

        //Act
        var action = () => source.Clone();

        //Assert
        action.Should().Throw<ArgumentNullException>().WithParameterName(nameof(source));
    }

    [TestMethod]
    public void Clone_WhenDealingWithSimpleType_UseJsonSerializationToClone()
    {
        //Arrange
        var instance = Fixture.Create<SimpleDummy>();

        //Act
        var clone = instance.Clone();

        //Assert
        instance.Should().BeEquivalentTo(clone);
    }

    //Cannot be serialized by System.Text because it doesn't have a public set accessor on its indexer
    public sealed class DummyCollection<T>(params T[] items) : IEnumerable<T>
    {
        private readonly List<T> _list = items.ToList();

        public T this[int index] => _list[index];

        public DummyCollection() : this(Array.Empty<T>())
        {

        }

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [TestMethod]
    public void Clone_WhenDealingWithUnserializableCollection_UseReflectionToClone()
    {
        //Arrange
        var instance = new DummyCollection<SimpleDummy>(Fixture.CreateMany<SimpleDummy>().ToArray());

        //Act
        var clone = instance.Clone();

        //Assert
        instance.Should().BeEquivalentTo(clone);
    }

    [TestMethod]
    public void Clone_WhenDealingWithComplexCollection_UseReflectionToClone()
    {
        //Arrange
        var instance = new ComplexCollection<SimpleDummy>(Fixture.CreateMany<Cell<SimpleDummy>>().ToArray())
        {
            FetchCount = Fixture.Create<int>()
        };

        //Act
        var clone = instance.Clone();

        //Assert
        instance.Should().BeEquivalentTo(clone);
    }

}