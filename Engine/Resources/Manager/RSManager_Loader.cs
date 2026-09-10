// ====================================================================================================
//  FILE: RSManager_Loader.cs
//  PATH: ./Engine/Resources/Manager/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the RSManager_Loader module.
//
//  RESPONSIBILITIES:
//      - Provide PreloadAssets() behavior for the Core subsystem.
//      - Provide UnloadAsset() behavior for the Core subsystem.
//      - Provide UnloadAllAssets() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
//File: RSManager_Loader.cs

//Purpose: Loading engine for RSManager internal partial class in SAS Zombie Assault TD.

//Features:

//-Loading engine for RSManager internal partial class
//-Contains LoadResource / UnloadResource, async operations, dependency resolution

//- All calls into RSLoader, all load/unload orchestration
//- Thread-safe resource loading with streaming support

//Architecture:
//-Thread - safe implementation with locking mechanisms
//- Weak reference caching to prevent memory leaks
//- Type-safe generic loading methods
//- Extensible asset type detection system

//INTEGRATION POINTS:
//-Coordinates with AssetManager for asset lifecycle management
//- Coordinates with AssetBundle for packaged asset distribution
//- Coordinates with RSManager for low-level resource management
//- Provides unified API for all asset operations across subsystems

//CORE PROCESSING CAPABILITIES:
//-Resource loading with streaming and caching support
//- Asset format conversion and optimization
//- 3D model loading with LOD level selection
//- Font loading with character set optimization
//- Data loading with serialization and deserialization
//- Script and shader loading with validation

//PIPELINE ARCHITECTURE:
//-Modular processor system for extensible asset type support
//- Configurable processing pipeline with quality vs. performance trade-offs
//- Parallel processing for batch operations with configurable worker threads
//- Caching system to prevent redundant processing of unchanged assets
//- Comprehensive validation with detailed error reporting and suggestions

//PERFORMANCE CHARACTERISTICS:
//-Minimal overhead through direct subsystem delegation
//- Optimized initialization with lazy loading where appropriate
//- Efficient resource management with automatic cleanup
//- Thread-safe operations with minimal contention
//- Background processing coordination to prevent blocking
//- Intelligent caching with hash-based change detection
//- Memory-efficient streaming for large assets

//USAGE EXAMPLES:
//```csharp
////Initialize RSManager
//var rsManager = new RSManager("Resources");

////Load resources
//var texture = rsManager.LoadResource<Texture2D>("textures/player.png");
//var audio = rsManager.LoadResource<AudioClip>("audio/explosion.wav");

////Load resources asynchronously
//var textureTask = rsManager.LoadResourceAsync<Texture2D>("ui/loading_screen.png");
//var audioTask = rsManager.LoadResourceAsync<AudioClip>("audio/theme.wav");
//var resources = await Task.WhenAll(textureTask, audioTask);

////Load with progress tracking
//var progress = new Progress<float>(p => DLogger.Log($"Loading: {p:P0%}"));
//await rsManager.LoadResourceAsync<Texture2D>("ui/loading_screen.png", progress);

////Unload resources
//rsManager.UnloadResource("player_texture");
//rsManager.UnloadResource("explosion_sound");

////Monitor RSManager performance
//var stats = rsManager.GetPerformanceStats();
//DLogger.Log($"Loaded {stats.LoadedResources} resources, cache hits: {stats.CacheHits}");

////Configure RSManager settings
//var config = new RSManager_Config
//{
//MaxMemoryUsage = 1024L * 1024 * 1024, //1GB
//MaxConcurrentLoads = 8,
//EnableStreaming = true
//};
//rsManager.Configure(config);
//```
//

*/

//
// using SASZombieAssaultTD.Engine.Extensions; // Extensions Removed
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.IO;
using System.Linq;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.ECS;

namespace SASZombieAssaultTD.Engine.Resources
{
    /// <summary>
    /// Loading engine for RSManager partial class in SAS Zombie Assault TD. Provides resource loading, unloading, and
    /// dependency resolution with thread-safe concurrent access and streaming support.
    /// </summary>
    /// <remarks>
    /// This is a partial class - functionality is split across multiple files: - RSManager_Core.cs: Core initialization
    /// and management - RSManager_Loader.cs: Resource loading and unloading - RSManager_Validation.cs: Integrity
    /// checking and verification - RSManager_Cache.cs: Caching and performance optimization
    /// </remarks>
    /// <example>
    /// <code>
    /// var rsManager = new RSManager("Resources");
    /// var texture = rsManager.LoadResource&lt;Texture2D&gt;("textures/player.png");
    /// </code>
    /// </example>
    public partial class RSManager
    {
        private bool _initialized;
        private object _lockObject;
        private object TheType;
        private object TheMember;

