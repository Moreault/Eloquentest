namespace ToolBX.Eloquentest;

/// <summary>
/// Provides a Fixture, Autocustomization support, and automatically fills the Instance's properties with values.
/// Use when you want your Instance's properties to have values without setting them manually yourself.
/// </summary>
public abstract class AutoFillTester<T> : Tester<T> where T : class
{
    protected internal override void AfterCreateInstance(T instance)
    {
        base.AfterCreateInstance(instance);
        foreach (var property in instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty))
        {
            try
            {
                property.SetValue(instance, Fixture.Create(property.PropertyType));
            }
            catch
            {
                //Ignored : We don't want everything to blow up because some properties can't be automatically generated
            }
        }
    }
}