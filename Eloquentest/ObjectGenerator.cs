namespace ToolBX.Eloquentest;

public abstract class ObjectGenerator
{
    public abstract T Create<T>();
    public abstract object Create(Type type);
    public abstract IEnumerable<T> CreateMany<T>();
    public virtual IEnumerable<T> CreateMany<T>(int count) => Enumerable.Range(0, count).Select(_ => Create<T>());
}