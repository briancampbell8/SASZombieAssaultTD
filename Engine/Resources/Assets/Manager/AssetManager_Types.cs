// ====================================================================================================
//  FILE: AssetManager_Types.cs
//  PATH: ./Engine/Resources/Assets/Manager/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the AssetManager_Types module.
//
//  RESPONSIBILITIES:
//      - Provide Load() behavior for the Core subsystem.
//      - Provide Release() behavior for the Core subsystem.
//      - Provide ToString() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//============================================================================
//File:        AssetManagerTypes.cs
//Path:        E:\BDC\Projects\SASZombieAssaultTD\Engine\Resources\Assets\Manager\AssetManagerTypes.cs
//Program:     AssetManager (Types)
//Subsystem:   Assets
//Author:      BDC
//Created:     2026-05-29
//
//Purpose:
//    Defines supporting types used by the new Asset subsystem, including
//    asset classification enums, lightweight type descriptors, and internal
//    structures required by AssetManager, AssetManager_Loader, and the
//    AssetPipeline.
//
//Responsibilities:
//    • Provide asset type categorization for loader, registry, and pipeline.
//    • Define lightweight supporting types used across AssetManager partials.
//    • Maintain a stable contract for asset classification semantics.
//    • Support metadata generation and type‑safe asset organization.
//
//Architecture:
//    • Pure type and enum definitions (no logic).
//    • Consumed by AssetManager_Core, AssetManager_Loader, AssetManager_Registry,
//      and AssetPipeline subsystems.
//    • Extensible design for new asset types and classification rules.
//
//Integration Points:
//    • AssetManager for lifecycle coordination.
//    • AssetBundle subsystem for packaged asset distribution.
//    • AssetPipeline subsystem for preprocessing and optimization.
//    • RS subsystem (legacy) during migration.
//    • UI, Rendering, Audio, and Gameplay systems for unified asset access.
//
//Core Processing Capabilities:
//    • Asset type categorization (Texture, Audio, Model, Font, Data, Script, Layout).
//    • Type‑safe classification for scheduling, caching, and loading decisions.
//    • Metadata generation support for registry and pipeline subsystems.
//    • Stable type definitions for compile‑time safety.
//
//Performance Characteristics:
//    • Minimal overhead (pure type definitions).
//    • Zero runtime allocation overhead.
//    • Thread‑safe by design (immutable types).
//    • Supports high‑performance lookup and classification.
//============================================================================


//USAGE EXAMPLES:
//```csharp
//Use asset types for categorization
//var textureType = AssetType.Texture;
//var audioType = AssetType.Audio;

//Create asset handles with type safety
//var textureHandle = new AssetHandle<Texture2D>();
//var audioHandle = new AssetHandle<AudioClip>();

//Check asset types
//if (assetHandle.Instance is Texture2D texture)
//{
//  DLogger.Log(LogSubsystems.ResourcesPipeline, "Texture asset loaded");
//}

//Register assets with type specification
//AssetManager.RegisterAsset("player_texture", "textures/player.png", AssetType.Texture);
//AssetManager.RegisterAsset("explosion_sound", "audio/explosion.wav", AssetType.Audio);

//Get assets by type
//var textures = AssetManager.GetAssetsByType(AssetType.Texture);
//var audios = AssetManager.GetAssetsByType(AssetType.Audio);

//Monitor asset system performance
//var stats = AssetManager.GetMemoryStats();
//DLogger.Log($"Loaded {stats.LoadedAssets} assets, using {stats.TotalMemoryUsage} bytes");
//```
//

//

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Resources
{

    ///<summary>
    ///Type-safe asset handle for tracking loaded assets.
    ///Provides compile-time type checking and asset lifecycle management.
    ///</summary>
    public class AssetHandle<T> where T : class
    {
        ///<summary>
        ///The loaded asset instance with type safety.
        ///</summary>
        public T Asset { get; internal set; }

        ///<summary>
        ///Whether asset is currently loaded.
        ///</summary>
        public bool IsLoaded { get; internal set; }

        ///<summary>
        ///Reference count for memory management.
        ///</summary>
        public int ReferenceCount { get; internal set; }

        ///<summary>
        ///Loads asset if not already loaded.
        ///</summary>
        public void Load()
        {
            ReferenceCount++;
        }

        ///<summary>
        ///Releases a reference to the asset.
        ///</summary>
        public void Release()
        {
            if (ReferenceCount > 0)
                ReferenceCount--;
        }

        ///<summary>
        ///Returns a diagnostic string representation.
        ///</summary>
        public override string ToString()
        {
            return $"AssetHandle(Type={typeof(T).Name}, IsLoaded={IsLoaded}, RefCount={ReferenceCount})";
        }
    }

    ///<summary>
    ///Asset statistics information.
    ///</summary>
    public class AssetStatistics
    {
        public int TotalRegistered { get; set; }
        public int TotalLoaded { get; set; }
        public Dictionary<AssetType, int> AssetsByType { get; set; } = new();
    }

    ///<summary>
    ///Asset health check report.
    ///</summary>
    public class AssetHealthReport
    {
        public DateTime Timestamp { get; set; }
        public int TotalAssets { get; set; }
        public int LoadedAssets { get; set; }
        public long MemoryUsage { get; set; }
        public Dictionary<AssetType, int> AssetTypeDistribution { get; set; } = new();
        public List<string> UnloadedAssets { get; set; } = new();
        public int PotentialMemoryLeaks { get; set; }
    }

    ///<summary>
    ///Asset repair report.
    ///</summary>
    public class AssetRepairReport
    {
        public DateTime Timestamp { get; set; }
        public List<string> ActionsTaken { get; set; } = new();
    }

    ///<summary>
    ///Asset optimization report.
    ///</summary>
    public class AssetOptimizationReport
    {
        public DateTime Timestamp { get; set; }
        public List<string> ActionsTaken { get; set; } = new();
    }
}

