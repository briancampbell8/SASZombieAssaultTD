/*
File:    RSDiscovery.cs
Author:  BDC
Created: 2026-02-07
Purpose: Scans resource directories and produces discovered resource entries
including keys, sources, and metadata.
Notes:   Stateless. First step in the resource pipeline.
Discover method returns empty list on error and logs details.
*/

using SASZombieAssaultTD.Engine.Assets;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;

namespace SASZombieAssaultTD.Engine.Resources
{
    /// <summary>
    /// Discovers resources on disk and produces RSSource + RSMetadata pairs.
    /// </summary>
    public sealed class RSDiscovery
    {
        private object TheType;
        private object TheMember;

        /// <summary>
        /// Scans the root directory and returns discovered resources.
        /// Returns empty list on error; logs error details.
        /// </summary>
        public IReadOnlyList<DiscoveredResource> Discover(RSLoadContext context)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var results = new List<DiscoveredResource>();

            string root = context.RootPath;
            if (!Directory.Exists(root))
                return results;

            IEnumerable<string> files;

            try
            {
                files = Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories);
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("Error", $"[Assets] Failed to enumerate files under '{root}': {ex.Message}");
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

                    RSType type = InferTypeFromExtension(extension);
                    if (type == RSType.Unknown)
                        continue;

                    string relative = AssetUtils.NormalizePath(Path.GetRelativePath(root, file));

                    var fileInfo = new FileInfo(file);
                    var metadata = new RSMetadata(relative, normalized, type, fileInfo.Length, extension);

                    bool isValid = ValidateMetadata(metadata);
                    if (!isValid)
                    {
                        Engine.Diagnostics.DebugLogger.LogDebug("Warn", $"[Assets] Invalid metadata for {metadata.Key}: Type={metadata.Type}, Format={metadata.Format ?? "unknown"}");
                        continue;
                    }

                    results.Add(new DiscoveredResource
                    {
                        Name = relative,
                        Path = normalized,
                        Type = type
                    });
                }
                catch (Exception ex)
                {
                    Engine.Diagnostics.DebugLogger.LogDebug("Error", $"[Assets] Failed to process file '{file}': {ex.Message}");
                    continue;
                }
            }

            return results;
        }

        /// <summary>
        /// Maps file extensions to resource types.
        /// </summary>
        private static RSType InferTypeFromExtension(string ext)
        {
            ext = ext.ToLowerInvariant();

            return ext switch
            {
                "png" or "jpg" or "jpeg" => RSType.Texture,
                "json" => RSType.Json,
                "wav" => RSType.Sound,
                "ogg" => RSType.Music,
                _ => RSType.Unknown
            };
        }

        /// <summary>
        /// Validates that metadata is consistent with the resource type.
        /// </summary>
        private static bool ValidateMetadata(RSMetadata metadata)
        {
            if (metadata is null)
                return false;

            if (string.IsNullOrWhiteSpace(metadata.Key))
                return false;

            if (string.IsNullOrWhiteSpace(metadata.Path))
                return false;

            if (metadata.Type == RSType.Unknown)
                return false;

            return true;
        }

        internal IReadOnlyList<DiscoveredResource> Discover(AssetLoadContext discoveryContext)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Discovered resource for asset discovery system.
    /// </summary>
    public class DiscoveredResource
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public RSType Type { get; set; }

        public DiscoveredResource()
        {
            Name = "";
            Path = "";
            Type = RSType.Unknown;
        }
    }
}


