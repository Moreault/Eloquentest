namespace ToolBX.Eloquentest.Extensions;

//TODO Move these to R4H.Extensions 3.0.0
internal static class MethodBaseExtensions
{
    public static bool HasParameters(this MethodBase methodInfo, params Func<ParameterInfo, bool>[] predicates) => HasParameters(methodInfo, (IEnumerable<Func<ParameterInfo, bool>>)predicates);

    public static bool HasParameters(this MethodBase methodInfo, IEnumerable<Func<ParameterInfo, bool>> predicates)
    {
        if (methodInfo is null) throw new ArgumentNullException(nameof(methodInfo));
        if (predicates is null) throw new ArgumentNullException(nameof(predicates));

        var list = predicates as IList<Func<ParameterInfo, bool>> ?? predicates.ToList();

        var parameters = methodInfo.GetParameters();
        if (parameters.Length != list.Count) return false;

        for (var i = 0; i < parameters.Length; i++)
        {
            if (!list[i](parameters[i])) return false;
        }
        return true;
    }

    public static bool HasParametersAssignableFrom(this MethodBase methodInfo, params Type?[] parameters) => methodInfo.HasParametersAssignableFrom((IEnumerable<Type?>)parameters);

    public static bool HasParametersAssignableFrom(this MethodBase methodInfo, IEnumerable<Type?> parameters)
    {
        if (methodInfo is null) throw new ArgumentNullException(nameof(methodInfo));
        if (parameters is null) throw new ArgumentNullException(nameof(parameters));

        var list = parameters as IList<Type?> ?? parameters.ToList();
        if (list.Count != methodInfo.GetParameters().Length) return false;

        for (var i = 0; i < list.Count; i++)
        {
            if (list[i] != null && !list[i]!.IsAssignableFrom(methodInfo.GetParameters()[i].ParameterType)) return false;
        }

        return true;
    }

    public static bool HasParametersAssignableTo(this MethodBase methodInfo, params Type?[] parameters) => methodInfo.HasParametersAssignableTo((IEnumerable<Type?>)parameters);

    public static bool HasParametersAssignableTo(this MethodBase methodInfo, IEnumerable<Type?> parameters)
    {
        if (methodInfo is null) throw new ArgumentNullException(nameof(methodInfo));
        if (parameters is null) throw new ArgumentNullException(nameof(parameters));

        var list = parameters as IList<Type?> ?? parameters.ToList();
        if (list.Count != methodInfo.GetParameters().Length) return false;

        for (var i = 0; i < list.Count; i++)
        {
            if (list[i] != null && !list[i]!.IsAssignableTo(methodInfo.GetParameters()[i].ParameterType)) return false;
        }

        return true;
    }
}