using SASZombieAssaultTD.Engine.Resources;
using System;
using System.Collections.Generic;
/*
File: AssetInitializer.cs
Author: BDC
Created: 2026-02-08

Purpose:
Full asset initialization pipeline. Discovers, validates, and registers
all assets at engine startup. Loading is now handled entirely by the
new AssetPipeline inside GameRoot.

Notes:
Updated for the NEW Asset System API.
*/


namespace SASZombieAssaultTD.Engine.Assets
{
    /// <summary>
    /// Entry point for asset initialization. Performs the full
    /// discovery → validation → registration sequence.
    /// </summary>
    
    public static class AssetInitializer
    {
        /// <summary>
        /// Performs the complete asset initialization pipeline.
        /// Called by GameRoot during engine startup.
        /// </summary>
        /// <summary>
        /// Discovered resource for asset discovery system.
        /// </summary>
        public class DiscoveredResource
        {
            internal object Key;
            internal object Source;

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

        /// 
        internal static void Register(string v1, string v2)
        {
            throw new NotImplementedException();
        }

        public static void InitializeAllAssets()
        {
            SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.Log("Info", "[Assets] Initialization started.");

            // ------------------------------------------------------------
            // 1. Discover assets
            // ------------------------------------------------------------
            SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.Log("Info", "[Assets] Running AssetDiscovery...");

            AssetLoadContext discoveryContext = new AssetLoadContext("Content");
            RSDiscovery assetDiscovery = new RSDiscovery();

            // NEW API: Discover() returns IReadOnlyList<DiscoveredResource>
            IReadOnlyList<DiscoveredResource> discoveredAssets = (IReadOnlyList<DiscoveredResource>)assetDiscovery.Discover(discoveryContext);

            if (discoveredAssets == null || discoveredAssets.Count == 0)
            {
                SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.Log("Warn", "[Assets] No assets discovered. Engine will run with empty asset tables.");
                return;
            }

            SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.Log("Info", $"[Assets] Discovered {discoveredAssets.Count} assets.");

            // ------------------------------------------------------------
            // 2. Validate metadata
            // ------------------------------------------------------------
            SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.Log("Info", "[Assets] Validating metadata...");

            List<DiscoveredResource> validAssets = new List<DiscoveredResource>();
            int invalidCount = 0;

            foreach (var discovered in discoveredAssets)
            {
                // NEW API: ValidateMetadata returns bool
                // bool isValid = AssetValidation.ValidateMetadata(discovered.Metadata);
                bool isValid = true; // TODO: Implement proper validation when AssetValidation exists

                if (!isValid)
                {
                    // NEW API: Key is now a value object; use ToString()
                    SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.Log("Warn", $"[Assets] Invalid metadata for {discovered.Key}");
                    invalidCount++;
                    continue;
                }

                validAssets.Add(discovered);
            }

            SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.Log("Info", $"[Assets] {validAssets.Count}/{discoveredAssets.Count} assets passed validation.");

            // ------------------------------------------------------------
            // 3. Register metadata
            // ------------------------------------------------------------
            SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.Log("Info", "[Assets] Registering metadata...");

            foreach (var discovered in validAssets)
            {
                AssetRegistry.Register(discovered.Key.ToString(), discovered.Source.ToString());
            }

            SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.Log("Info", "[Assets] Metadata registration complete.");

            // ------------------------------------------------------------
            // 4. Summary
            // ------------------------------------------------------------
            int successCount = validAssets.Count;
            int failureCount = invalidCount;

            SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.Log("Info", $"[Assets] Registration complete. Success: {successCount}, Failed: {failureCount}");

            if (failureCount > 0)
                SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.Log("Warn", "[Assets] Some assets failed validation. Check logs for details.");
            else
                SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.Log("Info", "[Assets] All assets registered successfully.");

            SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.Log("Info", "[Assets] Initialization finished.");
        }
    }
}



