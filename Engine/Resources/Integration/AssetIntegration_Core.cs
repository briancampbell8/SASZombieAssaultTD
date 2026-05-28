/* 
// PROGRAM: AssetIntegration_Core
// FILE PATH: Engine/Resources/AssetIntegration_Core.cs
// PURPOSE:
//   Core orchestration layer for the SAS Zombie Assault TD Asset System.
//   Provides unified public API, initialization, shutdown, asset loading,
//   retrieval, memory statistics, and system validation.
//
// ARCHITECTURAL ROLE:
//   - Facade entry point for all asset operations
//   - Coordinates AssetManager, AssetPipeline, and AssetBundle subsystems
//   - Ensures deterministic initialization and shutdown
//   - Provides thread-safe access to all asset operations
//
// DIAGNOSTICS:
//   - Logs initialization, shutdown, loading, validation, and errors
//   - Provides system status snapshots for debugging
//   - Includes defensive checks for invalid state transitions
//
// NOTES:
//   - Contains NO pipeline logic and NO bundle logic.
//   - Pipeline and bundle logic are isolated in dedicated integration units.
using SASZombieAssaultTD.Engine.Diagnostics;
*/

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AssetPipelineClass = SASZombieAssaultTD.Engine.Resources.AssetPipeline.AssetPipeline;

namespace SASZombieAssaultTD.Engine.Resources
{
    /// <summary>
    /// Asset system configuration for SAS Zombie Assault TD.
    /// </summary>
    public class AssetSystemConfig
    {
        public string AssetRootPath { get; set; } = string.Empty;
        public string BundlePath { get; set; } = string.Empty;
    }

    /// <summary>
    /// Asset system status for SAS Zombie Assault TD.
    /// </summary>
    public class AssetSystemStatus
    {
        public bool Initialized { get; set; }
        public bool BundleLoaded { get; set; }
        public string BundleName { get; set; } = string.Empty;
        public int BundleAssetCount { get; set; }
        public AssetMemoryStats MemoryStats { get; set; } = new AssetMemoryStats();
    }

    /// <summary>
    /// Asset system integration core for SAS Zombie Assault TD.
    /// </summary>
    public static partial class AssetSystem
    {
        private static readonly object _lock = new();
        private static bool _initialized;
        private static AssetManager? _manager;
        private static AssetPipelineClass? _pipeline;

        // NOTE: '_currentBundle' is declared in another partial of AssetSystem.

        // ---------------------------------------------------------------------
        // CORE INITIALIZATION
        // ---------------------------------------------------------------------

        public static void Initialize(string assetRootPath = "Assets", string? bundlePath = null)
        {
            lock (_lock)
            {
                if (_initialized)
                {
                    System.Diagnostics.Debug.WriteLine("Warning: AssetSystem: Initialize() called but system already initialized.");
                    return;
                }

                try
                {
                    _manager = new AssetManager(assetRootPath);
                    _pipeline = new AssetPipelineClass();

                    if (!string.IsNullOrWhiteSpace(bundlePath) && File.Exists(bundlePath))
                        LoadBundleInternal(bundlePath);

                    _initialized = true;
                    System.Diagnostics.Debug.WriteLine($"Info: AssetSystem: Initialized (Root={assetRootPath}, Bundle={bundlePath})");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error: AssetSystem: Initialization failed: {ex.Message}");
                    throw;
                }
            }
        }

        public static void Initialize(AssetSystemConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            lock (_lock)
            {
                if (_initialized)
                {
                    System.Diagnostics.Debug.WriteLine("Warning: AssetSystem: Initialize(config) called but system already initialized.");
                    return;
                }

                try
                {
                    _manager = new AssetManager(config.AssetRootPath);
                    _pipeline = new SASZombieAssaultTD.Engine.Resources.AssetPipeline.AssetPipeline(
                        config.AssetRootPath,
                        config.AssetRootPath);

                    if (!string.IsNullOrWhiteSpace(config.BundlePath) && File.Exists(config.BundlePath))
                        LoadBundleInternal(config.BundlePath);

                    _initialized = true;
                    System.Diagnostics.Debug.WriteLine("Info: AssetSystem: Initialized with custom configuration");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error: AssetSystem: Initialization failed: {ex.Message}");
                    throw;
                }
            }
        }

        // ---------------------------------------------------------------------
        // SHUTDOWN
        // ---------------------------------------------------------------------

        public static void Shutdown()
        {
            lock (_lock)
            {
                if (!_initialized)
                {
                    System.Diagnostics.Debug.WriteLine("Warning: AssetSystem: Shutdown() called but system not initialized.");
                    return;
                }

                try
                {
                    _manager?.Dispose();
                    _pipeline?.Dispose();
                    _currentBundle?.Dispose();

                    _manager = null;
                    _pipeline = null;
                    _currentBundle = null;

                    _initialized = false;

                    System.Diagnostics.Debug.WriteLine("Info: AssetSystem: Shutdown complete");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error: AssetSystem: Shutdown error: {ex.Message}");
                }
            }
        }

        // ---------------------------------------------------------------------
        // ASSET LOADING (PUBLIC API)
        // ---------------------------------------------------------------------

