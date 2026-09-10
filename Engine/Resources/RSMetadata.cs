// ====================================================================================================
//  FILE: RSMetadata.cs
//  PATH: ./Engine/Resources/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the RSMetadata module.
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
/*
File:    RSMetadata.cs
Author:  BDC
Created: 2026-02-07
Purpose: Describes metadata associated with a resource, including type, size, format, and tags.
Notes:   Mutable. Used by loaders, validation, and resource registries.
*/

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Resources
{
    ///<summary>
    ///Represents descriptive metadata for a resource.
    ///</summary>
    public sealed class RSMetadata
    {
        ///<summary>
        ///The unique key identifying this asset.
        ///</summary>
        public string Key { get; set; }

        ///<summary>
        ///The file system path to the asset.
        ///</summary>
        public string Path { get; set; }

        ///<summary>
        ///The category of the resource (Texture, Sound, Json, etc.).
        ///</summary>
        public RSType Type { get; set; }

        ///<summary>
        ///Optional file size in bytes, if known.
        ///</summary>
        public long? SizeBytes { get; set; }

        ///<summary>
        ///Optional format string (e.g., "png", "wav", "json").
        ///</summary>
        public string? Format { get; set; }

        ///<summary>
        ///Optional tags for classification or search.
        ///</summary>
        public IReadOnlyList<string> Tags { get; set; }

        public RSMetadata(
        string key,
        string path,
        RSType type,
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

        //Parameterless constructor for scenarios where properties are set afterward
        public RSMetadata(object key)
        {
            Key = string.Empty;
            Path = string.Empty;
            Type = RSType.Unknown;
            Tags = Array.Empty<string>();
        }
    }
}



