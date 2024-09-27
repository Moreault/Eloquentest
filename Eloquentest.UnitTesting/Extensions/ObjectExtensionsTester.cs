using System.Collections;
using ToolBX.Dummies;

namespace Eloquentest.UnitTesting.Extensions;

[TestClass]
public abstract class ObjectExtensionsTester
{
    private Dummy _dummy = new();

    public record SimpleGarbage
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
        var instance = _dummy.Create<SimpleGarbage>();

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
        var instance = new DummyCollection<SimpleGarbage>(_dummy.CreateMany<SimpleGarbage>().ToArray());

        //Act
        var clone = instance.Clone();

        //Assert
        instance.Should().BeEquivalentTo(clone);
    }

    [TestMethod]
    public void Clone_WhenDealingWithComplexCollection_UseReflectionToClone()
    {
        //Arrange
        var instance = new ComplexCollection<SimpleGarbage>(_dummy.CreateMany<Cell<SimpleGarbage>>().ToArray())
        {
            FetchCount = _dummy.Create<int>()
        };

        //Act
        var clone = instance.Clone();

        //Assert
        instance.Should().BeEquivalentTo(clone);
    }

}