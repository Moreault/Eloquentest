namespace Eloquentest.AutoFixture.Tests.Customizations;

[AutoCustomization]
public sealed class DummyEnumerableSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Garbage.ProperEnumerable<>))
        {
            var elementType = type.GetGenericArguments()[0];
            var stackType = typeof(List<>).MakeGenericType(elementType);
            var list = context.Resolve(stackType);

            return Activator.CreateInstance(typeof(Garbage.ProperEnumerable<>).MakeGenericType(elementType), list)!;
        }

        return new NoSpecimen();
    }
}