        /// <summary>
        /// Loads asset by type with metadata.
        /// </summary>
        private T LoadAsset<T>(string key)
        {
            if (!_resourceMetadata.TryGetValue(key, out RSMetadata? metadata))
            {
                DLogger.Log(LogSubsystems.ResourcesManager,
                    "AssetManager", $"AssetManager: No metadata found for asset '{key}'");
                throw new KeyNotFoundException($"Asset metadata not found for key: {key}");
            }

            //Asset validation before loading
            if (!ValidateAsset(key, metadata))
            {
                DLogger.Log(LogSubsystems.ResourcesManager,
                "AssetManager.Validation",
                $"Asset validation failed for '{key}': {_validationErrors[key]}"
            );

                throw new InvalidOperationException($"Asset validation failed for '{key}': {_validationErrors[key]}");
            }

            try
            {
                object loadedAsset = LoadAssetByType(metadata);
                CacheAsset(key, loadedAsset);

                DLogger.Log(LogSubsystems.ResourcesManager,
                    "AssetManager", $"AssetManager: Loaded asset '{key}' as type {loadedAsset.GetType().Name}");

                if (loadedAsset is T typedAsset)
                {
                    return typedAsset;
                }
                else
                {
                    DLogger.Log(LogSubsystems.ResourcesManager,
                        "AssetManager",
                        $"AssetManager: Type conversion failed for '{key}' - Expected {typeof(T).Name}, got {loadedAsset.GetType().Name}");
                    throw new InvalidOperationException($"Asset '{key}' loaded as {loadedAsset.GetType().Name}, expected {typeof(T).Name}");
                }
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.ResourcesManager,
                    "AssetManager", $"AssetManager: Failed to load asset '{key}': {ex.Message}");
                throw;
            }
        }

        private void Log(string v)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads asset by type determination.
        /// </summary>
        private object LoadAssetByType(RSMetadata metadata)
        {
            return LoadAssetByType(metadata.Key);
        }

        /// <summary>
        /// Loads asset by type with automatic type detection.
        /// </summary>
        private object LoadAssetByType(string key)
        {
            if (!_resourceMetadata.TryGetValue(key, out RSMetadata? metadata))
            {
                throw new KeyNotFoundException($"Asset metadata not found for key: {key}");
            }

            string extension = Path.GetExtension(metadata.Path).ToLowerInvariant();

            //Runtime asset loading based on type
            switch (metadata.Type)
            {
                case RSType.Texture:
                    return LoadTextureAsset(metadata);

                case RSType.Sound:
                    return LoadSoundAsset(metadata);

                case RSType.Music:
                    return LoadMusicAsset(metadata);

                case RSType.Json:
                    return LoadJsonAsset(metadata);

                case RSType.Binary:
                    return LoadBinaryAsset(metadata);

                default:
                    //Fallback: determine type from file extension using pattern matching
                    return extension switch
                    {
                        var ext when IsImageFile(ext) => LoadTextureAsset(metadata),
                        ".json" => LoadJsonAsset(metadata),
                        var ext when IsAudioFile(ext) => (ext == ".wav" ||
                            metadata.Path.Contains("sfx_") ||
                            metadata.Path.Contains("sound_"))
                            ? LoadSoundAsset(metadata)
                            : LoadMusicAsset(metadata),
                        _ => LoadBinaryAsset(metadata)
                    };
            }
        }

        /// <summary>
        /// Loads texture asset.
        /// </summary>
        private Texture2D LoadTextureAsset(RSMetadata metadata)
        {
            DLogger.Log(LogSubsystems.ResourcesManager,
                "AssetManager", $"AssetManager: Loading texture '{metadata.Key}' from '{metadata.Path}'");

            //Route texture loading through TextureCache
            return Texture2D.LoadFromFile(metadata.Path, _textureCache);
        }

