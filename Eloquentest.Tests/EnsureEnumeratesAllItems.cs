using AutoFixture.Kernel;
using ToolBX.Eloquentest.Customizations;

namespace Eloquentest.Tests;

[TestClass]
public sealed class EnsureEnumeratesAllItemsTests : Tester
{
    public sealed class DummyEnumerable<T> : IEnumerable<T>
    {
        private readonly List<T> _items = new();

        public DummyEnumerable() { }

        public DummyEnumerable(IEnumerable<T> items) => _items.AddRange(items);

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [AutoCustomization]
    public sealed class StackSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is Type type && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DummyEnumerable<>))
            {
                var elementType = type.GetGenericArguments()[0];
                var stackType = typeof(List<>).MakeGenericType(elementType);
                var list = context.Resolve(stackType);

                return Activator.CreateInstance(typeof(DummyEnumerable<>).MakeGenericType(elementType), list)!;
            }

            return new NoSpecimen();
        }
    }

    [TestMethod]
    public void Ensure_EnumeratesAllItems() => Ensure.EnumeratesAllItems<DummyEnumerable<int>, int>();
}