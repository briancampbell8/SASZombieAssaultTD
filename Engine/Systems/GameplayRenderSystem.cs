// =====================================================================================================
//  FILE: GameplayRenderSystem.cs
//  PATH: Engine/Systems/GameplayRenderSystem.cs
//  SUBSYSTEM: System / Gameplay Map Compositor
//
//  ROLE:
//      Dedicated rendering subsystem responsible for drawing the MeanStreets gameplay map
//      and any future gameplay-specific visual layers. It performs no HUD rendering,
//      no UI composition, and no simulation logic.
//
//  RESPONSIBILITIES:
//      - Draw the MeanStreets map texture to the screen.
//      - Provide deterministic rendering order for gameplay visuals.
//      - Integrate with RenderManager as an IRenderSystem participant.
//      - Prefer RenderSystem/IDrawingContext when available.
//
//  NON-RESPONSIBILITIES:
//      - HUD rendering (handled by HUDRenderer).
//      - Dynamic UI overlays (handled by ModernUIRenderer).
//      - Simulation or gameplay logic (handled by UpdateManager).
//      - Texture loading (handled by TextureManager).
//
//  ARCHITECTURAL NOTES:
//      - GameplayRenderSystem is registered before HUDRenderer to ensure HUD overlays
//        appear above gameplay visuals.
//      - Adapter path remains as fallback for transitional compatibility.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.Systems
{
    /// <summary>
    /// Primary gameplay render system responsible for drawing the MeanStreets background
    /// as a flexible windowed surface that matches the current viewport.
    /// </summary>
    public sealed class GameplayRenderSystem : IRenderSystem
    {
        private readonly Texture2D _meanStreetsTexture;

        // Optional RenderSystem binding for GPU-agnostic drawing.
        private RenderSystem? _renderSystem;

        public GameplayRenderSystem(Texture2D meanStreetsTexture)
        {
            _meanStreetsTexture = meanStreetsTexture ?? throw new ArgumentNullException(nameof(meanStreetsTexture));
        }

        /// <summary>
        /// Bind the engine RenderSystem so gameplay rendering can route through IDrawingContext.
        /// </summary>
        public void SetRenderSystem(RenderSystem renderSystem)
        {
            _renderSystem = renderSystem ?? throw new ArgumentNullException(nameof(renderSystem));
        }

        /// <summary>
        /// Renders the gameplay background using the current viewport size.
        /// </summary>
        public void Render(D3D11Adapter_Core adapter_Core)
        {
            if (adapter_Core == null)
                throw new ArgumentNullException(nameof(adapter_Core));

            // Determine viewport size
            int width = (int)adapter_Core.ViewportSize.X;
            int height = (int)adapter_Core.ViewportSize.Y;

            if (width <= 0 || height <= 0)
                return;

            // Build destination rectangle that fills the viewport
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);

            // Preferred path: RenderSystem → IDrawingContext → GPU
            if (_renderSystem != null)
            {
                _renderSystem.DrawTexture(_meanStreetsTexture, destRect, ColorRGBA.White);
                return;
            }

            // Fallback path: direct GPU adapter draw
            adapter_Core.DrawTexture(_meanStreetsTexture, destRect, ColorRGBA.White);
        }
    }
}
