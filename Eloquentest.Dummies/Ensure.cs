namespace ToolBX.Eloquentest.Dummies;

public sealed class Ensure : EnsureBase<Dummy>
{
    public static Ensure Instance { get; } = new();

    protected override ObjectGenerator Wrap(Dummy generator) => new DummyWrapper(generator);
}