namespace ToolBX.Eloquentest;

public class Tester
{
    protected IFixture Fixture { get; private set; }

    private IList<ICustomization> AutoCustomizations => _autocustomizations.Value;
    private readonly Lazy<IList<ICustomization>> _autocustomizations = new(() =>
    {
        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(y => y.GetCustomAttributes(typeof(AutoCustomizationAttribute), true).Any()).Select(x => Activator.CreateInstance(x) as ICustomization).ToList();
    });

    [ClassInitialize]
    public void ClassInitializeOnBaseClass()
    {
        InitializeClass();
    }

    /// <summary>
    /// Runs once per test class before <see cref="InitializeTest"/>.
    /// </summary>
    protected virtual void InitializeClass()
    {

    }

    //Named as such to avoid unintentional shadowing
    [TestInitialize]
    public void TestInitializeOnBaseClass()
    {
        InitializeTest();

        Fixture = new Fixture();
        foreach (var autoCustomization in AutoCustomizations)
            Fixture.Customize(autoCustomization);
    }

    /// <summary>
    /// Runs before every test method.
    /// </summary>
    protected virtual void InitializeTest()
    {

    }

    //Named as such to avoid unintentional shadowing
    [ClassCleanup]
    public void ClassCleanupOnBaseClass()
    {
        CleanupClass();
    }

    /// <summary>
    /// Runs after all tests on the class.
    /// </summary>
    protected virtual void CleanupClass()
    {

    }

    //Named as such to avoid unintentional shadowing
    [TestCleanup]
    public void TestCleanupOnBaseClass()
    {
        CleanupTest();
    }

    /// <summary>
    /// Runs after each test.
    /// </summary>
    protected virtual void CleanupTest()
    {

    }

