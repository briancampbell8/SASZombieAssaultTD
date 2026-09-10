// ====================================================================================================
//  FILE: RSLoadResult.cs
//  PATH: ./Engine/Resources/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the RSLoadResult module.
//
//  RESPONSIBILITIES:
//      - Provide Ok() behavior for the Core subsystem.
//      - Provide Fail() behavior for the Core subsystem.
//      - Provide ToString() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    AssetLoadResult.cs
Author:  BDC
Created: 2026-02-07
Purpose: Represents the result of an asset loading operation.
Notes:   Immutable. Used by loaders, registries, and validation systems.
*/

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Assets
{
    ///<summary>
    ///Represents the outcome of loading an asset.
    ///</summary>
    public sealed class AssetLoadResult
    {
        ///<summary>
        ///Whether the load operation succeeded.
        ///</summary>
        public bool Success { get; }

        ///<summary>
        ///The loaded asset instance, if successful.
        ///</summary>
        public object? Instance { get; }

        ///<summary>
        ///Optional error message if loading failed.
        ///</summary>
        public string? Error { get; }

        ///<summary>
        ///The key associated with the asset.
        ///</summary>
        public AssetKey Key { get; }

        private AssetLoadResult(AssetKey key, bool success, object? instance, string? error)
        {
            Key = key;
            Success = success;
            Instance = instance;
            Error = error;
        }

        public static AssetLoadResult Ok(AssetKey key, object instance)
        {
            return new AssetLoadResult(key, true, instance, null);
        }

        public static AssetLoadResult Fail(AssetKey key, string error)
        {
            return new AssetLoadResult(key, false, null, error);
        }

        public override string ToString()
        {
            return Success
            ? $"AssetLoadResult(OK, {Key})"
            : $"AssetLoadResult(FAIL, {Key}, Error={Error})";
        }
    }
}



