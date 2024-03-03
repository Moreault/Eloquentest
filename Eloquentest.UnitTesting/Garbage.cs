using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;
using ToolBX.Reflection4Humans.ValueEquality;

namespace Eloquentest.UnitTesting;

public static class Garbage
{
    public sealed class ValueEqual : IEquatable<ValueEqual>
    {
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }

        public int Id { get; init; }
        public string Name { get; init; } = null!;

        public bool Equals(ValueEqual? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Name == other.Name;
        }

        public override bool Equals(object? obj) => Equals(obj as ValueEqual);

        public static bool operator ==(ValueEqual? a, ValueEqual? b) => a is null && b is null || a is not null && a.Equals(b);

        public static bool operator !=(ValueEqual? a, ValueEqual? b) => !(a == b);
    }

    public sealed record RecordEquality
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
        public IFormatProvider FormatProvider { get; init; } = null!;
    }

    public sealed class NoValueEquality
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
        public IFormatProvider FormatProvider { get; init; } = null!;
    }

    public sealed class NullEqualityFail
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj is null) return true;
            if (ReferenceEquals(this, obj)) return true;
            return obj is NullEqualityFail other && Id == other.Id && Name == other.Name;
        }

        public override int GetHashCode() => HashCode.Combine(Id, Name);
    }

    public sealed class ReferenceEqualityFail
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return false;
            return obj is ReferenceEqualityFail other && Id == other.Id && Name == other.Name;
        }

        public override int GetHashCode() => HashCode.Combine(Id, Name);
    }

    public sealed class ValueEqualityFail
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is ValueEqualityFail other && Id != other.Id && Name != other.Name;
        }

        public override int GetHashCode() => HashCode.Combine(Id, Name);
    }

    public sealed class BadValueEquality
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return true;
        }

        public override int GetHashCode() => HashCode.Combine(Id, Name);
    }

    public sealed class OpEqualityNullFail1
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is OpEqualityNullFail1 other && Id == other.Id && Name == other.Name;
        }

        public override int GetHashCode() => HashCode.Combine(Id, Name);

        public static bool operator ==(OpEqualityNullFail1? a, OpEqualityNullFail1? b)
        {
            if (a is null && b is null) return false;
            throw new NotImplementedException();
        }

        public static bool operator !=(OpEqualityNullFail1? a, OpEqualityNullFail1? b) => !(a == b);
    }

    public sealed class OpEqualityNullFail2
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is OpEqualityNullFail2 other && Id == other.Id && Name == other.Name;
        }

        public override int GetHashCode() => HashCode.Combine(Id, Name);

        public static bool operator ==(OpEqualityNullFail2? a, OpEqualityNullFail2? b)
        {
            if (a is null && b is null) return true;
            if (a is null && b is not null) return true;
            throw new NotImplementedException();
        }

        public static bool operator !=(OpEqualityNullFail2? a, OpEqualityNullFail2? b) => !(a == b);
    }

    public sealed class OpEqualityNullFail3
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is OpEqualityNullFail3 other && Id == other.Id && Name == other.Name;
        }

        public override int GetHashCode() => HashCode.Combine(Id, Name);

        public static bool operator ==(OpEqualityNullFail3? a, OpEqualityNullFail3? b)
        {
            if (a is null && b is null) return true;
            if (a is null && b is not null) return false;
            if (a is not null && b is null) return true;
            throw new NotImplementedException();
        }

        public static bool operator !=(OpEqualityNullFail3? a, OpEqualityNullFail3? b) => !(a == b);
    }

    public sealed class OpEqualitySameReferenceFail
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is OpEqualitySameReferenceFail other && Id == other.Id && Name == other.Name;
        }

        public override int GetHashCode() => HashCode.Combine(Id, Name);

        public static bool operator ==(OpEqualitySameReferenceFail? a, OpEqualitySameReferenceFail? b)
        {
            if (a is null && b is null) return true;
            if (a is null && b is not null) return false;
            if (a is not null && b is null) return false;
            if (ReferenceEquals(a, b)) return false;
            throw new NotImplementedException();
        }

        public static bool operator !=(OpEqualitySameReferenceFail? a, OpEqualitySameReferenceFail? b) => !(a == b);
    }

    public sealed class OpEqualitySameValueFail
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is OpEqualitySameValueFail other && Id == other.Id && Name == other.Name;
        }

        public override int GetHashCode() => HashCode.Combine(Id, Name);

        public static bool operator ==(OpEqualitySameValueFail? a, OpEqualitySameValueFail? b)
        {
            if (a is null && b is null) return true;
            if (a is null && b is not null) return false;
            if (a is not null && b is null) return false;
            if (ReferenceEquals(a, b)) return true;
            return a.Id != b.Id && a.Name != b.Name;
        }

        public static bool operator !=(OpEqualitySameValueFail? a, OpEqualitySameValueFail? b) => !(a == b);
    }

    public sealed class OpEqualityDifferentValuesFail
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is OpEqualityDifferentValuesFail other && Id == other.Id && Name == other.Name;
        }

        public override int GetHashCode() => HashCode.Combine(Id, Name);

        public static bool operator ==(OpEqualityDifferentValuesFail? a, OpEqualityDifferentValuesFail? b)
        {
            if (a is null && b is null) return true;
            if (a is null && b is not null) return false;
            if (a is not null && b is null) return false;
            if (ReferenceEquals(a, b)) return true;
            return true;
        }

        public static bool operator !=(OpEqualityDifferentValuesFail? a, OpEqualityDifferentValuesFail? b) => !(a == b);
    }

    public sealed class OpInequalityNullFail
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is OpInequalityNullFail other && Id == other.Id && Name == other.Name;
        }

        public override int GetHashCode() => HashCode.Combine(Id, Name);

        public static bool operator ==(OpInequalityNullFail? a, OpInequalityNullFail? b)
        {
            if (a is null && b is null) return true;
            if (a is null && b is not null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(OpInequalityNullFail? a, OpInequalityNullFail? b)
        {
            if (a is null && b is null) return true;
            throw new NotImplementedException();
        }
    }

    public sealed class OpInequalityLeftNullFail
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is OpInequalityLeftNullFail other && Id == other.Id && Name == other.Name;
        }

        public override int GetHashCode() => HashCode.Combine(Id, Name);

        public static bool operator ==(OpInequalityLeftNullFail? a, OpInequalityLeftNullFail? b)
        {
            if (a is null && b is null) return true;
            if (a is null && b is not null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(OpInequalityLeftNullFail? a, OpInequalityLeftNullFail? b)
        {
            if (a is null && b is null) return false;
            if (a is null && b is not null) return false;

            throw new NotImplementedException();
        }
    }

    public sealed class OpInequalityRightNullFail
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is OpInequalityRightNullFail other && Id == other.Id && Name == other.Name;
        }

        public override int GetHashCode() => HashCode.Combine(Id, Name);

        public static bool operator ==(OpInequalityRightNullFail? a, OpInequalityRightNullFail? b)
        {
            if (a is null && b is null) return true;
            if (a is null && b is not null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(OpInequalityRightNullFail? a, OpInequalityRightNullFail? b)
        {
            if (a is null && b is null) return false;
            if (a is null && b is not null) return true;
            if (b is null && a is not null) return false;

            throw new NotImplementedException();
        }
    }

    public sealed class OpInequalityValueFail
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is OpInequalityValueFail other && Id == other.Id && Name == other.Name;
        }

        public override int GetHashCode() => HashCode.Combine(Id, Name);

        public static bool operator ==(OpInequalityValueFail? a, OpInequalityValueFail? b)
        {
            if (a is null && b is null) return true;
            if (a is null && b is not null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(OpInequalityValueFail? a, OpInequalityValueFail? b)
        {
            if (a is null && b is null) return false;
            if (a is null && b is not null) return true;
            if (b is null && a is not null) return true;
            return a.Id == b.Id && a.Name == b.Name;
        }
    }

    public sealed class OpInequalityAlwaysEqual
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is OpInequalityAlwaysEqual other && Id == other.Id && Name == other.Name;
        }

        public override int GetHashCode() => HashCode.Combine(Id, Name);

        public static bool operator ==(OpInequalityAlwaysEqual? a, OpInequalityAlwaysEqual? b)
        {
            if (a is null && b is null) return true;
            if (a is null && b is not null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(OpInequalityAlwaysEqual? a, OpInequalityAlwaysEqual? b)
        {
            if (a is null && b is null) return false;
            if (a is null && b is not null) return true;
            if (b is null && a is not null) return true;
            return false;
        }
    }

    public sealed class ConstantHashCode
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;

        public override int GetHashCode() => 14;
    }

    public sealed record ProperHashCode
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
    }

    public sealed class RandomHashCode
    {
        private static readonly List<int> HashCodes = new();

        public int Id { get; init; }
        public string Name { get; init; } = null!;

        private int? _hashCode;

        public override int GetHashCode()
        {
            if (_hashCode.HasValue)
                return _hashCode.Value;

            int rando;

            do
            {
                rando = Random.Shared.Next(int.MinValue, int.MaxValue);
            } while (HashCodes.Contains(rando));

            _hashCode = rando;
            HashCodes.Add(rando);
            return rando;
        }
    }

    public sealed class SameReferenceBadHashCode
    {
        private static readonly List<Tuple<SameReferenceBadHashCode, int>> HashCodes = new();

        public int Id { get; init; }
        public string Name { get; init; } = null!;

        public override int GetHashCode()
        {
            if (HashCodes.Any(x => ReferenceEquals(x.Item1, this)))
                return Random.Shared.Next(int.MinValue, int.MaxValue);
            if (HashCodes.Any(x => x.Item1.Id == Id))
                return HashCodes.Single(x => x.Item1.Id == Id).Item2;

            var hashy = HashCode.Combine(Id, Name);
            HashCodes.Add(new Tuple<SameReferenceBadHashCode, int>(this, hashy));
            return hashy;
        }
    }

    [JsonConverter(typeof(UnserializableJsonConverter))]
    public sealed class Unserializable
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
    }

    public sealed class UnserializableJsonConverter : JsonConverter<Unserializable>
    {
        public override Unserializable? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new JsonException("Cannot be deserialized!");
        }

        public override void Write(Utf8JsonWriter writer, Unserializable value, JsonSerializerOptions options)
        {
            throw new JsonException("Cannot be serialized!");
        }
    }

    [JsonConverter(typeof(BadSerializationJsonConverter))]
    public sealed class BadSerialization
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
    }

    public sealed class BadSerializationJsonConverter : JsonConverter<BadSerialization>
    {
        public override BadSerialization? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            while (reader.Read())
            {

            }

            return new BadSerialization
            {
                Id = 14,
                Name = "Seb"
            };
        }

        public override void Write(Utf8JsonWriter writer, BadSerialization value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("Id", value.Id);
            writer.WriteString("Name", value.Name);
            writer.WriteEndObject();
        }
    }

    public sealed record Splitted<T>
    {
        public IReadOnlyList<T> Remaining { get; init; } = Array.Empty<T>();

        public IReadOnlyList<T> Excluded { get; init; } = Array.Empty<T>();

        public bool Equals(Splitted<T>? other) => other != null && Remaining.SequenceEqual(other.Remaining) && Excluded.SequenceEqual(other.Excluded);

        public override int GetHashCode() => this.GetValueHashCode();
    }

    public sealed record ProperGetSetter
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
    }

    public sealed record BadGetSetter
    {
        public int Id { get; init; }

        public string Name
        {
            get => Id.ToString();
            init { }
        }
    }

    public sealed class ProperEnumerable<T> : IEnumerable<T>
    {
        private readonly List<T> _items = new();

        public ProperEnumerable() { }

        public ProperEnumerable(IEnumerable<T> items) => _items.AddRange(items);

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public sealed class BadEnumerable<T> : IEnumerable<T>
    {
        private readonly List<T> _items = new();

        public BadEnumerable() { }

        public BadEnumerable(IEnumerable<T> items) => _items.AddRange(items);

        public IEnumerator<T> GetEnumerator() => new Enumerator(_items);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<T>
        {
            private readonly List<T> _items;
            private int _index;

            public Enumerator(List<T> items)
            {
                _items = items;
                _index = -1;
            }

            public T Current => _items[_index];

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                // Skip every other item
                _index += 2; 
                return _index < _items.Count;
            }

            public void Reset()
            {
                _index = -1;
            }

            public void Dispose()
            {

            }
        }
    }

}