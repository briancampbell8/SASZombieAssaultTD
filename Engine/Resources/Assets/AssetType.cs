/*
//============================================================================
//File:        AssetType.cs
//Path:        Engine/Assets/AssetType.cs
//Program:     AssetType
//Subsystem:   Assets / Core Types
//Author:      BDC
//Created:     2026-05-29
//
//Purpose:
//    Defines the core asset type enumeration and supporting handle structures
//    used throughout the engine. Provides type-safe categorization,
//    identification, and lifecycle coordination for all asset classes.
//
//Architectural Role:
//    • Central asset type taxonomy for the engine
//    • Type-safe classification for AssetManager and AssetRegistryRuntime
//    • Foundation for asset handle and metadata systems
//    • Extensible type system for future asset categories
//
//Core Capabilities:
//    • Strongly-typed asset category enumeration
//    • Asset handle definitions for runtime tracking
//    • Type-safe generic handle operations
//    • Asset metadata association and validation
//
//Integration Points:
//    • AssetManager — asset loading, lifecycle, and dispatch
//    • AssetRegistryRuntime — runtime storage and lookup
//    • AssetBundle — packaged asset distribution
//    • RSManager — low-level resource access
//
//Performance Notes:
//    • Lightweight enum-based categorization
//    • Zero-allocation handle comparison paths
//    • Minimal overhead through direct subsystem delegation
//
//Usage Example:
//    var type = AssetType.Texture;
//    var handle = new AssetHandle<Texture2D>(key, instance);
//    if (handle.Instance is Texture2D tex)
//        Debug.WriteLine($"Loaded texture: {tex.Width}x{tex.Height}");
//============================================================================
*/

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Resources
{
    ///<summary>
    ///Asset types for management in SAS Zombie Assault TD.
    ///Provides categorization system for different asset types and type-safe handles.
    ///</summary>
    public enum AssetType
    {
        Unknown = 0,
        Texture = 1,
        Audio = 2,
        Model = 3,
        Font = 4,
        Data = 5,
        Script = 6,
        Layout = 7,
        Json = 8,
        Binary = 9,
        SpriteSheet = 10,
        Sound = 11,
        Music = 12,
        Shader = 13

        //Add new asset types here as needed
    }
}
