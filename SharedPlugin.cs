using System;
using System.Collections.Generic;
using ExileCore;
using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using ImGuiNET;

namespace ExileCoreShared;

/// <summary>
/// Settings for the SharedPlugin
/// </summary>
public class SharedPluginSettings : ISettings
{
    public ToggleNode Enable { get; set; } = new ToggleNode(true);
}

/// <summary>
/// Main plugin that registers shared utilities with PluginBridge.
/// Other plugins can access InputCoordinator, PluginLogger, and ConfigValidator through this.
/// </summary>
public class SharedPlugin : BaseSettingsPlugin<SharedPluginSettings>
{
    public override bool Initialise()
    {
        try
        {
            // Register InputCoordinator methods
            GameController.PluginBridge.SaveMethod("InputCoordinator.RequestControl", 
                new Func<string, int, bool>(InputCoordinator.RequestControl));
            GameController.PluginBridge.SaveMethod("InputCoordinator.ReleaseControl", 
                new Action<string>(InputCoordinator.ReleaseControl));
            GameController.PluginBridge.SaveMethod("InputCoordinator.IsOwner", 
                new Func<string, bool>(InputCoordinator.IsOwner));
            GameController.PluginBridge.SaveMethod("InputCoordinator.GetCurrentOwner", 
                new Func<string>(InputCoordinator.GetCurrentOwner));

            // Register PluginLogger methods
            GameController.PluginBridge.SaveMethod("PluginLogger.Log", 
                new Action<string, string, LogLevel>((pluginName, message, level) => PluginLogger.Log(pluginName, message, level)));
            GameController.PluginBridge.SaveMethod("PluginLogger.LogInfo", 
                new Action<string, string>(PluginLogger.LogInfo));
            GameController.PluginBridge.SaveMethod("PluginLogger.LogWarning", 
                new Action<string, string>(PluginLogger.LogWarning));
            GameController.PluginBridge.SaveMethod("PluginLogger.LogError", 
                new Action<string, string>(PluginLogger.LogError));

            // Register ConfigValidator methods
            GameController.PluginBridge.SaveMethod("ConfigValidator.Validate", 
                new Func<object, string, List<string>>(ConfigValidator.Validate));

            LogMessage("SharedPlugin initialized successfully - all utilities registered with PluginBridge", 5);
            return true;
        }
        catch (Exception ex)
        {
            LogError($"Failed to initialize SharedPlugin: {ex.Message}");
            return false;
        }
    }

    public override void Render()
    {
        // Nothing to render - this is a utility plugin
    }

    public override void DrawSettings()
    {
        // Basic info about registered utilities
        ImGui.Text("Shared Utilities Status:");
        ImGui.Text($"InputCoordinator: Active (Current Owner: {InputCoordinator.GetCurrentOwner() ?? "None"})");
        ImGui.Text($"PluginLogger: Active (Log file: {System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ExileCorePlugins", "plugin_log.txt")})");
        ImGui.Text("ConfigValidator: Active");
    }
} 