// =====================================================================================================
// FILE: D3D11Window.cs
// PATH: Engine/Platform/D3D11Window.cs
// SUBSYSTEM: Win32 / D3D11 Host Window
//
// ROLE:
//     Deterministic Win32/D3D11 host responsible for owning the native window handle, initializing the
//     GPU device/swap chain via D3D11DeviceCore, processing the OS message loop, and delegating
//     lifecycle control to GameRootMain. This class forms the bridge between the platform layer and
//     the engine’s composition root.
//
// RESPONSIBILITIES:
//     - Own the Win32 window handle and provide it to D3D11DeviceCore.
//     - Initialize the D3D11 device and swap chain before engine initialization.
//     - Execute a deterministic message pump for the engine host.
//     - Enforce the lifecycle contract: GPU Init → Initialize → Run Loop → Shutdown.
//     - Delegate all engine-facing lifecycle operations to GameRootMain.
//
// NON-RESPONSIBILITIES:
//     - Implementing rendering, frame updates, or game timing logic (handled by GameRootMain).
//     - Managing engine subsystems, assets, or game state.
//     - Implementing high-level UI or scene logic.
//
// ARCHITECTURAL NOTES:
//     - The window host is minimal and deterministic but is responsible for GPU bring-up.
//     - GameRootMain owns the update/render loop; the host only initializes GPU and pumps OS messages.
//     - No direct rendering or game logic is permitted inside this class.
// =====================================================================================================

using System;
using System.Runtime.InteropServices;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;

namespace SASZombieAssaultTD.Engine.Platform
{
    public sealed class D3D11Window : IDisposable
    {
        private readonly IntPtr _hwnd;
        private readonly D3D11DeviceCore _deviceCore;
        private bool _isRunning;

        public IntPtr Handle => _hwnd;
        public D3D11DeviceCore DeviceCore => _deviceCore;

        public object Width { get; internal set; }
        public object Height { get; internal set; }

        public D3D11Window(IntPtr hwnd, D3D11DeviceCore deviceCore)
        {
            _hwnd = hwnd != IntPtr.Zero
                ? hwnd
                : throw new ArgumentNullException(nameof(hwnd));

            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
        }

        public void Run(GameRootMain game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            // -------------------------------------------------------------------------------------------------
            // WIN32 STYLES EX-OVERRIDE — STRIP OPERATING SYSTEM DEFAULT WHITE BACKGROUND CANVAS
            // -------------------------------------------------------------------------------------------------
            const int GWL_EXSTYLE = -20;
            const int WS_EX_NOREDIRECTIONBITMAP = 0x00100000;

            int currentExStyle = GetWindowLong(_hwnd, GWL_EXSTYLE);
            SetWindowLong(_hwnd, GWL_EXSTYLE, currentExStyle | WS_EX_NOREDIRECTIONBITMAP);

            // 1. HARDWARE SIZE MEASUREMENT OVERRIDES
            int width = 1280;
            int height = 720;
            if (GetClientRect(_hwnd, out RECT rect))
            {
                width = rect.Right - rect.Left;
                height = rect.Bottom - rect.Top;

                _deviceCore.BackbufferWidth = width;
                _deviceCore.BackbufferHeight = height;
                _deviceCore.Width = width;
                _deviceCore.Height = height;
                _deviceCore.ViewportSize = new System.Numerics.Vector2(width, height);
            }

            // 2. BRING UP D3D11 HARDWARE LAYERS (CRITICAL: Clear old states to enforce live handle binding)
            _deviceCore.IsInitialized = false;
            _deviceCore.InitializeDeviceAndSwapChain(_hwnd);
            _deviceCore.CreateBackbufferTargets();

            // 3. INITIALIZE THE CORE ENGINE MANAGERS
            game.Initialize();

            // 4. PRIME TIMING PARAMETERS
            _isRunning = true;
            var lastTime = DateTime.Now;
            float frameAccumulator = 0f;
            const float targetFrameTime = 1f / 60f;

            // 5. THE AUTHORITATIVE HARMONIZED RUN LOOP
            while (_isRunning)
            {
                // STEP A: Pump OS Windows messages continuously to satisfy the window handle responsive contract
                while (PeekMessage(out var msg, IntPtr.Zero, 0, 0, PM_REMOVE))
                {
                    if (msg.message == WM_QUIT)
                    {
                        _isRunning = false;
                        break;
                    }

                    TranslateMessage(ref msg);
                    DispatchMessage(ref msg);
                }

                if (!_isRunning)
                    break;

                var now = DateTime.Now;
                var delta = (float)(now - lastTime).TotalSeconds;
                lastTime = now;

                frameAccumulator += delta;

                // STEP B: Drive deterministic fixed-step tick updates
                while (frameAccumulator >= targetFrameTime)
                {
                    game.Update(targetFrameTime);
                    frameAccumulator -= targetFrameTime;
                }

                // STEP B: Drive deterministic fixed-step tick updates
                while (frameAccumulator >= targetFrameTime)
                {
                    game.Update(targetFrameTime);
                    frameAccumulator -= targetFrameTime;
                }

                // STEP C: Force Direct3D clear, render pass execution, and presentation
                // FIX: Instead of trying to resolve the un-registered GameRootUpdateLoop out of the container,
                // we resolve the authoritative adapter context to execute the clear/render/present flow seamlessly.
                var context = game.SystemRegistry?.Resolve<D3D11Adapter_Core>();
                if (context != null)
                {
                    context.ClearScreen();
                    game.Render(context);
                    context.Present();
                }
            }

            // 6. CLEAN RELEASES
            game.Shutdown();
        }

        public void Dispose()
        {
            _deviceCore?.Dispose();
        }

        private const uint PM_REMOVE = 0x0001;
        private const uint WM_QUIT = 0x0012;

        [StructLayout(LayoutKind.Sequential)]
        private struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public System.Drawing.Point pt;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool PeekMessage(
            out MSG lpMsg,
            IntPtr hWnd,
            uint wMsgFilterMin,
            uint wMsgFilterMax,
            uint wRemoveMsg);

        [DllImport("user32.dll")]
        private static extern bool TranslateMessage(ref MSG lpMsg);

        [DllImport("user32.dll")]
        private static extern IntPtr DispatchMessage(ref MSG lpMsg);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    }
}
