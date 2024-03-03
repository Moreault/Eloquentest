namespace ToolBX.Eloquentest;

//public interface IObjectGenerator
//{
//    T Create<T>();
//    object Create(Type type);
//    IEnumerable<T> CreateMany<T>();
//}

public abstract class ObjectGenerator
{
    public abstract T Create<T>();
    public abstract object Create(Type type);
    public abstract IEnumerable<T> CreateMany<T>();
}