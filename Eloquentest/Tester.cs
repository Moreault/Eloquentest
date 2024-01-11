namespace ToolBX.Eloquentest;

/// <summary>
/// Basic tester which provides a Fixture and AutoCustomization support.
/// Use for static, extension methods and constructors.
/// </summary>
public abstract class Tester
{
    public TestContext TestContext { get; set; } = null!;

    protected JsonSerializerOptions JsonSerializerOptions => _jsonSerializerOptions.Value;
    private Lazy<JsonSerializerOptions> _jsonSerializerOptions = null!;

    private static readonly Dictionary<Type, bool> InitializedClasses = new();

    private bool IsClassInitialized
    {
        get
        {
            InitializedClasses.TryGetValue(GetType(), out var result);
            return result;
        }
        set => InitializedClasses[GetType()] = value;
    }

    protected IFixture Fixture { get; } = FixtureProvider.Create();

    [ClassInitialize]
    public void ClassInitializeOnBaseClass()
    {
        IsClassInitialized = true;
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
        if (!IsClassInitialized)
            ClassInitializeOnBaseClass();
        _jsonSerializerOptions = new(() => new JsonSerializerOptions());
        InitializeTest();
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

    protected object? InvokeMethod<T>(T instance, string methodName, params object[] parameters)
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));
        if (string.IsNullOrWhiteSpace(methodName)) throw new ArgumentNullException(nameof(methodName));

        var methodInfo = instance.GetType().GetSingleMethod(methodName);
        return methodInfo.Invoke(instance, parameters);
    }

    protected object? InvokeMethodAndIgnoreException<TInstance, TException>(TInstance instance, string methodName,
        params object[] parameters) where TException : Exception
    {
        try
        {
            return InvokeMethod(instance, methodName, parameters);
        }
        catch (TargetInvocationException e)
        {
            if (e.InnerException is not TException)
                throw;
        }

        return null;
    }

    protected TValue? GetFieldValue<TInstance, TValue>(TInstance instance, string fieldName)
    {
        var fieldInfo = typeof(TInstance).GetSingleField(fieldName);
        return (TValue?)fieldInfo.GetValue(instance);
    }

    protected void SetFieldValue<TInstance, TValue>(TInstance instance, string fieldName, TValue value)
    {
        var fieldInfo = typeof(TInstance).GetSingleField(fieldName);
        fieldInfo.SetValue(instance, value);
    }

    protected TValue? GetPropertyValue<TInstance, TValue>(TInstance instance, string propertyName)
    {
        var propertyInfo = typeof(TInstance).GetSingleProperty(propertyName);
        return (TValue?)propertyInfo.GetValue(instance);
    }

    protected void SetPropertyValue<TInstance, TValue>(TInstance instance, string propertyName, TValue value)
    {
        var propertyInfo = typeof(TInstance).GetSingleProperty(propertyName);
        propertyInfo = propertyInfo.DeclaringType!.GetProperty(propertyName);
        propertyInfo!.SetValue(instance, value,
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, null, null);
    }
}

/// <summary>
/// Provides a Fixture, AutoCustomization support and an Instance property.
/// Use for complex entities and services.
/// </summary>
public abstract class Tester<T> : Tester where T : class
{
    private List<Type> _autoInjects = new();

    private readonly IDictionary<Type, Mock> _mocks = new Dictionary<Type, Mock>();

    /// <summary>
    /// Parameters that were used to instantiate <see cref="Instance"/>.
    /// </summary>
    protected IReadOnlyList<object> ConstructorParameters => _constructorParameters;
    private readonly List<object> _constructorParameters = new();

    private readonly List<object> _overridenConstructorParameters = new();

    /// <summary>
    /// Instance of the class that is being tested.
    /// </summary>
    protected T Instance => _instance.Value;
    private Lazy<T> _instance = null!;

    protected Tester()
    {
        ResetInstance();
    }

