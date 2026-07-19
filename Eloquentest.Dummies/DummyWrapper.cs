namespace ToolBX.Eloquentest.Dummies;

public sealed class DummyWrapper : ObjectGenerator
{
    private readonly Dummy _unwrapped;

    public DummyWrapper()
    {
        _unwrapped = new Dummy();
    }

    public DummyWrapper(Dummy unwrapped)
    {
        _unwrapped = unwrapped ?? throw new ArgumentNullException(nameof(unwrapped));
    }

    public override T Create<T>() => _unwrapped.Create<T>();

    public override object Create(Type type) => _unwrapped.Create(type);

    public override IEnumerable<T> CreateMany<T>() => CreateMany<T>(3);

    public override IEnumerable<T> CreateMany<T>(int count)
    {
        for (var i = 0; i < count; i++)
            yield return _unwrapped.Create<T>();
    }
}