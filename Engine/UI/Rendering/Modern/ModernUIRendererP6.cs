// =====================================================================================================
//  FILE: ModernUIRendererP6.cs
//  PATH: Engine/UI/Rendering/Modern/ModernUIRendererP6.cs
//  SUBSYSTEM: Modern UI Rendering — Element Rendering Layer (Partial)
//
//  ROLE:
//      Performs per‑element rendering for the Modern UI pipeline. Converts UIElementBase layout primitives
//      into deterministic RenderCommand structures and submits them to the command construction subsystem.
//
//  RESPONSIBILITIES:
//      - Translate UIElementBase bounds and position into System.Numerics vectors.
//      - Perform viewport culling using the active P3 viewport.
//      - Acquire UIMaterial instances via P6_textureAtlas.
//      - Construct RenderCommand primitives for visible elements.
//      - Submit commands into the allocationless P5 command subsystem.
//
//  NON‑RESPONSIBILITIES:
//      - Batching, sorting, or command ordering (handled in ModernUIRendererP4).
//      - Command validation or submission policy (handled in ModernUIRendererP5).
//      - GPU resource ownership or lifetime management.
//      - Performance heuristics or adaptive quality logic.
//
//  ARCHITECTURAL NOTES:
//      - Uses element.Id as the canonical RenderCommand.ElementId.
//      - Bridges engine layout types to Vector2/Vector3 for deterministic command construction.
//      - Culling is strictly axis‑aligned and uses screen‑space bounds.
//      - Logging is restricted to error paths to preserve hot‑path determinism.
//      - This partial MUST own all fields it uses (P6_* naming).
// =====================================================================================================

using System;
using System.Numerics;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.TextureRendering;
using SASZombieAssaultTD.Engine.UI.Elements;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI.Rendering.Modern
{
    public partial class ModernUIRenderer
    {
        // --------------------------------------------------------------------
        // P6 OWNED FIELDS — NO CROSS-PARTIAL DEPENDENCIES
        // --------------------------------------------------------------------
        private UITextureAtlasManager P6_textureAtlas;
        private D3D11Adapter_Core P6_uiContext;
        private Vector2 P6_viewportSize;

        // --------------------------------------------------------------------
        // BINDING API
        // --------------------------------------------------------------------
        public void P6_SetContext(D3D11Adapter_Core uiContext, UITextureAtlasManager atlas)
        {
            P6_uiContext = uiContext ?? throw new ArgumentNullException(nameof(uiContext));
            P6_textureAtlas = atlas ?? throw new ArgumentNullException(nameof(atlas));
            P6_viewportSize = uiContext.ViewportSize;
        }

        public void P6_SetViewport(int width, int height)
        {
            if (width <= 0) width = 1;
            if (height <= 0) height = 1;

            P6_viewportSize = new Vector2(width, height);

            P6_uiContext?.SetScreenSize(width, height);
        }

        // --------------------------------------------------------------------
        // RENDER SINGLE ELEMENT
        // --------------------------------------------------------------------
        private void P6_RenderElement(UIElementBase element)
        {
            if (element == null)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "Error",
                    "[ModernUIRendererP6] RenderElement received NULL element.");
                return;
            }

            if (!P6_IsElementInViewport(element))
                return;

            try
            {
                UIAtlasMaterial material = P6_GetElementMaterial(element);

                Vector3 posNumeric = new Vector3(
                    element.Position.X,
                    element.Position.Y,
                    0.0f);

                var bounds = element.GetScreenBounds();

                Vector2 sizeNumeric = new Vector2(
                    bounds.Width,
                    bounds.Height);

                RenderCommand cmd = P5_CreateCommandForElement(
                    elementId: element.Id,
                    position: posNumeric,
                    size: sizeNumeric,
                    material: material);

                P5_SubmitCommand(in cmd);
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "Error",
                    $"[ModernUIRendererP6] RenderElement execution failed: {ex.Message}");
                throw;
            }
        }

        // --------------------------------------------------------------------
        // COMMAND CREATION (STRING → UINT)
        // --------------------------------------------------------------------
        private RenderCommand P5_CreateCommandForElement(string elementId, Vector3 position, Vector2 size, UIAtlasMaterial material)
        {
            if (elementId == null)
                throw new ArgumentNullException(nameof(elementId));

            uint numericId = unchecked((uint)elementId.GetHashCode());

            return P5_CreateCommandForElement(numericId, position, size, material);
        }

        private RenderCommand P5_CreateCommandForElement(uint numericId, Vector3 position, Vector2 size, UIAtlasMaterial material)
        {
            if (material == null)
                throw new ArgumentNullException(nameof(material));

            // Forward to the general UIMaterial overload to avoid duplicating command creation logic.
            return P5_CreateCommandForElement(
                numericId,
                position,
                size,
                (UIMaterial)material);
        }

        // --------------------------------------------------------------------
        // VIEWPORT CULLING
        // --------------------------------------------------------------------
        private bool P6_IsElementInViewport(UIElementBase element)
        {
            var bounds = element.GetScreenBounds();

            float left = element.Position.X;
            float right = element.Position.X + bounds.Width;
            float top = element.Position.Y;
            float bottom = element.Position.Y + bounds.Height;

            bool visible =
                right >= 0 &&
                left <= P6_viewportSize.X &&
                bottom >= 0 &&
                top <= P6_viewportSize.Y;

            return visible;
        }

        // --------------------------------------------------------------------
        // MATERIAL ACQUISITION
        // --------------------------------------------------------------------
        private UIAtlasMaterial P6_GetElementMaterial(UIElementBase element)
        {
            var mat = P6_textureAtlas.GetMaterialForElement(element.GetType().Name);

            if (mat is UIAtlasMaterial atlasMat)
                return atlasMat;

            if (P6_textureAtlas.DefaultMaterial is UIAtlasMaterial defaultMat)
                return defaultMat;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Error",
                "[ModernUIRendererP6] Texture atlas returned non-UIAtlasMaterial for element material and default material.");

            throw new InvalidCastException("UITextureAtlasManager returned an object that is not a UIAtlasMaterial.");
        }
    }
}
