// =====================================================================================================
//  FILE: HUDRenderer.cs
//  PATH: Engine/TextureRendering/HUD/HUDRenderer.cs
//  SUBSYSTEM: UI Rendering / HUD Texture Compositor
//
//  ROLE:
//      Pure HUD rendering subsystem. Draws HUD_Left and HUD_Right textures deterministically.
//      Preferentially routes HUD draw calls through RenderSystem/IDrawingContext, with a
//      fallback to direct D3D11Adapter_Core sprite draws when no RenderSystem is bound.
//
//  RESPONSIBILITIES:
//      - Retrieve HUD textures from HUDManager.
//      - Compute pixel-accurate rectangles for HUD panels.
//      - Issue draw calls via RenderSystem when available, otherwise via D3D11Adapter_Core.
//
//  NON-RESPONSIBILITIES:
//      - HUD state management (HUDManager).
//      - Dynamic UI overlays (ModernUIRenderer).
//      - Texture loading (TextureManager).
//      - GPU pipeline binding (D3D11DrawingContext + D3D11Adapter_Core).
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.Systems;
using SASZombieAssaultTD.Engine.UI.HUD;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.TextureRendering.HUD
{
    /// <summary>
    /// Deterministic HUD renderer that draws left and right HUD panels using the modern
    /// rendering pipeline. When a RenderSystem is bound, HUD panels are drawn via
    /// IDrawingContext; otherwise, a GPU-only adapter path is used.
    /// </summary>
    public sealed class HUDRenderer
    {
        private readonly HUDManager _hudManager;
        private readonly SystemManager _systemManager;

        private Texture2D? _hudLeft;
        private Texture2D? _hudRight;
        private bool _assetsLoaded;

        // Optional RenderSystem binding for GPU-agnostic drawing.
        private RenderSystem? _renderSystem;

        public HUDRenderer(HUDManager hudManager, SystemManager systemManager)
        {
            _hudManager = hudManager ?? throw new ArgumentNullException(nameof(hudManager));
            _systemManager = systemManager ?? throw new ArgumentNullException(nameof(systemManager));
        }

        /// <summary>
        /// Bind the engine RenderSystem so HUDRenderer can route draw calls through
        /// IDrawingContext instead of directly using the GPU adapter.
        /// </summary>
        public void SetRenderSystem(RenderSystem renderSystem)
        {
            _renderSystem = renderSystem ?? throw new ArgumentNullException(nameof(renderSystem));
        }

        // -------------------------------------------------------------------------------------------------
        // Asset Acquisition
        // -------------------------------------------------------------------------------------------------
        private void EnsureAssetsLoaded()
        {
            if (_assetsLoaded)
                return;

            var textureManager = _systemManager.Get<TextureManager>();
            if (textureManager == null)
                return;

            _hudLeft = textureManager.Get("HUD_Left");
            _hudRight = textureManager.Get("HUD_Right");

            if (_hudLeft != null && _hudRight != null)
                _assetsLoaded = true;
        }

        // -------------------------------------------------------------------------------------------------
        // Render Entry Point
        // -------------------------------------------------------------------------------------------------
        public void Render(D3D11Adapter_Core adapter_Core)
        {
            if (!_hudManager.IsVisible)
                return;

            EnsureAssetsLoaded();

            if (_hudLeft == null || _hudRight == null)
                return;

            var viewport = adapter_Core.ViewportSize;
            int screenWidth = (int)viewport.X;
            int screenHeight = (int)viewport.Y;

            if (screenWidth <= 0 || screenHeight <= 0)
                return;

            var white = ColorRGBA.White;

            // LEFT HUD: anchored to left edge, full height
            var leftPosition = new Vector2(0, 0);
            var leftSize = new Vector2(_hudLeft.Width, screenHeight);

            // RIGHT HUD: anchored to right edge, full height
            var rightPosition = new Vector2(screenWidth - _hudRight.Width, 0);
            var rightSize = new Vector2(_hudRight.Width, screenHeight);

            // Prefer RenderSystem/IDrawingContext when available.
            if (_renderSystem != null)
            {
                _renderSystem.DrawSprite(_hudLeft, leftPosition, leftSize, white);
                _renderSystem.DrawSprite(_hudRight, rightPosition, rightSize, white);
            }
            else
            {
                // Fallback: direct GPU adapter path.
                adapter_Core.DrawSprite(_hudLeft, leftPosition, leftSize, white);
                adapter_Core.DrawSprite(_hudRight, rightPosition, rightSize, white);
            }
        }
    }
}
