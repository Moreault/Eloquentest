namespace ToolBX.Eloquentest.Integration;

internal class ClassInterfaceBinding
{
    public Type Interface { get; init; }
    public Type Service { get; init; }
}

public abstract class IntegrationTester<T> : Tester where T : class
{
    /// <summary>
    /// Instance of the class that is being tested.
    /// </summary>
    protected T Instance => _instance.Value;
    private Lazy<T> _instance;

    private readonly IDictionary<Type, object> _services = new Dictionary<Type, object>();

    private static IList<ClassInterfaceBinding> _bindings;

    /// <summary>
    /// Parameters that were used to instantiate <see cref="Instance"/>.
    /// </summary>
    protected IReadOnlyList<object> ConstructorParameters => _constructorParameters;
    private readonly List<object> _constructorParameters = new();

    private readonly List<object> _overridenConstructorParameters = new();

    protected IntegrationTester()
    {
        ResetInstance();
    }

    private void ResetInstance()
    {
        _instance = new Lazy<T>(() =>
        {
            var parameters = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.Instance).OrderBy(x => x.GetParameters().Length).FirstOrDefault()?.GetParameters() ?? Array.Empty<ParameterInfo>();
            var interfaces = parameters.Where(x => x.ParameterType.IsInterface).Select(x => x.ParameterType).ToList();
            //TODO Also needs to take into account IServiceProvider and non-interface instances of stuff (although this should be rare enough)
            var instancedParameters = new List<object>();
            foreach (var i in interfaces)
            {
                instancedParameters.Add(InstantiateFromInterface(i));
            }
            return Activator.CreateInstance(typeof(T), instancedParameters.ToArray()) as T;
        });
    }

    private object InstantiateFromInterface(Type serviceInterface)
    {
        var binding = _bindings.SingleOrDefault(x => x.Interface == serviceInterface);
        if (binding == null)
        {
            var candidate = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Single(x => x.Name == serviceInterface.Name[1..]);
            binding = new ClassInterfaceBinding
            {
                Interface = serviceInterface,
                Service = candidate
            };
            _bindings.Add(binding);
        }

        var parameters = binding.Service.GetConstructors(BindingFlags.Public | BindingFlags.Instance).OrderBy(x => x.GetParameters().Length).FirstOrDefault()?.GetParameters() ?? Array.Empty<ParameterInfo>();
        var interfaces = parameters.Where(x => x.ParameterType.IsInterface).Select(x => x.ParameterType).ToList();
        var instancedParameters = new List<object>();

        foreach (var i in interfaces)
        {
            instancedParameters.Add(InstantiateFromInterface(i));
        }

        return Activator.CreateInstance(binding.Service, instancedParameters.ToArray());
    }

    protected TService GetService<TService>() where TService : class
    {
        return (TService)_services[typeof(TService)];
    }

    protected override void InitializeTest()
    {
        base.InitializeTest();

        if (!_bindings.Any())
            _bindings = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && !x.IsAbstract && x.GetCustomAttribute<AutoInjectAttribute>() != null)
                .Select<Type, ClassInterfaceBinding>(x =>
                {
                    var attribute = x.GetCustomAttribute<AutoInjectAttribute>();
                    if (attribute.Interface != null)
                        return new ClassInterfaceBinding
                        {
                            Interface = attribute.Interface,
                            Service = x
                        };

                    var interfaces = x.GetInterfaces();
                    if (interfaces.Length == 1) return new ClassInterfaceBinding
                    {
                        Interface = interfaces.Single(),
                        Service = x
                    };
                    var withSameName = interfaces.SingleOrDefault(y => y.Name == $"I{x.Name}");
                    if (withSameName != null) return new ClassInterfaceBinding
                    {
                        Interface = withSameName,
                        Service = x
                    };

                    var regex = new Regex(@"(?<=[A-Z])(?=[A-Z][a-z]) | (?<=[^A-Z])(?=[A-Z]) | (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

                    var splittedTypeName = regex.Replace(x.Name, " ").Split(' ');
                    var searchResult = new List<InterfaceSearchResult>();

                    var directInterfaces = x.GetDirectInterfaces();

                    foreach (var i in interfaces)
                    {
                        var splittedInterfaceName = regex.Replace(i.Name, " ").Split(' ');
                        var similarities = splittedInterfaceName.Sum(x => splittedTypeName.Count(y => x.Contains(y, StringComparison.InvariantCultureIgnoreCase)));

                        if (similarities > 0)
                            searchResult.Add(new InterfaceSearchResult
                            {
                                Interface = i,
                                Similarities = similarities,
                                IsInherited = !directInterfaces.Contains(i)
                            });
                    }

                    if (!searchResult.Any()) throw new Exception($"Can't inject service automatically : {x.Name} implements {interfaces.Length} interfaces but none of them are close to similar in name.");
                    searchResult = searchResult.OrderBy(y => y.IsInherited).ThenByDescending(y => y.Similarities).ToList();
                    if (searchResult.Count > 1 && searchResult[0].Similarities == searchResult[1].Similarities && searchResult[0].IsInherited == searchResult[1].IsInherited)
                        throw new Exception($"Can't inject service automatically : {x.Name} there is ambiguity between {searchResult[0].Interface.Name} and {searchResult[1].Interface.Name}. Either change interface names or specify the interface to use.");

                    return new ClassInterfaceBinding
                    {
                        Interface = searchResult.First().Interface,
                        Service = x
                    };
                }).ToList();
    }

    protected override void CleanupTest()
    {
        base.CleanupTest();
        _services.Clear();
        _constructorParameters.Clear();
        _overridenConstructorParameters.Clear();
        ResetInstance();
    }
}