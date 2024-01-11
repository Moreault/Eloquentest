namespace ToolBX.Eloquentest.Customizations.SpecimenBuilders;

[AutoCustomization(Order = AutoCustomizationAttribute.CustomizationOrder.Early)]
public sealed class StackSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Stack<>))
        {
            var elementType = type.GetGenericArguments()[0];
            var stackType = typeof(List<>).MakeGenericType(elementType);
            var list = context.Resolve(stackType);

            return Activator.CreateInstance(typeof(Stack<>).MakeGenericType(elementType), list)!;
        }

        return new NoSpecimen();
    }
}