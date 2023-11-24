namespace ToolBX.Eloquentest;

/// <summary>
/// Automatically covers test cases for records such as cloning and value equality.
/// </summary>
public abstract class RecordTester<T> : Tester where T : class
{
    [TestMethod]
    public void WhenUsingPrivateCloningConstructor_ThenCloneObject()
    {
        //Arrange
        var instance = Fixture.Create<T>();

        var constructor = typeof(T).GetSingleConstructor(x => x.IsPrivate && x.IsInstance() && x.HasParameters<T>());

        //Act
        var result = (T)constructor.Invoke(new object?[] { instance });

        //Assert
        Assert.IsFalse(ReferenceEquals(instance, result));
    }

    [TestMethod]
    public void Always_ShouldHaveValueEquality() => Ensure.ValueEquality<T>(Fixture);

    [TestMethod]
    public void Always_IdGetsAndSets() => Ensure.HasBasicGetSetFunctionality<T>(Fixture);
}