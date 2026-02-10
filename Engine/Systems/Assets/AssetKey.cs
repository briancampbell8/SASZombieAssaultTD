/*
    File:    AssetKey.cs
    Author:  BDC
    Created: 2026-02-07
    Purpose: Strongly-typed identifier for assets in the engine. Combines type and string key.
    Notes:   Immutable, value-based equality. Safe for use as dictionary keys and registries.
*/

using System;

namespace SASZombieAssaultTD.Engine.Systems.Assets
{
    /// <summary>
    /// Represents a unique identifier for an asset, combining its runtime type and a string key
    /// (such as a path, name, or logical identifier).
    /// </summary>
    public readonly struct AssetKey : IEquatable<AssetKey>
    {
        /// <summary>
        /// The runtime type of the asset (e.g., Texture2D, Sound, JsonData).
        /// </summary>
        public Type AssetType { get; }

        /// <summary>
        /// The string identifier for the asset (e.g., "ui/main_menu/background").
        /// </summary>
        public string Key { get; }

        public AssetKey(Type assetType, string key)
        {
            AssetType = assetType ?? throw new ArgumentNullException(nameof(assetType));
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        /// <summary>
        /// Creates an AssetKey for a specific asset type T.
        /// </summary>
        public static AssetKey For<T>(string key)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            return new AssetKey(typeof(T), key);
        }

        public bool Equals(AssetKey other)
        {
            return AssetType == other.AssetType &&
                   string.Equals(Key, other.Key, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj)
        {
            return obj is AssetKey other && Equals(other);
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

        public static bool operator ==(AssetKey left, AssetKey right) => left.Equals(right);
        public static bool operator !=(AssetKey left, AssetKey right) => !left.Equals(right);

        public override string ToString()
        {
            return $"{AssetType.Name}:{Key}";
        }
    }
}