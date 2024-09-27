namespace Eloquentest.AutoFixture.Tests.Customizations;

[AutoCustomization]
public class ComplexCollectionSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ComplexCollection<>))
        {
            var elementType = type.GetGenericArguments()[0];
            var cellType = typeof(Cell<>).MakeGenericType(elementType);

            var listType = typeof(List<>).MakeGenericType(cellType);
            var list = context.Resolve(listType);

            return Activator.CreateInstance(type, list)!;
        }

        return new NoSpecimen();
    }
}