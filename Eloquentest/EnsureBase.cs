using ToolBX.Eloquentest.Resources;

namespace ToolBX.Eloquentest;

/// <summary>
/// Test cases to common problems.
/// </summary>
public abstract class EnsureBase<TGenerator>
{
    protected abstract ObjectGenerator Wrap(TGenerator generator);

    /// <summary>
    /// Automatically tests your method with null or empty strings.
    /// </summary>
    public void WhenIsNullOrEmpty(Action<string> action)
    {
        if (action is null) throw new ArgumentNullException(nameof(action));
        foreach (var datarow in new[] { "", null! })
            action.Invoke(datarow);
    }

    /// <summary>
    /// Automatically tests your method with null, empty and white space strings.
    /// </summary>
    public void WhenIsNullOrWhiteSpace(Action<string> action)
    {
        if (action is null) throw new ArgumentNullException(nameof(action));
        foreach (var datarow in new[] { "", null!, " ", "\n", "\r", "\t" })
            action.Invoke(datarow);
    }

    /// <summary>
    /// Automatically tests all equality cases between two objects.
    /// </summary>
    public void ValueEquality<T>() => ValueEquality<T>(ObjectGeneratorProvider.Create(), new JsonSerializerOptions());

    /// <summary>
    /// Automatically tests all equality cases between two objects.
    /// </summary>
    public void ValueEquality<T>(TGenerator generator) => ValueEquality<T>(Wrap(generator), new JsonSerializerOptions());

    /// <summary>
    /// Automatically tests all equality cases between two objects.
    /// </summary>
    public void ValueEquality<T>(TGenerator generator, JsonSerializerOptions options) => ValueEquality<T>(Wrap(generator), options);

