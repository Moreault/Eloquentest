using AutoFixture.Kernel;
using System.Numerics;
using ToolBX.Eloquentest.Customizations;

namespace Eloquentest.Tests.Dummies;

[AutoCustomization]
public class ComplexCollectionSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ComplexCollection<>))
        {
            var elementType = type.GetGenericArguments()[0];
            var cellType = typeof(Cell<>).MakeGenericType(elementType);

            var listType = typeof(List<>).MakeGenericType(cellType);
            var list = context.Resolve(listType);

            return Activator.CreateInstance(type, list)!;
        }

        return new NoSpecimen();
    }
}

public readonly struct Vector2<T>(T x, T y) where T : INumber<T>
{
    public T X { get; } = x;
    public T Y { get; } = y;
}

public readonly struct Cell<T>(Vector2<int> index, T value)
{
    public Vector2<int> Index { get; } = index;
    public T Value { get; } = value;
}

public sealed class ComplexCollection<T>(IEnumerable<Cell<T>> items) : IEnumerable<Cell<T>>
{
    private readonly List<Cell<T>> _list = items.ToList();

    public int FetchCount { get; set; }

    public int Count => _list.Count;

    public ComplexCollection() : this(Array.Empty<Cell<T>>())
    {

    }

    public ComplexCollection(params Cell<T>[] items) : this(items as IEnumerable<Cell<T>>)
    {

    }

    public IEnumerator<Cell<T>> GetEnumerator() => _list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Equals(ComplexCollection<T>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return FetchCount == other.FetchCount && _list.SequenceEqual(other._list);
    }

    public override bool Equals(object? obj) => Equals(obj as ComplexCollection<T>);

    public override int GetHashCode()
    {
        return HashCode.Combine(_list, FetchCount);
    }

    public static bool operator ==(ComplexCollection<T>? a, ComplexCollection<T>? b) => a is null && b is null || a is not null && a.Equals(b);

    public static bool operator !=(ComplexCollection<T>? a, ComplexCollection<T>? b) => !(a == b);

    public static bool operator ==(ComplexCollection<T>? a, IEnumerable<Cell<T>>? b) => a is null && b is null || a is not null && a.Equals(b);

    public static bool operator !=(ComplexCollection<T>? a, IEnumerable<Cell<T>>? b) => !(a == b);
}