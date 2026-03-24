/*
File: RSPipeline.cs
Author: BDC
Created: 2026-02-08

Purpose:
Performs the actual loading of resources after discovery, validation,
and registration. Converts metadata into loaded runtime objects.

Notes:
Deterministic. Returns RSBatchLoadResult.
*/

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Resources;
using LoadTextureBytes = SASZombieAssaultTD.Engine.Resources.TextureLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Resources
{
    /// <summary>
    /// Resource system load context.
    /// </summary>
    public sealed class RSLoadContext
    {
        public List<DiscoveredResource> Assets { get; set; }
        public string RootPath { get; set; }
        public bool Recursive { get; set; }
        public Dictionary<string, object> Options { get; set; }

        public RSLoadContext()
        {
            Assets = new List<DiscoveredResource>();
            Options = new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Resource system batch load result.
    /// </summary>
    public sealed class RSBatchLoadResult
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<string> Errors { get; set; }
        public TimeSpan LoadTime { get; set; }

        public RSBatchLoadResult()
        {
            Errors = new List<string>();
        }
    }

    /// <summary>
    /// Loads all resources defined in the RSLoadContext.
    /// </summary>
    public static class RSPipeline
    {
        /// <summary>
        /// Loads all resources (textures + data) using the metadata provided
        /// in the load context. Returns a summary of successes/failures.
        /// </summary>
        public static RSBatchLoadResult LoadAll(RSLoadContext context)
        {
            ModernLoggingSystem.Log("INFO", "[Resources] Phase5: LoadOrder: Begin");

            int success = 0;
            int failure = 0;

            // Enforce deterministic load order: group by RSType, sort by Key
            var ordered = context.Assets
            .OrderBy(m => m.Type)
            .ThenBy(m => m.Name, StringComparer.Ordinal);

            foreach (var meta in ordered)
            {
                try
                {
                    switch (meta.Type)
                    {
                        case RSType.Texture:
                            LoadTexture(meta);
                            break;

                        case RSType.Sound:
                        case RSType.Music:
                            LoadAudio(meta);
                            break;

                        case RSType.Json:
                        case RSType.Binary:
                            LoadData(meta);
                            break;

                        default:
                            ModernLoggingSystem.Log("Warn", $"[Assets] Unknown asset type for {meta.Name}.");
                            failure++;
                            continue;
                    }

                    success++;
                }
                catch (Exception ex)
                {
                    ModernLoggingSystem.Log("Error", $"[Assets] Failed to load {meta.Name}: {ex.Message}");
                    failure++;
                }
            }

            ModernLoggingSystem.Log("INFO", "[Assets] Phase5: LoadOrder: Completed");
            return new RSBatchLoadResult { SuccessCount = success, FailureCount = failure };
        }

        /// <summary>
        /// Validates that the engine is ready at runtime: assets are registered
        /// and no null or empty entries exist in the registry.
        /// </summary>
        public static bool ValidateRuntimeReadiness()
        {
            try
            {
                if (RSRegistry.Count == 0)
                {
                    ModernLoggingSystem.Log("WARNING", "[Assets] RuntimeReadiness: No assets registered.");
                    return false;
                }

                foreach (var kvp in RSRegistry.All())
                {
                    if (string.IsNullOrWhiteSpace(kvp.Key) || string.IsNullOrWhiteSpace(kvp.Value))
                    {
                        ModernLoggingSystem.Log("WARNING", $"[Assets] RuntimeReadiness: Invalid registry entry: key='{kvp.Key}', path='{kvp.Value}'.");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"[Assets] RuntimeReadiness: Validation failed: {ex.Message}");
                return false;
            }
        }

        // ------------------------------------------------------------
        // Texture Loading
        // ------------------------------------------------------------
        private static void LoadTexture(DiscoveredResource meta)
        {
            if (!File.Exists(meta.Path))
                throw new FileNotFoundException($"Texture file not found: {meta.Path}");

            byte[] bytes = LoadTextureBytes(meta.Path);

            RSRegistry.Register(meta.Name, meta.Path);

            ModernLoggingSystem.Log("Info", $"[Assets] Loaded texture: {meta.Name}");
        }

        private static byte[] LoadTextureBytes(string path)
        {
            throw new NotImplementedException();
        }

        // ------------------------------------------------------------
        // Audio Loading
        // ------------------------------------------------------------
        private static void LoadAudio(DiscoveredResource meta)
        {
            if (!File.Exists(meta.Path))
                throw new FileNotFoundException($"Audio file not found: {meta.Path}");

            // Register the audio asset path for runtime retrieval.
            // Actual decoding is deferred to the audio subsystem at playback time.
            RSRegistry.Register(meta.Name, meta.Path);

            ModernLoggingSystem.Log("Info", $"[Assets] Loaded audio: {meta.Name} ({meta.Type})");
        }

        // ------------------------------------------------------------
        // Data / JSON Loading
        // ------------------------------------------------------------
        private static void LoadData(DiscoveredResource meta)
        {
            if (!File.Exists(meta.Path))
                throw new FileNotFoundException($"Data file not found: {meta.Path}");

            object data = Assets.DataLoader.Load(meta.Path);

            RSRegistry.Register(meta.Name, meta.Path);

            ModernLoggingSystem.Log("Info", $"[Assets] Loaded data: {meta.Name}");
        }
    }
}

































































































































































































































































