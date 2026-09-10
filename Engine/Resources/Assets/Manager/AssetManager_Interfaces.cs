// ====================================================================================================
//  FILE: AssetManager_Interfaces.cs
//  PATH: ./Engine/Resources/Assets/Manager/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the AssetManager_Interfaces module.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//============================================================================
//File:        AssetManager_Interfaces.cs
//Path:        E:\BDC\Projects\SASZombieAssaultTD\Engine\Resources\Assets\Manager\AssetManager_Interfaces.cs
//Program:     AssetManager (Interfaces)
//Subsystem:   Assets
//Author:      BDC
//Created:     2026-05-29
//
//Purpose:
//    Defines the interface contracts for the new Asset subsystem. Provides
//    unified abstractions for loading, caching, lifecycle management, and
//    type‑specific asset operations. Enables modularity, testability, and
//    subsystem extensibility.
//
//Responsibilities:
//    • Define the core interfaces consumed by AssetManager and its partials.
//    • Support extensible asset type systems with type‑specific handlers.
//    • Provide async/await‑based loading contracts for non‑blocking operations.
//    • Enable dependency injection for testing and modular architecture.
//    • Establish a stable API surface for all asset operations.
//
//Architecture:
//    • Interface Segregation Principle (ISP) for clean separation of concerns.
//    • Dependency Injection (DI) friendly design for testability.
//    • Async/await pattern for non‑blocking asset operations.
//    • Factory pattern support for manager instantiation and configuration.
//
//Performance Characteristics:
//    • Minimal interface overhead with direct method calls.
//    • Optimized for async operations with cancellation token support.
//    • Thread‑safe design with immutable interface definitions where possible.
//    • Memory‑efficient with streaming support for large assets.
//    • Supports background processing to avoid UI thread blocking.
//
//Integration Points:
//    • AssetBundle subsystem for packaged asset distribution.
//    • AssetPipeline subsystem for preprocessing and optimization.
//    • RS subsystem (legacy) for low‑level resource access during migration.
//    • UI, Rendering, Audio, and Gameplay systems for unified asset access.
//============================================================================

//USAGE EXAMPLES:
//```csharp
//var customManager = new CustomAssetManager();
//AssetSystem.RegisterManager(customManager);
//var texture = await AssetSystem.LoadAssetAsync<Texture2D>("player.png");
//var audio = await AssetSystem.LoadAssetAsync<AudioClip>("explosion.wav");
//var stats = AssetSystem.GetMemoryStats();
//DLogger.Log($"Loaded {stats.LoadedAssets} assets, using {stats.TotalMemoryUsage} bytes");
//var validation = await AssetSystem.ValidateAsync();
//if (!validation.IsValid)
//{
//    DLogger.Log($"Asset system validation failed: {string.Join(", ", validation.Errors)}");
//}
//```

//

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Threading;
using System.Threading.Tasks;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Resources
{
    ///<summary>
    ///Interface for asset manager implementations.
    ///Defines contracts for all asset management operations including loading,
    ///caching, lifecycle management, and performance monitoring.
    ///Supports extensible asset type system with type-specific implementations.
    ///</summary>
    ///<remarks>
    ///This interface provides the foundation for the asset management subsystem,
    ///enabling dependency injection, testability, and modularity while maintaining
    ///a clean separation of concerns through interface segregation.
    ///</remarks>
    public interface IAssetManager
    {
        ///<summary>
        ///Gets the current memory usage statistics for the asset manager.
        ///</summary>
        ///<returns>Memory usage statistics including loaded assets count and total memory usage.</returns>
        AssetMemoryStats GetMemoryStats();

        ///<summary>
        ///Validates the integrity of the asset manager and all loaded assets.
        ///</summary>
        ///<param name="cancellationToken">Cancellation token for the validation operation.</param>
        ///<returns>Validation result indicating system integrity and any issues found.</returns>
        Task<AssetSystemValidationResult> ValidateAsync(CancellationToken cancellationToken = default);

        ///<summary>
        ///Gets the current configuration settings for the asset manager.
        ///</summary>
        ///<returns>Current configuration settings including memory limits and loading options.</returns>
        AssetManagerConfig GetConfiguration();

        ///<summary>
        ///Forces garbage collection of unused assets to free memory.
        ///</summary>
        ///<param name="force">If true, forces immediate garbage collection; otherwise uses heuristics.</param>
        void GarbageCollect(bool force = false);

        ///<summary>
        ///Preloads critical assets to ensure they're available when needed.
        ///</summary>
        ///<param name="assetPaths">Array of asset paths to preload.</param>
        ///<param name="cancellationToken">Cancellation token for the preload operation.</param>
        ///<returns>Task representing the preload operation.</returns>
        Task PreloadAssetsAsync(string[] assetPaths, CancellationToken cancellationToken = default);

        ///<summary>
        ///Shuts down the asset manager and releases all resources.
        ///</summary>
        ///<param name="dispose">If true, disposes all managed resources; otherwise unloads assets only.</param>
        void Shutdown(bool dispose = true);
    }

    ///<summary>
    ///Result of asset system validation operation.
    ///</summary>
    public class AssetSystemValidationResult
    {
        ///<summary>
        ///Gets whether the asset system validation passed.
        ///</summary>
        public bool IsValid { get; set; }

        ///<summary>
        ///List of validation errors found during system validation.
        ///</summary>
        public List<string> Errors { get; set; } = new();

        ///<summary>
        ///List of warnings found during system validation.
        ///</summary>
        public List<string> Warnings { get; set; } = new();

        //============================================================================
        //COMMENTED OUT — DUPLICATE TYPE DEFINITIONS
        //Reason:
        //AssetMemoryStats has a single canonical definition in AssetManager_Core.cs.
        //These versions were created by region collapse / fallout and must remain
        //commented out for audit and historical reference.
        //============================================================================

        /*
        ///<summary>
        ///Memory usage statistics for the asset management system.
        ///</summary>
        public class AssetMemoryStats
        {
            ///<summary>
            ///Gets the total number of assets currently loaded in memory.
            ///</summary>
            public int LoadedAssets { get; set; }

            ///<summary>
            ///Nested duplicate definition — region collapse fallout.
            ///</summary>
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

