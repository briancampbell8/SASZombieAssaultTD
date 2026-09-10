// ====================================================================================================
//  FILE: AssetManager_Config.cs
//  PATH: ./Engine/Resources/Assets/Manager/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the AssetManager_Config module.
//
//  RESPONSIBILITIES:
//      - Provide CreateDefault() behavior for the Core subsystem.
//      - Provide Create() behavior for the Core subsystem.
//      - Provide CreateHighPerformance() behavior for the Core subsystem.
//      - Provide CreateLowMemory() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//============================================================================
//File:        AssetManager_Config.cs
//Path:        E:\BDC\Projects\SASZombieAssaultTD\Engine\Resources\Assets\Manager\AssetManager_Config.cs
//Program:     AssetManager_Config
//Author:      BDC
//Created:     2026-05-29
//
//Purpose:
//     Authoritative configuration class for the new Asset subsystem. Defines
//     memory limits, concurrency settings, and streaming behavior used by
//     AssetManager and AssetManager_Loader during asset loading and cache
//     management.
//
//Responsibilities:
//     • Provide deterministic configuration values for the Asset subsystem.
//     • Define maximum memory usage for loaded assets.
//     • Define concurrency limits for async load operations.
//     • Enable or disable streaming behavior for large assets.
//     • Serve as the single source of truth for AssetManager runtime settings.
//
//Notes:
//     • This program replaces the legacy RS configuration path.
//     • All Asset subsystem programs under Engine/Resources/Assets depend on
//       this configuration class.
//     • Thread‑safety, weak‑reference caching, and type‑safe loading are
//       implemented in the manager/loader, not in this configuration class.
//============================================================================


//INTEGRATION POINTS:
//- Coordinates with AssetManager for asset lifecycle management
//- Coordinates with AssetBundle for packaged asset distribution
//- Coordinates with RSManager for low-level resource management
//- Provides unified API for all asset operations across subsystems

//CORE PROCESSING CAPABILITIES:
//- Texture loading with streaming and caching support
//- Audio loading with format conversion and optimization
//- 3D model loading with LOD level selection
//- Font loading with character set optimization
//- Data loading with serialization and deserialization
//- Script and shader loading with validation

//PIPELINE ARCHITECTURE:
//- Modular processor system for extensible asset type support
//- Configurable processing pipeline with quality vs. performance trade-offs
//- Parallel processing for batch operations with configurable worker threads
//- Caching system to prevent redundant processing of unchanged assets
//- Comprehensive validation with detailed error reporting and suggestions

//PERFORMANCE CHARACTERISTICS:
//- Minimal overhead through direct subsystem delegation
//- Optimized initialization with lazy loading where appropriate
//- Efficient resource management with automatic cleanup
//- Thread-safe operations with minimal contention
//- Background processing coordination to prevent blocking
//- Intelligent caching with hash-based change detection
//- Memory-efficient streaming for large assets
//USAGE EXAMPLES:
//```csharp
//Create custom configuration
//var config = new AssetManagerConfig
//{
//   MaxMemoryUsage = 1024L * 1024 * 1024, //1GB
//   MaxConcurrentLoads = 8,
//   EnableStreaming = true
//};
//var assetManager = new AssetManager("Assets", config);
//

//

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Resources
{
    ///<summary>
    ///Configuration settings controlling behavior of AssetManager subsystem for SAS Zombie Assault TD.
    ///Defines memory limits, concurrency, and streaming options used during asset loading
    ///and cache management. This is an authoritative configuration class.
    ///</summary>
    ///<remarks>
    ///This configuration system integrates with AssetManager, AssetBundle, and RSManager
    ///to provide unified configuration management across all asset subsystems.
    ///</remarks>
    ///<example>
    ///<code>
    ///var config = new AssetManagerConfig
    ///{
    ///    MaxMemoryUsage = 1024L * 1024 * 1024,
    ///    MaxConcurrentLoads = 8,
    ///    EnableStreaming = true
    ///};
    ///var assetManager = new AssetManager("Assets", config);
    ///</code>
    ///</example>
    public sealed class AssetManagerConfig
    {
        ///<summary>
        ///Maximum memory usage in bytes for the asset manager.
        ///</summary>
        public long MaxMemoryUsage { get; set; } = 1024L * 1024 * 1024; //1GB

        ///<summary>
        ///Maximum number of concurrent asset loads.
        ///</summary>
        public int MaxConcurrentLoads { get; set; } = 8;

        ///<summary>
        ///Whether to enable streaming for large assets.
        ///</summary>
        public bool EnableStreaming { get; set; } = true;

        ///<summary>
        ///Creates a new AssetManagerConfig with default settings.
        ///</summary>
        ///<returns>Default configuration instance.</returns>
        public static AssetManager_Config CreateDefault()
        {
            return new AssetManager_Config();
        }

        ///<summary>
        ///Creates a new AssetManagerConfig with specified parameters.
        ///</summary>
        ///<param name="maxMemoryUsage">Maximum memory usage in bytes.</param>
        ///<param name="maxConcurrentLoads">Maximum concurrent loads.</param>
        ///<param name="enableStreaming">Whether to enable streaming.</param>
        ///<returns>Configuration instance with specified settings.</returns>
        public static AssetManager_Config Create(long maxMemoryUsage, int maxConcurrentLoads, bool enableStreaming)
        {
            return new AssetManager_Config
            {
                MaxMemoryUsage = maxMemoryUsage,
                MaxConcurrentLoads = maxConcurrentLoads,
                EnableStreaming = enableStreaming
            };
        }

        ///<summary>
        ///Creates a high-performance configuration.
        ///</summary>
        ///<returns>High-performance configuration instance.</returns>
        public static AssetManager_Config CreateHighPerformance() => new AssetManager_Config
        {
            MaxMemoryUsage = 2048L * 1024 * 1024, //2GB
            MaxConcurrentLoads = 16,
            EnableStreaming = true
        };

        ///<summary>
        ///Creates a low-memory configuration.
        ///</summary>
        ///<returns>Low-memory configuration instance.</returns>
        public static AssetManagerConfig CreateLowMemory()
        {
            return new AssetManagerConfig
            {
                MaxMemoryUsage = 512L * 1024 * 1024, //512MB
                MaxConcurrentLoads = 2,
                EnableStreaming = false
            };
        }
    }

    public class AssetManager_Config
    {
        internal long MaxMemoryUsage;
        internal int MaxConcurrentLoads;
        internal bool EnableStreaming;
    }
}

