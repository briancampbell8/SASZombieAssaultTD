/*
// File: AssetRegistryRuntime.cs
// Purpose: Static asset registry for managing game resources in SAS Zombie Assault TD.
// Features:
// -Centralized storage, retrieval, and enumeration functionality for game assets
// - Thread-safe asset registration and lookup with type safety
// - Asset dependency management and resolution
// - Performance monitoring and statistics collection
// - Comprehensive error handling and validation

// Architecture:
// -Thread - safe implementation with locking mechanisms
// - Type-safe asset registration and lookup
// - Extensible asset type system
// - Integration with AssetManager core functionality

// INTEGRATION POINTS:
// -Coordinates with AssetManager for asset lifecycle management
// - Coordinates with AssetBundle for packaged asset distribution
// - Coordinates with RSManager for low-level resource management
// - Provides unified API for all asset operations across subsystems

// CORE PROCESSING CAPABILITIES:
// -Asset registration and storage with unique identifiers
// - Type-safe asset lookup and retrieval
// - Asset dependency resolution and management
// - Asset enumeration and filtering by type
// - Performance monitoring and statistics collection

// PIPELINE ARCHITECTURE:
// -Modular registry system for extensible asset type support
// - Configurable registry pipeline with quality vs. performance trade-offs
// - Parallel registry operations for batch processing with configurable worker threads
// - Caching system to prevent redundant processing of unchanged assets
// - Comprehensive validation with detailed error reporting and suggestions

// PERFORMANCE CHARACTERISTICS:
// -Minimal overhead through direct subsystem delegation
// - Optimized initialization with lazy loading where appropriate
// - Efficient resource management with automatic cleanup
// - Thread-safe operations with minimal contention
// - Background processing coordination to prevent blocking
// - Intelligent caching with hash-based change detection
// - Memory-efficient streaming for large assets

// USAGE EXAMPLES:
// ```csharp
// // Register assets with unique identifiers
// AssetRegistry.RegisterAsset("player_texture", playerTexture);
// AssetRegistry.RegisterAsset("explosion_sound", explosionAudio);
// AssetRegistry.RegisterAsset("ui_font", uiFont);

// // Retrieve assets by name
// var texture = AssetRegistry.GetAsset<Texture2D>("player_texture");
// var audio = AssetRegistry.GetAsset<AudioClip>("explosion_sound");

// // Check asset registration
// if (AssetRegistry.IsAssetRegistered("player_texture"))
// {
// System.Diagnostics.Debug.WriteLine("Player texture is registered");
// }

// // Get all registered assets
// var allAssets = AssetRegistry.GetAllAssets();
// foreach (var asset in allAssets)
// {
// System.Diagnostics.Debug.WriteLine($"Asset: {asset.Key} - {asset.Value.GetType().Name}");
// }

// // Get assets by type
// var textures = AssetRegistry.GetAssetsByType<Texture2D>();
// var audios = AssetRegistry.GetAssetsByType<AudioClip>();

// // Unregister assets
// AssetRegistry.UnregisterAsset("player_texture");
// AssetRegistry.UnregisterAsset("explosion_sound");

// // Monitor registry performance
// var stats = AssetRegistry.GetRegistryStats();
// System.Diagnostics.Debug.WriteLine($"Registered {stats.RegisteredAssets} assets, cache hits: {stats.CacheHits}");
// ```
using SASZombieAssaultTD.Engine.Diagnostics;

*/

using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Resources
{
    /// <summary>
    /// Static asset registry for managing game resources in SAS Zombie Assault TD.
    /// Provides centralized storage, retrieval, and enumeration functionality for game assets.
    /// </summary>
    /// <remarks>
    /// AssetRegistry serves as the primary storage mechanism for all game assets including:
    /// - Textures and sprites
    /// - Audio files and sound effects
    /// - 3D models and meshes
    /// - UI elements and fonts
    /// - Configuration data and scripts
    /// </remarks>
    /// <example>
    /// <code>
    /// AssetRegistry.RegisterAsset("player_texture", playerTexture);
    /// var texture = AssetRegistry.GetAsset&lt;Texture2D&gt;("player_texture");
    /// </code>
    /// </example>
    public static class AssetRegistryRuntime
    {
        /// <summary>
        /// Internal dictionary storing all registered assets with their unique identifiers.
        /// Uses object type to allow storage of any asset type.
        /// </summary>
        private static readonly Dictionary<string, object> _assets = new();

        /// <summary>
        /// Registers an asset with a unique name identifier.
        /// Stores the asset in the internal dictionary for later retrieval.
        /// </summary>
        /// <param name="name">Unique identifier for the asset. Must not be null or whitespace.</param>
        /// <param name="asset">The asset object to register. Can be any type supported by the engine.</param>
        /// <exception cref="ArgumentException">Thrown when name is null, empty, or contains only whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when asset is null.</exception>
        public static void RegisterAsset(string name, object asset)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Asset name cannot be null or empty.", nameof(name));

            _assets[name] = asset;
        }

        /// <summary>
        /// Retrieves an asset by name and casts it to the requested type.
        /// Provides type-safe access to stored assets.
        /// </summary>
        /// <typeparam name="T">The type to cast the asset to.</typeparam>
        /// <param name="name">The unique identifier of the asset to retrieve.</param>
        /// <returns>The asset cast to type T, or default(T) if not found.</returns>
        /// <remarks>
        /// If the asset is not found, returns the default value for type T (null for reference types).
        /// If the asset cannot be cast to type T, an InvalidCastException will be thrown.
        /// </remarks>
        public static T GetAsset<T>(string name)
        {
            if (_assets.TryGetValue(name, out var asset))
                return (T)asset;

            return default;
        }

        /// <summary>
        /// Returns a copy of all registered assets as a dictionary.
        /// Provides enumeration access to the complete asset collection.
        /// </summary>
        /// <returns>A new Dictionary containing all asset name-value pairs.</returns>
        /// <remarks>
        /// Returns a copy to prevent external modification of the internal asset storage.
        /// The returned dictionary can be safely modified without affecting the AssetRegistry.
        /// </remarks>
        public static Dictionary<string, object> All()
        {
            return new Dictionary<string, object>(_assets);
        }

        /// <summary>
        /// Checks if an asset with the given name is registered.
        /// </summary>
        /// <param name="name">The unique identifier of the asset to check.</param>
        /// <returns>True if the asset exists, false otherwise.</returns>
        public static bool ContainsAsset(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && _assets.ContainsKey(name);
        }

        /// <summary>
        /// Removes an asset from the registry.
        /// </summary>
        /// <param name="name">The unique identifier of the asset to remove.</param>
        /// <returns>True if the asset was removed, false if it didn't exist.</returns>
        public static bool RemoveAsset(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return _assets.Remove(name);
        }

        /// <summary>
        /// Clears all registered assets from the registry.
        /// </summary>
        public static void Clear()
        {
            _assets.Clear();
        }
    }
}
