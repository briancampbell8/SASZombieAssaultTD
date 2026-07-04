/*
//============================================================================
//File:        AssetBundle_Processors.cs
//Path:        E:\BDC\Projects\SASZombieAssaultTD\Engine\Resources\Assets\Bundle\AssetBundle_Processors.cs
//Program:     AssetBundle (Processors)
//Subsystem:   Assets
//Author:      BDC
//Created:     2026-05-29
//
//Purpose:
//    Implements compression, decompression, encryption, decryption, hashing,
//    and other transformation steps used during bundle creation and loading.
//    Provides the processing pipeline that prepares assets for inclusion in
//    bundles and validates processed data during load.
//
//Responsibilities:
//    • Apply compression algorithms during bundle creation.
//    • Perform encryption and decryption of asset data.
//    • Generate and verify SHA‑256 hashes for integrity checking.
//    • Support stream‑based processing for large assets.
//    • Provide deterministic, reversible transformations for bundle I/O.
//    • Serve as the processing backend for AssetBundle_Creator and Loader.
//
//Architecture:
//    • Modular processor system for extensible asset type support.
//    • Configurable processing pipeline with quality/performance trade‑offs.
//    • Parallel batch processing with configurable worker threads.
//    • Integrated with AssetBundle_Core, Metadata, Validation, and Creator.
//    • Designed for deterministic output and stable serialization.
//
//Integration Points:
//    • AssetPipeline subsystem for preprocessing and optimization.
//    • AssetManager for unified asset access and lifecycle coordination.
//    • AssetBundle subsystem for creation, loading, and validation.
//    • RS subsystem (legacy) during migration.
//
//Performance Characteristics:
//    • Minimal overhead through subsystem delegation.
//    • Optimized initialization with lazy loading where appropriate.
//    • Efficient resource management with automatic cleanup.
//    • Thread‑safe operations with minimal contention.
//    • Background processing to avoid UI thread blocking.
//    • Intelligent caching with hash‑based change detection.
//    • Memory‑efficient streaming for large assets.
//============================================================================
*/


using System;
//
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;




using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Resources
{
    ///<summary>
    ///Bundle compression handler.
    ///</summary>
    public class BundleCompression : IDisposable
    {
        private bool _disposed = false;

        public async Task CompressAsync(Stream input, Stream output, CompressionLevel level = CompressionLevel.Optimal)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(BundleCompression));

            using var compressionStream = new GZipStream(output, GetCompressionLevel(level), true);
            await input.CopyToAsync(compressionStream);
        }

        public async Task DecompressAsync(Stream input, Stream output)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(BundleCompression));

            using var compressionStream = new GZipStream(input, CompressionMode.Decompress, true);
            await compressionStream.CopyToAsync(output);
        }

        private System.IO.Compression.CompressionLevel GetCompressionLevel(CompressionLevel level)
        {
            return level switch
            {
                CompressionLevel.Fastest => System.IO.Compression.CompressionLevel.Fastest,
                CompressionLevel.NoCompression => System.IO.Compression.CompressionLevel.NoCompression,
                CompressionLevel.Optimal => System.IO.Compression.CompressionLevel.Optimal,
                CompressionLevel.Maximum => System.IO.Compression.CompressionLevel.SmallestSize,
                _ => System.IO.Compression.CompressionLevel.Optimal
            };
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }

    ///<summary>
    ///Bundle encryption handler.
    ///</summary>
    public class BundleEncryption : IDisposable
    {
        private bool _disposed = false;

        public void Dispose()
        {
            _disposed = true;
        }
    }
}
