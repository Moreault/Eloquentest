namespace ToolBX.Eloquentest;

/// <summary>
/// Can be used to mock IServiceCollection.
/// </summary>
public class FakeServiceCollection : List<ServiceDescriptor>, IServiceCollection
{

}