namespace ToolBX.Eloquentest.Extensions;

public static class ObjectExtensions
{
    /// <summary>
    /// Clones an object for unit testing purposes.
    /// </summary>
    public static T Clone<T>(this T source)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));
        try
        {
            var json = JsonSerializer.Serialize(source);
            return JsonSerializer.Deserialize<T>(json)!;
        }
        catch (Exception e)
        {
            throw new Exception($"Unable to clone object of type {source.GetType().GetHumanReadableName()}", e);
        }
    }
}