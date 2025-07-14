using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ExileCoreShared;

/// <summary>
/// Centralized coordinator for managing input control across multiple plugins.
/// Plugins request temporary ownership before performing input actions (e.g., mouse moves, key presses).
/// This prevents conflicts like interrupted movement or failed casts.
/// </summary>
public static class InputCoordinator
{
    private static readonly ConcurrentDictionary<string, DateTime> _ownership = new ConcurrentDictionary<string, DateTime>();
    private static readonly object _lock = new object();
    private const int DefaultOwnershipMs = 200; // Default lock time to prevent rapid conflicts

    /// <summary>
    /// Requests temporary control for a plugin. Returns true if granted.
    /// </summary>
    /// <param name="pluginName">Unique name of the requesting plugin (e.g., "ReAgent")</param>
    /// <param name="durationMs">How long to hold control (default: 200ms)</param>
    /// <returns>True if control was granted, false if another plugin holds it</returns>
    public static bool RequestControl(string pluginName, int durationMs = DefaultOwnershipMs)
    {
        lock (_lock)
        {
            // Clean up expired ownerships
            foreach (var entry in _ownership.ToArray())
            {
                if (entry.Value < DateTime.Now)
                {
                    _ownership.TryRemove(entry.Key, out _);
                }
            }

            // Check if slot is free
            if (_ownership.IsEmpty)
            {
                _ownership[pluginName] = DateTime.Now.AddMilliseconds(durationMs);
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Checks if a plugin currently has control.
    /// </summary>
    /// <param name="pluginName">Name of the plugin to check</param>
    /// <returns>True if the plugin has active control</returns>
    public static bool IsOwner(string pluginName)
    {
        return _ownership.TryGetValue(pluginName, out var endTime) && endTime > DateTime.Now;
    }

    /// <summary>
    /// Releases control early for a plugin.
    /// </summary>
    /// <param name="pluginName">Name of the plugin releasing control</param>
    public static void ReleaseControl(string pluginName)
    {
        _ownership.TryRemove(pluginName, out _);
    }

    /// <summary>
    /// Gets the current owner (or null if free).
    /// </summary>
    public static string GetCurrentOwner()
    {
        lock (_lock)
        {
            foreach (var entry in _ownership)
            {
                if (entry.Value > DateTime.Now)
                {
                    return entry.Key;
                }
            }
            return null;
        }
    }
} 