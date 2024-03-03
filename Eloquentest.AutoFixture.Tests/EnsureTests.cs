using AutoFixture;
using ToolBX.Eloquentest.AutoFixture;

namespace Eloquentest.AutoFixture.Tests;

[TestClass]
public sealed class EnsureTests : EnsureTester<Ensure, Fixture>
{
    protected override ObjectGenerator Wrap(Fixture generator) => new FixtureWrapper(generator);
}