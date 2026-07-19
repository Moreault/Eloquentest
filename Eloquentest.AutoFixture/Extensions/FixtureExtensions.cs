namespace ToolBX.Eloquentest.AutoFixture.Extensions;

public static class FixtureExtensions
{
    private const int DefaultDirectoryCount = 3;

    public static byte[] CreateFile(this IFixture fixture)
    {
        return fixture.CreateMany<byte>().ToArray();
    }

    public static IEnumerable<byte[]> CreateManyFiles(this IFixture fixture, int count = int.MinValue)
    {
        count = count <= 0 ? fixture.RepeatCount : count;
        var files = new List<byte[]>();
        for (var i = 0; i < count; i++)
            files.Add(fixture.CreateFile());
        return files;
    }
    public static string CreateSystemPath(this IFixture fixture, int directoryCount = DefaultDirectoryCount)
    {
        return fixture.CreateManySystemPaths(directoryCount).First();
    }

    //TODO Generate only valid path characters
    public static IEnumerable<string> CreateManySystemPaths(this IFixture fixture, int directoryCount = DefaultDirectoryCount, int count = int.MinValue)
    {
        count = count <= 0 ? fixture.RepeatCount : count;
        var paths = new List<string>();
        for (var i = 0; i < count; i++)
        {
            var path = string.Join(Path.AltDirectorySeparatorChar.ToString(), fixture.CreateMany<string>(directoryCount));
            paths.Add(path);
        }
        return paths;
    }

    public static string CreateFilePath(this IFixture fixture, int directoryCount = DefaultDirectoryCount)
    {
        return fixture.CreateManyFilePaths(directoryCount).First();
    }

    //TODO Generate only valid path characters
    public static IEnumerable<string> CreateManyFilePaths(this IFixture fixture, int directoryCount = DefaultDirectoryCount, int count = int.MinValue)
    {
        count = count <= 0 ? fixture.RepeatCount : count;
        var paths = new List<string>();
        for (var i = 0; i < count; i++)
        {
            var path = $"{string.Join(Path.AltDirectorySeparatorChar.ToString(), fixture.CreateMany<string>(directoryCount))}{Path.AltDirectorySeparatorChar}{fixture.Create<string>()}.{fixture.CreateFileExtension()}";
            paths.Add(path);
        }
        return paths;
    }

    public static string CreateFileExtension(this IFixture fixture, int length = 3)
    {
        return fixture.CreateManyFileExtensions(length).First();
    }

    //TODO Generate only valid path characters
    public static IEnumerable<string> CreateManyFileExtensions(this IFixture fixture, int length = 3, int count = int.MinValue)
    {
        count = count <= 0 ? fixture.RepeatCount : count;
        return fixture.CreateMany<string>(count).Select(x => x.Substring(0, length));
    }

    public static IDictionary<TKey, TValue> CreateDictionary<TKey, TValue>(this IFixture fixture, int count = int.MinValue) where TKey : notnull
    {
        count = count <= 0 ? fixture.RepeatCount : count;
        return fixture.CreateMany<KeyValuePair<TKey, TValue>>(count).ToDictionary(x => x.Key, x => x.Value);
    }

    public static T CreateLesserThan<T>(this IFixture fixture, T value) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        return fixture.CreateManyLesserThan(value).First();
    }

    public static IEnumerable<T> CreateManyLesserThan<T>(this IFixture fixture, T value, int count = int.MinValue) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        count = count <= 0 ? fixture.RepeatCount : count;
        return fixture.Create<Generator<T>>().Where(x => x.CompareTo(value) < 0).Distinct().Take(count);
    }

    public static T CreateLesserThanOrEqualTo<T>(this IFixture fixture, T value) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        return fixture.CreateManyLesserThanOrEqualTo(value).First();
    }

    public static IEnumerable<T> CreateManyLesserThanOrEqualTo<T>(this IFixture fixture, T value, int count = int.MinValue) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        count = count <= 0 ? fixture.RepeatCount : count;
        return fixture.Create<Generator<T>>().Where(x => x.CompareTo(value) <= 0).Distinct().Take(count);
    }

    public static T CreateGreaterThan<T>(this IFixture fixture, T value) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        return fixture.CreateManyGreaterThan(value).First();
    }

    public static IEnumerable<T> CreateManyGreaterThan<T>(this IFixture fixture, T value, int count = int.MinValue) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        count = count <= 0 ? fixture.RepeatCount : count;
        return fixture.Create<Generator<T>>().Where(x => x.CompareTo(value) > 0).Distinct().Take(count);
    }

    public static T CreateGreaterThanOrEqualTo<T>(this IFixture fixture, T value) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        return fixture.CreateManyGreaterThanOrEqualTo(value).First();
    }

    public static IEnumerable<T> CreateManyGreaterThanOrEqualTo<T>(this IFixture fixture, T value, int count = int.MinValue) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        count = count <= 0 ? fixture.RepeatCount : count;
        return fixture.Create<Generator<T>>().Where(x => x.CompareTo(value) >= 0).Distinct().Take(count);
    }

    public static T CreateBetween<T>(this IFixture fixture, T minValue, T maxValue) where T : INumber<T>
    {
        return fixture.CreateManyBetween(minValue, maxValue).First();
    }

    public static IEnumerable<T> CreateManyBetween<T>(this IFixture fixture, T minValue, T maxValue, int count = int.MinValue) where T : INumber<T>
    {
        count = count <= 0 ? fixture.RepeatCount : count;
        var output = new List<T>();
        for (var i = 0; i < count; i++)
            output.Add(PseudoRandom.Next(minValue, maxValue));
        return output;
    }

    public static IEnumerable<int> CreateManyNonDivisibleBy(this IFixture fixture, int value, int count = int.MinValue)
    {
        count = count <= 0 ? fixture.RepeatCount : count;
        var numbers = new List<int>();
        for (var i = 0; i < count; i++)
            numbers.Add(fixture.CreateNonDivisibleBy(value));
        return numbers;
    }

    public static int CreateNonDivisibleBy(this IFixture fixture, int value)
    {
        if (value is 1 or -1) throw new ArgumentException("Every integer is divisible by 1.", nameof(value));
        var number = fixture.Create<int>();
        return number % value == 0 ? number + 1 : number;
    }

    public static int CreateDivisbleBy(this IFixture fixture, int value) => fixture.Create<int>() * value;

    public static IEnumerable<int> CreateManyDivisbleBy(this IFixture fixture, int value, int count = int.MinValue)
    {
        count = count <= 0 ? fixture.RepeatCount : count;
        var numbers = new List<int>();
        for (var i = 0; i < count; i++)
            numbers.Add(fixture.CreateDivisbleBy(value));
        return numbers;
    }

    /// <summary>
    /// Returns a floating-point value between 0 and 1.
    /// </summary>
    public static float CreateNormalizedFloat(this IFixture fixture) => fixture.CreateBetween(0f, 100f) / 100.0f;

    public static object Create(this IFixture fixture, Type type)
    {
        if (fixture is null) throw new ArgumentNullException(nameof(fixture));
        if (type is null) throw new ArgumentNullException(nameof(type));

        var method = typeof(SpecimenFactory).GetSingleMethod(x => x.Name == nameof(Fixture.Create) && x.IsPublic && x.ContainsGenericParameters && x.HasParameters<ISpecimenBuilder>())!;
        var generic = method.MakeGenericMethod(type);
        return generic.Invoke(null, new object?[] { fixture })!;
    }
}