    private void ResetInstance()
    {
        _instance = new Lazy<T>(() =>
        {
            var instance = InstanceProvider.Create<T>(Fixture, _overridenConstructorParameters, (IReadOnlyDictionary<Type, Mock>)_mocks);

            if (instance.InstancedParameters.OfType<IServiceProvider>().Any())
            {
                if (!_autoInjects.Any())
                {
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

                            var regex = new Regex(
                                @"(?<=[A-Z])(?=[A-Z][a-z]) | (?<=[^A-Z])(?=[A-Z]) | (?<=[A-Za-z])(?=[^A-Za-z])",
                                RegexOptions.IgnorePatternWhitespace);

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

                            if (!searchResult.Any())
                                throw new Exception($"Can't inject service automatically : {x.Name} implements {interfaces.Length} interfaces but none of them are close to similar in name.");
                            searchResult = searchResult.OrderBy(y => y.IsInherited)
                                .ThenByDescending(y => y.Similarities).ToList();
                            if (searchResult.Count > 1 &&
                                searchResult[0].Similarities == searchResult[1].Similarities &&
                                searchResult[0].IsInherited == searchResult[1].IsInherited)
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

            foreach (var mock in instance.Mocks)
            {
                if (_mocks.ContainsKey(mock.Key)) continue;
                _mocks[mock.Key] = mock.Value;
            }

            AfterCreateInstance(instance.Value);
            return instance.Value;
        });
    }

    protected internal virtual void AfterCreateInstance(T instance)
    {

    }

    private void AddMock(Type type)
    {
        var typeArgs = new[] { type };
        var mockType = typeof(Mock<>);
        var constructed = mockType.MakeGenericType(typeArgs);
        _mocks[type] = (Activator.CreateInstance(constructed) as Mock)!;
    }

    protected override void CleanupTest()
    {
        base.CleanupTest();
        _mocks.Clear();
        _constructorParameters.Clear();
        _overridenConstructorParameters.Clear();
        ResetInstance();
    }

    /// <summary>
    /// Overrides <see cref="Instance"/> constructor parameters. Call before first accessing the <see cref="Instance"/> property.
    /// </summary>
    protected void ConstructWith(params object[] parameters)
    {
        if (parameters == null) throw new ArgumentNullException(nameof(parameters));
        if (_instance.IsValueCreated) throw new InvalidOperationException($"Can't override constructor parameters : the {nameof(ConstructWith)} method must be called before accessing the {nameof(Instance)} property.");
        //TODO Ensure that the parameters passed correpond to those of T's constructor
        _overridenConstructorParameters.AddRange(parameters);
    }

    /// <summary>
    /// Invokes a method by name on <see cref="Instance"/>.
    /// </summary>
    protected object? InvokeMethod(string methodName, params object[] parameters) => InvokeMethod(Instance, methodName, parameters);

    /// <summary>
    /// Invokes a method by name on <see cref="Instance"/>.
    /// </summary>
    protected object? InvokeMethodAndIgnoreException<TException>(string methodName, params object[] parameters) where TException : Exception => InvokeMethodAndIgnoreException<T, TException>(Instance, methodName, parameters);

    /// <summary>
    /// Returns field value by name on <see cref="Instance"/>.
    /// </summary>
    protected TValue? GetFieldValue<TValue>(string fieldName) => GetFieldValue<T, TValue>(Instance, fieldName);

    /// <summary>
    /// Returns property value by name on <see cref="Instance"/>.
    /// </summary>
    protected TValue? GetPropertyValue<TValue>(string propertyName) => GetPropertyValue<T, TValue>(Instance, propertyName);

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
        if (!_mocks.ContainsKey(typeof(TMock)))
            AddMock(typeof(TMock));
        return (Mock<TMock>)_mocks[typeof(TMock)];
    }

    /// <summary>
    /// Sets up an options object (typically information found in an appsettings.json file.)
    /// </summary>
    protected TOptions SetupOptions<TOptions>(TOptions? options = null) where TOptions : class
    {
        options ??= Fixture.Create<TOptions>();
        GetMock<IOptions<TOptions>>().Setup(x => x.Value).Returns(options);
        return options;
    }

    /// <summary>
    /// Adds service of type <see cref="TService"/> to <see cref="IServiceProvider"/>.
    /// </summary>
    protected void AddToServiceProvider<TService>() where TService : class => AddToServiceProvider(typeof(TService));

    /// <summary>
    /// Adds service of specified type to <see cref="IServiceProvider"/>.
    /// </summary>
    protected void AddToServiceProvider(Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (!_mocks.ContainsKey(typeof(IServiceProvider)))
            AddMock(typeof(IServiceProvider));
        if (!_mocks.ContainsKey(type))
            AddMock(type);
        GetMock<IServiceProvider>().Setup(x => x.GetService(type)).Returns(_mocks[type].Object);
    }

    /// <summary>
    /// Adds a service of type <see cref="TService"/> to <see cref="IServiceProvider"/> with a specific (non-mocked) instance.
    /// </summary>
    protected void AddToServiceProvider<TService>(object instance) where TService : class => AddToServiceProvider(typeof(TService), instance);

    /// <summary>
    /// Adds a service of the specified type to <see cref="IServiceProvider"/> with a specific (non-mocked) instance.
    /// </summary>
    protected void AddToServiceProvider(Type type, object instance)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (instance == null) throw new ArgumentNullException(nameof(instance));
        GetMock<IServiceProvider>().Setup(x => x.GetService(type)).Returns(instance);
    }
}