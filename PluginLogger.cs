using System;
using System.IO;
using System.Text;

namespace ExileCoreShared;

/// <summary>
/// Centralized logger for plugins. Supports levels (Info/Warning/Error) and writes to console/file.
/// Plugins use this via PluginBridge for loose coupling.
/// </summary>
public static class PluginLogger
{
    private static readonly string LogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ExileCorePlugins", "plugin_log.txt");
    private static readonly object _lock = new object();

    static PluginLogger()
    {
        // Ensure log directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));
    }

    /// <summary>
    /// Logs a message at the specified level.
    /// </summary>
    /// <param name="pluginName">Name of the plugin logging (e.g., "Follower")</param>
    /// <param name="message">The log message</param>
    /// <param name="level">Log level (default: Info)</param>
    public static void Log(string pluginName, string message, LogLevel level = LogLevel.Info)
    {
        lock (_lock)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var logEntry = $"[{timestamp}] [{level}] [{pluginName}] {message}";

            // Write to console
            Console.WriteLine(logEntry);

            // Append to file
            File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
        }
    }

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    public static void LogInfo(string pluginName, string message) => Log(pluginName, message, LogLevel.Info);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    public static void LogWarning(string pluginName, string message) => Log(pluginName, message, LogLevel.Warning);

    /// <summary>
    /// Logs an error message.
    /// </summary>
    public static void LogError(string pluginName, string message) => Log(pluginName, message, LogLevel.Error);

    /// <summary>
    /// Clears the log file.
    /// </summary>
    public static void ClearLog()
    {
        lock (_lock)
        {
            if (File.Exists(LogFilePath))
            {
                File.Delete(LogFilePath);
            }
        }
    }
}

/// <summary>
/// Log levels for PluginLogger.
/// </summary>
public enum LogLevel
{
    Info,
    Warning,
    Error
} 