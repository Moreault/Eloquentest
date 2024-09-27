namespace ToolBX.Eloquentest;

public abstract class ObjectGenerator
{
    public abstract T Create<T>();
    public abstract object Create(Type type);
    public abstract IEnumerable<T> CreateMany<T>();
}