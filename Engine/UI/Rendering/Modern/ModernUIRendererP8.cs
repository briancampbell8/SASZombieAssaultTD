// =====================================================================================================
//  FILE: ModernUIRendererP8.cs
//  PATH: Engine/UI/Rendering/Modern/ModernUIRendererP8.cs
//  SUBSYSTEM: Modern UI Rendering — Resource Validation & Volatile Cleanup (Partial)
//
//  ROLE:
//      Provides resource validation helpers and volatile cache cleanup for the Modern UI renderer.
//      Ensures texture resources are available and clears transient effect/target dictionaries safely.
//
//  RESPONSIBILITIES:
//      - Validate texture resource availability via the UI texture atlas manager.
//      - Provide a safe check for named UI texture assets.
//      - Clear volatile effect and render target caches.
//      - Log errors and telemetry for resource validation and cleanup paths.
//
//  NON‑RESPONSIBILITIES:
//      - Element rendering or command construction.
//      - Batching, sorting, or command ordering.
//      - GPU resource ownership or lifetime management.
//      - Performance heuristics or adaptive quality logic.
//
//  ARCHITECTURAL NOTES:
//      - This partial MUST own all fields it uses (P8_* naming).
//      - No cross‑partial field access is permitted.
//      - Logging is restricted to non‑hot paths.
// =====================================================================================================

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI.Rendering.Modern
{
    public partial class ModernUIRenderer
    {
        // --------------------------------------------------------------------
        // P8 OWNED FIELDS — NO CROSS-PARTIAL DEPENDENCIES
        // --------------------------------------------------------------------
        private UITextureAtlasManager P8_textureAtlasManager;
        private readonly Dictionary<string, UIMaterial> P8_effects = new();
        private readonly Dictionary<string, object> P8_renderTargets = new();

        // --------------------------------------------------------------------
        // BINDING API
        // --------------------------------------------------------------------
        public void P8_SetResources(UITextureAtlasManager atlasManager)
        {
            P8_textureAtlasManager = atlasManager;
        }

        // --------------------------------------------------------------------
        // RESOURCE VALIDATION
        // --------------------------------------------------------------------
        public bool P8_IsTextureResourceAvailable(string textureName)
        {
            if (string.IsNullOrWhiteSpace(textureName))
            {
                DLogger.Log(
                    LogSubsystems.ResourcesPipeline,
                    "Error",
                    "[ModernUIRendererP8] Resource lookup failed: Provided asset name is null or empty."
                );
                return false;
            }

            try
            {
                var material = P8_textureAtlasManager.GetMaterialForElement(textureName);
                return material != null;
            }
            catch (Exception ex)
            {
                DLogger.Log(
                    LogSubsystems.ResourcesPipeline,
                    "Error",
                    $"[ModernUIRendererP8] IsTextureResourceAvailable check failed for '{textureName}': {ex.Message}"
                );
                return false;
            }
        }

        // --------------------------------------------------------------------
        // RELEASE VOLATILE RESOURCES
        // --------------------------------------------------------------------
        private void P8_ReleaseVolatileResources()
        {
            try
            {
                P8_effects.Clear();
                P8_renderTargets.Clear();

                DLogger.Log(
                    LogSubsystems.ResourcesPipeline,
                    "Telemetry",
                    "[ModernUIRendererP8] Subsystem volatile resource caches cleared successfully."
                );
            }
            catch (Exception ex)
            {
                DLogger.Log(
                    LogSubsystems.ResourcesPipeline,
                    "Error",
                    $"[ModernUIRendererP8] ReleaseVolatileResources encountered an error: {ex.Message}"
                );
            }
        }
    }
}
