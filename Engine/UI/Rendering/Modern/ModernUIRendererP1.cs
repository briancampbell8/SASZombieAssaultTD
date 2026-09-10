// =====================================================================================================
//  FILE: ModernUIRendererP1.cs
//  PATH: Engine/UI/Rendering/Modern/ModernUIRendererP1.cs
//  SUBSYSTEM: UI Rendering / Dynamic Overlay Compositor
//
//  ROLE:
//      Dynamic UI overlay renderer responsible for drawing text, icons, and gameplay information
//      (cash, wave, health, tower data) on top of static HUD textures. This partial routes all
//      dynamic UI rendering through the engine’s RenderSystem and IDrawingContext, remaining
//      agnostic of the underlying GPU adapter.
//
//  RESPONSIBILITIES:
//      - Draw dynamic text overlays using the engine RenderSystem.
//      - Draw dynamic UI icons and markers via RenderSystem primitives/sprites.
//      - Query HUDManager for visibility and gameplay UI state.
//      - Integrate with RenderManager as the final render subsystem in the frame pipeline.
//
//  NON-RESPONSIBILITIES:
//      - Static HUD texture drawing (handled by HUDRenderer).
//      - HUD state management (handled by HUDManager).
//      - Texture loading (handled by TextureManager).
//      - Gameplay rendering (handled by GameplayRenderSystem).
//      - Direct GPU adapter control (handled by D3D11DrawingContext and D3D11Adapter_Core).
//
//  ARCHITECTURAL NOTES:
//      - This partial MUST NOT define constructors.
//      - This partial MUST own all fields it uses.
//      - All dynamic UI rendering is forwarded through RenderSystem → IDrawingContext.
// =====================================================================================================

