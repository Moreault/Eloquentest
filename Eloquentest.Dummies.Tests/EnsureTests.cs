namespace Eloquentest.Dummies.Tests;

[TestClass]
public sealed class EnsureTests : EnsureTester<Ensure, Dummy>
{
    protected override ObjectGenerator Wrap(Dummy generator) => new DummyWrapper(generator);
}