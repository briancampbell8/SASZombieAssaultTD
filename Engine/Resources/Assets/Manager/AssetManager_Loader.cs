//============================================================================
//File:        AssetManager_Loader.cs
//Path:        E:\BDC\Projects\SASZombieAssaultTD\Engine\Resources\Assets\Manager\
//Program:     AssetManager_Loader
//Author:      BDC
//Created:     2026-05-29
//
//Purpose:
//     Loader subsystem for the new AssetManager. Implements the high‑level
//     asset loading pipeline for the Asset subsystem, including caching,
//     async/sync loading, handle creation, and concurrency control.
//
//Notes:
//     • RSHandle is non‑generic. All typed access must use RSHandle.As<T>().
//     • All RSHandle<T> usages have been removed and corrected.
//============================================================================

using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

//
using SASZombieAssaultTD.Engine.Resources;
using SASZombieAssaultTD.Engine.Interfaces;

using EngineResources = SASZombieAssaultTD.Engine.Resources;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Resources
{
    public partial class AssetManager
    {
        private readonly Dictionary<string, object> _loadedAssets = new();
        private readonly Dictionary<string, Task<object>> _loadingTasks = new();
        private readonly Dictionary<string, RSHandle> _handles = new();
        private SemaphoreSlim? _loadingSemaphore;

        private void EnsureLoaderInitialized()
        {
            if (_loadingSemaphore != null)
                return;

            var maxLoads = _config.MaxConcurrentLoads <= 0
                ? 1
                : _config.MaxConcurrentLoads;

            _loadingSemaphore = new SemaphoreSlim(maxLoads);

            Debug.WriteLine($"[AssetManager] Loader initialized (MaxConcurrentLoads={maxLoads})");
        }

        //====================================================================
        //Handle-Based API
        //====================================================================

        public RSHandle LoadAsset<T>(string key, AssetPriority priority = AssetPriority.Normal)
            where T : class
        {
            ThrowIfDisposed();
            EnsureLoaderInitialized();

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Asset key cannot be null or empty", nameof(key));

            var fullPath = GetAssetPath(key);

            Debug.WriteLine($"[AssetManager] LoadAsset<T> requested (Key={key}, FullPath={fullPath}, Priority={priority})");

            if (_handles.TryGetValue(fullPath, out var existing))
            {
                Debug.WriteLine($"[AssetManager] Reusing existing handle (Key={key})");
                return existing;
            }

            var handle = new RSHandle(
                new RSKey(typeof(T), key),
                instance: null! //instance will be populated later
            );

            _handles[fullPath] = handle;

            Debug.WriteLine($"[AssetManager] Created new handle (Key={key}, Priority={priority})");

            return handle;
        }

        internal RSHandle? GetHandle(string key)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(key))
                return null;

            var fullPath = GetAssetPath(key);

            _handles.TryGetValue(fullPath, out var handle);

            Debug.WriteLine($"[AssetManager] GetHandle (Key={key}) => {(handle == null ? "NULL" : "FOUND")}");

            return handle;
        }

        public void UnloadAsset(string key, bool force = false)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(key))
                return;

            var fullPath = GetAssetPath(key);

            Debug.WriteLine($"[AssetManager] UnloadAsset (Key={key}, Force={force})");

            _handles.Remove(fullPath);
            _loadedAssets.Remove(fullPath);

            Debug.WriteLine($"[AssetManager] UnloadAsset completed (Key={key})");
        }

        public AssetMetadata GetMetadata(string key)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(key))
                return null;

            var fullPath = GetAssetPath(key);

            if (_handles.TryGetValue(fullPath, out var handle))
            {
                Debug.WriteLine($"[AssetManager] GetMetadata (Key={key}) => FOUND");
                return null;
            }

            Debug.WriteLine($"[AssetManager] GetMetadata (Key={key}) => NULL");
            return null;
        }

        public int CollectGarbage(bool aggressive = false)
        {
            ThrowIfDisposed();

            Debug.WriteLine($"[AssetManager] CollectGarbage started (Aggressive={aggressive})");

            int removed = 0;

            foreach (var kvp in new Dictionary<string, RSHandle>(_handles))
            {
                var key = kvp.Key;

                if (aggressive)
                {
                    _handles.Remove(key);
                    _loadedAssets.Remove(key);
                    removed++;
                }
            }

            Debug.WriteLine($"[AssetManager] CollectGarbage completed (Removed={removed})");

            return removed;
        }

        //====================================================================
        //Legacy Path-Based API
        //====================================================================

        public async Task<RSHandle> LoadAsync<T>(string path, AssetPriority priority = AssetPriority.Normal)
            where T : class
        {
            ThrowIfDisposed();
            EnsureLoaderInitialized();

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Asset path cannot be null or empty", nameof(path));

            var fullPath = GetAssetPath(path);

            if (_loadedAssets.TryGetValue(fullPath, out var cachedAsset) &&
                cachedAsset is T typedAsset)
            {
                Debug.WriteLine($"[AssetManager] LoadAsync cache hit (Path={path})");

                var handle = new RSHandle(new RSKey(typeof(T), path), typedAsset);
                return handle;
            }

            if (_loadingTasks.TryGetValue(fullPath, out var existingTask))
            {
                Debug.WriteLine($"[AssetManager] LoadAsync joining existing task (Path={path})");

                var result = await existingTask;
                return new RSHandle(new RSKey(typeof(T), path), result);
            }

            Debug.WriteLine($"[AssetManager] LoadAsync starting new load (Path={path})");

            var loadingTask = LoadAssetAsync<T>(fullPath, priority);
            _loadingTasks[fullPath] = loadingTask;

            try
            {
                var asset = await loadingTask;

                _loadedAssets[fullPath] = asset;

                return new RSHandle(new RSKey(typeof(T), path), asset);
            }
            finally
            {
                _loadingTasks.Remove(fullPath);
            }
        }

        public T Load<T>(string path) where T : class
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Asset path cannot be null or empty", nameof(path));

            var fullPath = GetAssetPath(path);

            if (_loadedAssets.TryGetValue(fullPath, out var cachedAsset) &&
                cachedAsset is T typedAsset)
            {
                Debug.WriteLine($"[AssetManager] Load cache hit (Path={path})");
                return typedAsset;
            }

            Debug.WriteLine($"[AssetManager] Load performing sync load (Path={path})");

            var asset = LoadAssetSync<T>(fullPath);

            _loadedAssets[fullPath] = asset;

            return asset as T;
        }

        public T GetAsset<T>(string path) where T : class
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(path))
                return null;

            var fullPath = GetAssetPath(path);

            if (_loadedAssets.TryGetValue(fullPath, out var cachedAsset) &&
                cachedAsset is T typedAsset)
            {
                Debug.WriteLine($"[AssetManager] GetAsset cache hit (Path={path})");
                return typedAsset;
            }

            Debug.WriteLine($"[AssetManager] GetAsset miss (Path={path})");

            return null;
        }

        public void Unload(string path)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(path))
                return;

            var fullPath = GetAssetPath(path);

            Debug.WriteLine($"[AssetManager] Unload (Path={path})");

            _loadedAssets.Remove(fullPath);
        }

        public void ClearCache()
        {
            ThrowIfDisposed();

            Debug.WriteLine("[AssetManager] ClearCache invoked");

            _loadedAssets.Clear();
        }

        public int LoadedAssetCount => _loadedAssets.Count;

        //====================================================================
        //Internal Load Logic
        //====================================================================

        private async Task<object> LoadAssetInternalAsync<T>(string fullPath, RSHandle handle)
            where T : class
        {
            EnsureLoaderInitialized();

            Debug.WriteLine($"[AssetManager] LoadAssetInternalAsync START (Path={fullPath})");

            await _loadingSemaphore!.WaitAsync();

            try
            {
                var asset = await LoadAssetAsync<T>(fullPath, AssetPriority.Normal);

                _loadedAssets[fullPath] = asset;

                Debug.WriteLine($"[AssetManager] LoadAssetInternalAsync COMPLETE (Path={fullPath})");

                return asset;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AssetManager] LoadAssetInternalAsync ERROR (Path={fullPath}, Error={ex.Message})");
                throw;
            }
            finally
            {
                _loadingSemaphore!.Release();
            }
        }

        private async Task<object> LoadAssetAsync<T>(string fullPath, AssetPriority priority)
            where T : class
        {
            Debug.WriteLine($"[AssetManager] LoadAssetAsync (Path={fullPath}, Type={typeof(T).Name}, Priority={priority})");

            await Task.Delay(1);

            var type = typeof(T);

            if (type == typeof(string) && File.Exists(fullPath))
                return await File.ReadAllTextAsync(fullPath);

            if (type == typeof(byte[]) && File.Exists(fullPath))
                return await File.ReadAllBytesAsync(fullPath);

            return Activator.CreateInstance<T>();
        }

        private object LoadAssetSync<T>(string fullPath)
            where T : class
        {
            Debug.WriteLine($"[AssetManager] LoadAssetSync (Path={fullPath}, Type={typeof(T).Name})");

            var type = typeof(T);

            if (type == typeof(string) && File.Exists(fullPath))
                return File.ReadAllText(fullPath);

            if (type == typeof(byte[]) && File.Exists(fullPath))
                return File.ReadAllBytes(fullPath);

            return Activator.CreateInstance<T>();
        }

        internal void Release(RSHandle value)
        {
            NotImplementedGuard.Hit("AssetManager.Release(RSHandle) NOT IMPLEMENTED");
            throw new NotImplementedException();
        }
    }
}
