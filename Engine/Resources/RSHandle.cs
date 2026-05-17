/*
File:    AssetHandle.cs
Author:  BDC
Created: 2026-02-07
Purpose: Provides a lightweight reference wrapper for loaded assets.
Notes:   Immutable. Used by registries, loaders, and gameplay systems.
*/

using System;
using SASZombieAssaultTD.Engine.Assets;

namespace SASZombieAssaultTD.Engine.Resources
{
    /// <summary>
    /// Represents a reference to a loaded asset instance.
    /// </summary>
    public sealed class AssetHandle
    {
        /// <summary>
        /// The unique key identifying this asset.
        /// </summary>
        public AssetKey Key { get; }

        /// <summary>
        /// The loaded asset instance.
        /// </summary>
        public object Instance { get; }

        /// <summary>
        /// The type of the loaded asset instance.
        /// </summary>
        public Type InstanceType => Instance.GetType();

        public AssetHandle(AssetKey key, object instance)
        {
            Key = key;
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        /// <summary>
        /// Attempts to cast the asset instance to the specified type.
        /// </summary>
        public T As<T>() where T : class
        {
            return Instance as T
            ?? throw new InvalidCastException(
            $"Asset '{Key}' is not of type {typeof(T).Name}.");
        }

        public override string ToString()
        {
            return $"AssetHandle({Key}, Type={InstanceType.Name})";
        }
    }
}



