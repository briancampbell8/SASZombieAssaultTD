// ====================================================================================================
//  FILE: AssetBundle_Types.cs
//  PATH: ./Engine/Resources/Assets/Bundle/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the AssetBundle_Types module.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//============================================================================
//File:        AssetBundle_Types.cs
//Path:        E:\BDC\Projects\SASZombieAssaultTD\Engine\Resources\Assets\Bundle\AssetBundle_Types.cs
//Program:     AssetBundle (Types)
//Subsystem:   Assets
//Author:      BDC
//Created:     2026-05-29
//
//Purpose:
//    Defines all supporting data structures, configuration types, metadata
//    containers, validation results, statistics objects, and exception types
//    used by the AssetBundle subsystem. These types form the shared contracts
//    for bundle creation, loading, validation, diagnostics, and reporting.
//
//Responsibilities:
//    • Represent bundle header/entry metadata containers.
//    • Define configuration types for bundle creation and processing.
//    • Provide validation result structures and diagnostic reporting types.
//    • Define statistics objects for bundle creation and load operations.
//    • Provide strongly‑typed exception classes for deterministic error handling.
//    • Serve as the authoritative shared type library for all AssetBundle partials.
//
//Architecture:
//    • Pure type definitions (no I/O logic).
//    • Consumed by AssetBundle_Core, Loader, Processors, Metadata, Validation,
//      Accessors, and Creator implementations.
//    • Stable serialization contract — changes require migration planning.
//    • Designed for minimal memory overhead and fast lookup.
//
//Notes:
//    • Replaces all legacy RS bundle type definitions.
//    • No BGFX, no legacy backend references.
//    • All types must remain deterministic and serialization‑safe.
//============================================================================


using System;
using System.Collections.Generic;   //


namespace SASZombieAssaultTD.Engine.Resources
{
    ///<summary>
    ///Bundle header information.
    ///</summary>
    public class BundleHeader
    {
        public string Name { get; set; } = string.Empty;
        public BundleVersion Version { get; set; }

        public DateTime CreatedAt { get; set; }
        public long TotalSize { get; set; }
        public int AssetCount { get; set; }
        public string Checksum { get; set; } = string.Empty;
        public string Magic { get; set; } = string.Empty;
        public bool HasCompression { get; set; }
        public bool HasEncryption { get; set; }
    }

    ///<summary>
    ///Bundle entry for individual assets.
    ///</summary>
    public class BundleEntry
    {
        public string Path { get; set; } = string.Empty;
        public long OriginalSize { get; set; }
        public long CompressedSize { get; set; }
        public string Hash { get; set; } = string.Empty;
        public long Offset { get; set; }
        public bool IsCompressed { get; set; }
        public bool IsEncrypted { get; set; }
        public DateTime LastModified { get; set; }
    }

    ///<summary>
    ///Bundle creation options.
    ///</summary>
    public class BundleCreationOptions
    {
        public bool CompressBundle { get; set; } = true;
        public bool EncryptBundle { get; set; } = false;
        public System.IO.Compression.CompressionLevel CompressionLevel { get; set; } = System.IO.Compression.CompressionLevel.Optimal;
        public bool IncludeDependencies { get; set; } = true;
        public bool VerifyIntegrity { get; set; } = true;
        public Dictionary<string, object> CustomMetadata { get; set; } = new();
        public byte[] EncryptionKey { get; set; } = Array.Empty<byte>();
    }

    ///<summary>
    ///Bundle creation result.
    ///</summary>
    public class BundleCreationResult
    {
        public bool Success { get; set; }
        public string BundlePath { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public TimeSpan CreationTime { get; set; }
        public long BundleSize { get; set; }
        public int AssetCount { get; set; }
        public Dictionary<string, object> Statistics { get; set; } = new();
        public TimeSpan LoadTime { get; set; }
        public long MemoryUsage { get; set; }
    }

    ///<summary>
    ///Bundle statistics and performance metrics.
    ///</summary>
    public class BundleStatistics
    {
        public string BundlePath { get; set; } = string.Empty;
        public bool IsLoaded { get; set; }
        public int AssetCount { get; set; }
        public int LoadedAssetCount { get; set; }
        public long BundleSize { get; set; }
        public long TotalUncompressedSize { get; set; }
        public double CompressionRatio { get; set; }
        public DateTime CreatedAt { get; set; }
        public BundleVersion Version { get; set; }

        public bool HasCompression { get; set; }
        public bool HasEncryption { get; set; }
        public long TotalSize { get; set; }
        public long CompressedSize { get; set; }
        public DateTime LastAccessed { get; set; }
        public TimeSpan LoadTime { get; set; }
        public int CacheHits { get; set; }
        public int CacheMisses { get; set; }
        public long MemoryUsage { get; set; }
        public int ActiveHandles { get; set; }
        public Dictionary<BundleEntry, AssetMetadata> AssetMetadata { get; set; } = new();
        public int TotalAssets { get; set; }
    }

    ///<summary>
    ///Bundle repair report.
    ///</summary>
    public partial class BundleRepairReport
    {
        public DateTime Timestamp { get; set; }
        public List<string> ActionsTaken { get; set; } = new();
    }

    ///<summary>
    ///Bundle optimization report.
    ///</summary>
    public partial class BundleOptimizationReport
    {
        public DateTime Timestamp { get; set; }
        public List<string> ActionsTaken { get; set; } = new();
        public int AssetsOptimized { get; set; }
        public long MemoryFreed { get; set; }
    }

    ///<summary>
    ///Bundle format exception.
    ///</summary>
    public class BundleFormatException : Exception
    {
        public BundleFormatException(string message) : base(message) { }
        public BundleFormatException(string message, Exception innerException) : base(message, innerException) { }
    }

    ///<summary>
    ///Asset load exception.
    ///</summary>
    public class AssetLoadException : Exception
    {
        public string AssetPath { get; set; } = string.Empty;

        public AssetLoadException(string message) : base(message) { }
        public AssetLoadException(string message, Exception innerException) : base(message, innerException) { }
        public AssetLoadException(string assetPath, string message) : base(message) => AssetPath = assetPath;
        public AssetLoadException(string assetPath, string message, Exception innerException) : base(message, innerException) => AssetPath = assetPath;
    }
}

