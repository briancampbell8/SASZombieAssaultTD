// ====================================================================================================
//  FILE: RSKey.cs
//  PATH: ./Engine/Resources/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the RSKey module.
//
//  RESPONSIBILITIES:
//      - Provide Equals() behavior for the Core subsystem.
//      - Provide Equals() behavior for the Core subsystem.
//      - Provide GetHashCode() behavior for the Core subsystem.
//      - Provide ToString() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    RSKey.cs
Author:  BDC
Created: 2026-02-07
Purpose: Strongly-typed identifier for resources in the engine. Combines type and string key.
Notes:   Immutable, value-based equality. Safe for use as dictionary keys and registries.
*/

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Resources
{
    ///<summary>
    ///Represents a unique identifier for a resource, combining its runtime type and a string key
    ///(such as a path, name, or logical identifier).
    ///</summary>
    public readonly struct RSKey : IEquatable<RSKey>
    {
        ///<summary>
        ///The runtime type of the resource (e.g., Texture2D, Sound, JsonData).
        ///</summary>
        public Type AssetType { get; }

        ///<summary>
        ///The string identifier for the resource (e.g., "ui/main_menu/background").
        ///</summary>
        public string Key { get; }

        public RSKey(Type assetType, string key)
        {
            AssetType = assetType ?? throw new ArgumentNullException(nameof(assetType));
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        ///<summary>
        ///Creates an RSKey for a specific resource type T.
        ///</summary>
        public static RSKey For<T>(string key)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            return new RSKey(typeof(T), key);
        }

        public bool Equals(RSKey other)
        {
            return AssetType == other.AssetType &&
            string.Equals(Key, other.Key, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj)
        {
            return obj is RSKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 31) + AssetType.GetHashCode();
                hash = (hash * 31) + StringComparer.Ordinal.GetHashCode(Key);
                return hash;
            }
        }

        public static bool operator ==(RSKey left, RSKey right) => left.Equals(right);
        public static bool operator !=(RSKey left, RSKey right) => !left.Equals(right);

        ///<summary>
        ///Returns the string representation of the RSKey.
        ///</summary>
        public override string ToString()
        {
            return Key;
        }
    }
}