    /// <summary>
    /// Automatically tests all equality cases between two objects.
    /// </summary>
    private void ValueEquality<T>(ObjectGenerator generator, JsonSerializerOptions options)
    {
        if (generator is null) throw new ArgumentNullException(nameof(generator));
        if (options is null) throw new ArgumentNullException(nameof(options));

        IsCloneable<T>(generator, options);

        var methods = typeof(T).GetAllMethods(x => x.Name == "Equals" && x.ReturnType == typeof(bool) && x.HasParameters(1));

        var a = generator.Create<T>();
        T b = default!;

        foreach (var method in methods)
        {
            var parameterType = method.GetParameters().First();
            if (!typeof(T).IsAssignableTo(parameterType.ParameterType))
                continue;

            if (typeof(T).IsClass)
            {
                a = generator.Create<T>();
                b = default!;

                Assert.IsFalse((bool)method.Invoke(a, [b])!, AssertionFailures.NullOtherShouldNotBeEqual);
            }

            a = generator.Create<T>();
            b = a;
            Assert.IsTrue((bool)method.Invoke(a, [b!])!, AssertionFailures.SameReferenceShouldAlwaysBeEqual);

            a = generator.Create<T>();
            b = a.Clone(options);

            Assert.IsFalse(ReferenceEquals(a, b), AssertionFailures.ShouldNeverHappen);
            Assert.IsTrue((bool)method.Invoke(a, [b!])!, AssertionFailures.SameValueShouldBeEqual);

            a = generator.Create<T>();
            b = generator.Create<T>();
            Assert.IsFalse((bool)method.Invoke(a, [b!])!, AssertionFailures.DifferentValuesShouldNotBeEqual);
        }

        var equalityOperators = typeof(T).GetAllMethods(x => x.Name == "op_Equality");
        foreach (var method in equalityOperators)
        {
            var parameters = method.GetParameters();
            if (parameters.Any(x => !x.ParameterType.IsAssignableFrom(typeof(T))))
                continue;

            if (typeof(T).IsClass)
            {
                a = default!;
                b = default!;
                Assert.IsTrue((bool)method.Invoke(null, [a, b])!, "Two nulls should always return true when compared with '=='.");

                a = default!;
                b = generator.Create<T>();
                Assert.IsFalse((bool)method.Invoke(null, [a, b!])!, "'Left' is null and 'Right' is not. They should not be considered equal when compared with '=='.");

                a = generator.Create<T>();
                b = default!;
                Assert.IsFalse((bool)method.Invoke(null, [a!, b])!, "'Right' is null and 'Left' is not. They should not be considered equal when compared with '=='.");
            }

            a = generator.Create<T>();
            b = a;
            Assert.IsTrue((bool)method.Invoke(null, [a!, b!])!, "Two objects with the same reference should be considered equal when compared with '=='.");

            a = generator.Create<T>();
            b = a.Clone(options);
            Assert.IsTrue((bool)method.Invoke(null, [a!, b!])!, "Objects with the same value should be considered equal when compared with '=='.");

            a = generator.Create<T>();
            b = generator.Create<T>();
            Assert.IsFalse((bool)method.Invoke(null, [a!, b!])!, "Objects with different values should not be considered equal when compared with '=='.");
        }

        // Test using !=
        var inequalityOperators = typeof(T).GetAllMethods(x => x.Name == "op_Inequality");
        foreach (var method in inequalityOperators)
        {
            var parameters = method.GetParameters();
            if (parameters.Any(x => !x.ParameterType.IsAssignableFrom(typeof(T))))
                continue;

            if (typeof(T).IsClass)
            {
                a = default!;
                b = default!;
                Assert.IsFalse((bool)method.Invoke(null, [a, b])!, "Two nulls should always return false when compared with '!='.");

                a = default!;
                b = generator.Create<T>();
                Assert.IsTrue((bool)method.Invoke(null, [a, b!])!, "'Left' is null and 'Right' is not. This should be true when compared with '!='.");

                a = generator.Create<T>();
                b = default!;
                Assert.IsTrue((bool)method.Invoke(null, [a!, b])!, "'Right' is null and 'Left' is not. This should be true when compared with '!='.");
            }

            a = generator.Create<T>();
            b = a;
            Assert.IsFalse((bool)method.Invoke(null, [a!, b!])!, "Same references should return false when compared with '!='.");

            a = generator.Create<T>();
            b = a.Clone(options);
            Assert.IsFalse((bool)method.Invoke(null, [a!, b!])!, "Objects with the same value should return false when compared with '!='.");

            a = generator.Create<T>();
            b = generator.Create<T>();
            Assert.IsTrue((bool)method.Invoke(null, [a!, b!])!, "Objects with different values should return true when compared with '!='.");
        }
    }

    private void IsCloneable<T>(ObjectGenerator generator, JsonSerializerOptions options)
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
    public void Equality<T1, T2>(T1? current, T2? other)
    {
        var equalsMethod = typeof(T1).GetSingleMethodOrDefault(x => x.Name == "Equals" && x.ReturnType == typeof(bool) && x.HasParameters<T2>());
        if (equalsMethod is not null)
            Assert.IsTrue((bool)equalsMethod.Invoke(current, [other!])!, "Was not considered equal using the Equals method.");

        var equalsSystemObject = typeof(T1).GetSingleMethod(x => x.Name == "Equals" && x.ReturnType == typeof(bool) && x.HasParameters<object>() && x.ReflectedType == typeof(T1));
        Assert.IsTrue((bool)equalsSystemObject.Invoke(current, [other!])!, "Was not considered equal using the Equals method.");

        var equalityOperator = typeof(T1).GetSingleMethod(x => x.Name == "op_Equality" && x.HasParameters<T1, T2>());
        Assert.IsTrue((bool)equalityOperator.Invoke(null, [current!, other!])!, "Was not considered equal using the '==' operator.");

        var inequalityOperator = typeof(T1).GetSingleMethod(x => x.Name == "op_Inequality" && x.HasParameters<T1, T2>());
        Assert.IsFalse((bool)inequalityOperator.Invoke(null, [current!, other!])!, "Was expecting false with '!='.");
    }

