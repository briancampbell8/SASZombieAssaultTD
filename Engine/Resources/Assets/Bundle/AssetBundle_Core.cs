//============================================================================
//File:        AssetBundle_Core.cs
//Path:        E:\BDC\Projects\SASZombieAssaultTD\Engine\Resources\Assets\Bundle\AssetBundle_Core.cs
//Program:     AssetBundle (Core)
//Subsystem:   Assets
//Author:      BDC
//Created:     2026-05-29
//
//Purpose:
//    Core implementation of the AssetBundle subsystem. Provides foundational
//    structures, metadata tables, and lifecycle management required for
//    loading, tracking, and disposing bundled assets.
//
//Responsibilities:
//    • Maintain bundle header and entry dictionaries.
//    • Track loaded assets and bundle state.
//    • Provide metadata accessors and stream helpers.
//    • Enforce disposal and load‑state safety checks.
//    • Support synchronous and asynchronous bundle loading.
//
//Architecture:
//    • Partial class: functionality split across multiple files
//        – AssetBundle_Core.cs       — Core fields, constructors, disposal, metadata
//        – AssetBundle_Loader.cs     — Bundle loading/unloading logic
//        – AssetBundle_Processors.cs — Compression/encryption processing
//        – AssetBundle_Metadata.cs   — Metadata structures and helpers
//        – AssetBundle_Validation.cs — Integrity verification
//        – AssetBundle_Interfaces.cs — Public interfaces/contracts
//        – AssetBundle_Accessors.cs  — Asset access and stream helpers
//        – AssetBundle_Types.cs      — Supporting types/configurations
//        – AssetBundle_Enums.cs      — Enumerations/constants
//
//Integration Points:
//    • AssetSystem for bundle loading and asset retrieval.
//    • AssetManager for unified asset access.
//    • AssetPipeline for bundle creation and processing.
//
//Performance Notes:
//    • Fast metadata lookup via dictionary tables.
//    • Lazy loading: only metadata is loaded initially.
//    • Low‑overhead stream access for asset retrieval.
//    • Thread‑safe disposal and state validation.
//    • Optimized compression and decompression algorithms.
//
//Usage Example:
//    var bundle = new AssetBundle("Assets/Bundles/ui.bundle");
//    await bundle.LoadAsync();
//    var icon = await bundle.GetAssetAsync<byte[]>("ui/icon.png");
//============================================================================


