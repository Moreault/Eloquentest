namespace Eloquentest.Dummies.Tests
{
    [AutoCustomization]
    public sealed class ProperEnumerableCustomization : ListCustomizationBase
    {
        public override IEnumerable<Type> Types { get; } = [typeof(Garbage.ProperEnumerable<>)];
        protected override object Convert<T>(IEnumerable<T> source) => new Garbage.ProperEnumerable<T>(source);
    }

    [AutoCustomization]
    public sealed class BadEnumerableCustomization : ListCustomizationBase
    {
        public override IEnumerable<Type> Types { get; } = [typeof(Garbage.BadEnumerable<>)];
        protected override object Convert<T>(IEnumerable<T> source) => new Garbage.BadEnumerable<T>(source);
    }
}
