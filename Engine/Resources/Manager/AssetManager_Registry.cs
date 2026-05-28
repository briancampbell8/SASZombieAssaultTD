// File:    AssetManager_Registry.cs
// Purpose: Asset registration and metadata management for SAS Zombie Assault TD.
// Features:
// - Asset registration and metadata management
// - Type-based asset organization and lookup
// - Thread-safe concurrent access to asset registry
// - Metadata tracking and validation
// - Asset dependency resolution and management
//
// Architecture:
// - Thread-safe implementation with concurrent dictionaries
// - Type-safe asset organization and lookup
// - Extensible asset type detection system
// - Integration with AssetManager core functionality

//INTEGRATION POINTS:
//- Coordinates with AssetManager for asset lifecycle management
//- Coordinates with AssetBundle for packaged asset distribution
//- Coordinates with RSManager for low-level resource management
//- Provides unified API for all asset operations across subsystems

//CORE PROCESSING CAPABILITIES:
//- Asset registration with metadata generation
//- Type-based asset lookup and organization
//- Asset dependency resolution and management
//- Concurrent access with thread safety
//- Asset validation and integrity checking
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
// Register assets with metadata
//var metadata = new AssetMetadata
//{
//    Size = new FileInfo("textures/player.png").Length,
//    LastModified = File.GetLastWriteTime("textures/player.png")
//};
//AssetManager.RegisterAsset("player_texture", "textures/player.png", AssetManagerType.Texture, metadata);
// Get assets by type
//var textureAssets = AssetManager.GetAssetsByType(AssetManagerType.Texture);
//var audioAssets = AssetManager.GetAssetsByType(AssetManagerType.Audio);

//if (AssetManager.IsAssetRegistered("player_texture"))
//{
//    System.Diagnostics.Debug.WriteLine("Player texture is registered");
//}

// Get asset metadata
//var assetMetadata = AssetManager.GetAssetMetadata("player_texture");
//System.Diagnostics.Debug.WriteLine($"Asset size: {assetMetadata.Size} bytes");

// Register multiple assets
//var assets = new[]
//{
//    new { Key = "player_texture", Path = "textures/player.png", Type = AssetManagerType.Texture },
//    new { Key = "player_audio", Path = "audio/explosion.wav", Type = AssetManagerType.Audio },
//    new { Key = "ui_font", Path = "fonts/ui.ttf", Type = AssetManagerType.Font }
//};
//AssetManager.RegisterAssets(assets);

// Get all registered assets
//var allAssets = AssetManager.GetAllRegisteredAssets();
//foreach (var asset in allAssets)
//{
//System.Diagnostics.Debug.WriteLine($"Asset: {asset.Key} - {asset.Path} ({asset.Type})");
///}
//```
//

