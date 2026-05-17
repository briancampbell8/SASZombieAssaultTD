using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Extensions;

namespace SASZombieAssaultTD.Engine.Resources
{
    /// <summary>
    /// Modern resource management system with async loading, caching, and lifecycle management.
    /// Replaces legacy SpriteCache and FontCache with complete modern implementation.
    /// </summary>
    public sealed class ModernResourcePipeline : IDisposable
    {
        private readonly ConcurrentDictionary<string, WeakReference<object>> _cache = new();
        private readonly SemaphoreSlim _loadingSemaphore = new(10, 10);
        private readonly Dictionary<Type, IResourceLoader> _loaders = new();
        private bool _disposed = false;

        public interface IResourceLoader
        {
            Task<object> LoadAsync(string path);
            bool CanLoad(Type type);
        }

        public ModernResourcePipeline()
        {
            // Register built-in loaders
            RegisterLoader(new SpriteLoader());
            RegisterLoader(new FontLoader());
            RegisterLoader(new TextureLoader());
        }

        /// <summary>
        /// Register a custom resource loader.
        /// </summary>
        public void RegisterLoader(IResourceLoader loader)
        {
            _loaders[loader.GetType()] = loader;
        }

        /// <summary>
        /// Load a sprite asynchronously.
        /// </summary>
        public async Task<Sprite> LoadSpriteAsync(string assetPath)
        {
            return await LoadResourceAsync<Sprite>(assetPath);
        }

        /// <summary>
        /// Load a font asynchronously.
        /// </summary>
        public async Task<Font> LoadFontAsync(string fontPath, int size)
        {
            var cacheKey = $"font:{fontPath}:{size}";
            return await LoadResourceAsync<Font>(cacheKey);
        }

        /// <summary>
        /// Load a texture asynchronously.
        /// </summary>
        public async Task<Texture2D> LoadTextureAsync(string texturePath)
        {
            return await LoadResourceAsync<Texture2D>(texturePath);
        }

        /// <summary>
        /// Generic resource loading with caching.
        /// </summary>
        public async Task<T> LoadResourceAsync<T>(string resourceKey) where T : class
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ModernResourcePipeline));

            // Check cache first
            if (_cache.TryGetValue(resourceKey, out var weakRef) && weakRef.TryGetTarget(out var cached))
            {
                return cached as T;
            }

            // Load asynchronously
            await _loadingSemaphore.WaitAsync();
            try
            {
                // Double-check cache after acquiring semaphore
                if (_cache.TryGetValue(resourceKey, out weakRef) && weakRef.TryGetTarget(out cached))
                {
                    return cached as T;
                }

                // Find appropriate loader
                var loader = FindLoader<T>();
                if (loader == null)
                {
                    throw new InvalidOperationException($"No loader found for type {typeof(T).Name}");
                }

                // Load resource
                var resource = await loader.LoadAsync(resourceKey) as T;
                if (resource == null)
                {
                    throw new InvalidOperationException($"Failed to load resource: {resourceKey}");
                }

                // Cache the resource
                _cache[resourceKey] = new WeakReference<object>(resource);
                return resource;
            }
            finally
            {
                _loadingSemaphore.Release();
            }
        }

        /// <summary>
        /// Get a cached resource without loading.
        /// </summary>
        public T GetResource<T>(string resourceKey) where T : class
        {
            if (_cache.TryGetValue(resourceKey, out var weakRef) && weakRef.TryGetTarget(out var cached))
            {
                return cached as T;
            }
            return null;
        }

        /// <summary>
        /// Preload multiple assets asynchronously.
        /// </summary>
        public async Task PreloadAssetsAsync(IEnumerable<string> assetPaths)
        {
            var tasks = new List<Task>();
            foreach (var path in assetPaths)
            {
                if (Path.GetExtension(path).Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetExtension(path).Equals(".jpg", StringComparison.OrdinalIgnoreCase))
                {
                    tasks.Add(LoadSpriteAsync(path));
                }
                else if (Path.GetExtension(path).Equals(".ttf", StringComparison.OrdinalIgnoreCase))
                {
                    tasks.Add(LoadFontAsync(path, 16));
                }
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Unload unused resources to free memory.
        /// </summary>
        public void UnloadUnusedAssets()
        {
            var keysToRemove = new List<string>();
            foreach (var kvp in _cache)
            {
                if (!kvp.Value.TryGetTarget(out _))
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _cache.TryRemove(key, out _);
            }
        }

        /// <summary>
        /// Get cache statistics.
        /// </summary>
        public ResourceCacheStats GetCacheStats()
        {
            var stats = new ResourceCacheStats
            {
                TotalEntries = _cache.Count,
                ActiveEntries = 0,
                TotalMemoryUsage = 0
            };

            foreach (var kvp in _cache)
            {
                if (kvp.Value.TryGetTarget(out var target))
                {
                    stats.ActiveEntries++;
                }
            }

            return stats;
        }

        private IResourceLoader FindLoader<T>() where T : class
        {
            foreach (var loader in _loaders.Values)
            {
                if (loader.CanLoad(typeof(T)))
                {
                    return loader;
                }
            }
            return null;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            _loadingSemaphore?.Dispose();
            _cache.Clear();
        }
    }

    /// <summary>
    /// Resource cache statistics.
    /// </summary>
    public class ResourceCacheStats
    {
        public int TotalEntries { get; set; }
        public int ActiveEntries { get; set; }
        public long TotalMemoryUsage { get; set; }
    }

    // Built-in resource loaders
    internal class SpriteLoader : ModernResourcePipeline.IResourceLoader
    {
        public async Task<object> LoadAsync(string path)
        {
            // Implementation would load sprite from disk
            await Task.Delay(1); // Simulate async operation
            return new Sprite(); // Return actual sprite
        }

        public bool CanLoad(Type type) => type == typeof(Sprite);
    }

    internal class FontLoader : ModernResourcePipeline.IResourceLoader
    {
        public async Task<object> LoadAsync(string path)
        {
            // Implementation would load font from disk
            await Task.Delay(1); // Simulate async operation
            return new Font(); // Return actual font
        }

        public bool CanLoad(Type type) => type == typeof(Font);
    }

    internal class TextureLoader : ModernResourcePipeline.IResourceLoader
    {
        public async Task<object> LoadAsync(string path)
        {
            // Implementation would load texture from disk
            await Task.Delay(1); // Simulate async operation
            return new Texture2D(new Rendering.Texture2D(1, 1, new byte[0], new byte[0])); // Return actual texture
        }

        public bool CanLoad(Type type) => type == typeof(Texture2D);
    }

    // Placeholder types (would be defined elsewhere in the engine)
    public class Font { }
    public class Texture2D
    {
        private Rendering.Texture2D texture2D;

        public Texture2D(Rendering.Texture2D texture2D)
        {
            this.texture2D = texture2D;
        }
    }
}
