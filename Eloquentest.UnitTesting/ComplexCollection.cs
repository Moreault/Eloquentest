using System.Collections;
using System.Numerics;
using ToolBX.OPEX;

namespace Eloquentest.UnitTesting;

public readonly struct Vector2<T>(T x, T y) where T : INumber<T>
{
    public T X { get; } = x;
    public T Y { get; } = y;

    public bool Equals(Vector2<T> other) => X.Equals(other.X) && Y.Equals(other.Y);

    public override bool Equals(object? obj) => obj is Vector2<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public static bool operator ==(Vector2<T>? a, Vector2<T>? b) => a is null && b is null || a is not null && a.Equals(b);

    public static bool operator !=(Vector2<T>? a, Vector2<T>? b) => !(a == b);
}

public readonly struct Cell<T>(Vector2<int> index, T value)
{
    public Vector2<int> Index { get; } = index;
    public T Value { get; } = value;
}

public sealed class ComplexCollection<T>(IEnumerable<Cell<T>> items) : IEnumerable<Cell<T>>
{
    private readonly List<Cell<T>> _items = items.ToList();

    public T? this[int columnIndex, int rowIndex]
    {
        get => this[new Vector2<int>(columnIndex, rowIndex)];
        set => this[new Vector2<int>(columnIndex, rowIndex)] = value;
    }

    public T? this[Vector2<int> index]
    {
        get => _items.SingleOrDefault(x => x.Index == index).Value;
        set
        {
            var result = _items.IndexesOf(x => x.Index == index).TryGetSingle();

            if (result.IsSuccess)
                _items[result.Value] = new Cell<T>(index, value!);
            else
                _items.Add(new Cell<T>(index, value!));
        }
    }

    public int FetchCount { get; set; }

    public int Count => _items.Count;

    public ComplexCollection() : this(Array.Empty<Cell<T>>())
    {

    }

    public ComplexCollection(params Cell<T>[] items) : this(items as IEnumerable<Cell<T>>)
    {

    }

    public IEnumerator<Cell<T>> GetEnumerator() => _items.GetEnumerator();

    public bool Equals(ComplexCollection<T>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return FetchCount == other.FetchCount && _items.SequenceEqual(other._items);
    }

    public override bool Equals(object? obj) => Equals(obj as ComplexCollection<T>);

    public override int GetHashCode()
    {
        return HashCode.Combine(_items, FetchCount);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static bool operator ==(ComplexCollection<T>? a, ComplexCollection<T>? b) => a is null && b is null || a is not null && a.Equals(b);

    public static bool operator !=(ComplexCollection<T>? a, ComplexCollection<T>? b) => !(a == b);

    public static bool operator ==(ComplexCollection<T>? a, IEnumerable<Cell<T>>? b) => a is null && b is null || a is not null && a.Equals(b);

    public static bool operator !=(ComplexCollection<T>? a, IEnumerable<Cell<T>>? b) => !(a == b);
}