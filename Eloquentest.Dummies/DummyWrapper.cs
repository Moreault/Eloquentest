namespace ToolBX.Eloquentest.Dummies;

public sealed class DummyWrapper : IObjectGenerator
{
    private readonly Dummy _unwrapped;

    public DummyWrapper()
    {
        _unwrapped = new Dummy();
    }

    public T Create<T>() => _unwrapped.Create<T>();

    public object Create(Type type) => _unwrapped.Create(type);

    //TODO _unwrapped.CreateMany<T>() is not implemented
    public IEnumerable<T> CreateMany<T>()
    {
        for (var i = 0; i < 3; i++)
        {
            yield return _unwrapped.Create<T>();
        }
    }
}