namespace ToolBX.Eloquentest.AutoFixture;

public sealed class Ensure : EnsureBase<Fixture>
{
    public static Ensure Instance { get; } = new();

    protected override ObjectGenerator Wrap(Fixture generator) => new FixtureWrapper(generator);
}