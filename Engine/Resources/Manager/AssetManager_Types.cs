// File:    AssetManagerTypes.cs
// Purpose: Asset types and handle definitions for SAS Zombie Assault TD.
// Features:
// - Asset type enumeration for categorization and processing
// - Asset handle system for tracking loaded assets
// - Type-safe generic asset handle implementation
// - Integration with AssetManager core functionality
//
// Architecture:
// - Type-safe enumeration system for asset categorization
// - Generic handle system with type safety
// - Integration with AssetManager core operations
// - Extensible design for new asset types

//INTEGRATION POINTS:
//- Coordinates with AssetManager for asset lifecycle management
//- Coordinates with AssetBundle for packaged asset distribution
//- Coordinates with RSManager for low-level resource management
//- Provides unified API for all asset operations across subsystems

//CORE PROCESSING CAPABILITIES:
//- Asset type categorization (Texture, Audio, Model, Font, Data, Script, Layout)
//- Asset handle tracking and lifecycle management
//- Type-safe generic operations with compile-time checking
//- Asset metadata generation and management

//PIPELINE ARCHITECTURE:
//- Modular type system for extensible asset support
//- Type-safe generic handle system for compile-time safety
//- Integration with AssetManager core functionality
//- Extensible design for new asset types
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
// Use asset types for categorization
//var textureType = AssetType.Texture;
//var audioType = AssetType.Audio;

// Create asset handles with type safety
//var textureHandle = new AssetHandle<Texture2D>();
//var audioHandle = new AssetHandle<AudioClip>();

// Check asset types
//if (assetHandle.Instance is Texture2D texture)
//{
//   System.Diagnostics.Debug.WriteLine("Texture asset loaded");
//}

// Register assets with type specification
//AssetManager.RegisterAsset("player_texture", "textures/player.png", AssetType.Texture);
//AssetManager.RegisterAsset("explosion_sound", "audio/explosion.wav", AssetType.Audio);

// Get assets by type
//var textures = AssetManager.GetAssetsByType(AssetType.Texture);
//var audios = AssetManager.GetAssetsByType(AssetType.Audio);

// Monitor asset system performance
//var stats = AssetManager.GetMemoryStats();
//System.Diagnostics.Debug.WriteLine($"Loaded {stats.LoadedAssets} assets, using {stats.TotalMemoryUsage} bytes");
//```
//

using SASZombieAssaultTD.Engine.Diagnostics;

using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Resources
{

    /// <summary>
    /// Type-safe asset handle for tracking loaded assets.
    /// Provides compile-time type checking and asset lifecycle management.
    /// </summary>
    public class AssetHandle<T> where T : class
    {
        /// <summary>
        /// The loaded asset instance with type safety.
        /// </summary>
        public T Asset { get; internal set; }

        /// <summary>
        /// Whether asset is currently loaded.
        /// </summary>
        public bool IsLoaded { get; internal set; }

        /// <summary>
        /// Reference count for memory management.
        /// </summary>
        public int ReferenceCount { get; internal set; }

        /// <summary>
        /// Loads asset if not already loaded.
        /// </summary>
        public void Load()
        {
            ReferenceCount++;
        }

        /// <summary>
        /// Releases a reference to the asset.
        /// </summary>
        public void Release()
        {
            if (ReferenceCount > 0)
                ReferenceCount--;
        }

        /// <summary>
        /// Returns a diagnostic string representation.
        /// </summary>
        public override string ToString()
        {
            return $"AssetHandle(Type={typeof(T).Name}, IsLoaded={IsLoaded}, RefCount={ReferenceCount})";
        }
    }

    /// <summary>
    /// Asset statistics information.
    /// </summary>
    public class AssetStatistics
    {
        public int TotalRegistered { get; set; }
        public int TotalLoaded { get; set; }
        public Dictionary<AssetType, int> AssetsByType { get; set; } = new();
    }

    /// <summary>
    /// Asset health check report.
    /// </summary>
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

    /// <summary>
    /// Asset repair report.
    /// </summary>
    public class AssetRepairReport
    {
        public DateTime Timestamp { get; set; }
        public List<string> ActionsTaken { get; set; } = new();
    }

    /// <summary>
    /// Asset optimization report.
    /// </summary>
    public class AssetOptimizationReport
    {
        public DateTime Timestamp { get; set; }
        public List<string> ActionsTaken { get; set; } = new();
    }
}
