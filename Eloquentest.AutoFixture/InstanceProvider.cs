namespace ToolBX.Eloquentest.AutoFixture;

public static class InstanceProvider
{
    public static InstanceResult<T> Create<T>(Fixture? dummy = null, IEnumerable<object>? constructorParameterOverrides = null, IReadOnlyDictionary<Type, Mock>? overridenMocks = null!) where T : class
    {
        dummy ??= new Fixture();
        Func<Type, object> createMethod = x => dummy.Create(x);
        return Eloquentest.InstanceProvider.Create<T>(createMethod, constructorParameterOverrides, overridenMocks);
    }
}