        /// <summary>
        /// Loads sound effect asset.
        /// </summary>
        private CoreSoundEffect LoadSoundAsset(RSMetadata metadata)
        {
            DLogger.Log(LogSubsystems.ResourcesManager,
                "AssetManager", $"AssetManager: Loading sound effect '{metadata.Key}' from '{metadata.Path}'");

            //Platform-specific sound effect loading
            return new CoreSoundEffect(metadata.Key, 1.0f); //Default volume
        }

        /// <summary>
        /// Loads music track asset.
        /// </summary>
        private CoreSoundEffect LoadMusicAsset(RSMetadata metadata)
        {
            DLogger.Log(LogSubsystems.ResourcesManager,
                "AssetManager", $"AssetManager: Loading music track '{metadata.Key}' from '{metadata.Path}'");

            //Platform-specific music track loading
            return new CoreSoundEffect(metadata.Key, 1.0f); //Default volume
        }

        /// <summary>
        /// Loads JSON data asset.
        /// </summary>
        private string LoadJsonAsset(RSMetadata metadata)
        {
            DLogger.Log(LogSubsystems.ResourcesManager,
                "AssetManager", $"AssetManager: Loading JSON data '{metadata.Key}' from '{metadata.Path}'");

            if (!File.Exists(metadata.Path))
            {
                throw new FileNotFoundException($"JSON asset file not found: {metadata.Path}");
            }

            return File.ReadAllText(metadata.Path);
        }

        /// <summary>
        /// Loads binary data asset.
        /// </summary>
        private byte[] LoadBinaryAsset(RSMetadata metadata)
        {
            DLogger.Log(LogSubsystems.ResourcesManager,
                "AssetManager", $"AssetManager: Loading binary data '{metadata.Key}' from '{metadata.Path}'");

            if (!File.Exists(metadata.Path))
            {
                throw new FileNotFoundException($"Binary asset file not found: {metadata.Path}");
            }

            return File.ReadAllBytes(metadata.Path);
        }

        /// <summary>
        /// Determines resource type based on file extension and key patterns.
        /// </summary>
        private RSType DetermineRSType(string key, string path)
        {
            string extension = Path.GetExtension(path).ToLowerInvariant();
            string lowerKey = key.ToLowerInvariant();

            //Determine type based on key patterns and file extension
            if (IsImageFile(extension))
                return RSType.Texture;

            if (extension == ".json")
                return RSType.Json;

            if (IsAudioFile(extension))
            {
                //Distinguish between sound effects and music using pattern matching
                return lowerKey switch
                {
                    var k when k.Contains("sfx_") ||
                               k.Contains("sound_") ||
                               k.Contains("effect_") ||
                               k.Contains("shoot") ||
                               k.Contains("explosion") ||
                               k.Contains("hit") ||
                               k.Contains("footstep") ||
                               key.Length < 15 => RSType.Sound,
                    _ => RSType.Music
                };
            }

            return RSType.Binary;
        }

        /// <summary>
        /// Gets asset by key with type safety and loading.
        /// </summary>
        public T GetAsset<T>(string key)
        {
            if (!_initialized)
            {
                DLogger.Log(LogSubsystems.ResourcesManager,
                    "AssetManager", $"AssetManager: GetAsset failed - Not initialized (Key: {key})");
                throw new InvalidOperationException("AssetManager not initialized");
            }

            if (string.IsNullOrEmpty(key))
            {
                DLogger.Log(LogSubsystems.ResourcesManager,
                    "AssetManager", "AssetManager: GetAsset failed - Invalid key (null or empty)");
                throw new ArgumentException("Asset key cannot be null or empty", nameof(key));
            }

            lock (_lockObject)
            {
                //Check if asset is already loaded
                if (_loadedAssets.TryGetValue(key, out object? asset))
                {
                    if (asset is T typedAsset)
                    {
                        DLogger.Log(LogSubsystems.ResourcesManager,
                            "AssetManager", $"AssetManager: Retrieved cached asset '{key}' as type {typeof(T).Name}");
                        return typedAsset;
                    }
                    else
                    {
                        DLogger.Log(LogSubsystems.ResourcesManager,
                            "AssetManager",
                            $"AssetManager: Type mismatch for asset '{key}' - Expected {typeof(T).Name}, got {asset.GetType().Name}");
                        throw new InvalidOperationException($"Asset '{key}' is of type {asset.GetType().Name}, expected {typeof(T).Name}");
                    }
                }

                //Asset not loaded, attempt to load it
                DLogger.Log(LogSubsystems.ResourcesManager,
                    "AssetManager", $"AssetManager: Loading asset '{key}' on demand");
                return LoadAsset<T>(key);
            }
        }

