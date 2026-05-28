/*
File:    AssetMetadata.cs
Purpose: Metadata structure for engine assets.
Features: Asset format, size, and validation information.
*/

using System;

namespace SASZombieAssaultTD.Engine.Resources
{
    /// <summary>
    /// Metadata for an engine asset.
    /// Provides information about asset format, size, and validation.
    /// </summary>
    public class AssetMetadata
    {
        ///  Core Properties

        /// <summary>
        /// The asset format (e.g., "png", "wav", "json").
        /// </summary>
        public string Format { get; set; } = string.Empty;

        /// <summary>
        /// Size of the asset in bytes.
        /// </summary>
        public long SizeBytes { get; set; } = 0;

        /// <summary>
        /// Asset name/identifier.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Asset key/identifier (alias for Name property).
        /// Used for compatibility with validation systems.
        /// </summary>
        public string Key 
        { 
            get => Name; 
            set => Name = value; 
        }

        /// <summary>
        /// Asset path relative to asset root.
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Asset type category.
        /// </summary>
        public AssetType Type { get; set; } = AssetType.Unknown;

        /// <summary>
        /// When the asset was last modified.
        /// </summary>
        public DateTime LastModified { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Checksum for validation.
        /// </summary>
        public string? Checksum { get; set; }

        /// <summary>
        /// Whether this asset is critical for game operation.
        /// </summary>
        public bool IsCritical { get; set; } = false;

        /// 

        ///  Validation

        /// <summary>
        /// Validates the metadata.
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Format) && 
                   SizeBytes >= 0 && 
                   !string.IsNullOrEmpty(Name) &&
                   Type != AssetType.Unknown;
        }

        /// 
    }

    /// <summary>
    /// Types of assets in the engine.
    /// </summary>
    // NOTE: AssetType already defined in Engine/Resources/Manager/AssetType.cs
    //public enum AssetType
    //{
    //    Unknown,
    //    Texture,
    //    Audio,
    //    Font,
    //    Model,
    //    Animation,
    //    Shader,
    //    Configuration,
    //    Data,
    //    Script,
    //    SpriteSheet,
    //    Sound,
    //    Music,
    //    Json,
    //    Binary
    //}
}
