/*
    File:    AssetType.cs
    Author:  BDC
    Created: 2026-02-07
    Purpose: Defines the categories of assets supported by the engine.
    Notes:   Expandable enumeration used by loaders, registries, and metadata.
*/

namespace SASZombieAssaultTD.Engine.Systems.Assets
{
    /// <summary>
    /// Represents the general category of an asset.
    /// Used for classification, validation, and loader routing.
    /// </summary>
    public enum AssetType
    {
        Unknown = 0,

        // Visual assets
        Texture = 1,
        SpriteSheet = 2,

        // Audio assets
        Sound = 3,
        Music = 4,

        // Data assets
        Json = 5,
        Binary = 6,

        // Future expansion points
        Font = 7,
        Shader = 8
    }
}