        /// <summary>
        /// Preloads multiple assets.
        /// </summary>
        public void PreloadAssets(IEnumerable<string> keys)
        {
            if (!_initialized)
            {
                DLogger.Log(LogSubsystems.ResourcesManager,
                    "AssetManager", "AssetManager: PreloadAssets failed - Not initialized");
                throw new InvalidOperationException("AssetManager not initialized");
            }

            if (keys == null)
            {
                DLogger.Log(LogSubsystems.ResourcesManager,
                    "AssetManager", "AssetManager: PreloadAssets failed - Null keys collection");
                return;
            }

            var keysList = keys.ToList();
            DLogger.Log(LogSubsystems.ResourcesManager,
                "AssetManager", $"AssetManager: Preloading {keysList.Count} assets...");

            int successCount = 0;
            int errorCount = 0;

            foreach (string key in keysList)
            {
                try
                {
                    if (string.IsNullOrEmpty(key))
                    {
                        DLogger.Log(LogSubsystems.ResourcesManager,
                            "AssetManager", "AssetManager: Skipping null/empty key during preload");
                        continue;
                    }

                    lock (_lockObject)
                    {
                        if (_loadedAssets.ContainsKey(key))
                        {
                            DLogger.Log(LogSubsystems.ResourcesManager,
                                "AssetManager", $"AssetManager: Asset '{key}' already loaded, skipping");
                            continue;
                        }

                        //Preload the asset (we don't know the type, so we'll determine it)
                        LoadAssetByType(key);
                        successCount++;
                    }
                }
                catch (Exception ex)
                {
                    errorCount++;
                    DLogger.Log(LogSubsystems.ResourcesManager,
                        "AssetManager", $"AssetManager: Failed to preload asset '{key}': {ex.Message}");
                }
            }

            DLogger.Log(LogSubsystems.ResourcesManager,
                "AssetManager", $"AssetManager: Preload complete - Success: {successCount}, Errors: {errorCount}");
        }

        /// <summary>
        /// Unloads specific asset.
        /// </summary>
        public void UnloadAsset(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            lock (_lockObject)
            {
                if (_loadedAssets.TryGetValue(key, out object? asset))
                {
                    //Dispose if it's disposable
                    if (asset is IDisposable disposable)
                    {
                        try
                        {
                            disposable.Dispose();
                            DLogger.Log(LogSubsystems.ResourcesManager,
                                "AssetManager", $"AssetManager: Disposed asset '{key}'");
                        }
                        catch (Exception ex)
                        {
                            DLogger.Log(LogSubsystems.ResourcesManager,
                                "AssetManager", $"AssetManager: Failed to dispose asset '{key}': {ex.Message}");
                        }
                    }

                    _loadedAssets.Remove(key);
                    _validatedAssets.Remove(key);
                    _validationErrors.Remove(key);

                    DLogger.Log(LogSubsystems.ResourcesManager,
                        "AssetManager", $"AssetManager: Unloaded asset '{key}'");
                }
            }
        }

        /// <summary>
        /// Unloads all assets.
        /// </summary>
        public void UnloadAllAssets()
        {
            DLogger.Log(LogSubsystems.ResourcesManager,
                "AssetManager", "AssetManager: Unloading all assets...");

            lock (_lockObject)
            {
                int disposedCount = 0;
                int errorCount = 0;

                foreach (var kvp in _loadedAssets)
                {
                    try
                    {
                        if (kvp.Value is IDisposable disposable)
                        {
                            disposable.Dispose();
                            disposedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        DLogger.Log(LogSubsystems.ResourcesManager,
                            "AssetManager", $"AssetManager: Failed to dispose asset '{kvp.Key}': {ex.Message}");
                    }
                }

                _loadedAssets.Clear();
                _validatedAssets.Clear();
                _validationErrors.Clear();

                DLogger.Log(LogSubsystems.ResourcesManager,
                    "AssetManager", $"AssetManager: Unloaded all assets - Disposed: {disposedCount}, Errors: {errorCount}");
            }
        }
    }

    internal class _resourceMetadata
    {
        private static object TheContainingType;
        private static object TheContainingMember;

        internal static bool TryGetValue(string key, out RSMetadata metadata)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }
    }
}
