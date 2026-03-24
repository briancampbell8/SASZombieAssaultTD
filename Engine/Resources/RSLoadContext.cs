/*
File:    AssetLoadContext.cs
Author:  BDC
Created: 2026-02-07
Purpose: Provides contextual information for asset loading operations.
Notes:   Passed to loaders to supply environment, root paths, and configuration.
*/

using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Assets
{
    /// <summary>
    /// Represents contextual information used during asset loading.
    /// </summary>
    public sealed class AssetLoadContext
    {
        /// <summary>
        /// The root directory where assets are located.
        /// Example: "Content/" or "Assets/".
        /// </summary>
        public string RootDirectory { get; }

        /// <summary>
        /// Optional base path for resolving relative asset paths.
        /// </summary>
        public string? BasePath { get; }

        /// <summary>
        /// Optional service provider for dependency injection.
        /// Useful for loaders that need access to engine services.
        /// </summary>
        public IServiceProvider? Services { get; }

        /// <summary>
        /// List of asset metadata to be loaded.
        /// </summary>
        public List<AssetMetadata> Assets { get; set; }

        public AssetLoadContext(
        string rootDirectory,
        string? basePath = null,
        IServiceProvider? services = null)
        {
            RootDirectory = rootDirectory
            ?? throw new ArgumentNullException(nameof(rootDirectory));

            BasePath = basePath;
            Services = services;
            Assets = new List<AssetMetadata>();
        }

        /// <summary>
        /// Resolves a relative path using the root directory and optional base path.
        /// </summary>
        public string ResolvePath(string relativePath)
        {
            if (relativePath is null)
                throw new ArgumentNullException(nameof(relativePath));

            if (!string.IsNullOrEmpty(BasePath))
                return $"{RootDirectory}/{BasePath}/{relativePath}".Replace("//", "/");

            return $"{RootDirectory}/{relativePath}".Replace("//", "/");
        }

        public override string ToString()
        {
            return $"AssetLoadContext(Root='{RootDirectory}', Base='{BasePath}', Assets={Assets.Count})";
        }
    }
}


