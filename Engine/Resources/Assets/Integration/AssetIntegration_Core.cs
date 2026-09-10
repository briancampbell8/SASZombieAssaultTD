/*
//============================================================================
//File:        AssetIntegration_Core.cs
//Path:        E:\BDC\Projects\SASZombieAssaultTD\Engine\Resources\Assets\Integration\AssetIntegration_Core.cs
//Program:     AssetIntegration (Core)
//Subsystem:   Assets
//Author:      BDC
//Created:     2026-05-29
//
//Purpose:
//    Core orchestration layer for the SAS Zombie Assault TD Asset System.
//    Provides the unified public API for initialization, shutdown, asset
//    loading, retrieval, memory statistics, validation, and subsystem
//    coordination. Acts as the façade entry point for all asset operations.
//
//Responsibilities:
//    • Coordinate AssetManager, AssetPipeline, and AssetBundle subsystems.
//    • Provide deterministic initialization and shutdown sequences.
//    • Expose thread‑safe asset loading and retrieval APIs.
//    • Maintain global asset system state and lifecycle correctness.
//    • Provide memory statistics, diagnostics, and validation routines.
//    • Serve as the authoritative integration point for the entire Asset System.
//
//Architectural Role:
//    • Facade pattern — single entry point for all asset operations.
//    • Orchestrator — delegates work to Manager, Pipeline, and Bundle units.
//    • Thread‑safe — ensures safe concurrent access to asset operations.
//    • Deterministic — enforces strict initialization and shutdown ordering.
//    • Non‑intrusive — contains no pipeline or bundle logic directly.
//
//Diagnostics:
//    • Logs initialization, shutdown, loading, validation, and errors.
//    • Provides system status snapshots for debugging and profiling.
//    • Includes defensive checks for invalid state transitions.
//    • Integrates with Engine Diagnostics subsystem.
//
//Notes:
//    • Contains NO pipeline logic and NO bundle logic.
//    • Pipeline and bundle logic are isolated in dedicated integration units.
//    • No BGFX, no legacy backend references.
//============================================================================
*/

