using System.Text.Json;
using ToolBX.Eloquentest.Resources;

namespace Eloquentest.UnitTesting;

public abstract class EnsureTester<TEnsure, TGenerator> where TEnsure : EnsureBase<TGenerator>, new() where TGenerator : new()
{
    protected ObjectGenerator Generator { get; private set; } = null!;

    protected TEnsure Ensure { get; private set; } = null!;

    protected TGenerator UnwrappedGenerator { get; private set; } = default!;

    protected abstract ObjectGenerator Wrap(TGenerator generator);

    [TestInitialize]
    public void TestInitializeBase()
    {
        UnwrappedGenerator = new();
        Generator = Wrap(UnwrappedGenerator);
        Ensure = new();
    }

    [TestMethod]
    public void WhenIsNullOrEmpty_WhenActionIsNull_Throw()
    {
        //Arrange
        Action<string> action = null!;

        //Act
        var result = () => Ensure.WhenIsNullOrEmpty(action);

        //Assert
        result.Should().Throw<ArgumentNullException>().WithParameterName(nameof(action));
    }

    [TestMethod]
    public void WhenIsNullOrEmpty_WhenActionIsNotNull_ExecuteWithEmptyAndNullStrings()
    {
        //Arrange
        var stringsUsed = new List<string>();
        Action<string> action = x => stringsUsed.Add(x);

        //Act
        Ensure.WhenIsNullOrEmpty(action);

        //Assert
        stringsUsed.Should().BeEquivalentTo(["", null!]);
    }

    [TestMethod]
    public void WhenIsNullOrWhiteSpace_WhenActionIsNull_Throw()
    {
        //Arrange
        Action<string> action = null!;

        //Act
        var result = () => Ensure.WhenIsNullOrWhiteSpace(action);

        //Assert
        result.Should().Throw<ArgumentNullException>().WithParameterName(nameof(action));
    }

    [TestMethod]
    public void WhenIsNullOrWhiteSpace_WhenActionIsNotNull_ExecuteWithWhiteStrings()
    {
        //Arrange
        var stringsUsed = new List<string>();
        Action<string> action = x => stringsUsed.Add(x);

        //Act
        Ensure.WhenIsNullOrWhiteSpace(action);

        //Assert
        stringsUsed.Should().BeEquivalentTo(["", null!, " ", "\n", "\r", "\t"]);
    }

    [TestMethod]
    public void ValueEquality_WhenParameterlessAndTypeHasValueEquality_DoNotThrow()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.ValueEqual>();

