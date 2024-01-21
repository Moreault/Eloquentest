using System.Collections;
using System.Reflection.Emit;

namespace ToolBX.Eloquentest;

/// <summary>
/// Test cases to common problems.
/// </summary>
public static class Ensure
{
    /// <summary>
    /// Automatically tests your method with null or empty strings.
    /// </summary>
    public static void WhenIsNullOrEmpty(Action<string> action)
    {
        if (action is null) throw new ArgumentNullException(nameof(action));
        foreach (var datarow in new[] { "", null! })
            action.Invoke(datarow);
    }

    /// <summary>
    /// Automatically tests your method with null, empty and white space strings.
    /// </summary>
    public static void WhenIsNullOrWhiteSpace(Action<string> action)
    {
        if (action is null) throw new ArgumentNullException(nameof(action));
        foreach (var datarow in new[] { "", null!, " ", "\n", "\r", "\t" })
            action.Invoke(datarow);
    }

    /// <summary>
    /// Automatically tests all equality cases between two objects.
    /// </summary>
    public static void ValueEquality<T>() => ValueEquality<T>(ObjectGeneratorProvider.Create(), new JsonSerializerOptions());

    /// <summary>
    /// Automatically tests all equality cases between two objects.
    /// </summary>
    public static void ValueEquality<T>(IObjectGenerator fixture) => ValueEquality<T>(fixture ?? throw new ArgumentNullException(nameof(fixture)), new JsonSerializerOptions());

    /// <summary>
    /// Automatically tests all equality cases between two objects.
    /// </summary>
    public static void ValueEquality<T>(JsonSerializerOptions options) => ValueEquality<T>(ObjectGeneratorProvider.Create(), options ?? throw new ArgumentNullException(nameof(options)));

