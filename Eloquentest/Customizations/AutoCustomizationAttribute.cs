namespace ToolBX.Eloquentest.Customizations;

/// <summary>
/// Will automatically apply customizations project-wide when added to ICustomization or ISpecimenBuilder.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AutoCustomizationAttribute : Attribute
{
    public enum CustomizationOrder
    {
        Early,
        Unimportant,
        Late
    }

    public CustomizationOrder Order { get; set; } = CustomizationOrder.Unimportant;
}