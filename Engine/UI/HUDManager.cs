// =====================================================================================================
//  FILE: HUDManager.cs
//  PATH: Engine/UI/HUD/HUDManager.cs
//  SUBSYSTEM: UI / HUD Management
//
//  ROLE:
//      Pure management subsystem responsible for holding HUD visibility state and HUD texture references.
//      HUDManager performs no rendering, no layout computation, and no dynamic text processing. It serves
//      as the authoritative state container for HUDRenderer and ModernUIRenderer.
//
//  RESPONSIBILITIES:
//      - Retrieve HUD textures from TextureManager.
//      - Expose HUD visibility state to render subsystems.
//      - Provide HUD texture handles to HUDRenderer.
//      - Manage HUD lifecycle (load, unload).
//
//  NON-RESPONSIBILITIES:
//      - Rendering (handled by HUDRenderer).
//      - Dynamic UI overlays (handled by ModernUIRenderer).
//      - Layout computation (handled by HUDRenderer).
//      - Texture loading (handled by TextureManager).
//      - Render scheduling (handled by RenderManager).
//
//  ARCHITECTURAL NOTES:
//      - Managers only manage state; they do not perform pipeline work.
//      - HUDManager is not registered with RenderManager.
//      - HUDRenderer queries HUDManager for texture handles during its render pass.
//      - ModernUIRenderer queries HUDManager for visibility state when drawing overlays.
//      - SystemManager supervises and halts on anomalies.
//      - RenderManager schedules deterministic frame execution.
//      - UpdateManager ticks simulation systems.
//      - SceneManager orchestrates scene lifecycle.
//      - TextureManager loads and resolves assets.
//      - Finalizer validates frame integrity before Present().
//
//  VERSION:
//      Updated post-management realignment (July 2026).
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.Systems;
using SASZombieAssaultTD.Engine.TextureRendering;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI.HUD
{
    /// <summary>
    /// Pure HUD management subsystem. Holds HUD textures and visibility state.
    /// </summary>
    public sealed class HUDManager
    {
        // -------------------------------------------------------------------------------------------------
        // STATE
        // -------------------------------------------------------------------------------------------------

        public bool IsVisible { get; set; } = true;

        private readonly SystemRegistry _registry;

        private bool _assetsLoaded;
        private Texture2D? _hudLeft;
        private Texture2D? _hudRight;
        private SystemManager systemManager;

        /// <summary>
        /// Global authoritative instance set by the engine bootstrap.
        /// </summary>
        public static HUDManager? Instance { get; private set; }
        public bool IsUIVisible { get; internal set; }

        // -------------------------------------------------------------------------------------------------
        // CONSTRUCTION (ONLY ONE VALID CONSTRUCTOR)
        // -------------------------------------------------------------------------------------------------

        public HUDManager(SystemRegistry registry)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            Instance = this;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "HUDManager: Initialized.");
        }

        public HUDManager(SystemManager systemManager) => this.systemManager = systemManager;

        // -------------------------------------------------------------------------------------------------
        // ASSET ACQUISITION
        // -------------------------------------------------------------------------------------------------

        public void EnsureAssetsLoaded()
        {
            if (_assetsLoaded)
                return;

            var textureManager = _registry.Get<TextureManager>();
            if (textureManager == null)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "HUDManager: TextureManager unavailable.");
                return;
            }

            _hudLeft = textureManager.Get("HUD_Support");
            _hudRight = textureManager.Get("HUD_Main");

            if (_hudLeft == null || _hudRight == null)
                DLogger.Log(LogSubsystems.ResourcesPipeline, "HUDManager: HUD textures missing.");
            else
                DLogger.Log(LogSubsystems.ResourcesPipeline, "HUDManager: HUD textures loaded.");

            _assetsLoaded = true;
        }

        // -------------------------------------------------------------------------------------------------
        // ACCESSORS FOR HUDRenderer
        // -------------------------------------------------------------------------------------------------

        public Texture2D? GetLeftTexture()
        {
            EnsureAssetsLoaded();
            return _hudLeft;
        }

        public Texture2D? GetRightTexture()
        {
            EnsureAssetsLoaded();
            return _hudRight;
        }

        // -------------------------------------------------------------------------------------------------
        // HUD RENDER HOOK (DETERMINISTIC NO-OP)
        // -------------------------------------------------------------------------------------------------
        //
        // PURPOSE:
        //     Allow render subsystems to call into HUDManager without performing any rendering work.
        //     HUDManager remains a pure state manager; this method exists only as a safe hook.
        //
        // -------------------------------------------------------------------------------------------------

        internal void RenderHUD(D3D11Adapter_Core uiContext, SystemManager systemManager)
        {
            // Deterministic no-op: HUDManager does not render.
            // HUDRenderer and ModernUIRenderer perform actual drawing based on HUDManager state.

            EnsureAssetsLoaded();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "HUDManager.RenderHUD(): Deterministic no-op executed.");
        }
    }
}
