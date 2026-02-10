# Asset Pipeline Initialization

## Purpose
Wire the existing asset subsystems into a single initialization flow.

## Components
- AssetDiscovery: Scans asset directories.
- TextureLoader: Loads texture binaries.
- DataLoader: Loads JSON data.
- AssetRegistry: Stores loaded assets.
- AssetBundle: Convenience container for grouped assets.
- AssetInitializer: Orchestrates the initialization.

## Flow
1. AssetInitializer.Init() is called during engine startup.
2. Discovery finds assets under /Assets.
3. TextureLoader and DataLoader load resources.
4. AssetRegistry is populated.
5. Scenes query assets via AssetRegistry.

## Notes
- Initialization should be done before the window is shown.
- Failures should be logged via DebugLogger.