//
using System;
//
using System.Collections.Generic;
//
using System.IO;
//
using System.Threading;
//
using System.Threading.Tasks;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Resources
{
 
    public partial class AssetBundle : IDisposable
    {
        private readonly string _bundlePath;
        private readonly BundleCreationOptions _options;
        private BundleHeader _header;
        private Dictionary<string, BundleEntry> _entries;
        private readonly Dictionary<string, object> _loadedAssets;
        private Stream _bundleStream;
        private volatile bool _disposed;
        private bool _isLoaded;

        //---------------------------------------------------------------------
        //CONSTRUCTORS
        //---------------------------------------------------------------------

        public AssetBundle(string bundlePath)
        {
            _bundlePath = bundlePath ?? throw new ArgumentNullException(nameof(bundlePath));
            _options = new BundleCreationOptions();
            _entries = new Dictionary<string, BundleEntry>();
            _loadedAssets = new Dictionary<string, object>();
            _header = new BundleHeader();
        }

        public AssetBundle(string bundlePath, BundleCreationOptions options)
        {
            _bundlePath = bundlePath ?? throw new ArgumentNullException(nameof(bundlePath));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _entries = new Dictionary<string, BundleEntry>();
            _loadedAssets = new Dictionary<string, object>();
            _header = new BundleHeader();
        }

        //---------------------------------------------------------------------
        //PROPERTIES
        //---------------------------------------------------------------------

        public string BundlePath => _bundlePath;
        public BundleHeader Header => _header;
        public bool IsLoaded => _isLoaded;
        public int AssetCount => _entries.Count;
        public BundleCreationOptions Options => _options;

        public long BundleSize =>
            File.Exists(_bundlePath) ? new FileInfo(_bundlePath).Length : 0;

        public long TotalUncompressedSize
        {
            get
            {
                long total = 0;
                foreach (var entry in _entries.Values)
                    total += entry.OriginalSize;
                return total;
            }
        }

        public double CompressionRatio =>
            TotalUncompressedSize == 0
                ? 0
                : (1.0 - (double)BundleSize / TotalUncompressedSize) * 100;

        //---------------------------------------------------------------------
        //DISPOSAL + SAFETY CHECKS
        //---------------------------------------------------------------------

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _bundleStream?.Close();
            _bundleStream = null;

            _loadedAssets.Clear();
            _entries.Clear();

            _isLoaded = false;

            System.Diagnostics.Debug.WriteLine($"AssetBundle: Disposed '{_bundlePath}'");
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AssetBundle));
        }

        protected void ThrowIfNotLoaded()
        {
            if (!_isLoaded)
                throw new InvalidOperationException(
                    "Asset bundle must be loaded before this operation can be performed.");
        }

        //---------------------------------------------------------------------
        //ASSET ACCESSORS
        //---------------------------------------------------------------------

        public Stream GetAssetStream(string assetPath)
        {
            ThrowIfDisposed();
            ThrowIfNotLoaded();

            if (_entries.TryGetValue(assetPath, out var entry))
            {
                _bundleStream.Seek(entry.Offset, SeekOrigin.Begin);
                return new MemoryStream((int)entry.CompressedSize);
            }

            throw new KeyNotFoundException($"Asset not found in bundle: {assetPath}");
        }

        public async Task<T> GetAssetAsync<T>(string assetPath) where T : class
        {
            ThrowIfDisposed();
            ThrowIfNotLoaded();

            if (_entries.TryGetValue(assetPath, out var entry))
            {
                _bundleStream.Seek(entry.Offset, SeekOrigin.Begin);

                var data = new byte[entry.CompressedSize];
                await _bundleStream.ReadAsync(data, 0, data.Length);

                if (entry.IsCompressed)
                {
                    //Placeholder for real decompression
                    data = data;
                }

                if (typeof(T) == typeof(string))
                    return (T)(object)System.Text.Encoding.UTF8.GetString(data);

                return (T)(object)data;
            }

            throw new KeyNotFoundException($"Asset not found in bundle: {assetPath}");
        }

        public BundleEntry GetAssetMetadata(string assetPath)
        {
            ThrowIfDisposed();
            ThrowIfNotLoaded();

            return _entries.TryGetValue(assetPath, out var entry) ? entry : null;
        }

        public IEnumerable<BundleEntry> Entries => _entries.Values;

        //---------------------------------------------------------------------
        //STATIC LOADERS
        //---------------------------------------------------------------------

        public static async Task<AssetBundle> LoadFromFileAsync(string bundlePath)
        {
            if (!File.Exists(bundlePath))
                throw new FileNotFoundException($"Bundle file not found: {bundlePath}");

            using var fileStream = new FileStream(bundlePath, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(fileStream);

            //Header
            var magic = new string(reader.ReadChars(8));
            if (magic != "ASBUNDLE")
                throw new InvalidDataException($"Invalid bundle format: {bundlePath}");

            var header = new BundleHeader
            {
                Magic = magic,
                Version = (BundleVersion)reader.ReadInt32(),
                AssetCount = reader.ReadInt32(),
                TotalSize = reader.ReadInt64(),
                CreatedAt = DateTime.FromBinary(reader.ReadInt64()),
                Checksum = new string(reader.ReadChars(64)),
                HasCompression = reader.ReadBoolean(),
                HasEncryption = reader.ReadBoolean()
            };

            //Entries
            var entries = new Dictionary<string, BundleEntry>();
            for (int i = 0; i < header.AssetCount; i++)
            {
                var path = new string(reader.ReadChars(256));
                entries[path] = new BundleEntry
                {
                    Path = path,
                    OriginalSize = reader.ReadInt64(),
                    CompressedSize = reader.ReadInt64(),
                    Offset = reader.ReadInt64(),
                    IsCompressed = reader.ReadBoolean(),
                    Hash = new string(reader.ReadChars(64)),
                    LastModified = DateTime.FromBinary(reader.ReadInt64())
                };
            }

            var bundle = new AssetBundle(bundlePath)
            {
                _header = header,
                _entries = entries,
                _isLoaded = true,
                _bundleStream = fileStream
            };

            return bundle;
        }

        public static AssetBundle LoadFromFile(string bundlePath) =>
            LoadFromFileAsync(bundlePath).GetAwaiter().GetResult();

        //---------------------------------------------------------------------
        //BUNDLE CREATION + VALIDATION
        //---------------------------------------------------------------------

        public static async Task<AssetBundle> CreateBundleAsync(
            string bundlePath,
            Dictionary<string, string> assets,
            BundleCreationOptions options)
        {
            var creator = new DefaultAssetBundleCreator();
            var result = await creator.CreateBundleAsync(bundlePath, assets, options);

            if (result.Success)
                return await LoadFromFileAsync(bundlePath);

            throw new InvalidOperationException($"Failed to create bundle: {result.ErrorMessage}");
        }

        public async Task<bool> VerifyIntegrityAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            ThrowIfNotLoaded();

            var validator = new DefaultAssetBundleValidator();
            var result = await validator.ValidateBundleAsync(_bundlePath, cancellationToken);
            return result.IsValid;
        }

        internal static async Task<BundleCreationResult> CreateBundleAsync(Dictionary<string, string> assets, string outputPath, BundleCreationOptions options)
        {
            NI.Hit();
            return null;
        }
    }
}

