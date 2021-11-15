namespace ToolBX.Eloquentest.Extensions;

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
            var path = string.Join(Path.AltDirectorySeparatorChar.ToString(), fixture.CreateMany<string>());
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
            var path = $"{string.Join(Path.AltDirectorySeparatorChar.ToString(), fixture.CreateMany<string>())}{Path.AltDirectorySeparatorChar}{fixture.Create<string>()}.{fixture.CreateFileExtension()}";
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

    public static IDictionary<TKey, TValue> CreateDictionary<TKey, TValue>(this IFixture fixture, int count = int.MinValue)
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

    public static T CreateBetween<T>(this IFixture fixture, T minValue, T maxValue, int count = int.MinValue) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        return fixture.CreateManyBetween(minValue, maxValue).First();
    }

    public static IEnumerable<T> CreateManyBetween<T>(this IFixture fixture, T minValue, T maxValue, int count = int.MinValue) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        count = count <= 0 ? fixture.RepeatCount : count;
        return fixture.Create<Generator<T>>().Where(x => x.CompareTo(minValue) >= 0 && x.CompareTo(maxValue) <= 0).Distinct().Take(count);
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
        var number = fixture.Create<int>();
        var isDivisibleBy = number % value == 0;
        return isDivisibleBy ? number : number + 1;
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
}