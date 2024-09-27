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

    //TODO _unwrapped.CreateMany<T>() is not implemented
    public override IEnumerable<T> CreateMany<T>()
    {
        for (var i = 0; i < 3; i++)
        {
            yield return _unwrapped.Create<T>();
        }
    }
}