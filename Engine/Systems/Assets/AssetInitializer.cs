/*
File: AssetInitializer.cs
Author: BDC
Created: 2026-02-08

Purpose:
    Full asset initialization pipeline. Discovers, validates, registers,
    and loads ALL assets at engine startup.

Notes:
    Deterministic. Overwrites previous placeholder implementation.
*/

using SASZombieAssaultTD.Engine.Systems.Diagnostics;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Systems.Assets
{
    /// <summary>
    /// Entry point for asset initialization. This class performs the full
    /// discovery → validation → registration sequence.
    /// Loading is handled by AssetPipeline.LoadAll in GameRoot.
    /// </summary>
    public static class AssetInitializer
    {
        /// <summary>
        /// Performs the complete asset initialization pipeline.
        /// Called by GameRoot during engine startup.
        /// </summary>
        public static void InitializeAllAssets()
        {
            DebugLogger.Log("Info", "[Assets] Initialization started.");

            // ------------------------------------------------------------
            // 1. Discover assets
            // ------------------------------------------------------------
            DebugLogger.Log("Info", "[Assets] Running AssetDiscovery...");

            // Create load context with root directory
            AssetLoadContext discoveryContext = new AssetLoadContext("Content");
            AssetDiscovery assetDiscovery = new AssetDiscovery();

            // Discover assets from content root
            IReadOnlyList<DiscoveredAsset> discoveredAssets = assetDiscovery.Discover(discoveryContext);

            if (discoveredAssets == null || discoveredAssets.Count == 0)
            {
                DebugLogger.Log("Warn", "[Assets] No assets discovered. Engine will run with empty asset tables.");
                return;
            }

            DebugLogger.Log("Info", $"[Assets] Discovered {discoveredAssets.Count} assets.");

            // ------------------------------------------------------------
            // 2. Validate metadata
            // ------------------------------------------------------------
            DebugLogger.Log("Info", "[Assets] Validating metadata...");

            List<DiscoveredAsset> validAssets = new List<DiscoveredAsset>();
            int invalidCount = 0;

            foreach (var discovered in discoveredAssets)
            {
                bool isValid = AssetValidation.ValidateMetadata(discovered.Metadata);

                if (!isValid)
                {
                    DebugLogger.Log("Warn", $"[Assets] Invalid metadata for {discovered.Key.Key}: Type={discovered.Metadata.Type}, Format={discovered.Metadata.Format ?? "unknown"}");
                    invalidCount++;
                    continue;
                }

                validAssets.Add(discovered);
            }

            DebugLogger.Log("Info", $"[Assets] {validAssets.Count}/{discoveredAssets.Count} assets passed validation.");

            // ------------------------------------------------------------
            // 3. Register metadata
            // ------------------------------------------------------------
            DebugLogger.Log("Info", "[Assets] Registering metadata...");

            foreach (var discovered in validAssets)
            {
                // Register using the AssetKey's Key property and the Source's Identifier
                string keyName = discovered.Key.Key;
                string identifier = discovered.Source.Identifier;

                AssetRegistry.Register(keyName, identifier);
            }

            DebugLogger.Log("Info", "[Assets] Metadata registration complete.");

            // ------------------------------------------------------------
            // 4. Summary
            // ------------------------------------------------------------
            int successCount = validAssets.Count;
            int failureCount = invalidCount;

            DebugLogger.Log("Info", $"[Assets] Registration complete. Success: {successCount}, Failed: {failureCount}");

            if (failureCount > 0)
            {
                DebugLogger.Log("Warn", "[Assets] Some assets failed validation. Check logs for details.");
            }
            else
            {
                DebugLogger.Log("Info", "[Assets] All assets registered successfully.");
            }

            DebugLogger.Log("Info", "[Assets] Initialization finished.");
        }
    }
}