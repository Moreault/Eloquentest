namespace ToolBX.Eloquentest;

/// <summary>
/// Test cases to common problems.
/// </summary>
public static class Cases
{
    /// <summary>
    /// Test cases relating to strings.
    /// </summary>
    public static class Strings
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
    }

    /// <summary>
    /// Automatically tests all equality cases between two objects.
    /// </summary>
    public static void TestValueEquality<T>(IFixture? fixture = null)
    {
        fixture ??= FixtureProvider.Create();
        //TODO Use GetAllMethods() from 2.2.0
        var methods = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(x => x.Name == "Equals" && x.GetParameters().Length == 1 && x.ReturnType == typeof(bool)).ToList();

        foreach (var equals in methods)
        {
            T a;
            T b;

            //When b is null (only applicable for reference types)
            if (typeof(T).IsClass)
            {
                a = fixture.Create<T>();
                b = default!;

                Assert.IsFalse((bool)equals.Invoke(a, new object[] { b })!);
            }

            //When a and b are same reference
            a = fixture.Create<T>();
            b = a;
            Assert.IsTrue((bool)equals.Invoke(a, new object[] { b! })!);

            //When a and b are different references but equivalent objects
            a = fixture.Create<T>();
            b = a.Clone();

            Assert.IsFalse(ReferenceEquals(a, b));
            Assert.IsTrue((bool)equals.Invoke(a, new object[] { b! })!);

            //When a and b are different objects of same type
            a = fixture.Create<T>();
            b = fixture.Create<T>();
            Assert.IsFalse((bool)equals.Invoke(a, new object[] { b })!);
        }

        try
        {
            // Test using ==
            var equalsOperator = typeof(T).GetMethod("op_Equality", BindingFlags.Public | BindingFlags.Static);
            if (equalsOperator != null)
            {
                T a;
                T b;

                if (typeof(T).IsClass)
                {
                    //When a and b are null
                    a = default!;
                    b = default!;
                    Assert.IsTrue((bool)equalsOperator.Invoke(null, new object[] { a, b })!);

                    //When a is null
                    a = default!;
                    b = fixture.Create<T>();
                    Assert.IsFalse((bool)equalsOperator.Invoke(null, new object[] { a, b })!);

                    //When b is null
                    a = fixture.Create<T>();
                    b = default!;
                    Assert.IsFalse((bool)equalsOperator.Invoke(null, new object[] { a, b })!);
                }

                //When a and b are the same reference
                a = fixture.Create<T>();
                b = a;
                Assert.IsTrue((bool)equalsOperator.Invoke(null, new object[] { a, b })!);

                //When a and b are equivalent objects
                a = fixture.Create<T>();
                b = a.Clone();
                Assert.IsTrue((bool)equalsOperator.Invoke(null, new object[] { a, b })!);

                //When a and b are different objects of the same type
                a = fixture.Create<T>();
                b = fixture.Create<T>();
                Assert.IsFalse((bool)equalsOperator.Invoke(null, new object[] { a, b })!);
            }
        }
        catch (Exception e)
        {
            throw new Exception("Operator == test failed", e);
        }

        try
        {
            // Test using !=
            var notEqualsOperator = typeof(T).GetMethod("op_Inequality", BindingFlags.Public | BindingFlags.Static);
            if (notEqualsOperator != null)
            {
                T a;
                T b;

                if (typeof(T).IsClass)
                {
                    //When a and b are null
                    a = default!;
                    b = default!;
                    Assert.IsFalse((bool)notEqualsOperator.Invoke(null, new object[] { a, b })!);

                    //When a is null
                    a = default!;
                    b = fixture.Create<T>();
                    Assert.IsTrue((bool)notEqualsOperator.Invoke(null, new object[] { a, b })!);

                    //When b is null
                    a = fixture.Create<T>();
                    b = default!;
                    Assert.IsTrue((bool)notEqualsOperator.Invoke(null, new object[] { a, b })!);
                }

                //When a and b are the same reference
                a = fixture.Create<T>();
                b = a;
                Assert.IsFalse((bool)notEqualsOperator.Invoke(null, new object[] { a, b })!);

                //When a and b are equivalent objects
                a = fixture.Create<T>();
                b = a.Clone();
                Assert.IsFalse((bool)notEqualsOperator.Invoke(null, new object[] { a, b })!);

                //When a and b are different objects of the same type
                a = fixture.Create<T>();
                b = fixture.Create<T>();
                Assert.IsTrue((bool)notEqualsOperator.Invoke(null, new object[] { a, b })!);
            }
        }
        catch (Exception e)
        {
            throw new Exception("Operator != test failed", e);
        }
    }
}