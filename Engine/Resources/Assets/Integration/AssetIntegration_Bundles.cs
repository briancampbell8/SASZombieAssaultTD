//============================================================================
//File:        AssetIntegration_Bundles.cs
//Path:        E:\BDC\Projects\SASZombieAssaultTD\Engine\Resources\Assets\Integration\AssetIntegration_Bundles.cs
//Program:     AssetIntegration (Bundles)
//Subsystem:   Assets
//Author:      BDC
//Created:     2026-05-29
//
//Purpose:
//    Provides the integration layer between the AssetManager subsystem and the
//    AssetBundle subsystem. Handles bundle loading, unloading, metadata access,
//    integrity validation, and safe bundle switching for the unified Asset
//    System. Acts as the façade that coordinates bundle lifecycle operations
//    across AssetManager, AssetBundle, and legacy RS components.
//
//Responsibilities:
//    • Load and unload bundles through AssetBundle subsystem.
//    • Provide safe bundle switching with lifecycle guarantees.
//    • Expose metadata accessors for active bundles.
//    • Perform integrity validation and diagnostics.
//    • Coordinate bundle state with AssetManager and AssetPipeline.
//    • Provide unified error handling and logging for bundle operations.
//
//Architecture:
//    • Facade pattern simplifying access to bundle subsystems.
//    • Thread‑safe initialization and lifecycle management.
//    • Event‑driven notifications for bundle load/unload events.
//    • Configurable runtime behavior via AssetManager_Config.
//    • Integrated with AssetBundle_Core, Loader, Metadata, and Validation.
//
//Integration Points:
//    • AssetManager for unified asset lifecycle management.
//    • AssetBundle subsystem for packaged asset distribution.
//    • AssetPipeline subsystem for preprocessing and optimization.
//    • RS subsystem (legacy) during migration.
//    • UI, Rendering, Audio, and Gameplay systems for unified asset access.
//
//Performance Characteristics:
//    • Minimal overhead through subsystem delegation.
//    • Lazy initialization where appropriate.
//    • Efficient resource cleanup and bundle disposal.
//    • Thread‑safe operations with minimal contention.
//    • Background processing to avoid UI thread blocking.
//    • Intelligent caching with hash‑based change detection.
//
//Usage Example:
//    //Initialize AssetSystem with bundle
//    AssetSystem.Initialize("Assets", "game_assets.bundle");
//
//    //Load assets from bundle
//    var texture = await AssetSystem.LoadAssetAsync<Texture2D>("player.png");
//    var audio   = await AssetSystem.LoadAssetAsync<AudioClip>("explosion.wav");
//
//    //Switch bundles safely
//    await AssetSystem.SwitchBundleAsync("ui_assets.bundle");
//
//    //Get bundle metadata
//    var metadata = AssetSystem.GetBundleMetadata();
//    Debug.WriteLine($"Bundle: {metadata.Name}, Assets: {metadata.AssetCount}");
//
//    //Validate bundle integrity
//    var validation = await AssetSystem.ValidateBundleAsync();
//    if (!validation.IsValid)
//        Debug.WriteLine($"Bundle validation failed: {string.Join(", ", validation.Errors)}");
//============================================================================


//

using System;
using System.Linq;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Resources
{
    public static partial class AssetSystem
    {
        //---------------------------------------------------------------------
        //INTERNAL STATE
        //---------------------------------------------------------------------

        private static AssetBundle _currentBundle;
        private static readonly object _bundleLock = new object();

        //---------------------------------------------------------------------
        //PUBLIC BUNDLE API
        //---------------------------------------------------------------------

        public static void LoadBundle(string bundlePath, string password = null)
        {
            EnsureInitialized();
            LoadBundleInternal(bundlePath, password);
        }

        public static void UnloadBundle()
        {
            lock (_bundleLock)
            {
                _currentBundle?.Dispose();
                _currentBundle = null;

                System.Diagnostics.Debug.WriteLine("Info", "AssetSystem: Unloaded bundle");
            }
        }

        //---------------------------------------------------------------------
        //INTERNAL BUNDLE LOADER
        //---------------------------------------------------------------------

        private static void LoadBundleInternal(string bundlePath, string password = null)
        {
            lock (_bundleLock)
            {
                try
                {
                    _currentBundle?.Dispose();

                    System.Diagnostics.Debug.WriteLine("Info", $"AssetSystem: Loading bundle '{bundlePath}'");

                    _currentBundle = AssetBundle.LoadFromFile(bundlePath);

                    System.Diagnostics.Debug.WriteLine("Info",
                        $"AssetSystem: Bundle loaded ({_currentBundle.Entries.Count()} assets)");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error", $"AssetSystem: Failed to load bundle '{bundlePath}': {ex.Message}");
                    throw;
                }
            }
        }
    }
}
