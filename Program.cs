// =====================================================================================================
// FILE: Program.cs
// PATH: Engine/Program.cs
// SUBSYSTEM: Platform Abstraction Layer
//
// ROLE: Defines the minimal deterministic lifecycle contract for any engine-hosted program. This
// interface is implemented by engine hosts (e.g., GameRootMain) to provide a clean, engine-facing
// API for startup, execution entry, and deterministic shutdown operations.
//
// RESPONSIBILITIES:
// - Provide a strict, minimal lifecycle surface for program orchestration.
// - Enforce the structural sequencing contract: Initialize → Run Loop Execution → Shutdown.
// - Serve as the base contract for any future top-level engine-hosted program modules.
//
// NON-RESPONSIBILITIES:
// - Implementing deep frame-level update calculation rules or rendering commands directly.
// - Managing active systems registration pools, engine assets, or game states.
// - Handling discrete hardware device allocation boundaries.
//
// ARCHITECTURAL NOTES:
// - This interface replaces the legacy GameRoot partial lifecycle methods.
// - GameRootMain implements this interface and delegates to its subsystems:
//   • GameRootInitialization
//   • GameRootUpdateLoop
//   • GameRootStateController
//   • GameRootSystemRegistration
// - All engine-hosted programs MUST implement this interface without exception.
// =====================================================================================================

using System;
using System.Runtime.InteropServices;
using SASZombieAssaultTD.Engine;
using SASZombieAssaultTD.Engine.Platform;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;

namespace SASZombieAssaultTD
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            // -----------------------------------------------------------------------------------------
            // 1. Deterministic Win32 host creation
            // -----------------------------------------------------------------------------------------
            IntPtr hwnd = CreateMainWindowHandle();
            if (hwnd == IntPtr.Zero)
                throw new InvalidOperationException("Program.cs: Win32 window creation failed.");

            // -----------------------------------------------------------------------------------------
            // 2. Construct GPU device core (authoritative D3D11 HAL)
            // -----------------------------------------------------------------------------------------
            using var deviceCore = new D3D11DeviceCore(hwnd);

            // -----------------------------------------------------------------------------------------
            // 3. Wrap window + device core into deterministic engine host
            // -----------------------------------------------------------------------------------------
            using var window = new D3D11Window(hwnd, deviceCore);

            // -----------------------------------------------------------------------------------------
            // 4. Build composition root (GameRootMain)
            // -----------------------------------------------------------------------------------------
            GameRootMain gameRoot = EngineBootstrap.CreateGameRootMain(window, deviceCore);

            // -----------------------------------------------------------------------------------------
            // 5. Delegate lifecycle control to the engine host
            // -----------------------------------------------------------------------------------------
            window.Run(gameRoot);
        }

        // =================================================================================================
        // WIN32 WINDOW CREATION (Deterministic Host)
        // =================================================================================================

        private const string WindowClassName = "SASZombieAssaultTD_MainWindow";

        [DllImport("user32.dll", SetLastError = true)]
        private static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CreateWindowEx(
            uint dwExStyle,
            string lpClassName,
            string lpWindowName,
            uint dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct WNDCLASSEX
        {
            public uint cbSize;
            public uint style;
            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;
        }

        private static IntPtr WindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            return DefWindowProc(hWnd, msg, wParam, lParam);
        }

        private static IntPtr CreateMainWindowHandle()
        {
            IntPtr hInstance = GetModuleHandle(null);

            var wndClass = new WNDCLASSEX
            {
                cbSize = (uint)Marshal.SizeOf<WNDCLASSEX>(),
                style = 0,
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate((WndProcDelegate)WindowProc),
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = hInstance,
                hIcon = IntPtr.Zero,
                hCursor = IntPtr.Zero,
                hbrBackground = IntPtr.Zero,
                lpszMenuName = null,
                lpszClassName = WindowClassName,
                hIconSm = IntPtr.Zero
            };

            ushort atom = RegisterClassEx(ref wndClass);
            if (atom == 0)
                return IntPtr.Zero;

            const uint WS_OVERLAPPEDWINDOW = 0x00CF0000;
            const uint WS_VISIBLE = 0x10000000;

            return CreateWindowEx(
                0,
                WindowClassName,
                "SAS Zombie Assault TD",
                WS_OVERLAPPEDWINDOW | WS_VISIBLE,
                100,
                100,
                1280,
                720,
                IntPtr.Zero,
                IntPtr.Zero,
                hInstance,
                IntPtr.Zero);
        }

        private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    }
}
