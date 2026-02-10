/*
    File:    AssetBundle.cs
    Author:  BDC
    Created: 2026-02-07
    Purpose: Represents a collection of assets packaged together for loading.
    Notes:   Bundles map keys to sources and metadata. Used by loaders and registries.
*/

using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Systems.Assets
{
    /// <summary>
    /// Represents a group of assets packaged together.
    /// </summary>
    public sealed class AssetBundle
    {
        /// <summary>
        /// The name of the bundle (e.g., "ui", "level1", "common").
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Maps asset keys to their sources.
        /// </summary>
        public IReadOnlyDictionary<AssetKey, AssetSource> Sources { get; }

        /// <summary>
        /// Maps asset keys to their metadata.
        /// </summary>
        public IReadOnlyDictionary<AssetKey, AssetMetadata> Metadata { get; }

        public AssetBundle(
            string name,
            IDictionary<AssetKey, AssetSource> sources,
            IDictionary<AssetKey, AssetMetadata> metadata)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Sources = new Dictionary<AssetKey, AssetSource>(sources);
            Metadata = new Dictionary<AssetKey, AssetMetadata>(metadata);
        }

        /// <summary>
        /// Returns true if the bundle contains the given asset key.
        /// </summary>
        public bool Contains(AssetKey key)
        {
            return Sources.ContainsKey(key);
        }

        /// <summary>
        /// Attempts to retrieve the source for a given asset key.
        /// </summary>
        public bool TryGetSource(AssetKey key, out AssetSource? source)
        {
            return Sources.TryGetValue(key, out source);
        }

        /// <summary>
        /// Attempts to retrieve metadata for a given asset key.
        /// </summary>
        public bool TryGetMetadata(AssetKey key, out AssetMetadata? metadata)
        {
            return Metadata.TryGetValue(key, out metadata);
        }

        public override string ToString()
        {
            return $"AssetBundle(Name='{Name}', Count={Sources.Count})";
        }
    }
}