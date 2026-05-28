// ============================================================================
// File:    Program.cs
// Path:    Engine/Program.cs
// Author:  BDC
// Purpose: Main entry point for SASZombieAssaultTD using the D3D11 pipeline.
// Notes:   Modern engine-driven version. GameRoot owns all update/render logic.
// ============================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.Managers;
using SASZombieAssaultTD.Engine.Platform;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Rendering.D3D11;
using SASZombieAssaultTD.Engine.State;
using SASZombieAssaultTD.Engine.Systems;
using SASZombieAssaultTD.Engine.UI.Input;
using System;
using System.Diagnostics;

namespace SASZombieAssaultTD.Engine
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            DebugLogger.Initialize();
            DebugLogger.LogInfo("TEST: Markdown creation check");
            DebugLogger.LogInfo("Program.Main: starting SASZombieAssaultTD.");

            // ----------------------------------------------------------------
            // 1. Create D3D11 window (no framebuffer)
            // ----------------------------------------------------------------
            var window = new D3D11Window(1280, 720, "SAS Zombie Assault TD");

            try
            {
                window.Create();
            }
            catch (Exception ex)
            {
                DebugLogger.LogException("Program.Main: Exception during D3D11Window.Create()", ex);
                return;
            }

            if (window.Handle == IntPtr.Zero)
            {
                DebugLogger.LogError("Program.Main: window.Handle is NULL after Create().");
                return;
            }

            DebugLogger.LogInfo($"Program.Main: D3D11Window created, HWND=0x{window.Handle.ToString("X")}.");

            // ----------------------------------------------------------------
            // 2. Create D3D11 device core
            // ----------------------------------------------------------------
            D3D11DeviceCore deviceCore;

            try
            {
                deviceCore = new D3D11DeviceCore(window.Handle, 1280, 720, true);
            }
            catch (Exception ex)
            {
                DebugLogger.LogException("Program.Main: Exception during D3D11DeviceCore construction.", ex);
                return;
            }

            // ----------------------------------------------------------------
            // 3. Create high-level render context
            // ----------------------------------------------------------------
            RenderContextD3D11 renderContext;

            try
            {
                renderContext = new RenderContextD3D11(deviceCore);
            }
            catch (Exception ex)
            {
                DebugLogger.LogException("Program.Main: Exception during RenderContextD3D11 construction.", ex);
                return;
            }

            renderContext.SetClearColor(0.1f, 0.1f, 0.1f, 1.0f);

            // ----------------------------------------------------------------
            // 4. Build GameRoot (engine orchestrator)
            // ----------------------------------------------------------------
            var systemRegistry = new SystemRegistry();
            var systemManager = new SystemManager();
            var updateManager = new UpdateManager();
            var renderManager = new RenderManager();
            var inputState = new UIInputState();
            var focusManager = new UIFocusManager();
            var inputRouter = new UIInputRouter(inputState, focusManager);

            var stateMachine = new StateMachine();
            var renderContextAdapter = new Rendering.RenderContextD3D11Adapter(renderContext);

            GameRoot game;
            try
            {
                game = new GameRoot(
                    systemRegistry,
                    systemManager,
                    updateManager,
                    renderManager,
                    inputRouter,
                    stateMachine as IGameStateMachine,
                    renderContextAdapter);
            }
            catch (Exception ex)
            {
                DebugLogger.LogException("Program.Main: Exception during GameRoot construction.", ex);
                return;
            }

            // IProgram adapter with explicit cast logging
            IProgram program;
            try
            {
                program = (IProgram)game;
            }
            catch (Exception ex)
            {
                DebugLogger.LogException("Program.Main: Invalid cast to IProgram.", ex);
                return;
            }

            // ----------------------------------------------------------------
            // 5. Initialize engine
            // ----------------------------------------------------------------
            try
            {
                program.Initialize();
            }
            catch (Exception ex)
            {
                DebugLogger.LogException("Program.Main: Exception during GameRoot.Initialize().", ex);
                return;
            }

            DebugLogger.LogInfo("Program.Main: entering engine-driven main loop.");

            // ----------------------------------------------------------------
            // 6. Main loop (engine-driven)
            // ----------------------------------------------------------------
            var stopwatch = Stopwatch.StartNew();
            var last = stopwatch.Elapsed;

            while (true)
            {
                bool keepRunning;

                try
                {
                    keepRunning = window.PumpMessages();
                }
                catch (Exception ex)
                {
                    DebugLogger.LogException("Program.Main: Exception during window.PumpMessages().", ex);
                    break;
                }

                if (!keepRunning)
                {
                    DebugLogger.LogInfo("Program.Main: PumpMessages returned false.");
                    break;
                }

                var now = stopwatch.Elapsed;
                var delta = now - last;
                last = now;

                try
                {
                    program.Update(delta);
                    program.Render();
                }
                catch (Exception ex)
                {
                    DebugLogger.LogException("Program.Main: Exception during engine Update/Render.", ex);
                    break;
                }
            }

            // ----------------------------------------------------------------
            // 7. Shutdown
            // ----------------------------------------------------------------
            DebugLogger.LogInfo("Program.Main: shutting down.");

            try { program.Shutdown(); } catch { }
            try { renderContext.Dispose(); } catch { }
            try { deviceCore.Dispose(); } catch { }
            try { window.Dispose(); } catch { }

            DebugLogger.LogInfo("Program.Main: shutdown complete.");
        }
    }
}
