namespace ToolBX.Eloquentest;

public interface IObjectGenerator
{
    T Create<T>();
    object Create(Type type);
    IEnumerable<T> CreateMany<T>();
}