    private MethodInfo GetMethod<T>(T instance, string methodName, params object[] parameters)
    {
        var methodsWithName = instance.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).Where(x => x.Name == methodName).ToList();
        if (!methodsWithName.Any() && instance.GetType().BaseType != null)
            methodsWithName.AddRange(instance.GetType().BaseType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).Where(x => x.Name == methodName));

        if (!methodsWithName.Any())
            throw new Exception($"Can't get method '{methodName}' : There is no method by that name with {parameters.Length} parameters on type {instance.GetType()}.");

        if (methodsWithName.Count == 1)
            return methodsWithName.Single();

        var methodsWithSameNumberOfParameters = methodsWithName.Where(x => x.GetParameters().Length == parameters.Length).ToList();
        if (methodsWithSameNumberOfParameters.Count == 1)
            return methodsWithSameNumberOfParameters.Single();

        if (parameters.Any(x => x == null))
            throw new Exception($"Can't get method '{methodName}' : There are {methodsWithSameNumberOfParameters.Count} methods with this name and the same amount of parameters but the right one can't be determined because at least one of the parameters passed to {nameof(InvokeMethod)} is null.");

        foreach (var m in methodsWithSameNumberOfParameters)
        {
            for (var i = 0; i < parameters.Length; i++)
            {
                if (m.GetParameters()[i].ParameterType != parameters.GetType())
                    break;

                if (i == parameters.Length)
                    return m;
            }
        }

        throw new Exception($"Can't invoke method '{methodName}' : There is no method by that name with parameters of types {string.Join(", ", parameters.Select(x => x.GetType()))}.");
    }

    protected void InvokeMethod<T>(T instance, string methodName, params object[] parameters)
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));
        if (string.IsNullOrWhiteSpace(methodName)) throw new ArgumentNullException(nameof(methodName));
        var methodInfo = GetMethod(instance, methodName, parameters);
        methodInfo.Invoke(instance, parameters);
    }

    protected void InvokeMethodAndIgnoreException<TInstance, TException>(TInstance instance, string methodName, params object[] parameters) where TException : Exception
    {
        try
        {
            InvokeMethod(instance, methodName, parameters);
        }
        catch (TargetInvocationException e)
        {
            if (e.InnerException is not TException)
                throw;
        }
    }

    protected TValue GetFieldValue<TInstance, TValue>(TInstance instance, string fieldName)
    {
        var fieldInfo = GetField(instance, fieldName);
        return (TValue)fieldInfo.GetValue(instance);
    }

    protected void SetFieldValue<TInstance, TValue>(TInstance instance, string fieldName, TValue value)
    {
        var fieldInfo = GetField(instance, fieldName);
        fieldInfo.SetValue(instance, value);
    }

    private FieldInfo GetField<TInstance>(TInstance instance, string fieldName)
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));
        if (string.IsNullOrWhiteSpace(fieldName)) throw new ArgumentNullException(nameof(fieldName));

        var fieldInfo = FindFieldRecursively(instance.GetType(), fieldName);

        if (fieldInfo == null)
            throw new Exception($"Can't get the value of field '{fieldName}' : There is no field by that name on type {instance.GetType()}.");

        return fieldInfo;
    }

    private FieldInfo FindFieldRecursively(Type type, string name)
    {
        var memberInfo = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        return memberInfo ?? FindFieldRecursively(type.BaseType, name);
    }

    protected TValue GetPropertyValue<TInstance, TValue>(TInstance instance, string propertyName)
    {
        var propertyInfo = GetProperty(instance, propertyName);
        return (TValue)propertyInfo.GetValue(instance);
    }

    protected void SetPropertyValue<TInstance, TValue>(TInstance instance, string propertyName, TValue value)
    {
        var propertyInfo = GetProperty(instance, propertyName);
        propertyInfo = propertyInfo.DeclaringType.GetProperty(propertyName);
        propertyInfo.SetValue(instance, value, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, null, null);
    }

    private PropertyInfo GetProperty<TInstance>(TInstance instance, string propertyName)
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));
        if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException(nameof(propertyName));
        var propertyInfo = FindPropertyRecursively(instance.GetType(), propertyName);
        if (propertyInfo == null)
            throw new Exception($"Can't get the value of property '{propertyName}' : There is no property by that name on type {instance.GetType()}.");
        return propertyInfo;
    }

    private PropertyInfo FindPropertyRecursively(Type type, string name)
    {
        var memberInfo = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        return memberInfo ?? FindPropertyRecursively(type.BaseType, name);
    }

    private static bool _areAssembliesLoaded;
    // Source: https://dotnetstories.com/blog/Dynamically-pre-load-assemblies-in-a-ASPNET-Core-or-any-C-project-en-7155735300
    private static void LoadAllAssemblies(bool includeFramework = false)
    {
        if (_areAssembliesLoaded) return;

        var loaded = new ConcurrentDictionary<string, bool>();

        bool ShouldLoad(string assemblyName)
        {
            return (includeFramework || IsNotNetFramework(assemblyName)) && !loaded.ContainsKey(assemblyName);
        }

        bool IsNotNetFramework(string assemblyName)
        {
            return !assemblyName.StartsWith("Microsoft.")
                   && !assemblyName.StartsWith("System.")
                   && !assemblyName.StartsWith("Newtonsoft.")
                   && assemblyName != "netstandard";
        }

        void LoadReferencedAssembly(Assembly assembly)
        {
            // Check all referenced assemblies of the specified assembly
            foreach (var an in assembly.GetReferencedAssemblies().Where(a => ShouldLoad(a.FullName)))
            {
                // Load the assembly and load its dependencies
                LoadReferencedAssembly(Assembly.Load(an)); // AppDomain.CurrentDomain.Load(name)
                loaded.TryAdd(an.FullName, true);
            }
        }

        foreach (var a in AppDomain.CurrentDomain.GetAssemblies().Where(a => ShouldLoad(a.FullName)))
        {
            loaded.TryAdd(a.FullName, true);
        }

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => IsNotNetFramework(a.FullName)))
            LoadReferencedAssembly(assembly);

        _areAssembliesLoaded = true;
    }
}

public abstract class Tester<T> : Tester where T : class
{
    private IReadOnlyList<object> _constructorParameters;

    private readonly IList<object> _overridenConstructorParameters = new List<object>();
    private IList<Type> _autoInjects;

    private readonly IDictionary<Type, Mock> _mocks = new Dictionary<Type, Mock>();