    /// <summary>
    /// Automatically tests all equality cases between two objects.
    /// </summary>
    public static void ValueEquality<T>(IObjectGenerator fixture, JsonSerializerOptions options)
    {
        if (fixture is null) throw new ArgumentNullException(nameof(fixture));
        if (options is null) throw new ArgumentNullException(nameof(options));

        IsCloneable<T>(fixture, options);

        var methods = typeof(T).GetAllMethods(x => x.Name == "Equals" && x.ReturnType == typeof(bool) && x.HasParameters(1));

        var testedMethod = string.Empty;
        var testCase = "None";
        var a = fixture.Create<T>();
        T b = default!;
        try
        {
            foreach (var method in methods)
            {
                var parameterType = method.GetParameters().First();
                if (!typeof(T).IsAssignableTo(parameterType.ParameterType))
                    continue;

                testedMethod = $"{method.Name}({method.GetParameters().Single().GetType().Name})";

                testCase = "When B is null";
                if (typeof(T).IsClass)
                {
                    a = fixture.Create<T>();
                    b = default!;

                    Assert.IsFalse((bool)method.Invoke(a, new object[] { b })!);
                }

                testCase = "When A and B are the same reference";
                a = fixture.Create<T>();
                b = a;
                Assert.IsTrue((bool)method.Invoke(a, new object[] { b! })!);

                testCase = "When A and B are equivalent objects with different references";
                a = fixture.Create<T>();
                b = a.Clone(options);

                Assert.IsFalse(ReferenceEquals(a, b));
                Assert.IsTrue((bool)method.Invoke(a, new object[] { b! })!);

                testCase = "When A and B are different objects of the same type";
                a = fixture.Create<T>();
                b = fixture.Create<T>();
                Assert.IsFalse((bool)method.Invoke(a, new object[] { b! })!);

                testCase = "Generic error";
                testedMethod = string.Empty;
                a = default!;
                b = default!;
            }

            var equalityOperators = typeof(T).GetAllMethods(x => x.Name == "op_Equality");
            foreach (var method in equalityOperators)
            {
                var parameters = method.GetParameters();
                if (parameters.Any(x => !x.ParameterType.IsAssignableFrom(typeof(T))))
                    continue;

                testedMethod = "op_Equality (==)";
                if (typeof(T).IsClass)
                {
                    testCase = "When A and B are null";
                    a = default!;
                    b = default!;
                    Assert.IsTrue((bool)method.Invoke(null, new object[] { a, b })!);

                    testCase = "When A is null but B is not";
                    a = default!;
                    b = fixture.Create<T>();
                    Assert.IsFalse((bool)method.Invoke(null, new object[] { a, b! })!);

                    testCase = "When B is null but A is not";
                    a = fixture.Create<T>();
                    b = default!;
                    Assert.IsFalse((bool)method.Invoke(null, new object[] { a!, b })!);
                }

                testCase = "When A and B are the same reference";
                a = fixture.Create<T>();
                b = a;
                Assert.IsTrue((bool)method.Invoke(null, new object[] { a!, b! })!);

                testCase = "When A and B are equivalent objects with different references";
                a = fixture.Create<T>();
                b = a.Clone(options);
                Assert.IsTrue((bool)method.Invoke(null, new object[] { a!, b! })!);

                testCase = "When A and B are different objects of the same type";
                a = fixture.Create<T>();
                b = fixture.Create<T>();
                Assert.IsFalse((bool)method.Invoke(null, new object[] { a!, b! })!);

                testCase = "Generic error";
                testedMethod = string.Empty;
                a = default!;
                b = default!;
            }

            // Test using !=
            var inequalityOperators = typeof(T).GetAllMethods(x => x.Name == "op_Inequality");
            foreach (var method in inequalityOperators)
            {
                var parameters = method.GetParameters();
                if (parameters.Any(x => !x.ParameterType.IsAssignableFrom(typeof(T))))
                    continue;

                testedMethod = "op_Inequality (!=)";
                if (typeof(T).IsClass)
                {
                    testCase = "When A and B are null";
                    a = default!;
                    b = default!;
                    Assert.IsFalse((bool)method.Invoke(null, new object[] { a, b })!);

                    testCase = "When A is null but B is not";
                    a = default!;
                    b = fixture.Create<T>();
                    Assert.IsTrue((bool)method.Invoke(null, new object[] { a, b! })!);

                    testCase = "When B is null but A is not";
                    a = fixture.Create<T>();
                    b = default!;
                    Assert.IsTrue((bool)method.Invoke(null, new object[] { a!, b })!);
                }

                testCase = "When A and B are the same reference";
                a = fixture.Create<T>();
                b = a;
                Assert.IsFalse((bool)method.Invoke(null, new object[] { a!, b! })!);

                testCase = "When A and B are equivalent objects with different references";
                a = fixture.Create<T>();
                b = a.Clone(options);
                Assert.IsFalse((bool)method.Invoke(null, new object[] { a!, b! })!);

                testCase = "When A and B are different objects of the same type";
                a = fixture.Create<T>();
                b = fixture.Create<T>();
                Assert.IsTrue((bool)method.Invoke(null, new object[] { a!, b! })!);

                testCase = "Generic error";
                testedMethod = string.Empty;
                a = default!;
                b = default!;
            }

        }
        catch (Exception innerException)
        {
            var sb = new StringBuilder($"Value equality test failed : '{testCase}' with method '{testedMethod}'");

            if (a != null)
                sb.AppendLine($"A : {JsonSerializer.Serialize(a)}");
            if (b != null)
                sb.AppendLine($"B : {JsonSerializer.Serialize(b)}");

            throw new Exception(sb.ToString(), innerException is AssertFailedException ? null : innerException);
        }
    }

    private static void IsCloneable<T>(IObjectGenerator generator, JsonSerializerOptions options)
    {
        try
        {
            var original = generator.Create<T>();
            var clone = original.Clone(options);
            Assert.IsTrue(original.ValueEquals(clone));
        }
        catch (Exception innerException)
        {
            throw new Exception($"Unable to clone object of type {typeof(T).GetHumanReadableName()}", innerException);
        }
    }

