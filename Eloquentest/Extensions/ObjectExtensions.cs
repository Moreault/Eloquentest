﻿namespace ToolBX.Eloquentest.Extensions;

public static class ObjectExtensions
{
    /// <summary>
    /// Clones an object for unit testing purposes.
    /// </summary>
    public static T Clone<T>(this T source, JsonSerializerOptions? options = null)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        try
        {
            var json = JsonSerializer.Serialize(source, options);
            return JsonSerializer.Deserialize<T>(json, options) ?? throw new Exception($"Couldn't deserialize {source}");
        }
        catch
        {
            return source.ReflectionClone();
        }
    }

    private static T ReflectionClone<T>(this T source)
    {
        try
        {
            var fields = typeof(T).GetAllFields(x => x.IsInstance());
            var properties = typeof(T).GetAllProperties().Where(x => x.IsInstance() && x.CanWrite);

            var clone = Activator.CreateInstance<T>()!;

            foreach (var field in fields)
            {
                var value = field.GetValue(source);
                field.SetValue(clone, value);
            }

            foreach (var property in properties)
            {
                var indexParameters = property.GetIndexParameters();
                if (indexParameters.Length > 0)
                    continue;

                var value = property.GetValue(source);
                property.SetValue(clone, value);
            }

            return clone;

        }
        catch (Exception e)
        {
            throw new Exception($"Unable to clone object of type {source!.GetType().GetHumanReadableName()}", e);
        }
    }
}