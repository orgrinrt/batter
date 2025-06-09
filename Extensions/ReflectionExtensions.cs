using System.Reflection;
using SafeWarLogPatch.Utils;

namespace SafeWarLogPatch.Extensions;

public static class ReflectionExtensions
{
    public static void LogObjectPropertiesAndFields(this object obj, string objectName)
    {
        if (obj == null)
        {
            BatterLog.Warn($"{objectName} is null.");
            return;
        }

        var type = obj.GetType();
        BatterLog.Info($"[Reflection] {objectName} Properties and Fields:");

        // Log properties
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
            try
            {
                var value = property.GetValue(obj);
                BatterLog.Info($"  Property: {property.Name}, Value: {value?.ToString() ?? "null"}");
            }
            catch (Exception ex)
            {
                BatterLog.Error($"  Failed to read property {property.Name}: {ex.Message}");
            }

        // Log fields
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
            try
            {
                var value = field.GetValue(obj);
                BatterLog.Info($"  Field: {field.Name}, Value: {value?.ToString() ?? "null"}");
            }
            catch (Exception ex)
            {
                BatterLog.Error($"  Failed to read field {field.Name}: {ex.Message}");
            }
    }
}