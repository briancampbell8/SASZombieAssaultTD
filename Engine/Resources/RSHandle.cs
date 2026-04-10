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
    public class AssetHandle
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
        public Type InstanceType => Instance?.GetType() ?? typeof(object);

        /// <summary>
        /// The asset type for this handle.
        /// </summary>
        public AssetType AssetType { get; }

        /// <summary>
        /// Number of references to this asset.
        /// </summary>
        public int ReferenceCount { get; set; }

        /// <summary>
        /// Last time this asset was accessed.
        /// </summary>
        public DateTime LastAccessed { get; set; }

        /// <summary>
        /// Memory size of this asset in bytes.
        /// </summary>
        public long MemorySize { get; set; }

        /// <summary>
        /// Metadata for this asset.
        /// </summary>
        public AssetMetadata Metadata { get; set; }

        /// <summary>
        /// Gets whether the asset is loaded.
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                // Return true when the underlying asset is fully loaded and valid.
                return Instance != null;
            }
        }

        public AssetHandle(AssetKey key, object instance)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
            AssetType = AssetType.Unknown;
            ReferenceCount = 0;
            LastAccessed = DateTime.UtcNow;
            MemorySize = 0;
        }

        public AssetHandle(AssetKey key, AssetType assetType, AssetPriority priority)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            AssetType = assetType;
            ReferenceCount = 0;
            LastAccessed = DateTime.UtcNow;
            MemorySize = 0;
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

        public void Dispose()
        {
            // Cleanup logic here
        }
    }
}



