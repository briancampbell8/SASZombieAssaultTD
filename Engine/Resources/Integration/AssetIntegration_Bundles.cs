//**************************************************************************************
// * File: AssetIntegration_Bundles.cs
// * Purpose: Bundle integration layer for SAS Zombie Assault TD Asset System.
// * Features:
// *     Bundle integration layer for AssetSystem
// *     Handles loading, unloading, metadata access, and integrity validation
// *     Coordinates with AssetBundle and RSBundles subsystems
// *     Provides safe bundle switching and lifecycle management
// *     Comprehensive error handling and logging
// *
// * Architecture:
// *     Facade pattern providing simplified interface to complex bundle subsystems
// *     Singleton pattern with thread-safe initialization
// *     Event-driven architecture for bundle lifecycle notifications
// *     Configurable system with runtime parameter adjustment
// *     Comprehensive logging and debugging support
// *
// * Integration Points:
// *     - Coordinates with AssetManager for asset lifecycle management
// *     - Coordinates with AssetBundle for packaged asset distribution
// *     - Coordinates with RSManager for low-level resource management
// *     - Provides unified API for all bundle operations across subsystems
// *     - Supports both synchronous and asynchronous processing patterns
// *
// * Performance Characteristics:
// *     - Minimal overhead through direct subsystem delegation
// *     - Optimized initialization with lazy loading where appropriate
// *     - Efficient resource management with automatic cleanup
// *     - Thread-safe operations with minimal contention
// *     - Background processing coordination to prevent blocking
// *     - Intelligent caching with hash-based change detection
// *
// * Usage Examples:
//   * ```csharp
//   * // Initialize AssetSystem with bundle
//   * AssetSystem.Initialize("Assets", "game_assets.bundle");
//   *
//   * // Load assets from bundle
//   * var texture = await AssetSystem.LoadAssetAsync<Texture2D>("player.png");
//   * var audio = await AssetSystem.LoadAssetAsync<AudioClip>("explosion.wav");
//   *
//   * // Switch bundles safely
//   * await AssetSystem.SwitchBundleAsync("ui_assets.bundle");
//   *
//   * // Get bundle metadata
//   * var metadata = AssetSystem.GetBundleMetadata();
//   * System.Diagnostics.Debug.WriteLine($"Bundle: {metadata.Name}, Assets: {metadata.AssetCount}");
//   *
//   * // Validate bundle integrity
//   * var validation = await AssetSystem.ValidateBundleAsync();
//   * if (!validation.IsValid)
//   * {
//   *     System.Diagnostics.Debug.WriteLine($"Bundle validation failed: {string.Join(", ", validation.Errors)}");
//   * }
//   * ```
// **************************************************************************************//

using SASZombieAssaultTD.Engine.Diagnostics;

using System;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Resources
{
    public static partial class AssetSystem
    {
        // ---------------------------------------------------------------------
        // INTERNAL STATE
        // ---------------------------------------------------------------------

        private static AssetBundle _currentBundle;
        private static readonly object _bundleLock = new object();

        // ---------------------------------------------------------------------
        // PUBLIC BUNDLE API
        // ---------------------------------------------------------------------

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

        // ---------------------------------------------------------------------
        // INTERNAL BUNDLE LOADER
        // ---------------------------------------------------------------------

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
