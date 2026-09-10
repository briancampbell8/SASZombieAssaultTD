// ====================================================================================================
//  FILE: AssetBundle_Loader.cs
//  PATH: ./Engine/Resources/Assets/Bundle/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the AssetBundle_Loader module.
//
//  RESPONSIBILITIES:
//      - Provide LoadAsync() behavior for the Core subsystem.
//      - Provide Load() behavior for the Core subsystem.
//      - Provide Unload() behavior for the Core subsystem.
//      - Provide ContainsAsset() behavior for the Core subsystem.
//      - Provide GetAssetPaths() behavior for the Core subsystem.
//      - Provide GetAssetPaths() behavior for the Core subsystem.
//      - Provide GetStatistics() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//============================================================================
//File:        AssetBundle_Loader.cs
//Path:        E:\BDC\Projects\SASZombieAssaultTD\Engine\Resources\Assets\Bundle\AssetBundle_Loader.cs
//Program:     AssetBundle (Loader)
//Subsystem:   Assets
//Author:      BDC
//Created:     2026-05-29
//
//Purpose:
//    Implements synchronous and asynchronous loading logic for the
//    AssetBundle subsystem. Responsible for reading bundle headers,
//    validating metadata, initializing internal tables, and preparing
//    assets for retrieval.
//
//Responsibilities:
//    • Load bundle metadata and asset tables from disk.
//    • Validate bundle format, version, and integrity.
//    • Support async and sync loading workflows.
//    • Initialize internal lookup structures.
//    • Enforce load‑state correctness and prevent double‑loading.
//
//Architecture:
//    • Partial class: loader logic separated from core, metadata, processors,
//      validation, and accessors.
//    • Stream‑based loading for large bundles.
//    • Integrated with AssetManager and AssetPipeline subsystems.
//    • Full error reporting with deterministic exception types.
//
//Notes:
//    • This loader must remain deterministic and serialization‑safe.
//    • No BGFX, no legacy backend references.
//============================================================================

