using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Resources
{
    /// <summary>
    /// Static asset registry for managing game resources.
    /// Provides centralized storage, retrieval, and enumeration functionality for the SAS Zombie Assault TD engine.
    /// </summary>
    /// <remarks>
    /// AssetRegistry serves as the primary storage mechanism for all game assets including:
    /// - Textures and sprites
    /// - Audio files and sound effects  
    /// - 3D models and meshes
    /// - UI elements and fonts
    /// - Configuration data and scripts
    /// </remarks>
    public static class AssetRegistry
    {
        /// <summary>
        /// Internal dictionary storing all registered assets with their unique identifiers.
        /// Uses object type to allow storage of any asset type.
        /// </summary>
        private static readonly Dictionary<string, object> _assets = new();

        /// <summary>
        /// Registers an asset with a unique name identifier.
        /// Stores the asset in the internal dictionary for later retrieval.
        /// </summary>
        /// <param name="name">Unique identifier for the asset. Must not be null or whitespace.</param>
        /// <param name="asset">The asset object to register. Can be any type.</param>
        /// <exception cref="ArgumentException">Thrown when name is null, empty, or whitespace.</exception>
        /// <remarks>
        /// If an asset with the same name already exists, it will be overwritten.
        /// Consider using TryRegisterAsset if you want to avoid overwriting existing assets.
        /// </remarks>
        public static void RegisterAsset(string name, object asset)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Asset name cannot be null or empty.", nameof(name));

            _assets[name] = asset;
        }

        /// <summary>
        /// Retrieves an asset by name and casts it to the requested type.
        /// Provides type-safe access to stored assets.
        /// </summary>
        /// <typeparam name="T">The type to cast the asset to.</typeparam>
        /// <param name="name">The unique identifier of the asset to retrieve.</param>
        /// <returns>The asset cast to type T, or default(T) if not found.</returns>
        /// <remarks>
        /// If the asset is not found, returns the default value for type T (null for reference types).
        /// If the asset cannot be cast to type T, an InvalidCastException will be thrown.
        /// </remarks>
        public static T GetAsset<T>(string name)
        {
            if (_assets.TryGetValue(name, out var asset))
                return (T)asset;

            return default;
        }

        /// <summary>
        /// Returns a copy of all registered assets as a dictionary.
        /// Provides enumeration access to the complete asset collection.
        /// </summary>
        /// <returns>A new Dictionary containing all asset name-value pairs.</returns>
        /// <remarks>
        /// Returns a copy to prevent external modification of the internal asset storage.
        /// The returned dictionary can be safely modified without affecting the AssetRegistry.
        /// </remarks>
        public static Dictionary<string, object> All()
        {
            return new Dictionary<string, object>(_assets);
        }

        /// <summary>
        /// Checks if an asset with the given name is registered.
        /// </summary>
        /// <param name="name">The unique identifier of the asset to check.</param>
        /// <returns>True if the asset exists, false otherwise.</returns>
        public static bool ContainsAsset(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && _assets.ContainsKey(name);
        }

        /// <summary>
        /// Removes an asset from the registry.
        /// </summary>
        /// <param name="name">The unique identifier of the asset to remove.</param>
        /// <returns>True if the asset was removed, false if it didn't exist.</returns>
        public static bool RemoveAsset(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return _assets.Remove(name);
        }

        /// <summary>
        /// Clears all registered assets from the registry.
        /// </summary>
        public static void Clear()
        {
            _assets.Clear();
        }
    }
}
