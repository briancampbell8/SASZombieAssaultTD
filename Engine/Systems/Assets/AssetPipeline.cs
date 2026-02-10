/*
File: AssetPipeline.cs
Author: BDC
Created: 2026-02-08

Purpose:
    Performs the actual loading of assets after discovery, validation,
    and registration. Converts metadata into loaded runtime objects.

Notes:
    Deterministic. Returns AssetBatchLoadResult.
*/

using SASZombieAssaultTD.Engine.Systems.Diagnostics;
using System;
using System.IO;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Systems.Assets
{
    /// <summary>
    /// Loads all assets defined in the AssetLoadContext.
    /// </summary>
    public static class AssetPipeline
    {
        /// <summary>
        /// Loads all assets (textures + data) using the metadata provided
        /// in the load context. Returns a summary of successes/failures.
        /// </summary>
        public static AssetBatchLoadResult LoadAll(AssetLoadContext context)
        {
            DebugLogger.Log(DebugLogger.Phase5, "[Assets] Phase5: LoadOrder: Begin");

            int success = 0;
            int failure = 0;

            // Enforce deterministic load order: group by AssetType, sort by Key
            var ordered = context.Assets
                .OrderBy(m => m.Type)
                .ThenBy(m => m.Key, StringComparer.Ordinal);

            foreach (var meta in ordered)
            {
                try
                {
                    switch (meta.Type)
                    {
                        case AssetType.Texture:
                            LoadTexture(meta);
                            break;

                        case AssetType.Sound:
                        case AssetType.Music:
                            LoadAudio(meta);
                            break;

                        case AssetType.Json:
                        case AssetType.Binary:
                            LoadData(meta);
                            break;

                        default:
                            DebugLogger.Log("Warn", $"[Assets] Unknown asset type for {meta.Key}.");
                            failure++;
                            continue;
                    }

                    success++;
                }
                catch (Exception ex)
                {
                    DebugLogger.Log("Error", $"[Assets] Failed to load {meta.Key}: {ex.Message}");
                    failure++;
                }
            }

            DebugLogger.Log(DebugLogger.Phase5, "[Assets] Phase5: LoadOrder: Completed");
            return new AssetBatchLoadResult(success, failure);
        }

        /// <summary>
        /// Validates that the engine is ready at runtime: assets are registered
        /// and no null or empty entries exist in the registry.
        /// </summary>
        public static bool ValidateRuntimeReadiness()
        {
            try
            {
                if (AssetRegistry.Count == 0)
                {
                    DebugLogger.Log(DebugLogger.Phase5,
                        "[Assets] RuntimeReadiness: No assets registered.");
                    return false;
                }

                foreach (var kvp in AssetRegistry.All())
                {
                    if (string.IsNullOrWhiteSpace(kvp.Key) || string.IsNullOrWhiteSpace(kvp.Value))
                    {
                        DebugLogger.Log(DebugLogger.Phase5,
                            $"[Assets] RuntimeReadiness: Invalid registry entry: key='{kvp.Key}', path='{kvp.Value}'.");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.Log(DebugLogger.Phase5,
                    $"[Assets] RuntimeReadiness: Validation failed: {ex.Message}");
                return false;
            }
        }

        // ------------------------------------------------------------
        // Texture Loading
        // ------------------------------------------------------------
        private static void LoadTexture(AssetMetadata meta)
        {
            if (!File.Exists(meta.Path))
                throw new FileNotFoundException($"Texture file not found: {meta.Path}");

            byte[] bytes = TextureLoader.LoadTextureBytes(meta.Path);

            AssetRegistry.Register(meta.Key, meta.Path);

            DebugLogger.Log("Info", $"[Assets] Loaded texture: {meta.Key}");
        }

        // ------------------------------------------------------------
        // Audio Loading
        // ------------------------------------------------------------
        private static void LoadAudio(AssetMetadata meta)
        {
            if (!File.Exists(meta.Path))
                throw new FileNotFoundException($"Audio file not found: {meta.Path}");

            // Register the audio asset path for runtime retrieval.
            // Actual decoding is deferred to the audio subsystem at playback time.
            AssetRegistry.Register(meta.Key, meta.Path);

            DebugLogger.Log("Info", $"[Assets] Loaded audio: {meta.Key} ({meta.Type})");
        }

        // ------------------------------------------------------------
        // Data / JSON Loading
        // ------------------------------------------------------------
        private static void LoadData(AssetMetadata meta)
        {
            if (!File.Exists(meta.Path))
                throw new FileNotFoundException($"Data file not found: {meta.Path}");

            object data = DataLoader.Load(meta.Path);

            AssetRegistry.Register(meta.Key, meta.Path);

            DebugLogger.Log("Info", $"[Assets] Loaded data: {meta.Key}");
        }
    }
}