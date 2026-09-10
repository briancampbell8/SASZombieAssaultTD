// =====================================================================================================
//  FILE: Renderer-Initialize.cs
//  PATH: Engine/Render/LegacyRender/Renderer-Initialize.cs
//  SUBSYSTEM: Rendering (Legacy Driver Program)
//
//  ROLE:
//      Implements deterministic initialization and shutdown logic for the Renderer driver program.
//      This partial configures viewport state, GPU context stubs, default rendering parameters,
//      and prepares all renderer subsystems for safe operation.
//
//  RESPONSIBILITIES:
//      - Initialize renderer state, viewport, GPU context, and default configuration values.
//      - Provide safe shutdown behavior with deterministic cleanup.
//      - Expose a legacy GPU context initializer for compatibility with older subsystems.
//      - Emit diagnostic logs for all initialization and shutdown operations.
//
//  NON-RESPONSIBILITIES:
//      - Performing GPU resource allocation or real device initialization.
//      - Managing swapchains, render passes, or draw operations.
//      - Handling frame lifecycle, pacing, or presentation logic.
//      - Implementing color grading, screenshots, or GPU timing behavior.
//
//  ARCHITECTURAL NOTES:
//      - Initialization must remain deterministic and safe at any engine stage.
//      - GPU context and render target values are legacy placeholders until modernization.
//      - Shutdown must fully reset internal state without leaving dangling pointers.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Numerics;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Render
{
    public partial class Renderer
    {
        /// <summary>
        /// Initializes the renderer with default configuration and legacy GPU context stubs.
        /// </summary>
        public bool Initialize(int width = 800, int height = 600, bool vsync = true)
        {
            if (_isInitialized)
            {
                DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Warning, "Renderer: Already initialized");
                return false;
            }

            try
            {
                _viewportSize = new Vector3(width, height, 0f);
                _vsyncEnabled = vsync;

                _clearColor = Color.FromArgb(255, 30, 30, 60);

                _renderScale = 1.0f;

                _colorGradingEnabled = false;
                _colorGradeTint = Color.White;
                _colorGradeContrast = 1.0f;
                _colorGradeBrightness = 0.0f;

                _screenshotEnabled = true;
                _screenshotPath = "screenshots";

                _gpuTimingEnabled = false;
                _gpuFrameTimes = new List<long>();

                // Legacy GPU context placeholders
                _gpuContext = new IntPtr(1);
                _renderTarget = new IntPtr(2);

                _isInitialized = true;

                DLogger.Log(
                    LogSubsystems.Rendering,
                    LogEnums.LogLevel.Info,
                    $"Renderer: Initialized ({width}x{height}) with VSync={vsync}"
                );

                OnRendererInitialized?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                DLogger.Log(
                    LogSubsystems.Rendering,
                    LogEnums.LogLevel.Error,
                    $"Renderer: Failed to initialize — {ex.Message}"
                );
                return false;
            }
        }

        /// <summary>
        /// Shuts down the renderer and clears all legacy GPU context placeholders.
        /// </summary>
        public bool Shutdown()
        {
            if (!_isInitialized)
            {
                DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Warning, "Renderer: Not initialized");
                return false;
            }

            try
            {
                _renderTarget = IntPtr.Zero;
                _gpuContext = IntPtr.Zero;
                _isInitialized = false;

                DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Info, "Renderer: Shutdown completed");

                OnRendererShutdown?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                DLogger.Log(
                    LogSubsystems.Rendering,
                    LogEnums.LogLevel.Error,
                    $"Renderer: Failed to shutdown — {ex.Message}"
                );
                return false;
            }
        }

        /// <summary>
        /// Initializes the legacy GPU context stub. Safe to call before full initialization.
        /// </summary>
        public bool InitializeGpuContext()
        {
            if (_isInitialized)
                return true;

            try
            {
                _gpuContext = new IntPtr(1);

                DLogger.Log(
                    LogSubsystems.Rendering,
                    LogEnums.LogLevel.Info,
                    "Renderer: GPU context initialized"
                );

                return true;
            }
            catch (Exception ex)
            {
                DLogger.Log(
                    LogSubsystems.Rendering,
                    LogEnums.LogLevel.Error,
                    $"Renderer: Failed to initialize GPU context — {ex.Message}"
                );
                return false;
            }
        }
    }
}