        //Assert
        action.Should().NotThrow();
    }

    [TestMethod]
    public void ValueEquality_WhenParameterlessAndTNullEqualityReturnsTrue_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.NullEqualityFail>();

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage($"*{AssertionFailures.NullOtherShouldNotBeEqual}");
    }

    [TestMethod]
    public void ValueEquality_WhenParameterlessAndReferenceEqualityReturnsFalseForSameReference_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.ReferenceEqualityFail>();

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsTrue failed. Objects with the same reference should be equal.");
    }

    [TestMethod]
    public void ValueEquality_WhenParameterlessAndValueEqualityFail_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.ValueEqualityFail>();

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsTrue failed. Objects with the same value should be equal.");
    }

    [TestMethod]
    public void ValueEquality_WhenParameterlessAndDifferentValuesReturnTrue_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.BadValueEquality>();

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsFalse failed. Objects with different values should not be equal.");
    }

    [TestMethod]
    public void ValueEquality_WhenParameterlessAndTypeDoesNotHaveValueEquality_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.NoValueEquality>();

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsTrue failed. Objects with the same value should be equal.");
    }

    [TestMethod]
    public void ValueEquality_WhenParameterlessAndDoubleNullEqualityFails_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.OpEqualityNullFail1>();

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsTrue failed. Two nulls should always return true when compared with '=='.");
    }

    [TestMethod]
    public void ValueEquality_WhenParameterlessAndLeftIsNullButRightIsNotReturnsTrue_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.OpEqualityNullFail2>();

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsFalse failed. 'Left' is null and 'Right' is not. They should not be considered equal when compared with '=='.");
    }

    [TestMethod]
    public void ValueEquality_WhenParameterlessAndRightIsNullButLeftIsNotReturnsTrue_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.OpEqualityNullFail3>();

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsFalse failed. 'Right' is null and 'Left' is not. They should not be considered equal when compared with '=='.");
    }

    [TestMethod]
    public void ValueEquality_WhenParameterlessAndObjectsWithSameReferenceReturnsFalse_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.OpEqualitySameReferenceFail>();

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsTrue failed. Two objects with the same reference should be considered equal when compared with '=='.");
    }

    [TestMethod]
    public void ValueEquality_WhenParameterlessAndObjectsWithSameValueReturnsFalse_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.OpEqualitySameValueFail>();

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsTrue failed. Objects with the same value should be considered equal when compared with '=='.");
    }

    [TestMethod]
    public void ValueEquality_WhenParameterlessAndObjectsWithDifferentValuesReturnTrue_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.OpEqualityDifferentValuesFail>();

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsFalse failed. Objects with different values should not be considered equal when compared with '=='.");
    }

    [TestMethod]
    public void ValueInequality_WhenParameterlessAndDoubleNullEqualityFails_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.OpInequalityNullFail>();

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsFalse failed. Two nulls should always return false when compared with '!='.");
    }

    [TestMethod]
    public void ValueInequality_WhenParameterlessAndLeftIsNullButRightIsNotReturnsFalse_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.OpInequalityLeftNullFail>();

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsTrue failed. 'Left' is null and 'Right' is not. This should be true when compared with '!='.");
    }

    [TestMethod]
    public void ValueInequality_WhenParameterlessAndRightIsNullButLeftIsNotReturnsFalse_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.OpInequalityRightNullFail>();

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsTrue failed. 'Right' is null and 'Left' is not. This should be true when compared with '!='.");
    }

    [TestMethod]
    public void ValueInequality_WhenParameterlessAndHasInproperValueEquality_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.OpInequalityValueFail>();

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsFalse failed. Same references should return false when compared with '!='.");
    }

    [TestMethod]
    public void ValueInequality_WhenParameterlessAndInequalityAlwaysFalse_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.OpInequalityAlwaysEqual>();

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsTrue failed. Objects with different values should return true when compared with '!='.");
    }

    [TestMethod]
    public void ValueEquality_WhenPassingGeneratorAndHasValueEquality_DoNotThrow()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.ValueEqual>(UnwrappedGenerator);

        //Assert
        action.Should().NotThrow();
    }

    [TestMethod]
    public void ValueEquality_WhenPassingGeneratorAndTNullEqualityReturnsTrue_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.NullEqualityFail>(UnwrappedGenerator);

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsFalse failed. Objects should not be equal if one is null.");
    }

    [TestMethod]
    public void ValueEquality_WhenPassingGeneratorAndReferenceEqualityReturnsFalseForSameReference_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.ReferenceEqualityFail>(UnwrappedGenerator);

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsTrue failed. Objects with the same reference should be equal.");
    }

    [TestMethod]
    public void ValueEquality_WhenPassingGeneratorAndValueEqualityFail_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.ValueEqualityFail>(UnwrappedGenerator);

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsTrue failed. Objects with the same value should be equal.");
    }

    [TestMethod]
    public void ValueEquality_WhenPassingGeneratorAndTypeDoesNotHaveValueEquality_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.NoValueEquality>(UnwrappedGenerator);

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsTrue failed. Objects with the same value should be equal.");
    }

    [TestMethod]
    public void ValueEquality_WhenPassingGeneratorAndSerializerAndHasValueEquality_DoNotThrow()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.ValueEqual>(UnwrappedGenerator, new JsonSerializerOptions());

        //Assert
        action.Should().NotThrow();
    }

    [TestMethod]
    public void ValueEquality_WhenPassingGeneratorAndSerializerAndNullEqualityReturnsTrue_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.NullEqualityFail>(UnwrappedGenerator, new JsonSerializerOptions());

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsFalse failed. Objects should not be equal if one is null.");
    }

    [TestMethod]
    public void ValueEquality_WhenPassingGeneratorAndSerializerAndReferenceEqualityReturnsFalseForSameReference_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.ReferenceEqualityFail>(UnwrappedGenerator, new JsonSerializerOptions());

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsTrue failed. Objects with the same reference should be equal.");
    }

    [TestMethod]
    public void ValueEquality_WhenPassingGeneratorAndSerializerAndValueEqualityFail_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.ValueEqualityFail>(UnwrappedGenerator, new JsonSerializerOptions());

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsTrue failed. Objects with the same value should be equal.");
    }

    [TestMethod]
    public void ValueEquality_WhenPassingGeneratorAndSerializerAndTypeDoesNotHaveValueEquality_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.NoValueEquality>(UnwrappedGenerator, new JsonSerializerOptions());

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsTrue failed. Objects with the same value should be equal.");
    }

    [TestMethod]
    public void ValueEquality_WhenValuesAreEqual_DoNotThrow()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.ValueEqual>(UnwrappedGenerator);

        //Assert
        action.Should().NotThrow();
    }

    [TestMethod]
    public void ValueEquality_WhenObjectsAreRecords_DoNotThrow()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<Garbage.RecordEquality>(UnwrappedGenerator);

        //Assert
        action.Should().NotThrow();
    }

    [TestMethod]
    public void ValueEquality_WhenObjectIsComplexCollection_DoNotThrow()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueEquality<ComplexCollection<Garbage.RecordEquality>>(UnwrappedGenerator);

        //Assert
        action.Should().NotThrow();
    }

    [TestMethod]
    public void Equality_BetweenClones_DoNotThrow()
    {
        //Arrange
        var dummy1 = Generator.Create<Garbage.ValueEqual>();
        var dummy2 = dummy1.Clone();

        //Act
        var action = () => Ensure.Equality(dummy1, dummy2);

        //Assert
        action.Should().NotThrow();
    }

    [TestMethod]
    public void Equality_BetweenDifferentObjects_Throw()
    {
        //Arrange
        var dummy1 = Generator.Create<Garbage.ValueEqual>();
        var dummy2 = Generator.Create<Garbage.ValueEqual>();

        //Act
        var action = () => Ensure.Equality(dummy1, dummy2);

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsTrue failed. Was not considered equal using the Equals method.");
    }

    [TestMethod]
    public void Equality_WhenEqualsOperatorHasBadValueEquality_Throw()
    {
        //Arrange
        var dummy1 = Generator.Create<Garbage.OpEqualitySameValueFail>();
        var dummy2 = dummy1.Clone();

        //Act
        var action = () => Ensure.Equality(dummy1, dummy2);

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsTrue failed. Was not considered equal using the '==' operator.");
    }

    [TestMethod]
    public void Equality_WhenInequalityOperatorHasBadValueEquality_Throw()
    {
        //Arrange
        var dummy1 = Generator.Create<Garbage.OpInequalityValueFail>();
        var dummy2 = dummy1.Clone();

        //Act
        var action = () => Ensure.Equality(dummy1, dummy2);

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsFalse failed. Was expecting false with '!='.");
    }

    [TestMethod]
    public void Inequality_BetweenDifferentObjects_DoNotThrow()
    {
        //Arrange
        var dummy1 = Generator.Create<Garbage.ValueEqual>();
        var dummy2 = Generator.Create<Garbage.ValueEqual>();

        //Act
        var action = () => Ensure.Inequality(dummy1, dummy2);

        //Assert
        action.Should().NotThrow();
    }

    [TestMethod]
    public void Inequality_BetweenSameValues_Throw()
    {
        //Arrange
        var dummy1 = Generator.Create<Garbage.ValueEqual>();
        var dummy2 = dummy1.Clone();

        //Act
        var action = () => Ensure.Inequality(dummy1, dummy2);

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsFalse failed. Was expecting false using the Equals method.");
    }

    [TestMethod]
    public void Inequality_WhenEqualsOperatorHasBadValueEquality_Throw()
    {
        //Arrange
        var dummy1 = Generator.Create<Garbage.OpEqualityDifferentValuesFail>();
        var dummy2 = Generator.Create<Garbage.OpEqualityDifferentValuesFail>();

        //Act
        var action = () => Ensure.Inequality(dummy1, dummy2);

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsFalse failed. Was expecting false using the '==' operator.");
    }

    [TestMethod]
    public void Inequality_WhenInequalityOperatorHasBadValueEquality_Throw()
    {
        //Arrange
        var dummy1 = Generator.Create<Garbage.OpInequalityValueFail>();
        var dummy2 = Generator.Create<Garbage.OpInequalityValueFail>();

        //Act
        var action = () => Ensure.Inequality(dummy1, dummy2);

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.IsTrue failed. Was expecting true using the '!=' operator.");
    }

    [TestMethod]
    public void ValueHashCode_WhenTypeHasProperValueHasCodeEquality_DoNotThrow()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueHashCode<Garbage.ProperHashCode>();

        //Assert
        action.Should().NotThrow();
    }

    [TestMethod]
    public void ValueHashCode_WhenTwoDifferentObjectsProduceSameHashCode_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueHashCode<Garbage.ConstantHashCode>();

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.AreNotEqual failed. Expected any value except:<14>. Actual:<14>. Two objects with different values should produce different hash code.");
    }

    [TestMethod]
    public void ValueHashCode_WhenTwoDifferentObjectsWithSameValueDoNotProduceSameHashCode_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueHashCode<Garbage.RandomHashCode>();

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.AreEqual failed. * Two objects with the same value should produce the same hash code.");
    }

    [TestMethod]
    public void ValueHashCode_WhenSameReferenceDoesNotProduceSameHashCode_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueHashCode<Garbage.SameReferenceBadHashCode>();

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage("Assert.AreEqual failed. * Two objects with the same reference should produce the same hash code.");
    }

    [TestMethod]
    public void ValueHashCode_WhenHasReadOnlyLists_DoNoThrow()
    {
        //Arrange

        //Act
        var action = () => Ensure.ValueHashCode<Garbage.Splitted<Garbage.ValueEqual>>();

        //Assert
        action.Should().NotThrow();
    }

    [TestMethod]
    public void IsJsonSerializable_WhenIsEasilySerializable_DoNotThrow()
    {
        //Arrange

        //Act
        var action = () => Ensure.IsJsonSerializable<Garbage.ValueEqual>(UnwrappedGenerator);

        //Assert
        action.Should().NotThrow();
    }

    [TestMethod]
    public void IsJsonSerializable_WhenCannotBeSerialized_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.IsJsonSerializable<Garbage.Unserializable>(UnwrappedGenerator);

        //Assert
        action.Should().Throw<JsonException>();
    }

    [TestMethod]
    public void IsJsonSerializable_WhenHasBadSerializer_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.IsJsonSerializable<Garbage.BadSerialization>(UnwrappedGenerator);

        //Assert
        action.Should().Throw<AssertFailedException>();
    }

    [TestMethod]
    public void HasBasicGetSetFunctionality_WhenAllPropertiesHaveGettersAndSetters_DoNotThrow()
    {
        //Arrange

        //Act
        var action = () => Ensure.HasBasicGetSetFunctionality<Garbage.ProperGetSetter>();

        //Assert
        action.Should().NotThrow();
    }

    [TestMethod]
    public void HasBasicGetSetFunctionality_WhenOnePropertyDoesNotSet_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.HasBasicGetSetFunctionality<Garbage.BadGetSetter>();

        //Assert
        action.Should().Throw<AssertFailedException>().WithMessage($"Assert.AreEqual failed. * Property {nameof(Garbage.ProperGetSetter.Name)} has a public get/set but it is not returning the value that it was set.");
    }

    [TestMethod]
    public void HasBasicGetSetFunctionality_WhenPropertyIsValid_DoNotThrow()
    {
        //Arrange

        //Act
        var action = () => Ensure.HasBasicGetSetFunctionality<Garbage.BadGetSetter>(nameof(Garbage.BadGetSetter.Id));

        //Assert
        action.Should().NotThrow();
    }

    [TestMethod]
    public void HasBasicGetSetFunctionality_WhenPropertyIsNotValid_Throw()
    {
        //Arrange

        //Act
        var action = () => Ensure.HasBasicGetSetFunctionality<Garbage.BadGetSetter>(nameof(Garbage.BadGetSetter.Name));

        //Assert
        action.Should().Throw<AssertFailedException>();
    }

    [TestMethod]
    public void SucceedsMultipleTimes_WhenActionIsNull_Throw()
    {
        //Arrange
        Action action = null!;
        var times = Generator.Create<int>();

        //Act
        var result = () => Ensure.SucceedsMultipleTimes(action, times);

        //Assert
        result.Should().Throw<ArgumentNullException>().WithParameterName(nameof(action));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(-3)]
    public void SucceedsMultipleTimes_WhenTimesIsZeroOrNegative_Throw(int times)
    {
        //Arrange
        var action = () => { };

        //Act
        var result = () => Ensure.SucceedsMultipleTimes(action, times);

        //Assert
        result.Should().Throw<ArgumentOutOfRangeException>().WithParameterName(nameof(times));
    }

    [TestMethod]
    public void SucceedsMultipleTimes_WhenThereIsNoFailure_DoNotThrow()
    {
        //Arrange
        var action = () => { };

        //Act
        var result = () => Ensure.SucceedsMultipleTimes(action);

        //Assert
        result.Should().NotThrow();
    }

    [TestMethod]
    public void SucceedsMultipleTimes_WhenThereIsFailure_Throw()
    {
        //Arrange
        var i = 0;
        var action = () =>
        {
            if (i > 1) throw new InvalidOperationException("This isn't allowed!");
            i++;
        };

        //Act
        var result = () => Ensure.SucceedsMultipleTimes(action);

        //Assert
        result.Should().Throw<InvalidOperationException>().WithMessage("This isn't allowed!");
    }

}