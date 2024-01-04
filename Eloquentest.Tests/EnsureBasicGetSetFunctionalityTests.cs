namespace Eloquentest.Tests;

[TestClass]
public sealed class EnsureBasicGetSetFunctionalityTests : Tester
{
    public sealed record Dummy
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
        public string Description { get; init; } = null!;
    }

    [TestMethod]
    public void Always_EnsureIdHasBasicGetSetFunctionality() => Ensure.HasBasicGetSetFunctionality<Dummy>(nameof(Dummy.Id));

    [TestMethod]
    public void Always_EnsureNameHasBasicGetSetFunctionality() => Ensure.HasBasicGetSetFunctionality<Dummy>(nameof(Dummy.Name));

    [TestMethod]
    public void Always_EnsureDescriptionHasBasicGetSetFunctionality() => Ensure.HasBasicGetSetFunctionality<Dummy>(nameof(Dummy.Description));
}