#nullable enable

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AssetPipelineClass = SASZombieAssaultTD.Engine.Resources.AssetPipeline.AssetPipeline;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Resources
{
    ///<summary>
    ///Asset system configuration for SAS Zombie Assault TD.
    ///</summary>
    public class AssetSystemConfig
    {
        public string AssetRootPath { get; set; } = string.Empty;
        public string BundlePath { get; set; } = string.Empty;
    }

    ///<summary>
    ///Asset system status for SAS Zombie Assault TD.
    ///</summary>
    public class AssetSystemStatus
    {
        public bool Initialized { get; set; }
        public bool BundleLoaded { get; set; }
        public string BundleName { get; set; } = string.Empty;
        public int BundleAssetCount { get; set; }
        public AssetMemoryStats MemoryStats { get; set; } = new AssetMemoryStats();
    }

    ///<summary>
    ///Asset system integration core for SAS Zombie Assault TD.
    ///</summary>
    public static partial class AssetSystem
    {
        private static readonly object _lock = new();
        private static bool _initialized;
        private static AssetManager? _manager;
        private static AssetPipelineClass? _pipeline;

        //NOTE: '_currentBundle' is declared in another partial of AssetSystem.

        //---------------------------------------------------------------------
        //CORE INITIALIZATION
        //---------------------------------------------------------------------

        public static void Initialize(string assetRootPath = "Assets", string? bundlePath = null)
        {
            lock (_lock)
            {
                if (_initialized)
                {
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "Warning: AssetSystem: Initialize() called but system already initialized.");
                    return;
                }

                try
                {
                    _manager = new AssetManager(assetRootPath);
                    _pipeline = new AssetPipelineClass();

                    if (!string.IsNullOrWhiteSpace(bundlePath) && File.Exists(bundlePath))
                        LoadBundleInternal(bundlePath);

                    _initialized = true;
                    DLogger.Log($"Info: AssetSystem: Initialized (Root={assetRootPath}, Bundle={bundlePath})");
                }
                catch (Exception ex)
                {
                    DLogger.Log($"Error: AssetSystem: Initialization failed: {ex.Message}");
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
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "Warning: AssetSystem: Initialize(config) called but system already initialized.");
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
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "Info: AssetSystem: Initialized with custom configuration");
                }
                catch (Exception ex)
                {
                    DLogger.Log($"Error: AssetSystem: Initialization failed: {ex.Message}");
                    throw;
                }
            }
        }

        //---------------------------------------------------------------------
        //SHUTDOWN
        //---------------------------------------------------------------------

        public static void Shutdown()
        {
            lock (_lock)
            {
                if (!_initialized)
                {
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "Warning: AssetSystem: Shutdown() called but system not initialized.");
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

                    DLogger.Log(LogSubsystems.ResourcesPipeline, "Info: AssetSystem: Shutdown complete");
                }
                catch (Exception ex)
                {
                    DLogger.Log($"Error: AssetSystem: Shutdown error: {ex.Message}");
                }
            }
        }

        //---------------------------------------------------------------------
        //ASSET LOADING (PUBLIC API)
        //---------------------------------------------------------------------

        public static async Task<T?> LoadAssetAsync<T>(string key, AssetPriority priority = AssetPriority.Normal)
            where T : class
        {
            EnsureInitialized();

            //1) Try bundle first, if present
            if (_currentBundle != null)
            {
                try
                {
                    var stream = _currentBundle.GetAssetStream(key);
                    if (stream != null)
                    {
                        DLogger.Log($"Debug: AssetSystem: Loading '{key}' from bundle");
                        return await LoadFromStreamAsync<T>(stream, key);
                    }
                }
                catch (Exception ex)
                {
                    DLogger.Log($"Warning: AssetSystem: Bundle load failed for '{key}': {ex.Message}");
                    //fall through to file-system path
                }
            }

            //2) Fallback to file system via AssetManager
            DLogger.Log($"Debug: AssetSystem: Loading '{key}' from file system");
            var handle = _manager!.LoadAsset<T>(key, priority);

            return handle.As<T>();
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
                    DLogger.Log($"Error: AssetSystem: Failed to load '{key}': {ex.Message}");
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
            DLogger.Log($"Info: AssetSystem: Preloading {assetKeys.Length} critical assets");

            //Use existing async load path as a preload mechanism.
            await LoadAssetsAsync(assetKeys, AssetPriority.Critical);
        }

        //---------------------------------------------------------------------
        //ASSET RETRIEVAL
        //---------------------------------------------------------------------

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
                    DLogger.Log($"Warning: AssetSystem: Bundle get failed for '{key}': {ex.Message}");
                }
            }

            return _manager!.GetAsset<T>(key);
        }

        public static bool IsAssetLoaded(string key)
        {
            EnsureInitialized();

            var handle = _manager!.GetHandle(key);
            var handleLoaded = handle != null && handle.Instance != null;

            return handleLoaded || (_currentBundle?.GetAssetMetadata(key) != null);
        }

        public static void UnloadAsset(string key, bool force = false)
        {
            EnsureInitialized();
            DLogger.Log($"Debug: AssetSystem: Unloading '{key}' (Force={force})");
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

        //---------------------------------------------------------------------
        //MEMORY + STATUS
        //---------------------------------------------------------------------

        public static int CollectGarbage(bool aggressive = false)
        {
            EnsureInitialized();
            DLogger.Log($"Debug: AssetSystem: CollectGarbage (Aggressive={aggressive})");
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

        //---------------------------------------------------------------------
        //VALIDATION
        //---------------------------------------------------------------------

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

        //---------------------------------------------------------------------
        //INTERNAL CHECK
        //---------------------------------------------------------------------

        private static void EnsureInitialized()
        {
            if (!_initialized)
                throw new InvalidOperationException("AssetSystem is not initialized. Call Initialize() first.");
        }
    }
}
