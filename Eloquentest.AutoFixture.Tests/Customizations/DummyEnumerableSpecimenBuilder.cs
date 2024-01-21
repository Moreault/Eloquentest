namespace Eloquentest.AutoFixture.Tests.Customizations;

[AutoCustomization]
public sealed class DummyEnumerableSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(EnsureTester.DummyEnumerable<>))
        {
            var elementType = type.GetGenericArguments()[0];
            var stackType = typeof(List<>).MakeGenericType(elementType);
            var list = context.Resolve(stackType);

            return Activator.CreateInstance(typeof(EnsureTester.DummyEnumerable<>).MakeGenericType(elementType), list)!;
        }

        return new NoSpecimen();
    }
}