    /// <summary>
    /// Tests equals methods, equality and inequality operators between the two objects for equality.
    /// </summary>
    public static void Equality<T1, T2>(T1? current, T2? other)
    {
        var equalsMethod = typeof(T1).GetSingleMethod(x => x.Name == "Equals" && x.ReturnType == typeof(bool) && x.HasParameters<T2>());
        Assert.IsTrue((bool)equalsMethod.Invoke(current, new object[] { other! })!);

        var equalityOperator = typeof(T1).GetSingleMethod(x => x.Name == "op_Equality" && x.HasParameters<T1, T2>());
        Assert.IsTrue((bool)equalityOperator.Invoke(null, new object[] { current!, other! })!);

        var inequalityOperator = typeof(T1).GetSingleMethod(x => x.Name == "op_Inequality" && x.HasParameters<T1, T2>());
        Assert.IsFalse((bool)inequalityOperator.Invoke(null, new object[] { current!, other! })!);
    }

    /// <summary>
    /// Tests equals methods, equality and inequality operators between the two objects for inequality.
    /// </summary>
    public static void Inequality<T1, T2>(T1 current, T2 other)
    {
        var equalsMethod = typeof(T1).GetSingleMethod(x => x.Name == "Equals" && x.ReturnType == typeof(bool) && x.HasParameters<T2>());
        Assert.IsFalse((bool)equalsMethod.Invoke(current, new object[] { other! })!);

        var equalityOperator = typeof(T1).GetSingleMethod(x => x.Name == "op_Equality" && x.HasParameters<T1, T2>());
        Assert.IsFalse((bool)equalityOperator.Invoke(null, new object[] { current!, other! })!);

        var inequalityOperator = typeof(T1).GetSingleMethod(x => x.Name == "op_Inequality" && x.HasParameters<T1, T2>());
        Assert.IsTrue((bool)inequalityOperator.Invoke(null, new object[] { current!, other! })!);
    }

    /// <summary>
    /// Automatically tests that two equivalent instances of the same type produce the same hash code and that two different objects do not.
    /// </summary>
    public static void ValueHashCode<T>() => ValueHashCode<T>(ObjectGeneratorProvider.Create(), new JsonSerializerOptions());

    /// <summary>
    /// Automatically tests that two equivalent instances of the same type produce the same hash code and that two different objects do not.
    /// </summary>
    public static void ValueHashCode<T>(IObjectGenerator generator) => ValueHashCode<T>(generator ?? throw new ArgumentNullException(nameof(generator)), new JsonSerializerOptions());

    /// <summary>
    /// Automatically tests that two equivalent instances of the same type produce the same hash code and that two different objects do not.
    /// </summary>
    public static void ValueHashCode<T>(JsonSerializerOptions options) => ValueHashCode<T>(ObjectGeneratorProvider.Create(), options ?? throw new ArgumentNullException(nameof(options)));

    /// <summary>
    /// Automatically tests that two equivalent instances of the same type produce the same hash code and that two different objects do not.
    /// </summary>
    public static void ValueHashCode<T>(IObjectGenerator generator, JsonSerializerOptions options)
    {
        if (generator is null) throw new ArgumentNullException(nameof(generator));
        if (options is null) throw new ArgumentNullException(nameof(options));

        IsCloneable<T>(generator, options);

        T a = default!;
        T b = default!;

        var testCase = "None";
        try
        {
            testCase = "When A and B are different objects of the same type then they should not be equal";
            a = generator.Create<T>()!;
            b = generator.Create<T>()!;
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());

            testCase = "When A and B are equivalent objects with different references then they should be equal";
            a = generator.Create<T>()!;
            b = a.Clone(options)!;
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());

