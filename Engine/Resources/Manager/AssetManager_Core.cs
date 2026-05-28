// ============================================================================
// File Path: Engine/Resources/Manager/AssetManager_Core.cs
// File: AssetManager_Core.cs
// Program: AssetManager (Core)
// Subsystem: Resources
//
// Purpose:
//     Core initialization, configuration, disposal, and root‑level operations
//     for the AssetManager subsystem. Provides the foundational lifecycle
//     management, path resolution, memory statistics, and safety guards used
//     by all other AssetManager partials.
//
// Responsibilities:
//     - Initialize asset system with default or custom configuration
//     - Manage asset root directory and path resolution
//     - Provide memory usage statistics
//     - Enforce disposal safety and lifecycle correctness
//     - Coordinate with loader, registry, and validation partials
//
// Architecture:
//     - Partial class: functionality split across multiple files
//         • AssetManager_Core.cs       — initialization, disposal, core logic
//         • AssetManager_Loader.cs     — loading, caching, async operations
//         • AssetManager_Registry.cs   — registration and metadata
//         • AssetManager_Validation.cs — health checks and optimization
//         • AssetManagerTypes.cs       — supporting types and configuration
//
// Integration Points:
//     - AssetBundle system for packaged asset loading
//     - AssetPipeline for preprocessing and optimization
//     - RSManager for low‑level resource handling
//     - UI, Rendering, Audio, and Gameplay systems for unified asset access
//
// Performance Notes:
//     - Thread‑safe initialization
//     - Minimal overhead through subsystem delegation
//     - Lazy loading where appropriate
//     - Efficient cleanup and memory tracking
//
// Usage Example:
//     var manager = new AssetManager("Assets/");
//     var texture = await manager.LoadAsync<Texture2D>("textures/player.png");
//     var stats = manager.GetMemoryStats();
//     System.Diagnostics.Debug.WriteLine(stats.TotalMemoryUsage);
// ============================================================================

using System;
using System.IO;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Resources
{
    public partial class AssetManager : IDisposable
    {
        // --------------------------------------------------------------------
        // FIELDS
        // --------------------------------------------------------------------

        /// <summary>
        /// Root directory path where assets are stored.
        /// All relative asset paths are resolved against this directory.
        /// </summary>
        private readonly string _assetRootPath;

        /// <summary>
        /// Configuration settings for the asset manager.
        /// </summary>
        private readonly AssetManagerConfig _config;

        /// <summary>
        /// Indicates whether this instance has been disposed.
        /// Ensures lifecycle correctness across all partials.
        /// </summary>
        private volatile bool _disposed;

        // --------------------------------------------------------------------
        // CONSTRUCTORS
        // --------------------------------------------------------------------

        /// <summary>
        /// Initializes a new AssetManager instance with default configuration.
        /// Ensures the asset root directory exists before any operations occur.
        /// </summary>
        public AssetManager(string assetRootPath)
        {
            _assetRootPath = assetRootPath ?? throw new ArgumentNullException(nameof(assetRootPath));
            _config = new AssetManagerConfig();

            EnsureAssetDirectory();
        }

        /// <summary>
        /// Initializes a new AssetManager instance with custom configuration.
        /// Ensures the asset root directory exists before any operations occur.
        /// </summary>
        public AssetManager(string assetRootPath, AssetManagerConfig config)
        {
            _assetRootPath = assetRootPath ?? throw new ArgumentNullException(nameof(assetRootPath));
            _config = config ?? throw new ArgumentNullException(nameof(config));

            EnsureAssetDirectory();
        }

        // --------------------------------------------------------------------
        // PROPERTIES
        // --------------------------------------------------------------------

        /// <summary>
        /// Absolute path to the asset root directory.
        /// </summary>
        public string AssetRootPath => _assetRootPath;

        /// <summary>
        /// Current configuration settings for the asset manager.
        /// </summary>
        public AssetManagerConfig Configuration => _config;

        // --------------------------------------------------------------------
        // PATH RESOLUTION
        // --------------------------------------------------------------------

        /// <summary>
        /// Resolves a relative asset path to an absolute file path.
        /// Provides consistent path handling across all loaders.
        /// </summary>
        public string GetAssetPath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("Asset path cannot be null or empty", nameof(relativePath));

            return Path.Combine(_assetRootPath, relativePath);
        }

        /// <summary>
        /// Checks if an asset file exists at the specified relative path.
        /// </summary>
        public bool AssetExists(string relativePath)
        {
            return File.Exists(GetAssetPath(relativePath));
        }

        // --------------------------------------------------------------------
        // MEMORY STATISTICS
        // --------------------------------------------------------------------

        /// <summary>
        /// Gets current memory usage statistics for the asset manager.
        /// Delegates detailed tracking to loader and registry partials.
        /// </summary>
        public AssetMemoryStats GetMemoryStats()
        {
            return new AssetMemoryStats
            {
                LoadedAssets = LoadedAssetCount,
                TotalMemoryUsage = EstimateMemoryUsage(),
                MaxMemoryUsage = _config.MaxMemoryUsage
            };
        }

        /// <summary>
        /// Estimates the current memory usage of all loaded assets.
        /// Placeholder implementation — real logic in Loader partial.
        /// </summary>
        private long EstimateMemoryUsage()
        {
            return LoadedAssetCount * 1024;
        }

        // --------------------------------------------------------------------
        // DISPOSAL
        // --------------------------------------------------------------------

        /// <summary>
        /// Releases all resources used by the asset manager.
        /// Ensures cache and registry cleanup across all partials.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            ClearCache();
            ClearRegistry();

            System.Diagnostics.Debug.WriteLine("AssetManager: Disposed");
        }

        /// <summary>
        /// Throws if this instance has been disposed.
        /// Used by all partials to enforce lifecycle correctness.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AssetManager));
        }

        /// <summary>
        /// Compatibility alias for Dispose().
        /// </summary>
        public void Release()
        {
            Dispose();
        }

        // --------------------------------------------------------------------
        // INTERNAL HELPERS
        // --------------------------------------------------------------------

        /// <summary>
        /// Ensures the asset root directory exists.
        /// Prevents file‑system errors during asset loading.
        /// </summary>
        private void EnsureAssetDirectory()
        {
            if (!Directory.Exists(_assetRootPath))
                Directory.CreateDirectory(_assetRootPath);
        }
    }

    // ------------------------------------------------------------------------
    // SUPPORTING TYPES
    // ------------------------------------------------------------------------

    public class AssetMemoryStats
    {
        public int LoadedAssets { get; set; }
        public int TotalAssets { get; set; }
        public long TotalMemoryUsage { get; set; }
        public long MaxMemoryUsage { get; set; }
    }
}