///<summary>
///Loads the asset bundle asynchronously from disk.
///</summary>
///<param name="cancellationToken">
///Optional cancellation token to cancel the loading operation.
///</param>
///<returns>
///Task representing the loading operation.
///</returns>
///<exception cref="InvalidOperationException">
///Thrown when the bundle is already loaded.
///</exception>
///<exception cref="FileNotFoundException">
///Thrown when the bundle file doesn't exist.
///</exception>
///<exception cref="BundleFormatException">
///Thrown when the bundle format is invalid or corrupted.
///</exception>
///<example>
///<code>
///await bundle.LoadAsync();
///DLogger.Log($"Loaded {bundle.AssetCount} assets");
///</code>
///</example>
//

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Resources
{
    public partial class AssetBundle
    {
        private readonly SemaphoreSlim _loadingSemaphore = new(1, 1);
        private readonly Dictionary<string, Task<object>> _loadingTasks = new();


        public async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (_isLoaded)
                throw new InvalidOperationException("Bundle is already loaded.");

            if (!File.Exists(_bundlePath))
                throw new FileNotFoundException($"Bundle file not found: {_bundlePath}");

            await _loadingSemaphore.WaitAsync(cancellationToken);
            try
            {
                //Open bundle stream
                _bundleStream = new FileStream(_bundlePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);

                //Read bundle header
                await ReadBundleHeaderAsync(cancellationToken);

                //Read bundle entries
                await ReadBundleEntriesAsync(cancellationToken);

                _isLoaded = true;

                DLogger.Log($"AssetBundle: Loaded '{_bundlePath}' with {_entries.Count} assets");
            }
            catch (Exception ex)
            {
                //Clean up on error
                _bundleStream?.Close();
                _bundleStream = null;
                _entries.Clear();
                _isLoaded = false;

                throw new BundleFormatException($"Failed to load bundle '{_bundlePath}': {ex.Message}", ex);
            }
            finally
            {
                _loadingSemaphore.Release();
            }
        }

        ///<summary>
        ///Loads the asset bundle synchronously from disk.
        ///</summary>
        ///<exception cref="InvalidOperationException">
        ///Thrown when the bundle is already loaded.
        ///</exception>
        ///<exception cref="FileNotFoundException">
        ///Thrown when the bundle file doesn't exist.
        ///</exception>
        ///<exception cref="BundleFormatException">
        ///Thrown when the bundle format is invalid or corrupted.
        ///</exception>
        ///<example>
        ///<code>
        ///bundle.Load();
        ///DLogger.Log($"Loaded {bundle.AssetCount} assets");
        ///</code>
        ///</example>
        public void Load()
        {
            ThrowIfDisposed();

            if (_isLoaded)
                throw new InvalidOperationException("Bundle is already loaded.");

            if (!File.Exists(_bundlePath))
                throw new FileNotFoundException($"Bundle file not found: {_bundlePath}");

            try
            {
                //Open bundle stream
                _bundleStream = new FileStream(_bundlePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                //Read bundle header
                ReadBundleHeader();

                //Read bundle entries
                ReadBundleEntries();

                _isLoaded = true;

                DLogger.Log($"AssetBundle: Loaded '{_bundlePath}' with {_entries.Count} assets");
            }
            catch (Exception ex)
            {
                //Clean up on error
                _bundleStream?.Close();
                _bundleStream = null;
                _entries.Clear();
                _isLoaded = false;

                throw new BundleFormatException($"Failed to load bundle '{_bundlePath}': {ex.Message}", ex);
            }
        }

        ///<summary>
        ///Unloads the asset bundle and releases all resources.
        ///</summary>
        ///<remarks>
        ///This method clears all loaded assets from memory and closes the bundle stream.
        ///The bundle can be loaded again after unloading.
        ///</remarks>
        ///<example>
        ///<code>
        ///bundle.Unload();
        /////Bundle can be loaded again later
        ///await bundle.LoadAsync();
        ///</code>
        ///</example>
        public void Unload()
        {
            ThrowIfDisposed();

            if (!_isLoaded)
                return;

            //Close bundle stream
            _bundleStream?.Close();
            _bundleStream = null;

            //Clear loaded assets and entries
            _loadedAssets.Clear();
            _entries.Clear();

            _isLoaded = false;

            DLogger.Log($"AssetBundle: Unloaded '{_bundlePath}'");
        }

        ///<summary>
        ///Gets an asset from the bundle asynchronously.
        ///</summary>
        ///<typeparam name="T">Type of asset to load.</typeparam>
        ///<param name="assetPath">Path of the asset within the bundle.</param>
        ///<param name="cancellationToken">Optional cancellation token.</param>
        ///<returns>The loaded asset of the specified type.</returns>
        ///<exception cref="InvalidOperationException">
        ///Thrown when the bundle is not loaded.
        ///</exception>
        ///<exception cref="KeyNotFoundException">
        ///Thrown when the asset is not found in the bundle.
        ///</exception>
        ///<exception cref="AssetLoadException">
        ///Thrown when the asset fails to load or convert to the specified type.
        ///</exception>
        ///<example>
        ///<code>
        ///var texture = await bundle.GetAssetAsync&lt;Texture2D&gt;("textures/player.png");
        ///</code>
        ///</example>
        public async Task<T> GetAssetAsync<T>(string assetPath, CancellationToken cancellationToken = default) where T : class
        {
            ThrowIfDisposed();
            ThrowIfNotLoaded();

            if (string.IsNullOrWhiteSpace(assetPath))
                throw new ArgumentException("Asset path cannot be null or empty", nameof(assetPath));

            //Check if already loaded
            if (_loadedAssets.TryGetValue(assetPath, out var cachedAsset) && cachedAsset is T typedAsset)
            {
                return typedAsset;
            }

            //Check if currently loading
            if (_loadingTasks.TryGetValue(assetPath, out var existingTask))
            {
                var result = await existingTask;
                return result as T;
            }

            //Start loading
            var loadingTask = LoadAssetAsync<T>(assetPath, cancellationToken);
            _loadingTasks[assetPath] = loadingTask;

            try
            {
                var asset = await loadingTask;
                _loadedAssets[assetPath] = asset;
                return asset as T;
            }
            finally
            {
                _loadingTasks.Remove(assetPath);
            }
        }

        ///<summary>
        ///Gets an asset from the bundle synchronously.
        ///</summary>
        ///<typeparam name="T">Type of asset to load.</typeparam>
        ///<param name="assetPath">Path of the asset within the bundle.</param>
        ///<returns>The loaded asset of the specified type.</returns>
        ///<exception cref="InvalidOperationException">
        ///Thrown when the bundle is not loaded.
        ///</exception>
        ///<exception cref="KeyNotFoundException">
        ///Thrown when the asset is not found in the bundle.
        ///</exception>
        ///<exception cref="AssetLoadException">
        ///Thrown when the asset fails to load or convert to the specified type.
        ///</exception>
        ///<example>
        ///<code>
        ///var texture = bundle.GetAsset&lt;Texture2D&gt;("textures/player.png");
        ///</code>
        ///</example>
        public T GetAsset<T>(string assetPath) where T : class
        {
            ThrowIfDisposed();
            ThrowIfNotLoaded();

            if (string.IsNullOrWhiteSpace(assetPath))
                throw new ArgumentException("Asset path cannot be null or empty", nameof(assetPath));

            //Check if already loaded
            if (_loadedAssets.TryGetValue(assetPath, out var cachedAsset) && cachedAsset is T typedAsset)
            {
                return typedAsset;
            }

            //Load synchronously
            var asset = LoadAsset<T>(assetPath);
            _loadedAssets[assetPath] = asset;
            return asset as T;
        }

        ///<summary>
        ///Checks if an asset exists in the bundle.
        ///</summary>
        ///<param name="assetPath">Path of the asset within the bundle.</param>
        ///<returns>True if the asset exists, false otherwise.</returns>
        ///<example>
        ///<code>
        ///if (bundle.ContainsAsset("textures/player.png"))
        ///{
        ///    var texture = await bundle.GetAssetAsync&lt;Texture2D&gt;("textures/player.png");
        ///}
        ///</code>
        ///</example>
        public bool ContainsAsset(string assetPath)
        {
            ThrowIfDisposed();
            ThrowIfNotLoaded();

            return !string.IsNullOrWhiteSpace(assetPath) && _entries.ContainsKey(assetPath);
        }

        ///<summary>
        ///Gets all asset paths in the bundle.
        ///</summary>
        ///<returns>Collection of all asset paths in the bundle.</returns>
        ///<example>
        ///<code>
        ///foreach (var assetPath in bundle.GetAssetPaths())
        ///{
        ///    DLogger.Log($"Asset: {assetPath}");
        ///}
        ///</code>
        ///</example>
        public IEnumerable<string> GetAssetPaths()
        {
            ThrowIfDisposed();
            ThrowIfNotLoaded();

            return _entries.Keys;
        }

        ///<summary>
        ///Gets asset paths matching a pattern.
        ///</summary>
        ///<param name="pattern">Pattern to match (supports wildcards).</param>
        ///<returns>Collection of matching asset paths.</returns>
        ///<example>
        ///<code>
        ///var textures = bundle.GetAssetPaths("textures//.png");
        ///</code>
        ///</example>
        public IEnumerable<string> GetAssetPaths(string pattern)
        {
            ThrowIfDisposed();
            ThrowIfNotLoaded();

            return _entries.Keys.Where(path => PathMatchesPattern(path, pattern));
        }

        ///<summary>
        ///Gets bundle statistics and performance metrics.
        ///</summary>
        ///<returns>BundleStatistics with detailed information.</returns>
        ///<example>
        ///<code>
        ///var stats = bundle.GetStatistics();
        ///DLogger.Log($"Compression: {stats.CompressionRatio:F1}%");
        ///</code>
        ///</example>
        public BundleStatistics GetStatistics()
        {
            ThrowIfDisposed();
            ThrowIfNotLoaded();

            return new BundleStatistics
            {
                BundlePath = _bundlePath,
                IsLoaded = _isLoaded,
                AssetCount = _entries.Count,
                LoadedAssetCount = _loadedAssets.Count,
                BundleSize = BundleSize,
                TotalUncompressedSize = TotalUncompressedSize,
                CompressionRatio = CompressionRatio,
                CreatedAt = _header.CreatedAt,
                Version = _header.Version,
                HasCompression = _entries.Values.Any(e => e.IsCompressed),
                HasEncryption = _entries.Values.Any(e => e.IsEncrypted)
            };
        }

        ///<summary>
        ///Reads the bundle header from the stream asynchronously.
        ///</summary>
        private async Task ReadBundleHeaderAsync(CancellationToken cancellationToken)
        {
            using var reader = new BinaryReader(_bundleStream, System.Text.Encoding.UTF8, leaveOpen: true);

            //Read magic number
            var magic = reader.ReadString();
            if (magic != "ASBUNDLE")
                throw new BundleFormatException("Invalid bundle format: missing magic number");

            //Read version
            var version = reader.ReadInt32();
            _header.Version = (BundleVersion)version;

            //Read header fields
            _header.Name = reader.ReadString();
            _header.CreatedAt = DateTime.FromBinary(reader.ReadInt64());
            _header.TotalSize = reader.ReadInt64();
            _header.AssetCount = reader.ReadInt32();
            _header.Checksum = reader.ReadString();
        }

        ///<summary>
        ///Reads the bundle header from the stream synchronously.
        ///</summary>
        private void ReadBundleHeader()
        {
            using var reader = new BinaryReader(_bundleStream, System.Text.Encoding.UTF8, leaveOpen: true);

            //Read magic number
            var magic = reader.ReadString();
            if (magic != "ASBUNDLE")
                throw new BundleFormatException("Invalid bundle format: missing magic number");

            //Read version
            var version = reader.ReadInt32();
            _header.Version = (BundleVersion)version;

            //Read header fields
            _header.Name = reader.ReadString();
            _header.CreatedAt = DateTime.FromBinary(reader.ReadInt64());
            _header.TotalSize = reader.ReadInt64();
            _header.AssetCount = reader.ReadInt32();
            _header.Checksum = reader.ReadString();
        }

        ///<summary>
        ///Reads bundle entries from the stream asynchronously.
        ///</summary>
        private async Task ReadBundleEntriesAsync(CancellationToken cancellationToken)
        {
            using var reader = new BinaryReader(_bundleStream, System.Text.Encoding.UTF8, leaveOpen: true);

            for (int i = 0; i < _header.AssetCount; i++)
            {
                var entry = new BundleEntry
                {
                    Path = reader.ReadString(),
                    OriginalSize = reader.ReadInt64(),
                    CompressedSize = reader.ReadInt64(),
                    Hash = reader.ReadString(),
                    Offset = reader.ReadInt64(),
                    IsCompressed = reader.ReadBoolean(),
                    IsEncrypted = reader.ReadBoolean()
                };

                _entries[entry.Path] = entry;
            }
        }

        ///<summary>
        ///Reads bundle entries from the stream synchronously.
        ///</summary>
        private void ReadBundleEntries()
        {
            using var reader = new BinaryReader(_bundleStream, System.Text.Encoding.UTF8, leaveOpen: true);

            for (int i = 0; i < _header.AssetCount; i++)
            {
                var entry = new BundleEntry
                {
                    Path = reader.ReadString(),
                    OriginalSize = reader.ReadInt64(),
                    CompressedSize = reader.ReadInt64(),
                    Hash = reader.ReadString(),
                    Offset = reader.ReadInt64(),
                    IsCompressed = reader.ReadBoolean(),
                    IsEncrypted = reader.ReadBoolean()
                };

                _entries[entry.Path] = entry;
            }
        }

        ///<summary>
        ///Loads an asset asynchronously from the bundle.
        ///</summary>
        private async Task<object> LoadAssetAsync<T>(string assetPath, CancellationToken cancellationToken) where T : class
        {
            if (!_entries.TryGetValue(assetPath, out var entry))
                throw new KeyNotFoundException($"Asset not found in bundle: {assetPath}");

            //Seek to asset position
            _bundleStream.Seek(entry.Offset, SeekOrigin.Begin);

            //Read asset data
            var assetData = new byte[entry.CompressedSize];
            await _bundleStream.ReadExactlyAsync(assetData, 0, assetData.Length, cancellationToken);

            //Decrypt if needed
            if (entry.IsEncrypted)
            {
                assetData = await DecryptAssetDataAsync(assetData, cancellationToken);
            }

            //Decompress if needed
            if (entry.IsCompressed)
            {
                assetData = await DecompressAssetDataAsync(assetData, cancellationToken);
            }

            //Convert to target type
            return ConvertAssetDataToType<T>(assetData, assetPath);
        }

        ///<summary>
        ///Loads an asset synchronously from the bundle.
        ///</summary>
        private object LoadAsset<T>(string assetPath) where T : class
        {
            if (!_entries.TryGetValue(assetPath, out var entry))
                throw new KeyNotFoundException($"Asset not found in bundle: {assetPath}");

            //Seek to asset position
            _bundleStream.Seek(entry.Offset, SeekOrigin.Begin);

            //Read asset data
            var assetData = new byte[entry.CompressedSize];
            _bundleStream.ReadExactly(assetData, 0, assetData.Length);

            //Decrypt if needed
            if (entry.IsEncrypted)
            {
                assetData = DecryptAssetData(assetData);
            }

            //Decompress if needed
            if (entry.IsCompressed)
            {
                assetData = DecompressAssetData(assetData);
            }

            //Convert to target type
            return ConvertAssetDataToType<T>(assetData, assetPath);
        }

        ///<summary>
        ///Checks if a path matches a pattern (simple wildcard support).
        ///</summary>
        private static bool PathMatchesPattern(string path, string pattern)
        {
            //Simple implementation - in production, use proper pattern matching
            if (string.IsNullOrEmpty(pattern))
                return true;

            if (pattern.Contains('*'))
            {
                var parts = pattern.Split('*');
                foreach (var part in parts)
                {
                    if (!string.IsNullOrEmpty(part) && !path.Contains(part))
                        return false;
                }
                return true;
            }

            return path.Equals(pattern, StringComparison.OrdinalIgnoreCase);
        }

        ///<summary>
        ///Converts asset data to the specified type.
        ///</summary>
        private object ConvertAssetDataToType<T>(byte[] assetData, string assetPath) where T : class
        {
            var AssetManagerType = typeof(T);

            if (AssetManagerType == typeof(string))
            {
                return System.Text.Encoding.UTF8.GetString(assetData);
            }
            else if (AssetManagerType == typeof(byte[]))
            {
                return assetData;
            }
            else if (AssetManagerType == typeof(Texture2D))
            {
                //Would load texture here
                return null; //Placeholder
            }

            //Default: try to create instance
            return Activator.CreateInstance<T>();
        }

        ///<summary>
        ///Decrypts asset data (placeholder implementation).
        ///</summary>
        private async Task<byte[]> DecryptAssetDataAsync(byte[] encryptedData, CancellationToken cancellationToken)
        {
            //Placeholder decryption - in real implementation, use proper encryption
            await Task.Delay(1, cancellationToken);
            return encryptedData;
        }

        ///<summary>
        ///Decrypts asset data synchronously (placeholder implementation).
        ///</summary>
        private byte[] DecryptAssetData(byte[] encryptedData)
        {
            //Placeholder decryption - in real implementation, use proper encryption
            return encryptedData;
        }

        ///<summary>
        ///Decompresses asset data (placeholder implementation).
        ///</summary>
        private async Task<byte[]> DecompressAssetDataAsync(byte[] compressedData, CancellationToken cancellationToken)
        {
            //Placeholder decompression - in real implementation, use proper compression
            await Task.Delay(1, cancellationToken);
            return compressedData;
        }

        ///<summary>
        ///Decompresses asset data synchronously (placeholder implementation).
        ///</summary>
        private byte[] DecompressAssetData(byte[] compressedData)
        {
            //Placeholder decompression - in real implementation, use proper compression
            return compressedData;
        }
    }
}

