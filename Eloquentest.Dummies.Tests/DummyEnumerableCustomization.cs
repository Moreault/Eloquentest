namespace Eloquentest.Dummies.Tests
{
    [AutoCustomization]
    public sealed class DummyEnumerableCustomization : ListCustomizationBase
    {
        public override IEnumerable<Type> Types { get; } = [typeof(EnsureTester.DummyEnumerable<>)];
        protected override object Convert<T>(IEnumerable<T> source) => new EnsureTester.DummyEnumerable<T>(source);
    }
}
