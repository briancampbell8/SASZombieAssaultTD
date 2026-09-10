// ====================================================================================================
//  FILE: AssetLoader.cs
//  PATH: ./Engine/Resources/Assets/
//  MODULE: Core
//
//  CHANGE LOG:
//      [2026‑07‑29 / BDC]:
//          - Removed legacy CPU Texture2D loading pipeline.
//          - Integrated GPU texture loading via TextureManager.AddFromPng().
//          - Updated type detection to use ID3D11Texture2D instead of Render.Textures.Texture2D.
//          - Updated metadata classification for GPU textures.
//          - Removed placeholder Texture2D.Create() usage.
//          - Removed obsolete audio references.
//          - Updated LoadTextureAsync() to load PNG bytes and create GPU textures.
//
// ====================================================================================================

using System;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using SASZombieAssaultTD.Engine.Resources;
using Vortice.Direct3D11;
using Font = SASZombieAssaultTD.Engine.TextRendering.Font;
using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.Render
{
    public class AssetLoader
    {
        private readonly Dictionary<string, WeakReference> _loadedAssets = new();
        private readonly Dictionary<string, AssetMetadata> _assetMetadata = new();
        private readonly object _lock = new();
        private readonly string _assetRootPath;
        private D3D11DeviceCore _deviceCore;

        public AssetLoader(string assetRootPath = "Assets")
        {
            _assetRootPath = assetRootPath ?? throw new ArgumentNullException(nameof(assetRootPath));

            if (!Directory.Exists(_assetRootPath))
            {
                Directory.CreateDirectory(_assetRootPath);
                DLogger.Log(LogSubsystems.ResourcesAssets, "Info", $"AssetLoader: Created asset directory at {_assetRootPath}");
            }
        }

        public async Task<T> LoadAssetAsync<T>(string assetPath) where T : class
        {
            if (string.IsNullOrWhiteSpace(assetPath))
                throw new ArgumentException("Asset path cannot be null or empty", nameof(assetPath));

            var fullPath = Path.Combine(_assetRootPath, assetPath);

            if (TryGetFromCache<T>(assetPath, out var cachedAsset))
            {
                DLogger.Log(LogSubsystems.ResourcesAssets, "Debug", $"AssetLoader: Retrieved {assetPath} from cache");
                return cachedAsset;
            }

            var asset = await LoadAssetByType<T>(fullPath, assetPath);

            CacheAsset(assetPath, asset);

            var metadata = CreateMetadata<T>(assetPath, fullPath);

            lock (_lock)
                _assetMetadata[assetPath] = metadata;

            DLogger.Log(LogSubsystems.ResourcesAssets, "Info", $"AssetLoader: Loaded asset {assetPath} ({typeof(T).Name})");
            return asset;
        }

        public async Task<Dictionary<string, object>> LoadAssetsAsync(IEnumerable<string> assetPaths)
        {
            if (assetPaths == null)
                throw new ArgumentNullException(nameof(assetPaths));

            var loadTasks = assetPaths.Select(async path =>
            {
                try
                {
                    var asset = await LoadAssetAsync<object>(path);
                    return new { Path = path, Asset = asset };
                }
                catch (Exception ex)
                {
                    DLogger.Log(LogSubsystems.ResourcesAssets, "Error", $"AssetLoader: Failed to load {path}: {ex.Message}");
                    return new { Path = path, Asset = (object)null };
                }
            });

            var results = await Task.WhenAll(loadTasks);

            return results.Where(r => r.Asset != null)
                          .ToDictionary(r => r.Path, r => r.Asset);
        }

        public void UnloadAsset(string assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath)) return;

            lock (_lock)
            {
                _loadedAssets.Remove(assetPath);
                _assetMetadata.Remove(assetPath);
            }

            DLogger.Log(LogSubsystems.ResourcesAssets, "Info", $"AssetLoader: Unloaded asset {assetPath}");
        }

        public AssetMetadata GetMetadata(string assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
                return null;

            lock (_lock)
            {
                return _assetMetadata.TryGetValue(assetPath, out var metadata) ? metadata : null;
            }
        }

        public IEnumerable<AssetMetadata> GetAllMetadata()
        {
            lock (_lock)
                return _assetMetadata.Values.ToList();
        }

        public void ClearCache()
        {
            lock (_lock)
            {
                _loadedAssets.Clear();
                _assetMetadata.Clear();
            }

            DLogger.Log(LogSubsystems.ResourcesAssets, "Info", "AssetLoader: Cleared all cached assets");
        }

        public void CollectGarbage()
        {
            lock (_lock)
            {
                var deadKeys = _loadedAssets.Where(kvp => !kvp.Value.IsAlive)
                                            .Select(kvp => kvp.Key)
                                            .ToList();

                foreach (var key in deadKeys)
                {
                    _loadedAssets.Remove(key);
                    _assetMetadata.Remove(key);
                }
            }

            DLogger.Log(LogSubsystems.ResourcesAssets, "Info", $"AssetLoader: Garbage collection completed");
        }

        private bool TryGetFromCache<T>(string assetPath, out T asset) where T : class
        {
            lock (_lock)
            {
                if (_loadedAssets.TryGetValue(assetPath, out var weakRef) && weakRef.IsAlive)
                {
                    asset = weakRef.Target as T;
                    return asset != null;
                }
            }

            asset = null;
            return false;
        }

        private void CacheAsset(string assetPath, object asset)
        {
            lock (_lock)
                _loadedAssets[assetPath] = new WeakReference(asset);
        }

        private async Task<T> LoadAssetByType<T>(string fullPath, string assetPath) where T : class
        {
            var type = typeof(T);
            var extension = Path.GetExtension(fullPath).ToLowerInvariant();

            if (typeof(ID3D11Texture2D).IsAssignableFrom(type))
            {
                return await LoadTextureAsync(fullPath) as T;
            }
            else if (typeof(Font).IsAssignableFrom(type))
            {
                return await LoadFontAsync(fullPath) as T;
            }
            else if (typeof(string).IsAssignableFrom(type))
            {
                return await LoadTextAsync(fullPath) as T;
            }
            else if (extension == ".json")
            {
                return await LoadJsonAsync<T>(fullPath) as T;
            }
            else
            {
                throw new NotSupportedException($"Asset type {type.Name} with extension {extension} is not supported");
            }
        }

        private async Task<object> LoadTextureAsync(string path)
        {
            byte[] pngBytes = await File.ReadAllBytesAsync(path);

            // Fix: Pass your class's device variable here (adjust '_deviceCore' to match your actual field name)
            var textureManager = new TextureManager(_deviceCore);
            var gpuTexture = GetGpuTexture(textureManager, path, pngBytes);

            return gpuTexture;
        }


        static object GetGpuTexture(TextureManager textureManager, string path, byte[] pngBytes)
        {
            return textureManager.AddFromPng(path, pngBytes);
        }
        private async Task<object> LoadFontAsync(string path)
        {
            await Task.Delay(1);
            return new Font();
        }

        private async Task<object> LoadTextAsync(string path)
        {
            return await File.ReadAllTextAsync(path);
        }

        private async Task<object> LoadJsonAsync<T>(string path)
        {
            var json = await File.ReadAllTextAsync(path);
            return Activator.CreateInstance<T>();
        }

        private AssetMetadata CreateMetadata<T>(string assetPath, string fullPath)
        {
            var fileInfo = new FileInfo(fullPath);
            var type = DetermineAssetType<T>();

            return new AssetMetadata
            {
                Key = assetPath,
                Name = Path.GetFileNameWithoutExtension(assetPath),
                Path = assetPath,
                Type = type,
                Format = fileInfo.Extension.TrimStart('.'),
                SizeBytes = fileInfo.Exists ? fileInfo.Length : 0,
                LastModified = fileInfo.Exists ? fileInfo.LastWriteTime : DateTime.MinValue,
                IsCritical = IsCriticalAsset(assetPath)
            };
        }

        private AssetType DetermineAssetType<T>()
        {
            var type = typeof(T);

            if (typeof(ID3D11Texture2D).IsAssignableFrom(type))
                return AssetType.Texture;
            else if (typeof(Font).IsAssignableFrom(type))
                return AssetType.Font;
            else if (typeof(string).IsAssignableFrom(type))
                return AssetType.Json;
            else
                return AssetType.Unknown;
        }

        private bool IsCriticalAsset(string assetPath)
        {
            var criticalPatterns = new[]
            {
                "ui/",
                "fonts/",
                "sounds/",
                "textures/ui/"
            };

            return criticalPatterns.Any(pattern =>
                assetPath.StartsWith(pattern, StringComparison.OrdinalIgnoreCase));
        }

        public void Dispose()
        {
            ClearCache();
        }

        internal async Task<T> LoadAsync<T>(string path) where T : class
        {
            return await LoadAssetAsync<T>(path);
        }
    }
}
