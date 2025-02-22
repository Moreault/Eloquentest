﻿namespace ToolBX.Eloquentest;

public sealed record InstanceResult<T>
{
    public required T Value { get; init; }

    /// <summary>
    /// Mocks the <see cref="Value"/> object was instanced with.
    /// </summary>
    public required IReadOnlyDictionary<Type, Mock> Mocks
    {
        get => _mocks;
        init => _mocks = value?.ToImmutableDictionary() ?? throw new ArgumentNullException(nameof(value));
    }
    private readonly IReadOnlyDictionary<Type, Mock> _mocks = null!;

    /// <summary>
    /// Parameters the <see cref="Value"/> object was instanced with.
    /// </summary>
    public IReadOnlyList<object> InstancedParameters
    {
        get => _instancedParameters;
        init => _instancedParameters = value?.ToImmutableList() ?? throw new ArgumentNullException(nameof(value));
    }
    private readonly IReadOnlyList<object> _instancedParameters = null!;
}

internal static class InstanceProvider
{
    internal static InstanceResult<T> Create<T>(Func<Type, object> createMethod, IEnumerable<object>? constructorParameterOverrides = null, IReadOnlyDictionary<Type, Mock>? overridenMocks = null!) where T : class
    {
        constructorParameterOverrides ??= Array.Empty<object>();
        overridenMocks ??= ImmutableDictionary<Type, Mock>.Empty;
        var parameters = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.Instance).MinBy(x => x.GetParameters().Length)?.GetParameters() ?? [];

        var interfaces = parameters.Where(x => x.ParameterType.IsInterface).Select(x => x.ParameterType).ToList();

        var mocks = overridenMocks.ToDictionary(x => x.Key, x => x.Value);

        foreach (var type in interfaces.Where(x => !mocks.ContainsKey(x)))
            mocks[type] = MockUtils.CreateFrom(type);

        var instancedParameters = new List<object>();
        foreach (var parameterInfo in parameters)
        {
            var overriden = constructorParameterOverrides.SingleOrDefault(x => x.GetType().IsAssignableTo(parameterInfo.ParameterType));
            if (overriden is not null)
            {
                instancedParameters.Add(overriden);
            }
            else if (parameterInfo.ParameterType.IsAbstract)
            {
                instancedParameters.Add(mocks[parameterInfo.ParameterType].Object);
            }
            else
            {
                instancedParameters.Add(createMethod(parameterInfo.ParameterType));
            }
        }

        var instance = (Activator.CreateInstance(typeof(T), instancedParameters.ToArray()) as T)!;

        return new InstanceResult<T>
        {
            Value = instance,
            Mocks = mocks,
            InstancedParameters = instancedParameters
        };
    }
}