    /// <summary>
    /// Tests equals methods, equality and inequality operators between the two objects for inequality.
    /// </summary>
    public void Inequality<T1, T2>(T1 current, T2 other)
    {
        var equalsMethod = typeof(T1).GetSingleMethodOrDefault(x => x.Name == "Equals" && x.ReturnType == typeof(bool) && x.HasParameters<T2>());
        if (equalsMethod is not null)
            Assert.IsFalse((bool)equalsMethod.Invoke(current, [other!])!, "Was expecting false using the Equals method.");

        var equalsSystemObject = typeof(T1).GetSingleMethod(x => x.Name == "Equals" && x.ReturnType == typeof(bool) && x.HasParameters<object>() && x.ReflectedType == typeof(T1));
        Assert.IsFalse((bool)equalsSystemObject.Invoke(current, [other!])!, "Was expecting false using the Equals method.");

        var equalityOperator = typeof(T1).GetSingleMethod(x => x.Name == "op_Equality" && x.HasParameters<T1, T2>());
        Assert.IsFalse((bool)equalityOperator.Invoke(null, [current!, other!])!, "Was expecting false using the '==' operator.");

        var inequalityOperator = typeof(T1).GetSingleMethod(x => x.Name == "op_Inequality" && x.HasParameters<T1, T2>());
        Assert.IsTrue((bool)inequalityOperator.Invoke(null, [current!, other!])!, "Was expecting true using the '!=' operator.");
    }


    /// <summary>
    /// Automatically tests that two equivalent instances of the same type produce the same hash code and that two different objects do not.
    /// </summary>
    public void ValueHashCode<T>() => ValueHashCode<T>(ObjectGeneratorProvider.Create(), new JsonSerializerOptions());

    /// <summary>
    /// Automatically tests that two equivalent instances of the same type produce the same hash code and that two different objects do not.
    /// </summary>
    public void ValueHashCode<T>(JsonSerializerOptions options) => ValueHashCode<T>(ObjectGeneratorProvider.Create(), options ?? throw new ArgumentNullException(nameof(options)));

    /// <summary>
    /// Automatically tests that two equivalent instances of the same type produce the same hash code and that two different objects do not.
    /// </summary>
    public void ValueHashCode<T>(TGenerator generator, JsonSerializerOptions options) => ValueHashCode<T>(Wrap(generator), options);

    /// <summary>
    /// Automatically tests that two equivalent instances of the same type produce the same hash code and that two different objects do not.
    /// </summary>
    private void ValueHashCode<T>(ObjectGenerator generator, JsonSerializerOptions options)
    {
        if (generator is null) throw new ArgumentNullException(nameof(generator));
        if (options is null) throw new ArgumentNullException(nameof(options));

        IsCloneable<T>(generator, options);

        var a = generator.Create<T>()!;
        var b = generator.Create<T>()!;
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode(), "Two objects with different values should produce different hash code.");

