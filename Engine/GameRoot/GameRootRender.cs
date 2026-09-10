// =====================================================================================================
//  FILE: GameRootRender.cs
//  PATH: Engine/Platform/GameRootRender.cs
//  SUBSYSTEM: Platform Abstraction Layer
//
//  ROLE:
//      Encapsulates deterministic render-phase logic for the engine host. This class is responsible for
//      coordinating system-level rendering and UI rendering. GameRootMain delegates its render
//      responsibilities to this standalone class.
//
//  RESPONSIBILITIES:
//      - Dispatch render calls to all registered engine systems.
//      - Execute UI rendering through the ModernUIRenderer.
//      - Maintain deterministic render sequencing.
//      - Provide engine-level diagnostics for render failures.
//
//  NON-RESPONSIBILITIES:
//      - Game logic.
//      - Asset management.
//      - Window or device management.
//      - Update-phase logic (handled by GameRootUpdate).
//
//  ARCHITECTURAL NOTES:
//      - This class replaces the former partial-method render implementation.
//      - GameRootMain composes and invokes this class directly.
//      - Strict Option B architecture: concrete subsystem types, no interface indirection.
//      - All render-phase exceptions are logged and rethrown for engine-level crash handling.
//
//  AUTHOR: BDC
//  CREATED: 2026-07-17
//  LAST UPDATED: 2026-07-17
// =====================================================================================================

using System;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.UI.Rendering.Modern;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.Systems;

namespace SASZombieAssaultTD.Engine.GameRoot
{
    /// <summary>
    /// Encapsulates deterministic render-phase logic for the engine host.
    /// </summary>
    internal sealed class GameRootRender
    {
        private readonly RenderManager _renderManager;
        private readonly ModernUIRenderer _uiRenderer;
        private D3D11Adapter_Core renderContext;

        public GameRootRender(RenderManager renderManager, ModernUIRenderer uiRenderer)
        {
            _renderManager = renderManager ?? throw new ArgumentNullException(nameof(renderManager));
            _uiRenderer = uiRenderer ?? throw new ArgumentNullException(nameof(uiRenderer));
        }

        /// <summary>
        /// Executes a single deterministic render pass.
        /// </summary>
        public void Execute()
        {
            try
            {
                // 1. Render game systems
                _renderManager.RenderAll(renderContext);


                // 2. Render UI
                _uiRenderer.Render();
            }
            catch (Exception ex)
            {
                DLogger.Log(
                    LogSubsystems.GameRoot,
                    LogEnums.LogLevel.Error,
                    $"Render failure: {ex.Message}",
                    "GameRootRender.Execute");

                throw;
            }
        }
    }
}