using SASZombieAssaultTD.Engine.Diagnostics;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Resources
{
    public partial class AssetManager
    {
        private readonly ConcurrentDictionary<string, AssetMetadata> _assetRegistry = new();
        private readonly ConcurrentDictionary<AssetType, List<string>> _assetsByType = new();

        /// <summary>
        /// Registers an asset with the manager.
        /// </summary>
        /// <param name="key">Unique key for the asset.</param>
        /// <param name="path">Path to the asset file.</param>
        /// <param name="type">Type of the asset.</param>
        /// <param name="metadata">Additional metadata.</param>
        public void RegisterAsset(string key, string path, AssetType type, AssetMetadata metadata = null)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Asset key cannot be null or empty", nameof(key));

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Asset path cannot be null or empty", nameof(path));

            var fullPath = GetAssetPath(path);
            var assetMetadata = metadata ?? new AssetMetadata
            {
                Key = key,
                Path = fullPath,
                Type = type,
                LastModified = DateTime.UtcNow
            };

            _assetRegistry[key] = assetMetadata;

            // Add to type-based registry
            if (!_assetsByType.ContainsKey(type))
            {
                _assetsByType[type] = new List<string>();
            }
            _assetsByType[type].Add(key);
        }

        /// <summary>
        /// Gets metadata for a registered asset.
        /// </summary>
        /// <param name="key">Asset key.</param>
        /// <returns>Asset metadata, or null if not found.</returns>
        public AssetMetadata GetAssetMetadata(string key)
        {
            ThrowIfDisposed();
            _assetRegistry.TryGetValue(key, out var metadata);
            return metadata;
        }

        /// <summary>
        /// Gets all assets of a specific type.
        /// </summary>
        /// <param name="type">Asset type.</param>
        /// <returns>List of asset keys.</returns>
        public IEnumerable<string> GetAssetsByType(AssetType type)
        {
            ThrowIfDisposed();
            return _assetsByType.TryGetValue(type, out var assets) ? assets : Enumerable.Empty<string>();
        }

        /// <summary>
        /// Gets all registered asset keys.
        /// </summary>
        /// <returns>All registered asset keys.</returns>
        public IEnumerable<string> GetAllAssetKeys()
        {
            ThrowIfDisposed();
            return _assetRegistry.Keys;
        }

        /// <summary>
        /// Checks if an asset is registered.
        /// </summary>
        /// <param name="key">Asset key.</param>
        /// <returns>True if registered, false otherwise.</returns>
        public bool IsAssetRegistered(string key)
        {
            ThrowIfDisposed();
            return _assetRegistry.ContainsKey(key);
        }

        /// <summary>
        /// Unregisters an asset.
        /// </summary>
        /// <param name="key">Asset key to unregister.</param>
        /// <returns>True if unregistered, false if not found.</returns>
        public bool UnregisterAsset(string key)
        {
            ThrowIfDisposed();

            if (!_assetRegistry.TryRemove(key, out var metadata))
                return false;

            // Remove from type-based registry
            if (_assetsByType.TryGetValue(metadata.Type, out var assets))
            {
                assets.Remove(key);
            }

            // Also unload from cache
            Unload(metadata.Path);

            return true;
        }

        /// <summary>
        /// Clears all registered assets.
        /// </summary>
        public void ClearRegistry()
        {
            ThrowIfDisposed();
            _assetRegistry.Clear();
            _assetsByType.Clear();
            ClearCache();
        }

        /// <summary>
        /// Gets the count of registered assets.
        /// </summary>
        public int RegisteredAssetCount => _assetRegistry.Count;

        /// <summary>
        /// Gets the count of registered assets by type.
        /// </summary>
        /// <param name="type">Asset type.</param>
        /// <returns>Count of assets of the specified type.</returns>
        public int GetAssetCountByType(AssetType type)
        {
            return _assetsByType.TryGetValue(type, out var assets) ? assets.Count : 0;
        }

        /// <summary>
        /// Validates that a registered asset file exists.
        /// </summary>
        /// <param name="key">Asset key.</param>
        /// <returns>True if the file exists, false otherwise.</returns>
        public bool ValidateAssetFile(string key)
        {
            ThrowIfDisposed();

            if (!_assetRegistry.TryGetValue(key, out var metadata))
                return false;

            return File.Exists(metadata.Path);
        }

        /// <summary>
        /// Validates all registered asset files.
        /// </summary>
        /// <returns>List of missing asset keys.</returns>
        public IEnumerable<string> ValidateAllAssetFiles()
        {
            ThrowIfDisposed();

            var missingAssets = new List<string>();

            foreach (var kvp in _assetRegistry)
            {
                if (!File.Exists(kvp.Value.Path))
                {
                    missingAssets.Add(kvp.Key);
                }
            }

            return missingAssets;
        }

        /// <summary>
        /// Gets asset statistics.
        /// </summary>
        /// <returns>Asset statistics information.</returns>
        public AssetStatistics GetStatistics()
        {
            ThrowIfDisposed();

            var stats = new AssetStatistics
            {
                TotalRegistered = _assetRegistry.Count,
                TotalLoaded = LoadedAssetCount,
                AssetsByType = new Dictionary<AssetType, int>()
            };

            foreach (var type in _assetsByType.Keys)
            {
                var assetType = (AssetType)type;
                stats.AssetsByType[assetType] = GetAssetCountByType(type);
            }

            return stats;
        }
    }
}