        a = generator.Create<T>()!;
        b = a.Clone(options)!;
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode(), "Two objects with the same value should produce the same hash code.");

        a = generator.Create<T>()!;
        b = a;
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode(), "Two objects with the same reference should produce the same hash code.");
    }

    /// <summary>
    /// Tests that type can be serialized to JSON and back using Microsoft's System.Text.Json.
    /// </summary>
    public void IsJsonSerializable<T>() => IsJsonSerializable<T>(ObjectGeneratorProvider.Create(), new JsonSerializerOptions());

    /// <summary>
    /// Tests that type can be serialized to JSON and back using Microsoft's System.Text.Json.
    /// </summary>
    public void IsJsonSerializable<T>(TGenerator fixture) => IsJsonSerializable<T>(Wrap(fixture ?? throw new ArgumentNullException(nameof(fixture))), new JsonSerializerOptions());

    /// <summary>
    /// Tests that type can be serialized to JSON and back using Microsoft's System.Text.Json.
    /// </summary>
    private void IsJsonSerializable<T>(ObjectGenerator fixture) => IsJsonSerializable<T>(fixture ?? throw new ArgumentNullException(nameof(fixture)), new JsonSerializerOptions());

    /// <summary>
    /// Tests that type can be serialized to JSON and back using Microsoft's System.Text.Json.
    /// </summary>
    public void IsJsonSerializable<T>(JsonSerializerOptions options) => IsJsonSerializable<T>(ObjectGeneratorProvider.Create(), options ?? throw new ArgumentNullException(nameof(options)));

    /// <summary>
    /// Tests that type can be serialized to JSON and back using Microsoft's System.Text.Json.
    /// </summary>
    public void IsJsonSerializable<T>(TGenerator fixture, JsonSerializerOptions options) => IsJsonSerializable<T>(Wrap(fixture), options);

    /// <summary>
    /// Tests that type can be serialized to JSON and back using Microsoft's System.Text.Json.
    /// </summary>
    private void IsJsonSerializable<T>(ObjectGenerator fixture, JsonSerializerOptions options)
    {
        if (fixture is null) throw new ArgumentNullException(nameof(fixture));
        if (options is null) throw new ArgumentNullException(nameof(options));

        var instance = fixture.Create<T>();

        var json = JsonSerializer.Serialize(instance, options);
        var deserialized = JsonSerializer.Deserialize<T>(json, options);
        Assert.IsTrue(instance.ValueEquals(deserialized), "Was not serialized and deserialized back into an object of equivalent value. You may need to use a custom JsonSerializer.");
    }

    /// <summary>
    /// Tests that every property on <see cref="T"/> with public getter and setter accessors function as intended.
    /// </summary>
    public void HasBasicGetSetFunctionality<T>(TGenerator? fixture = default) => HasBasicGetSetFunctionality<T>(fixture == null ? null : Wrap(fixture));

    /// <summary>
    /// Tests that every property on <see cref="T"/> with public getter and setter accessors function as intended.
    /// </summary>
    private void HasBasicGetSetFunctionality<T>(ObjectGenerator? fixture = null)
    {
        fixture ??= ObjectGeneratorProvider.Create();

        var properties = typeof(T).GetAllProperties(x => x.CanRead && x.CanWrite && x.IsPublic() && x.IsInstance());

        foreach (var property in properties)
        {
            var instance = fixture.Create<T>();
            var value = fixture.Create(property.PropertyType);
            property.SetValue(instance, value);
            Assert.AreEqual(value, property.GetValue(instance), $"Property {property.Name} has a public get/set but it is not returning the value that it was set.");
        }
    }

    /// <summary>
    /// Tests that the specified property has basic get/set functionality.
    /// </summary>
    public void HasBasicGetSetFunctionality<T>(string propertyName) => HasBasicGetSetFunctionality<T>(ObjectGeneratorProvider.Create(), propertyName);

    /// <summary>
    /// Tests that the specified property has basic get/set functionality.
    /// </summary>
    public void HasBasicGetSetFunctionality<T>(TGenerator fixture, string propertyName) => HasBasicGetSetFunctionality<T>(Wrap(fixture), propertyName);

    /// <summary>
    /// Tests that the specified property has basic get/set functionality.
    /// </summary>
    private void HasBasicGetSetFunctionality<T>(ObjectGenerator fixture, string propertyName)
    {
        if (fixture is null) throw new ArgumentNullException(nameof(fixture));
        if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException(nameof(propertyName));

        var propertyInfo = typeof(T).GetSingleProperty(propertyName);

        var instance = fixture.Create<T>();
        var value = fixture.Create(propertyInfo.PropertyType);

        propertyInfo.SetValue(instance, value);
        var retrievedValue = propertyInfo.GetValue(instance);
        Assert.AreEqual(value, retrievedValue, $"Property {propertyName} has a public get/set but it is not returning the value that it was set.");
    }

    /// <summary>
    /// Tests a method multiple times. Useful for testing flaky test methods.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void SucceedsMultipleTimes(Action action, int times = 3)
    {
        if (action is null) throw new ArgumentNullException(nameof(action));
        if (times < 1) throw new ArgumentOutOfRangeException(nameof(times));
        for (var i = 0; i < times; i++)
            action.Invoke();
    }
}