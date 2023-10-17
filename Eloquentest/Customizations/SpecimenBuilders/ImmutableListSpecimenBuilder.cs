namespace ToolBX.Eloquentest.Customizations.SpecimenBuilders;

[AutoCustomization(Order = AutoCustomizationAttribute.CustomizationOrder.Early)]
public class ImmutableListSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        var type = request as Type ?? request.GetType();

        if (type.IsGenericType)
        {
            var genericType = type.GenericTypeArguments.First();
            var immutableListOf = typeof(ImmutableList<>).MakeGenericType(genericType);
            if (type == immutableListOf || type == typeof(IImmutableList<>).MakeGenericType(genericType))
            {
                var method = typeof(ImmutableList)
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Single(x => x is { Name: "Create", IsGenericMethod: true } && x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType.IsArray)
                    .MakeGenericMethod(genericType);

                var array = context.Resolve(genericType.MakeArrayType());
                var parameters = new[] { array };

                return method.Invoke(null, parameters)!;
            }
        }

        return new NoSpecimen();
    }
}