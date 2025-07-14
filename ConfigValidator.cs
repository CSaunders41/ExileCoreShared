using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ExileCoreShared;

/// <summary>
/// Utility for validating plugin configuration objects.
/// Checks for common issues like null values, invalid ranges, missing files, etc.
/// Reports errors via PluginLogger.
/// </summary>
public static class ConfigValidator
{
    /// <summary>
    /// Validates a settings object and returns a list of validation errors.
    /// </summary>
    /// <param name="settings">The settings object to validate</param>
    /// <param name="pluginName">Name of the plugin for logging</param>
    /// <returns>List of error messages (empty if valid)</returns>
    public static List<string> Validate(object settings, string pluginName)
    {
        var errors = new List<string>();

        if (settings == null)
        {
            errors.Add("Settings object is null");
            LogError(pluginName, "Settings object is null");
            return errors;
        }

        var properties = settings.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            var value = prop.GetValue(settings);

            // Check for null on non-nullable types
            if (value == null && prop.PropertyType.IsValueType && !prop.PropertyType.IsGenericType)
            {
                errors.Add($"Property '{prop.Name}' is null but should have a value");
            }

            // Check file paths
            if (prop.PropertyType == typeof(string) && value is string path && (path.Contains(".") || path.Contains("/")))
            {
                if (!File.Exists(path) && !Directory.Exists(path))
                {
                    errors.Add($"Invalid path in '{prop.Name}': {path} does not exist");
                }
            }

            // Check numeric ranges (example: assume attributes or naming conventions for min/max)
            if (prop.PropertyType == typeof(int) && value is int intVal)
            {
                if (prop.Name.Contains("Timeout") && intVal < 0)
                {
                    errors.Add($"Invalid timeout in '{prop.Name}': {intVal} (must be positive)");
                }
            }

            // Add more validation rules as needed (e.g., key bindings, enums)
        }

        // Log errors if any
        foreach (var error in errors)
        {
            LogError(pluginName, error);
        }

        return errors;
    }

    private static void LogError(string pluginName, string message)
    {
        // Use PluginLogger if available (via bridge in actual plugin)
        // For now, console fallback
        Console.WriteLine($"[ConfigValidator] [{pluginName}] Error: {message}");
    }
} 