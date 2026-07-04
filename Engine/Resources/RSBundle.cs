/*
//============================================================================
//File:        RSBundle.cs
//Path:        E:\BDC\Projects\SASZombieAssault4\Resources\RSBundle.cs
//Program:     RSBundle
//Subsystem:   Resources / Bundles
//Author:      BDC
//Created:     2026-02-07
//
//Purpose:
//    Represents a collection of low-level resources packaged together for
//    loading and lookup by the RS (Resource System) layer. Bundles map keys
//    to sources and metadata and are consumed by higher-level loaders and
//    registries.
//
//Notes:
//    This is the RS-layer equivalent of an asset bundle. It is intentionally
//    lightweight and untyped, leaving type resolution to AssetManager.
//============================================================================
*/

using System;
using System.Collections.Generic;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Assets
{
    ///<summary>
    ///Represents a group of resources packaged together for RSManager.
    ///</summary>
    public sealed class RSBundle
    {
        ///<summary>
        ///The name of the bundle (e.g., "ui", "level1", "common").
        ///</summary>
        public string Name { get; }

        ///<summary>
        ///Maps resource keys to their sources.
        ///</summary>
        public IReadOnlyDictionary<AssetKey, AssetSource> Sources { get; }

        ///<summary>
        ///Maps resource keys to their metadata.
        ///</summary>
        public IReadOnlyDictionary<AssetKey, AssetMetadata> Metadata { get; }

        public RSBundle(
            string name,
            IDictionary<AssetKey, AssetSource> sources,
            IDictionary<AssetKey, AssetMetadata> metadata)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Sources = new Dictionary<AssetKey, AssetSource>(sources);
            Metadata = new Dictionary<AssetKey, AssetMetadata>(metadata);
        }

        ///<summary>
        ///Returns true if the bundle contains the given resource key.
        ///</summary>
        public bool Contains(AssetKey key)
        {
            return Sources.ContainsKey(key);
        }

        ///<summary>
        ///Attempts to retrieve the source for a given resource key.
        ///</summary>
        public bool TryGetSource(AssetKey key, out AssetSource? source)
        {
            return Sources.TryGetValue(key, out source);
        }

        ///<summary>
        ///Attempts to retrieve metadata for a given resource key.
        ///</summary>
        public bool TryGetMetadata(AssetKey key, out AssetMetadata? metadata)
        {
            return Metadata.TryGetValue(key, out metadata);
        }

        public override string ToString()
        {
            return $"RSBundle(Name='{Name}', Count={Sources.Count})";
        }
    }
}
