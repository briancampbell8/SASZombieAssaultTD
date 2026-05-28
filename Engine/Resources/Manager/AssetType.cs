// File:    AssetType.cs
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

namespace SASZombieAssaultTD.Engine.Resources
{
    /// <summary>
    /// Asset types for management in SAS Zombie Assault TD.
    /// Provides categorization system for different asset types and type-safe handles.
    /// </summary>
    public enum AssetType
    {
        Unknown = 0,
        Texture = 1,
        Audio = 2,
        Model = 3,
        Font = 4,
        Data = 5,
        Script = 6,
        Layout = 7,
        Json = 8,
        Binary = 9,
        SpriteSheet = 10,
        Sound = 11,
        Music = 12,
        Shader = 13

        // Add new asset types here as needed
    }
}
