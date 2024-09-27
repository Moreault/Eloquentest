namespace ToolBX.Eloquentest.Dummies;

public static class InstanceProvider
{
    public static InstanceResult<T> Create<T>(IDummy? dummy = null, IEnumerable<object>? constructorParameterOverrides = null, IReadOnlyDictionary<Type, Mock>? overridenMocks = null!) where T : class
    {
        dummy ??= new Dummy();
        Func<Type, object> createMethod = x => dummy.Create(x);
        return Eloquentest.InstanceProvider.Create<T>(createMethod, constructorParameterOverrides, overridenMocks);
    }
}