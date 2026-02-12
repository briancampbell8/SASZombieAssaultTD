/*
    File:    AssetDiscovery.cs
    Author:  BDC
    Created: 2026-02-07
    Purpose: Scans asset directories and produces discovered asset entries
             including keys, sources, and metadata.
    Notes:   Stateless. First step in the asset pipeline.
             Discover method returns empty list on error and logs details.
*/

using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Systems.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;

namespace SASZombieAssaultTD.Engine.Systems.Assets
{
    /// <summary>
    /// Discovers assets on disk and produces AssetSource + AssetMetadata pairs.
    /// </summary>
    public sealed class AssetDiscovery
    {
        /// <summary>
        /// Scans the root directory and returns discovered assets.
        /// Returns empty list on error; logs error details.
        /// </summary>
        public IReadOnlyList<DiscoveredAsset> Discover(AssetLoadContext context)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var results = new List<DiscoveredAsset>();

            string root = context.RootDirectory;
            if (!Directory.Exists(root))
                return results;

            IEnumerable<string> files;

            try
            {
                files = Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories);
            }
            catch (Exception ex)
            {
                DebugLogger.Log("Error", $"[Assets] Failed to enumerate files under '{root}': {ex.Message}");
                return results;
            }

            foreach (string file in files)
            {
                try
                {
                    string normalized = AssetUtils.NormalizePath(file);
                    string? extension = AssetUtils.GetExtension(normalized);

                    if (extension is null)
                        continue;

                    AssetType type = InferTypeFromExtension(extension);
                    if (type == AssetType.Unknown)
                        continue;

                    string relative = AssetUtils.NormalizePath(Path.GetRelativePath(root, file));

                    var key = new AssetKey(GetRuntimeType(type), relative);
                    var source = new AssetSource(SourceType.File, normalized);
                    var fileInfo = new FileInfo(file);
                    var metadata = new AssetMetadata(relative, normalized, type, fileInfo.Length, extension);

                    bool isValid = AssetValidation.ValidateMetadata(metadata);
                    if (!isValid)
                    {
                        DebugLogger.Log("Warn", $"[Assets] Invalid metadata for {key.Key}: Type={metadata.Type}, Format={metadata.Format ?? "unknown"}");
                        continue;
                    }

                    results.Add(new DiscoveredAsset(key, source, metadata));
                }
                catch (Exception ex)
                {
                    DebugLogger.Log("Error", $"[Assets] Failed to process file '{file}': {ex.Message}");
                    continue;
                }
            }

            return results;
        }

        /// <summary>
        /// Backward-compatible alias for callers using the previous API name.
        /// </summary>
        public IReadOnlyList<DiscoveredAsset> DiscoverAssets(AssetLoadContext context)
        {
            return Discover(context);
        }

        /// <summary>
        /// Maps file extensions to asset types.
        /// </summary>
        private static AssetType InferTypeFromExtension(string ext)
        {
            ext = ext.ToLowerInvariant();

            return ext switch
            {
                "png" or "jpg" or "jpeg" => AssetType.Texture,
                "json" => AssetType.Json,
                "wav" => AssetType.Sound,
                "ogg" => AssetType.Music,
                _ => AssetType.Unknown
            };
        }

        /// <summary>
        /// Maps AssetType to the runtime type used by AssetKey.
        /// </summary>
        private static Type GetRuntimeType(AssetType type)
        {
            return type switch
            {
                AssetType.Texture => typeof(Texture2D),
                AssetType.SpriteSheet => typeof(object),
                AssetType.Sound => typeof(object),
                AssetType.Music => typeof(object),
                AssetType.Json => typeof(System.Text.Json.JsonDocument),
                AssetType.Binary => typeof(byte[]),
                _ => typeof(object)
            };
        }
    }

    /// <summary>
    /// Represents a discovered asset before loading.
    /// </summary>
    public sealed class DiscoveredAsset
    {
        public AssetKey Key { get; }
        public AssetSource Source { get; }
        public AssetMetadata Metadata { get; }

        public DiscoveredAsset(AssetKey key, AssetSource source, AssetMetadata metadata)
        {
            Key = key;
            Source = source;
            Metadata = metadata;
        }
    }
}