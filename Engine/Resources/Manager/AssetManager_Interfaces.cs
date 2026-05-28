// File:    AssetManager_Interfaces.cs
// Purpose: Interface definitions for asset management system in SAS Zombie Assault TD.
// Features:
// - Defines contracts for all asset management operations
// - Supports extensible asset type system with type-specific implementations
// - Provides unified interface for loading, caching, and lifecycle management
// - Enables dependency injection pattern for testability and modularity
// - Async/await pattern for non-blocking operations
//
// Architecture:
// - Interface segregation principle for clean separation of concerns
// - Dependency injection pattern for testability and modularity
// - Async/await pattern for non-blocking operations
// - Factory pattern for manager instantiation and configuration

// Performance Characteristics:
// - Minimal interface overhead with direct method calls
// - Optimized for async operations with proper cancellation token support
// - Thread-safe design with immutable interfaces where possible
// - Memory-efficient with streaming support for large assets
// - Background processing capabilities to prevent UI blocking

// INTEGRATION POINTS:
// - Coordinates with AssetBundle for packaged asset distribution
// - Coordinates with AssetPipeline for processing and optimization
// - Coordinates with RSManager for low-level resource management
// - Provides unified API for all asset operations across subsystems

// USAGE EXAMPLES:
// ```csharp
// var customManager = new CustomAssetManager();
// AssetSystem.RegisterManager(customManager);
// var texture = await AssetSystem.LoadAssetAsync<Texture2D>("player.png");
// var audio = await AssetSystem.LoadAssetAsync<AudioClip>("explosion.wav");
// var stats = AssetSystem.GetMemoryStats();
// System.Diagnostics.Debug.WriteLine($"Loaded {stats.LoadedAssets} assets, using {stats.TotalMemoryUsage} bytes");
// var validation = await AssetSystem.ValidateAsync();
// if (!validation.IsValid)
// {
//     System.Diagnostics.Debug.WriteLine($"Asset system validation failed: {string.Join(", ", validation.Errors)}");
// }
// ```

using SASZombieAssaultTD.Engine.Diagnostics;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SASZombieAssaultTD.Engine.Resources
{
    /// <summary>
    /// Interface for asset manager implementations.
    /// Defines contracts for all asset management operations including loading,
    /// caching, lifecycle management, and performance monitoring.
    /// Supports extensible asset type system with type-specific implementations.
    /// </summary>
    /// <remarks>
    /// This interface provides the foundation for the asset management subsystem,
    /// enabling dependency injection, testability, and modularity while maintaining
    /// a clean separation of concerns through interface segregation.
    /// </remarks>
    public interface IAssetManager
    {
        /// <summary>
        /// Gets the current memory usage statistics for the asset manager.
        /// </summary>
        /// <returns>Memory usage statistics including loaded assets count and total memory usage.</returns>
        AssetMemoryStats GetMemoryStats();

        /// <summary>
        /// Validates the integrity of the asset manager and all loaded assets.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for the validation operation.</param>
        /// <returns>Validation result indicating system integrity and any issues found.</returns>
        Task<AssetSystemValidationResult> ValidateAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current configuration settings for the asset manager.
        /// </summary>
        /// <returns>Current configuration settings including memory limits and loading options.</returns>
        AssetManagerConfig GetConfiguration();

        /// <summary>
        /// Forces garbage collection of unused assets to free memory.
        /// </summary>
        /// <param name="force">If true, forces immediate garbage collection; otherwise uses heuristics.</param>
        void GarbageCollect(bool force = false);

        /// <summary>
        /// Preloads critical assets to ensure they're available when needed.
        /// </summary>
        /// <param name="assetPaths">Array of asset paths to preload.</param>
        /// <param name="cancellationToken">Cancellation token for the preload operation.</param>
        /// <returns>Task representing the preload operation.</returns>
        Task PreloadAssetsAsync(string[] assetPaths, CancellationToken cancellationToken = default);

        /// <summary>
        /// Shuts down the asset manager and releases all resources.
        /// </summary>
        /// <param name="dispose">If true, disposes all managed resources; otherwise unloads assets only.</param>
        void Shutdown(bool dispose = true);
    }

    /// <summary>
    /// Result of asset system validation operation.
    /// </summary>
    public class AssetSystemValidationResult
    {
        /// <summary>
        /// Gets whether the asset system validation passed.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// List of validation errors found during system validation.
        /// </summary>
        public List<string> Errors { get; set; } = new();

        /// <summary>
        /// List of warnings found during system validation.
        /// </summary>
        public List<string> Warnings { get; set; } = new();

        // ============================================================================
        // COMMENTED OUT — DUPLICATE TYPE DEFINITIONS
        // Reason:
        // AssetMemoryStats has a single canonical definition in AssetManager_Core.cs.
        // These versions were created by region collapse / fallout and must remain
        // commented out for audit and historical reference.
        // ============================================================================

        /*
        /// <summary>
        /// Memory usage statistics for the asset management system.
        /// </summary>
        public class AssetMemoryStats
        {
            /// <summary>
            /// Gets the total number of assets currently loaded in memory.
            /// </summary>
            public int LoadedAssets { get; set; }

            /// <summary>
            /// Nested duplicate definition — region collapse fallout.
            /// </summary>
            public class AssetMemoryStats
            {
                public int LoadedAssets { get; set; }
                public long TotalMemoryUsage { get; set; }
                public double CacheHitRate { get; set; }
                public int GarbageCollections { get; set; }
            }
        }
        */
    }
}
