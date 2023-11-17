using System.Text;

namespace ToolBX.Eloquentest;

/// <summary>
/// Test cases to common problems.
/// </summary>
public static class Cases
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
    public static void TestValueEquality<T>(IFixture? fixture = null)
    {
        fixture ??= FixtureProvider.Create();
        //TODO Use GetAllMethods() from 2.2.0
        var methods = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(x => x.Name == "Equals" && x.GetParameters().Length == 1 && x.ReturnType == typeof(bool)).ToList();

        var testedMethod = string.Empty;
        var testCase = "None";
        T a = default!;
        T b = default!;
        try
        {
            foreach (var equals in methods)
            {
                testedMethod = $"{equals.Name}({equals.GetParameters().Single().GetType().Name})";

                testCase = "When B is null";
                if (typeof(T).IsClass)
                {
                    a = fixture.Create<T>();
                    b = default!;

                    Assert.IsFalse((bool)equals.Invoke(a, new object[] { b })!);
                }

                testCase = "When A and B are the same reference";
                a = fixture.Create<T>();
                b = a;
                Assert.IsTrue((bool)equals.Invoke(a, new object[] { b! })!);

                testCase = "When A and B are equivalent objects with different references";
                a = fixture.Create<T>();
                b = a.Clone();

                Assert.IsFalse(ReferenceEquals(a, b));
                Assert.IsTrue((bool)equals.Invoke(a, new object[] { b! })!);

                testCase = "When A and B are different objects of the same type";
                a = fixture.Create<T>();
                b = fixture.Create<T>();
                Assert.IsFalse((bool)equals.Invoke(a, new object[] { b })!);
            }

            var equalsOperator = typeof(T).GetMethod("op_Equality", BindingFlags.Public | BindingFlags.Static);
            if (equalsOperator != null)
            {
                testedMethod = "op_Equality (==)";
                if (typeof(T).IsClass)
                {
                    testCase = "When A and B are null";
                    a = default!;
                    b = default!;
                    Assert.IsTrue((bool)equalsOperator.Invoke(null, new object[] { a, b })!);

                    testCase = "When A is null but B is not";
                    a = default!;
                    b = fixture.Create<T>();
                    Assert.IsFalse((bool)equalsOperator.Invoke(null, new object[] { a, b })!);

                    testCase = "When B is null but A is not";
                    a = fixture.Create<T>();
                    b = default!;
                    Assert.IsFalse((bool)equalsOperator.Invoke(null, new object[] { a, b })!);
                }

                testCase = "When A and B are the same reference";
                a = fixture.Create<T>();
                b = a;
                Assert.IsTrue((bool)equalsOperator.Invoke(null, new object[] { a, b })!);

                testCase = "When A and B are equivalent objects with different references";
                a = fixture.Create<T>();
                b = a.Clone();
                Assert.IsTrue((bool)equalsOperator.Invoke(null, new object[] { a, b })!);

                testCase = "When A and B are different objects of the same type";
                a = fixture.Create<T>();
                b = fixture.Create<T>();
                Assert.IsFalse((bool)equalsOperator.Invoke(null, new object[] { a, b })!);
            }

            // Test using !=
            var notEqualsOperator = typeof(T).GetMethod("op_Inequality", BindingFlags.Public | BindingFlags.Static);
            if (notEqualsOperator != null)
            {
                testedMethod = "op_Inequality (!=)";

                if (typeof(T).IsClass)
                {
                    testCase = "When A and B are null";
                    a = default!;
                    b = default!;
                    Assert.IsFalse((bool)notEqualsOperator.Invoke(null, new object[] { a, b })!);

                    testCase = "When A is null but B is not";
                    a = default!;
                    b = fixture.Create<T>();
                    Assert.IsTrue((bool)notEqualsOperator.Invoke(null, new object[] { a, b })!);

                    testCase = "When B is null but A is not";
                    a = fixture.Create<T>();
                    b = default!;
                    Assert.IsTrue((bool)notEqualsOperator.Invoke(null, new object[] { a, b })!);
                }

                testCase = "When A and B are the same reference";
                a = fixture.Create<T>();
                b = a;
                Assert.IsFalse((bool)notEqualsOperator.Invoke(null, new object[] { a, b })!);

                testCase = "When A and B are equivalent objects with different references";
                a = fixture.Create<T>();
                b = a.Clone();
                Assert.IsFalse((bool)notEqualsOperator.Invoke(null, new object[] { a, b })!);

                testCase = "When A and B are different objects of the same type";
                a = fixture.Create<T>();
                b = fixture.Create<T>();
                Assert.IsTrue((bool)notEqualsOperator.Invoke(null, new object[] { a, b })!);
            }
        }
        catch
        {
            var sb = new StringBuilder($"Value equality test failed : '{testCase}' with method '{testedMethod}'");

            if (a != null)
                sb.AppendLine($"A : {JsonSerializer.Serialize(a)}");
            if (b != null)
                sb.AppendLine($"B : {JsonSerializer.Serialize(a)}");

            throw new Exception(sb.ToString());
        }
    }
}