/*
    File:    AssetMetadata.cs
    Author:  BDC
    Created: 2026-02-07
    Purpose: Describes metadata associated with an asset, including type, size, format, and tags.
    Notes:   Mutable. Used by loaders, validation, and asset registries.
*/

using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Systems.Assets
{
    /// <summary>
    /// Represents descriptive metadata for an asset.
    /// </summary>
    public sealed class AssetMetadata
    {
        /// <summary>
        /// The unique key identifying this asset.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The file system path to the asset.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The category of the asset (Texture, Sound, Json, etc.).
        /// </summary>
        public AssetType Type { get; set; }

        /// <summary>
        /// Optional file size in bytes, if known.
        /// </summary>
        public long? SizeBytes { get; set; }

        /// <summary>
        /// Optional format string (e.g., "png", "wav", "json").
        /// </summary>
        public string? Format { get; set; }

        /// <summary>
        /// Optional tags for classification or search.
        /// </summary>
        public IReadOnlyList<string> Tags { get; set; }

        public AssetMetadata(
            string key,
            string path,
            AssetType type,
            long? sizeBytes = null,
            string? format = null,
            IReadOnlyList<string>? tags = null)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Type = type;
            SizeBytes = sizeBytes;
            Format = format;
            Tags = tags ?? Array.Empty<string>();
        }

        // Parameterless constructor for scenarios where properties are set afterward
        public AssetMetadata()
        {
            Key = string.Empty;
            Path = string.Empty;
            Type = AssetType.Unknown;
            Tags = Array.Empty<string>();
        }
    }
}