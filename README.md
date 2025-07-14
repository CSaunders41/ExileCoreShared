# ExileCoreShared

A shared library for Path of Exile ExileCore plugins that provides common utilities like input coordination, logging, and configuration validation.

## Installation

1. Place this folder in your ExileApi `Plugins/Source/` directory
2. Ensure the following DLLs are in your ExileApi root folder:
   - `ExileCore.dll`
   - `ImGui.NET.dll`
   - `System.Numerics.Vectors.dll`

## Expected Folder Structure

```
ExileApi-Compiled-3.26.0.0.4/
├── ExileCore.dll
├── ImGui.NET.dll
├── System.Numerics.Vectors.dll
└── Plugins/
    └── Source/
        └── ExileCoreShared/
            ├── ConfigValidator.cs
            ├── ExileCoreShared.csproj
            ├── InputCoordinator.cs
            ├── PluginLogger.cs
            └── SharedPlugin.cs
```

## Troubleshooting

### Build Errors: "ExileCore could not be found"

1. Verify `ExileCore.dll` exists in the ExileApi root folder (3 levels up from this project)
2. Check that the ExileCore.dll is not corrupted
3. Ensure you're using the correct version of ExileApi

### Alternative Setup

If the automatic path detection fails, you can manually edit `ExileCoreShared.csproj` and adjust the `HintPath` values:

```xml
<Reference Include="ExileCore">
  <HintPath>path/to/your/ExileCore.dll</HintPath>
  <Private>false</Private>
</Reference>
```

## Components

- **InputCoordinator**: Prevents input conflicts between plugins
- **PluginLogger**: Centralized logging with plugin identification
- **ConfigValidator**: Validates plugin configuration settings
- **SharedPlugin**: Main plugin that registers utilities with PluginBridge

## Usage

Other plugins can access these utilities through the PluginBridge system. 