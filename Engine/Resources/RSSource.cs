// ====================================================================================================
//  FILE: RSSource.cs
//  PATH: ./Engine/Resources/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the RSSource module.
//
//  RESPONSIBILITIES:
//      - Provide ToString() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    AssetSource.cs
Author:  BDC
Created: 2026-02-07
Purpose: Represents the origin of an asset (file, memory, bundle, etc.).
Notes:   Used by loaders, registries, and validation systems.
*/

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Assets
{
    ///<summary>
    ///Represents the origin of an asset.
    ///</summary>
    public sealed class AssetSource
    {
        ///<summary>
        ///The type of source (File, Memory, Bundle, etc.).
        ///</summary>
        public SourceType Type { get; }

        ///<summary>
        ///The path or identifier associated with the source.
        ///For file sources, this is a file path.
        ///For bundle sources, this is a bundle key.
        ///</summary>
        public string Identifier { get; }

        ///<summary>
        ///Optional raw data for memory-based sources.
        ///</summary>
        public byte[]? Data { get; }

        public AssetSource(SourceType type, string identifier, byte[]? data = null)
        {
            Type = type;
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            Data = data;
        }

        public override string ToString()
        {
            return $"AssetSource(Type={Type}, Id='{Identifier}')";
        }
    }

    ///<summary>
    ///Defines the type of asset source.
    ///</summary>
    public enum SourceType
    {
        File = 0,
        Memory = 1,
        Bundle = 2,
        Remote = 3
    }
}



