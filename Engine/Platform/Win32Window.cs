/* ====================================================================================================
 *  FILE:       Win32Window.cs
 *  PATH:       Engine/Platform/
 *  SUBSYSTEM:  Platform.Win32
 *  ROLE:       Win32 window host that owns the HWND, pumps messages, and drives the hybrid
 *              CPU-framebuffer → GPU-present rendering pipeline via GameRoot.Render.
 *
 *  RESPONSIBILITIES:
 *      - Register and create the Win32 window (HWND).
 *      - Pump the Win32 message loop and signal shutdown on WM_QUIT.
 *      - Own the D3D11DeviceCore and D3D11FramebufferPresenter lifetimes.
 *      - Own the WindowHost that provides the engine-level IRenderContext.
 *      - Run the main render loop:
 *          1) Call GameRoot.Render(IRenderContext) for CPU rendering.
 *          2) Upload the framebuffer via D3D11FramebufferPresenter.
 *          3) Present via D3D11DeviceCore.RenderFramebufferAndPresent().
 *
 *  NON-RESPONSIBILITIES:
 *      - Game logic, ECS, or simulation (delegated to GameRoot and subsystems).
 *      - CPU-side rendering details (delegated to RenderManager / Framebuffer).
 *      - GPU pipeline setup beyond what D3D11DeviceCore already encapsulates.
 *      - Any GDI-based presentation (StretchDIBits is no longer used).
 *
 *  DEPENDENCIES:
 *      - GameRoot (engine root that exposes Render(IRenderContext)).
 *      - WindowHost (logical window + primary Framebuffer / IRenderContext).
 *      - Framebuffer (CPU-side deterministic render target).
 *      - D3D11DeviceCore (D3D11 device, swap chain, fullscreen quad pipeline).
 *      - D3D11FramebufferPresenter (CPU framebuffer → GPU texture bridge).
 *
 *  CALLED BY:
 *      - Engine bootstrap code to create the window and run the main loop.
 *
 *  CALLS INTO:
 *      - GameRoot.Render(IRenderContext).
 *      - WindowHost.CreateRenderContext().
 *      - D3D11FramebufferPresenter.EnsureSize / Upload.
 *      - D3D11DeviceCore.RenderFramebufferAndPresent().
 *
 *  ARCHITECTURAL NOTES:
 *      - This host is platform-specific (Win32) and must not leak into engine logic.
 *      - Presentation is now fully GPU-based; GDI/StretchDIBits are intentionally removed.
 *      - This file is complete and must not be split into partials.
 * ==================================================================================================== */