        public static async Task<T?> LoadAssetAsync<T>(string key, AssetPriority priority = AssetPriority.Normal)
            where T : class
        {
            EnsureInitialized();

            // 1) Try bundle first, if present
            if (_currentBundle != null)
            {
                try
                {
                    var stream = _currentBundle.GetAssetStream(key);
                    if (stream != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Debug: AssetSystem: Loading '{key}' from bundle");
                        return await LoadFromStreamAsync<T>(stream, key);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Warning: AssetSystem: Bundle load failed for '{key}': {ex.Message}");
                    // fall through to file-system path
                }
            }

            // 2) Fallback to file system via AssetManager
            System.Diagnostics.Debug.WriteLine($"Debug: AssetSystem: Loading '{key}' from file system");
            var handle = _manager!.LoadAsset<T>(key, priority);

            // If the handle is not loaded yet and supports async, you could await here.
            // For now, we assume LoadAsset<T> returns a handle whose Value is ready or will be ready.
            return (T?)handle.Value;
        }

        public static async Task<Dictionary<string, object?>> LoadAssetsAsync(
            IEnumerable<string> keys,
            AssetPriority priority = AssetPriority.Normal)
        {
            EnsureInitialized();

            var results = new Dictionary<string, object?>();
            var tasks = keys.Select(async key =>
            {
                try
                {
                    var asset = await LoadAssetAsync<object>(key, priority);
                    lock (results)
                        results[key] = asset;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error: AssetSystem: Failed to load '{key}': {ex.Message}");
                }
            });

            await Task.WhenAll(tasks);
            return results;
        }

        public static async Task PreloadCriticalAssetsAsync(params string[] assetKeys)
        {
            if (assetKeys == null || assetKeys.Length == 0)
                return;

            EnsureInitialized();
            System.Diagnostics.Debug.WriteLine($"Info: AssetSystem: Preloading {assetKeys.Length} critical assets");

            await _manager!.PreloadAssetsAsync(assetKeys, AssetPriority.Critical);
        }

        // ---------------------------------------------------------------------
        // ASSET RETRIEVAL
        // ---------------------------------------------------------------------

        public static T GetAsset<T>(string key) where T : class
        {
            EnsureInitialized();

            if (_currentBundle != null)
            {
                try
                {
                    var stream = _currentBundle.GetAssetStream(key);
                    if (stream != null)
                        return LoadFromStreamSync<T>(stream, key);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Warning: AssetSystem: Bundle get failed for '{key}': {ex.Message}");
                }
            }

            return _manager!.GetAsset<T>(key);
        }

        public static bool IsAssetLoaded(string key)
        {
            EnsureInitialized();
            return _manager!.GetHandle(key)?.IsLoaded == true ||
                   (_currentBundle?.GetAssetMetadata(key) != null);
        }

        public static void UnloadAsset(string key, bool force = false)
        {
            EnsureInitialized();
            System.Diagnostics.Debug.WriteLine($"Debug: AssetSystem: Unloading '{key}' (Force={force})");
            _manager!.UnloadAsset(key, force);
        }

        public static AssetMetadata GetMetadata(string key)
        {
            EnsureInitialized();

            if (_currentBundle != null)
            {
                var entry = _currentBundle.GetAssetMetadata(key);
                if (entry != null)
                    return ConvertBundleEntryToMetadata(entry);
            }

            return _manager!.GetMetadata(key);
        }

        // ---------------------------------------------------------------------
        // MEMORY + STATUS
        // ---------------------------------------------------------------------

        public static int CollectGarbage(bool aggressive = false)
        {
            EnsureInitialized();
            System.Diagnostics.Debug.WriteLine($"Debug: AssetSystem: CollectGarbage (Aggressive={aggressive})");
            return _manager!.CollectGarbage(aggressive);
        }

        public static AssetMemoryStats GetMemoryStats()
        {
            EnsureInitialized();
            return _manager!.GetMemoryStats();
        }

        public static AssetSystemStatus GetStatus()
        {
            lock (_lock)
            {
                return new AssetSystemStatus
                {
                    Initialized = _initialized,
                    BundleLoaded = _currentBundle != null,
                    BundleName = _currentBundle?.Header.Name ?? string.Empty,
                    BundleAssetCount = _currentBundle?.Entries.Count() ?? 0,
                    MemoryStats = _initialized ? _manager!.GetMemoryStats() : new AssetMemoryStats()
                };
            }
        }

        // ---------------------------------------------------------------------
        // VALIDATION
        // ---------------------------------------------------------------------

        public static async Task<AssetSystemValidationResult> ValidateAsync()
        {
            EnsureInitialized();

            var result = new AssetSystemValidationResult { IsValid = true };

            if (_currentBundle != null)
            {
                var ok = await _currentBundle.VerifyIntegrityAsync();
                if (!ok)
                {
                    result.IsValid = false;
                    result.Errors.Add("Bundle integrity check failed");
                }
            }

            try
            {
                var stats = _manager!.GetMemoryStats();
                if (stats.LoadedAssets == 0)
                    result.Warnings.Add("No assets loaded");
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.Errors.Add($"Asset manager validation failed: {ex.Message}");
            }

            return result;
        }

        // ---------------------------------------------------------------------
        // INTERNAL CHECK
        // ---------------------------------------------------------------------

        private static void EnsureInitialized()
        {
            if (!_initialized)
                throw new InvalidOperationException("AssetSystem is not initialized. Call Initialize() first.");
        }
    }
}