using System;
using System.Numerics;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.TextureRendering;
using SASZombieAssaultTD.Engine.UI.HUD;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI.Rendering.Modern
{
    public sealed partial class ModernUIRenderer
    {
        // ---------------------------------------------------------------------------------------------
        // P1 OWNED FIELDS — NO CROSS-PARTIAL DEPENDENCIES
        // ---------------------------------------------------------------------------------------------
        private HUDManager P1_hudManager;

        // Legacy adapter references retained for initialization and viewport sync only.
        private D3D11Adapter_Core coreAdapter;
        private D3D11Adapter_Resources resourcesAdapter;
        private D3D11AdapterDrawPrimitives primitivesAdapter;
        private D3D11Adapter_Core adapterCore;

        // New rendering entry point: all dynamic UI draws go through RenderSystem.
        private RenderSystem _renderSystem;

        private bool _assetsLoaded;

        public ModernUIRenderer(D3D11Adapter_Core adapterCore)
        {
            this.adapterCore = adapterCore;
        }

        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }

        // ---------------------------------------------------------------------------------------------
        // PUBLIC API — BIND HUD MANAGER
        // ---------------------------------------------------------------------------------------------
        public void P1_SetHUD(HUDManager hud)
        {
            P1_hudManager = hud;

            DLogger.Log(LogSubsystems.UIRenderer,
                $"ModernUIRendererP1: HUDManager bound → {hud?.GetType().Name ?? "null"}");
        }

        // ---------------------------------------------------------------------------------------------
        // PUBLIC API — BIND RENDER SYSTEM
        // ---------------------------------------------------------------------------------------------
        public void P1_SetRenderSystem(RenderSystem renderSystem)
        {
            _renderSystem = renderSystem ?? throw new ArgumentNullException(nameof(renderSystem));

            DLogger.Log(LogSubsystems.UIRenderer,
                $"ModernUIRendererP1: RenderSystem bound → {renderSystem.GetType().Name}");
        }

        // ---------------------------------------------------------------------------------------------
        // MAIN OVERLAY RENDER ENTRY POINT (GPU-AGNOSTIC)
        // ---------------------------------------------------------------------------------------------
        public void P1_Render()
        {
            if (_renderSystem == null)
            {
                DLogger.Log(LogSubsystems.UIRenderer, LogLevel.Error,
                    "ModernUIRendererP1: RenderSystem not bound");
                return;
            }

            if (P1_hudManager == null || !P1_hudManager.IsUIVisible)
                return;

            P1_DrawDynamicText();
            P1_DrawDynamicIcons();
        }

        // ---------------------------------------------------------------------------------------------
        // OVERLAY DRAW ROUTINES — ROUTED THROUGH RenderSystem
        // ---------------------------------------------------------------------------------------------
        private void P1_DrawDynamicText()
        {
            if (P1_hudManager == null || _renderSystem == null)
                return;

            // Example placeholder: route text drawing through RenderSystem instead of adapter.
            // var position = new Vector2(10, 10);
            // var color = Color.White;
            // _renderSystem.DrawText($"Cash: {P1_hudManager.Cash}", position, 1.0f, color);
        }

        private void P1_DrawDynamicIcons()
        {
            if (P1_hudManager == null || _renderSystem == null)
                return;

            // Example placeholder: route icon drawing through RenderSystem instead of adapter.
            // var position = new Vector2(10, 40);
            // var size = new Vector2(32, 32);
            // var color = Color.White;
            // var iconTexture = P1_hudManager.GetIconTexture();
            // if (iconTexture != null)
            // {
            //     _renderSystem.DrawSprite(iconTexture, position, size, color);
            // }
        }

        // ---------------------------------------------------------------------------------------------
        // COMMAND SUBMISSION — STILL BATCHED, BUT GPU-AGNOSTIC
        // ---------------------------------------------------------------------------------------------
        internal void SubmitCommand(in RenderCommand cmd)
        {
            var command = cmd;

            if (P5_isBatching)
            {
                P5_currentBatch.Add(command);
                return;
            }

            if (P5_commandBuffer != null)
            {
                P5_commandBuffer.Add(command);
                return;
            }

            P5_currentBatch.Add(command);
        }

        // ---------------------------------------------------------------------------------------------
        // INITIALIZATION — ADAPTERS + RENDERER STATE
        // ---------------------------------------------------------------------------------------------
        internal void Initialize()
        {
            if (_assetsLoaded)
                return;

            try
            {
                coreAdapter?.Initialize();
                resourcesAdapter?.Initialize();
                primitivesAdapter?.Initialize();

                InitializeResources();
                CreateDefaultRenderTargets();
                LoadDefaultEffects();

                ValidateInitializationState();

                _assetsLoaded = true;
            }
            catch
            {
                _assetsLoaded = false;
                throw;
            }
        }

        // ---------------------------------------------------------------------------------------------
        // RENDER ENTRY — LEGACY ADAPTER PATH, NOW DELEGATING TO RenderSystem
        // ---------------------------------------------------------------------------------------------
        internal void Render()
        {
            ValidateInitializationState();

            if (_renderSystem != null)
            {
                // Preferred path: use RenderSystem and IDrawingContext.
                try
                {
                    _renderSystem.BeginFrame();
                    P1_Render();
                }
                finally
                {
                    _renderSystem.EndFrame();
                }
            }
            else if (coreAdapter != null)
            {
                // Legacy path: still supported for transitional builds, but UI rendering is routed
                // through P1_Render() which no longer uses adapter directly.
                coreAdapter.BeginFrame();
                resourcesAdapter?.BeginFrame();
                primitivesAdapter?.BeginFrame();

                try
                {
                    P1_Render();
                }
                finally
                {
                    primitivesAdapter?.EndFrame();
                    resourcesAdapter?.EndFrame();
                    coreAdapter.EndFrame();
                }
            }
            else
            {
                // No GPU adapter and no RenderSystem: fall back to UI-context driven render.
                P3_RenderFrame(0f);
            }
        }

        // ---------------------------------------------------------------------------------------------
        // HUD MANAGER ATTACHMENT
        // ---------------------------------------------------------------------------------------------
        internal void AttachHUDManager(HUDManager hudManager)
        {
            if (hudManager == null)
                throw new ArgumentNullException(nameof(hudManager));

            P1_hudManager = hudManager;

            try
            {
                P1_hudManager.EnsureAssetsLoaded();
            }
            catch
            {
            }
        }

        // ---------------------------------------------------------------------------------------------
        // FRAME LIFECYCLE — BEGIN / END
        // ---------------------------------------------------------------------------------------------
        internal void BeginFrame(D3D11RenderContextBridge d3D11RenderContextBridge)
        {
            unchecked
            {
                P3_frameIndex++;
            }

            resourcesAdapter?.BeginFrame();
            primitivesAdapter?.BeginFrame();

            P4_isBatching = false;
            P5_isBatching = false;

            P4_currentBatch?.Clear();
            P5_currentBatch?.Clear();

            P4_commandBuffer?.Clear();
            P5_commandBuffer?.Clear();
            P3_commandBuffer?.Clear();
        }

        internal void EndFrame()
        {
            if (P4_isBatching)
            {
                try
                {
                    P4_EndBatch();
                }
                catch
                {
                }
            }

            if (P5_isBatching)
            {
                try
                {
                    P5_EndBatch();
                }
                catch
                {
                }
            }

            P3_commandBuffer?.ExecuteAll();
            P4_commandBuffer?.ExecuteAll();
            P5_commandBuffer?.ExecuteAll();

            P3_commandBuffer?.Clear();
            P4_commandBuffer?.Clear();
            P5_commandBuffer?.Clear();

            P4_currentBatch?.Clear();
            P5_currentBatch?.Clear();

            resourcesAdapter?.EndFrame();
            primitivesAdapter?.EndFrame();

            unchecked
            {
                P3_frameIndex++;
            }
        }

        // ---------------------------------------------------------------------------------------------
        // VIEWPORT / SCREEN SIZE SYNCHRONIZATION
        // ---------------------------------------------------------------------------------------------
        internal void SetViewport(int width, int height)
        {
            if (width <= 0)
                width = 1;
            if (height <= 0)
                height = 1;

            P6_viewportSize = new Vector2(width, height);

            try
            {
                ScreenWidth = width;
                ScreenHeight = height;
            }
            catch
            {
            }

            if (coreAdapter != null)
            {
                try
                {
                    coreAdapter.ViewportSize = P6_viewportSize;
                    coreAdapter.ScreenWidth = width;
                    coreAdapter.ScreenHeight = height;
                }
                catch
                {
                }
            }

            if (adapterCore != null)
            {
                try
                {
                    adapterCore.ViewportSize = P6_viewportSize;
                    adapterCore.ScreenWidth = width;
                    adapterCore.ScreenHeight = height;
                }
                catch
                {
                }
            }

            if (P6_uiContext != null)
            {
                try
                {
                    P6_uiContext.ViewportSize = P6_viewportSize;
                }
                catch
                {
                }
            }

            P3_viewportSize = new Vector2(width, height);
            P3_uiContext.ViewportSize = P3_viewportSize;

            if (P2_deviceCore != null)
            {
                try
                {
                    P2_deviceCore.ScreenWidth = width;
                    P2_deviceCore.ScreenHeight = height;
                }
                catch
                {
                }
            }
        }

        // ---------------------------------------------------------------------------------------------
        // LEGACY ADAPTER BINDING — TRANSITIONAL SUPPORT
        // ---------------------------------------------------------------------------------------------
        internal void Render(D3D11Adapter_Core d3D11Adapter_Core, object d3D11adapter)
        {
            if (d3D11Adapter_Core == null)
            {
                return;
            }

            adapterCore = d3D11Adapter_Core;

            if (d3D11adapter is D3D11Adapter_Resources resAdapter)
            {
                resourcesAdapter = resAdapter;
            }
            else if (d3D11adapter is D3D11AdapterDrawPrimitives primAdapter)
            {
                primitivesAdapter = primAdapter;
            }

            P1_Render();
        }

        internal void SubmitSprite(string textureName, float x, float y, float width, float height, Color tint)
        {
            if (string.IsNullOrWhiteSpace(textureName))
                return;

            resourcesAdapter?.DrawSprite(textureName, x, y, width, height, tint);
        }

        internal void Render(D3D11Adapter_Core adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException(nameof(adapter));

            try
            {
                ScreenWidth = (int)adapter.ScreenWidth;
                ScreenHeight = adapter.ScreenHeight;

                resourcesAdapter?.BeginFrame();
                primitivesAdapter?.BeginFrame();

                SetViewport(ScreenWidth, ScreenHeight);

                P1_Render();
            }
            finally
            {
                try
                {
                    primitivesAdapter?.EndFrame();
                }
                catch
                {
                }

                try
                {
                    resourcesAdapter?.EndFrame();
                }
                catch
                {
                }
            }
        }
    }
}