using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Rendering.D3D11;
using System;
using System.Runtime.InteropServices;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Platform
{
    public sealed class Win32Window : IDisposable
    {
        private readonly int _width;
        private readonly int _height;
        private readonly string _title;
        private readonly bool _vsync;

        private readonly WndProcDelegate _wndProcDelegate;

        private IntPtr _hwnd;
        private bool _disposed;

        private WindowHost? _windowHost;
        private D3D11DeviceCore? _deviceCore;
        private D3D11FramebufferPresenter? _presenter;

        private const int WS_OVERLAPPEDWINDOW = 0x00CF0000;
        private const int CW_USEDEFAULT = unchecked((int)0x80000000);
        private const int WM_DESTROY = 0x0002;
        private const int WM_QUIT = 0x0012;
        private const int WM_SIZE = 0x0005;

        public Win32Window(int width, int height, string title, bool vsync)
        {
            _width = width;
            _height = height;
            _title = string.IsNullOrWhiteSpace(title) ? "SAS Zombie Assault TD" : title;
            _vsync = vsync;

            _wndProcDelegate = WndProc; //keep delegate rooted
        }

        ///<summary>
        ///Creates the Win32 window and initializes the D3D11 device, presenter, and WindowHost.
        ///Must be called before Run.
        ///</summary>
        public void Create()
        {
            WNDCLASS wc = new WNDCLASS
            {
                lpfnWndProc = _wndProcDelegate,
                lpszClassName = "SASWindowClass"
            };

            ushort classAtom = RegisterClass(ref wc);
            if (classAtom == 0)
            {
                int err = Marshal.GetLastWin32Error();
                throw new InvalidOperationException($"Failed to register window class. Win32Error={err}");
            }

            _hwnd = CreateWindowEx(
                0,
                wc.lpszClassName,
                _title,
                WS_OVERLAPPEDWINDOW,
                CW_USEDEFAULT, CW_USEDEFAULT,
                _width, _height,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
            );

            if (_hwnd == IntPtr.Zero)
            {
                int err = Marshal.GetLastWin32Error();
                throw new InvalidOperationException($"Failed to create window. Win32Error={err}");
            }

            ShowWindow(_hwnd, 1);

            //Initialize engine-level window host and GPU pipeline.
            _windowHost = new WindowHost(_width, _height, _title);
            _deviceCore = new D3D11DeviceCore(_hwnd, _width, _height, _vsync);
            _presenter = new D3D11FramebufferPresenter(_deviceCore);
        }

        ///<summary>
        ///Runs the main message and render loop using the provided GameRoot.
        ///</summary>
        ///<param name="gameRoot">Engine root that exposes Render(IRenderContext).</param>
        public void Run(GameRoot gameRoot)
        {
            if (gameRoot == null) throw new ArgumentNullException(nameof(gameRoot));
            if (_windowHost == null || _deviceCore == null || _presenter == null)
                throw new InvalidOperationException("Win32Window.Create must be called before Run.");

            var renderContext = _windowHost.CreateRenderContext();
            if (renderContext is not Framebuffer framebuffer)
                throw new InvalidOperationException("WindowHost must return a Framebuffer as the render context.");

            while (PumpMessages())
            {
                //CPU-side rendering into the framebuffer.
                gameRoot.Render(renderContext);

                //Ensure GPU texture matches framebuffer size and upload pixels.
                _presenter.EnsureSize(framebuffer);
                _presenter.Upload(framebuffer);

                if (_presenter.ShaderResourceView != null)
                {
                    _deviceCore.RenderFramebufferAndPresent(_presenter.ShaderResourceView);
                }
            }
        }

        ///<summary>
        ///Pumps OS messages. Returns false when a quit message is received.
        ///</summary>
        private bool PumpMessages()
        {
            MSG msg;
            while (PeekMessage(out msg, IntPtr.Zero, 0, 0, 1))
            {
                if (msg.message == WM_QUIT)
                    return false;

                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }

            return true;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _presenter?.Dispose();
            _deviceCore?.Dispose();
            _windowHost = null;

            if (_hwnd != IntPtr.Zero)
            {
                DestroyWindow(_hwnd);
                _hwnd = IntPtr.Zero;
            }

            _disposed = true;
        }

        private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case WM_DESTROY:
                    PostQuitMessage(0);
                    break;

                case WM_SIZE:
                    OnResize(lParam);
                    break;
            }

            return DefWindowProc(hWnd, msg, wParam, lParam);
        }

        private void OnResize(IntPtr lParam)
        {
            if (_deviceCore == null || _presenter == null)
                return;

            int width = (short)(lParam.ToInt32() & 0xFFFF);
            int height = (short)((lParam.ToInt32() >> 16) & 0xFFFF);

            if (width <= 0 || height <= 0)
                return;

            _deviceCore.Resize(width, height);
            //Framebuffer/WindowHost resize policy can be added here if you choose to support dynamic sizes.
        }

        //-------------------------
        //Win32 Structs & Imports
        //-------------------------

        [StructLayout(LayoutKind.Sequential)]
        private struct WNDCLASS
        {
            public uint style;
            public WndProcDelegate lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
        }

        private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public int pt_x;
            public int pt_y;
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern ushort RegisterClass([In] ref WNDCLASS lpWndClass);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateWindowEx(
            int dwExStyle,
            string lpClassName,
            string lpWindowName,
            int dwStyle,
            int x, int y,
            int nWidth, int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam
        );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool TranslateMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr DispatchMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern void PostQuitMessage(int exitCode);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool DestroyWindow(IntPtr hWnd);
    }
}