    /// <summary>
    /// Instance of the class that is being tested.
    /// </summary>
    protected T Instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            var parameters = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.Instance).OrderBy(x => x.GetParameters().Length).FirstOrDefault()?.GetParameters() ?? new ParameterInfo[0];

            _mocks.Clear();

            var interfaces = parameters.Where(x => x.ParameterType.IsInterface).Select(x => x.ParameterType).ToList();

            foreach (var i in interfaces)
            {
                var typeArgs = new[] { i };
                var mockType = typeof(Mock<>);
                var constructed = mockType.MakeGenericType(typeArgs);
                _mocks[i] = Activator.CreateInstance(constructed) as Mock;
            }

            var instancedParameters = new List<object>();
            foreach (var p in parameters)
            {
                if (_overridenConstructorParameters.Any(x => x.GetType() == p.ParameterType))
                {
                    instancedParameters.Add(_overridenConstructorParameters.Single(x => x.GetType() == p.ParameterType));
                }
                else if (p.ParameterType.IsAbstract)
                {
                    instancedParameters.Add(_mocks[p.ParameterType].Object);
                }
                else
                {
                    instancedParameters.Add(new SpecimenContext(Fixture).Resolve(p.ParameterType));
                }
            }

            if (instancedParameters.OfType<IServiceProvider>().Any())
            {
                if (_autoInjects.IsNullOrEmpty())
                {
                    //TODO Uncomment if some services appear to not be loaded
                    //LoadAllAssemblies();

                    var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                        .Where(x => x.IsClass && !x.IsAbstract && x.GetCustomAttribute<AutoInjectAttribute>() != null)
                        .Select(x =>
                        {
                            var attribute = x.GetCustomAttribute<AutoInjectAttribute>();
                            if (attribute!.Interface != null)
                                return attribute.Interface;

                            var interfaces = x.GetInterfaces();
                            if (interfaces.Length == 1) return interfaces.Single();
                            var withSameName = interfaces.SingleOrDefault(y => y.Name == $"I{x.Name}");
                            if (withSameName != null) return withSameName;

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

                            return searchResult.First().Interface;
                        });

                    _autoInjects = types.ToList();
                }

                foreach (var type in _autoInjects)
                {
                    AddToServiceProvider(type);
                }
            }

            _constructorParameters = instancedParameters.ToArray();
            _instance = Activator.CreateInstance(typeof(T), instancedParameters.ToArray()) as T;

            return _instance;
        }
    }
    private T _instance;

    /// <summary>
    /// Overrides an automatically-instanced constructor parameter with your own implementation.
    /// </summary>
    protected void OverrideConstructorParameter<TParameter>(TParameter value)
    {
        _overridenConstructorParameters.Add(value);
    }

    /// <summary>
    /// Invokes a method by name on <see cref="Instance"/>.
    /// </summary>
    protected void InvokeMethod(string methodName, params object[] parameters) => InvokeMethod(Instance, methodName, parameters);

    /// <summary>
    /// Invokes a method by name on <see cref="Instance"/>.
    /// </summary>
    protected void InvokeMethodAndIgnoreException<TException>(string methodName, params object[] parameters) where TException : Exception => InvokeMethodAndIgnoreException<T, TException>(Instance, methodName, parameters);

    /// <summary>
    /// Returns field value by name on <see cref="Instance"/>.
    /// </summary>
    protected TValue GetFieldValue<TValue>(string fieldName) => GetFieldValue<T, TValue>(Instance, fieldName);

    /// <summary>
    /// Returns property value by name on <see cref="Instance"/>.
    /// </summary>
    protected TValue GetPropertyValue<TValue>(string propertyName) => GetPropertyValue<T, TValue>(Instance, propertyName);

    /// <summary>
    /// Sets field value by name on <see cref="Instance"/>.
    /// </summary>
    protected void SetFieldValue<TValue>(string fieldName, TValue value) => SetFieldValue(Instance, fieldName, value);

    /// <summary>
    /// Sets property value by name on <see cref="Instance"/>.
    /// </summary>
    protected void SetPropertyValue<TValue>(string propertyName, TValue value) => SetPropertyValue(Instance, propertyName, value);

    /// <summary>
    /// Returns a mock of type <see cref="TMock"/> that was injected into <see cref="Instance"/> during instantiation. 
    /// </summary>
    protected Mock<TMock> GetMock<TMock>() where TMock : class
    {
        return (Mock<TMock>)_mocks[typeof(TMock)];
    }

    /// <summary>
    /// Adds service of type <see cref="TService"/> to <see cref="IServiceProvider"/>.
    /// </summary>
    protected void AddToServiceProvider<TService>() where TService : class
    {
        if (_mocks.ContainsKey(typeof(IServiceProvider)))
        {
            var mock = new Mock<TService>();
            if (!_mocks.ContainsKey(typeof(TService)))
                _mocks[typeof(TService)] = mock;
            GetMock<IServiceProvider>().Setup(x => x.GetService(typeof(TService))).Returns(mock.Object);
        }
    }

    /// <summary>
    /// Adds service of specified type to <see cref="IServiceProvider"/>.
    /// </summary>
    protected void AddToServiceProvider(Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        var mock = (Mock)Activator.CreateInstance(typeof(Mock<>).MakeGenericType(type));
        if (_mocks.ContainsKey(typeof(IServiceProvider)))
        {
            if (!_mocks.ContainsKey(type))
                _mocks[type] = mock;
            GetMock<IServiceProvider>().Setup(x => x.GetService(type)).Returns(mock.Object);
        }
    }
}