            testCase = "When A and B are the same reference then they should be equal";
            a = generator.Create<T>()!;
            b = a;
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        catch
        {
            var sb = new StringBuilder($"Hash code consistency failed : {testCase}");

            sb.AppendLine($"A : {JsonSerializer.Serialize(a)}");
            sb.AppendLine($"B : {JsonSerializer.Serialize(b)}");

            throw new Exception(sb.ToString());
        }
    }

    /// <summary>
    /// Tests that type can be serialized to JSON and back using Microsoft's System.Text.Json.
    /// </summary>
    public static void IsJsonSerializable<T>() => IsJsonSerializable<T>(ObjectGeneratorProvider.Create(), new JsonSerializerOptions());

    /// <summary>
    /// Tests that type can be serialized to JSON and back using Microsoft's System.Text.Json.
    /// </summary>
    public static void IsJsonSerializable<T>(IObjectGenerator fixture) => IsJsonSerializable<T>(fixture ?? throw new ArgumentNullException(nameof(fixture)), new JsonSerializerOptions());

    /// <summary>
    /// Tests that type can be serialized to JSON and back using Microsoft's System.Text.Json.
    /// </summary>
    public static void IsJsonSerializable<T>(JsonSerializerOptions options) => IsJsonSerializable<T>(ObjectGeneratorProvider.Create(), options ?? throw new ArgumentNullException(nameof(options)));

    /// <summary>
    /// Tests that type can be serialized to JSON and back using Microsoft's System.Text.Json.
    /// </summary>
    public static void IsJsonSerializable<T>(IObjectGenerator fixture, JsonSerializerOptions options)
    {
        if (fixture is null) throw new ArgumentNullException(nameof(fixture));
        if (options is null) throw new ArgumentNullException(nameof(options));

        var instance = fixture.Create<T>();
        var json = JsonSerializer.Serialize(instance, options);
        var deserialized = JsonSerializer.Deserialize<T>(json, options);
        Assert.IsTrue(instance.ValueEquals(deserialized));
    }

    /// <summary>
    /// Tests that every property on <see cref="T"/> with public getter and setter accessors function as intended.
    /// </summary>
    public static void HasBasicGetSetFunctionality<T>(IObjectGenerator? fixture = null)
    {
        fixture ??= ObjectGeneratorProvider.Create();

        var properties = typeof(T).GetAllProperties(x => x.IsGet() && x.IsSet() && x.IsPublic() && x.IsInstance());

        foreach (var property in properties)
        {
            var instance = fixture.Create<T>();
            var value = fixture.Create(property.PropertyType);
            property.SetValue(instance, value);
            Assert.AreEqual(value, property.GetValue(instance));
        }
    }

    /// <summary>
    /// Tests that the specified property has basic get/set functionality.
    /// </summary>
    public static void HasBasicGetSetFunctionality<T>(string propertyName) => HasBasicGetSetFunctionality<T>(ObjectGeneratorProvider.Create(), propertyName);

    /// <summary>
    /// Tests that the specified property has basic get/set functionality.
    /// </summary>
    public static void HasBasicGetSetFunctionality<T>(IObjectGenerator fixture, string propertyName)
    {
        if (fixture is null) throw new ArgumentNullException(nameof(fixture));
        if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException(nameof(propertyName));

        var propertyInfo = typeof(T).GetSingleProperty(propertyName);

        var instance = fixture.Create<T>();
        var value = fixture.Create(propertyInfo.PropertyType);

        propertyInfo.SetValue(instance, value);
        var retrievedValue = propertyInfo.GetValue(instance);
        Assert.AreEqual(value, retrievedValue);
    }

    /// <summary>
    /// Tests that a collection correctly enumerates.
    /// </summary>
    public static void EnumeratesAllItems<TCollection, TItem>(IObjectGenerator? fixture = null) where TCollection : IEnumerable<TItem>
    {
        fixture ??= ObjectGeneratorProvider.Create();

        var instance = fixture.Create<TCollection>()!;
        Assert.IsTrue(instance.Any(), $"Collection of type '{typeof(TCollection).GetHumanReadableName()}' was created without items. It may require custom handling such as a Customization or a SpecimenBuilder (in AutoFixture).");

        var enumeratedItems = new List<TItem>();
        foreach (var item in instance)
            enumeratedItems.Add(item);

        Assert.IsTrue(instance.SequenceEqual(enumeratedItems));

        enumeratedItems.Clear();

        var nonGeneric = (IEnumerable)instance;

        foreach (var item in nonGeneric)
            enumeratedItems.Add((TItem)item);

        Assert.IsTrue(instance.SequenceEqual(enumeratedItems));
    }
}