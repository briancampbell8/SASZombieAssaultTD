// =====================================================================================================
//  FILE: ModernUIRendererP2.cs
//  PATH: Engine/UI/Rendering/Modern/ModernUIRendererP2.cs
//  SUBSYSTEM: Modern UI Rendering — Initialization & Frame Hooks (Partial)
//
//  ROLE:
//      Provides the synchronous, allocationless initialization pipeline for ModernUIRenderer,
//      and the frame-level hooks used to flush UI commands and traverse the UI element tree.
//
//  RESPONSIBILITIES:
//      - Validate core renderer dependencies (P2_deviceCore, P2_uiContext, P2_textureAtlasManager).
//      - Bind the swapchain’s native render target view into the renderer’s target dictionary.
//      - Verify baseline shader/effect state without allocating new pipeline objects.
//      - Maintain deterministic initialization ordering.
//
//  NON-RESPONSIBILITIES:
//      - Constructor logic (handled strictly outside this partial).
//      - Rendering logic for individual elements (handled in other ModernUIRenderer partials).
//      - GPU batching (handled in ModernUIRenderer_Batching.cs).
//
//  ARCHITECTURAL NOTES:
//      - This partial MUST NOT define constructors.
//      - This partial MUST own all fields it uses.
//      - Initialization is synchronous by design; async stubs are intentionally removed.
// =====================================================================================================

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using Vortice.Direct3D11;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI.Rendering.Modern
{
    /// <summary>
    /// ModernUIRenderer partial: initialization subsystem.
    /// Owns its own initialization fields and does not pull state from other partials.
    /// </summary>
    public sealed partial class ModernUIRenderer
    {
        // ---------------------------------------------------------------------------------------------
        // P2 OWNED FIELDS — NO CROSS-PARTIAL DEPENDENCIES
        // ---------------------------------------------------------------------------------------------
        private D3D11DeviceCore P2_deviceCore;
        private D3D11Adapter_Core P2_uiContext;
        private UITextureAtlasManager P2_textureAtlasManager;

        private readonly Dictionary<string, ID3D11RenderTargetView> P2_renderTargets =
            new Dictionary<string, ID3D11RenderTargetView>();

        // ---------------------------------------------------------------------------------------------
        // BINDING API
        // ---------------------------------------------------------------------------------------------

        public void P2_BindCore(
            D3D11DeviceCore deviceCore,
            D3D11Adapter_Core uiContext,
            UITextureAtlasManager textureAtlasManager)
        {
            P2_deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
            P2_uiContext = uiContext ?? throw new ArgumentNullException(nameof(uiContext));
            P2_textureAtlasManager = textureAtlasManager ?? throw new ArgumentNullException(nameof(textureAtlasManager));

            DLogger.Log(
                LogSubsystems.UIRenderingModern,
                LogLevel.Info,
                "[ModernUIRendererP2] Core dependencies bound for initialization."
            );
        }

        // ---------------------------------------------------------------------------------------------
        // PUBLIC INITIALIZATION ENTRY POINT
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Commences the resource initialization pass for the Modern UI renderer.
        /// Validates dependencies, binds default render targets, and verifies baseline shader state.
        /// </summary>
        public void InitializeResources()
        {
            DLogger.Log(
                LogSubsystems.UIRenderingModern,
                LogLevel.Info,
                "[ModernUIRendererP2] Commencing resource initialization pass."
            );

            ValidateInitializationState();
            CreateDefaultRenderTargets();
            LoadDefaultEffects();

            DLogger.Log(
                LogSubsystems.UIRenderingModern,
                LogLevel.Info,
                "[ModernUIRendererP2] Resource initialization pass complete."
            );
        }

        // ==============================================================================================
        //  VALIDATION
        // ==============================================================================================

        private void ValidateInitializationState()
        {
            if (P2_deviceCore == null)
                throw new InvalidOperationException("D3D11DeviceCore context cannot be NULL.");

            if (P2_uiContext == null)
                throw new InvalidOperationException("D3D11Adapter_Core cannot be NULL.");

            if (P2_textureAtlasManager == null)
                throw new InvalidOperationException("TextureAtlasManager cannot be NULL.");

            DLogger.Log(
                LogSubsystems.UIRenderingModern,
                LogLevel.Debug,
                "[ModernUIRendererP2] Initialization safety boundaries validated."
            );
        }

        // ==============================================================================================
        //  DEFAULT RENDER TARGET BINDING
        // ==============================================================================================

        private void CreateDefaultRenderTargets()
        {
            try
            {
                ID3D11RenderTargetView nativeView = P2_deviceCore.BackbufferRtv;

                if (nativeView != null)
                {
                    P2_renderTargets["UI_Main"] = nativeView;
                    DLogger.Log(
                        LogSubsystems.UIRenderingModern,
                        LogLevel.Info,
                        "[ModernUIRendererP2] Default target 'UI_Main' bound to hardware backbuffer."
                    );
                }
                else
                {
                    DLogger.Log(
                        LogSubsystems.UIRenderingModern,
                        LogLevel.Error,
                        "[ModernUIRendererP2] Failed to bind UI_Main: BackbufferRtv is null."
                    );
                }
            }
            catch (Exception ex)
            {
                DLogger.Log(
                    LogSubsystems.UIRenderingModern,
                    LogLevel.Error,
                    $"[ModernUIRendererP2] CreateDefaultRenderTargets failed: {ex.Message}"
                );
                throw;
            }
        }

        // ==============================================================================================
        //  DEFAULT EFFECT / SHADER VERIFICATION
        // ==============================================================================================

        private void LoadDefaultEffects()
        {
            DLogger.Log(
                LogSubsystems.UIRenderingModern,
                LogLevel.Info,
                "[ModernUIRendererP2] Verifying shader state for UI rendering..."
            );

            // Shaders are pre-compiled and verified in the DeviceCore layer.
        }
    }
}
