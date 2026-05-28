// ============================================================================
// File:        D3D11Window.cs
// Author:      BDC
// Created:     2026-05-14
// Purpose:     Provides a Win32 window for D3D11 rendering. Exposes HWND and
//              handles message pumping. No rendering logic is performed here.
//
// Responsibilities:
// - Register Win32 window class
// - Create and manage the application window
// - Expose HWND for D3D11 swap-chain creation
// - Pump and dispatch Win32 messages
// - Handle window destruction and cleanup
//
// Dependencies:
// - Win32 API (user32.dll)
//
// Thread Safety:
// - All operations must occur on the main UI thread.
//
// Architectural Notes:
// - This class replaces the legacy Win32Window and removes all framebuffer
//   and GDI responsibilities.
// - Rendering is performed exclusively through the D3D11 swap-chain.
// - This class provides only HWND and message pump functionality.
// ============================================================================

using System;
using System.Runtime.InteropServices;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Platform
{
    public sealed class D3D11Window : IDisposable
    {
        private IntPtr _hwnd;
        private bool _disposed;

        public IntPtr Handle => _hwnd;

        private readonly int _width;
        private readonly int _height;
        private readonly string _title;

        private readonly WndProcDelegate _wndProcDelegate;

        public D3D11Window(int width, int height, string title)
        {
            Engine.Diagnostics.DebugLogger.Trace("D3D11Window.Ctor.Start", $"width={width}, height={height}, title={title}");

            _width = width;
            _height = height;
            _title = title;
            _wndProcDelegate = WndProc;

            Engine.Diagnostics.DebugLogger.Trace("D3D11Window.Ctor.End", "OK");
        }

        public void Create()
        {
            Engine.Diagnostics.DebugLogger.Trace("D3D11Window.Create.Start", "Begin");

            IntPtr hInstance = GetModuleHandle(null);
            Engine.Diagnostics.DebugLogger.Trace("D3D11Window.Create.hInstance", $"hInstance=0x{hInstance.ToString("X")}");

            IntPtr hCursor = LoadCursor(IntPtr.Zero, (IntPtr)32512); // IDC_ARROW

            WNDCLASSEX wc = new WNDCLASSEX
            {
                cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(WNDCLASSEX)),
                style = 0,
                lpfnWndProc = _wndProcDelegate,
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = hInstance,
                hIcon = IntPtr.Zero,
                hCursor = hCursor,
                hbrBackground = IntPtr.Zero,
                lpszMenuName = null,
                lpszClassName = "SASD3D11WindowClass",
                hIconSm = IntPtr.Zero
            };

            Engine.Diagnostics.DebugLogger.Trace("D3D11Window.Create.RegisterClass", "Calling RegisterClassEx");
            ushort atom = RegisterClassEx(ref wc);
            Engine.Diagnostics.DebugLogger.Trace("D3D11Window.Create.RegisterClass.Result", $"atom={atom}");

            if (atom == 0)
            {
                Engine.Diagnostics.DebugLogger.Trace("D3D11Window.Create.Error", "RegisterClassEx failed.");
                throw new Exception("Failed to register window class.");
            }

            Engine.Diagnostics.DebugLogger.Trace("D3D11Window.Create.CreateWindowEx", "Calling CreateWindowEx");

            _hwnd = CreateWindowEx(
                0,
                wc.lpszClassName,
                _title,
                0x00CF0000,
                unchecked((int)0x80000000),
                unchecked((int)0x80000000),
                _width,
                _height,
                IntPtr.Zero,
                IntPtr.Zero,
                hInstance,
                IntPtr.Zero
            );

            Engine.Diagnostics.DebugLogger.Trace("D3D11Window.Create.HWND", $"_hwnd={_hwnd}");

            if (_hwnd == IntPtr.Zero)
            {
                Engine.Diagnostics.DebugLogger.Trace("D3D11Window.Create.Error", "CreateWindowEx returned NULL HWND.");
                throw new Exception("Failed to create window.");
            }

            Engine.Diagnostics.DebugLogger.Trace("D3D11Window.Create.ShowWindow", "Calling ShowWindow");
            ShowWindow(_hwnd, 1);

            Engine.Diagnostics.DebugLogger.Trace("D3D11Window.Create.End", "OK");
        }

        public bool PumpMessages()
        {
            Engine.Diagnostics.DebugLogger.Trace("D3D11Window.PumpMessages.Start", "Begin");

            MSG msg;
            while (PeekMessage(out msg, IntPtr.Zero, 0, 0, 1))
            {
                Engine.Diagnostics.DebugLogger.Trace(
                    "D3D11Window.PumpMessages.Message",
                    $"msg={msg.message}, hwnd={msg.hwnd}, wParam={msg.wParam}, lParam={msg.lParam}");

                if (msg.message == 0x0012) // WM_QUIT
                {
                    Engine.Diagnostics.DebugLogger.Trace("D3D11Window.PumpMessages.Quit", "WM_QUIT received");
                    return false;
                }

                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }

            Engine.Diagnostics.DebugLogger.Trace("D3D11Window.PumpMessages.End", "Continue");
            return true;
        }

        private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            Engine.Diagnostics.DebugLogger.Trace(
                "D3D11Window.WndProc",
                $"hWnd={hWnd}, msg={msg}, wParam={wParam}, lParam={lParam}");

            if (msg == 0x0002) // WM_DESTROY
            {
                Engine.Diagnostics.DebugLogger.Trace("D3D11Window.WndProc.WM_DESTROY", "Posting quit message");
                PostQuitMessage(0);
            }

            return DefWindowProc(hWnd, msg, wParam, lParam);
        }

        public void Dispose()
        {
            Engine.Diagnostics.DebugLogger.Trace("D3D11Window.Dispose.Start", $"disposed={_disposed}, hwnd={_hwnd}");

            if (_disposed)
            {
                Engine.Diagnostics.DebugLogger.Trace("D3D11Window.Dispose.Skip", "Already disposed");
                return;
            }

            if (_hwnd != IntPtr.Zero)
            {
                Engine.Diagnostics.DebugLogger.Trace("D3D11Window.Dispose.DestroyWindow", "Calling DestroyWindow");
                DestroyWindow(_hwnd);
                _hwnd = IntPtr.Zero;
            }

            _disposed = true;

            Engine.Diagnostics.DebugLogger.Trace("D3D11Window.Dispose.End", "OK");
        }

        private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WNDCLASSEX
        {
            public uint cbSize;
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
            public IntPtr hIconSm;
        }

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

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadCursor(IntPtr hInstance, IntPtr lpCursorName);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern ushort RegisterClassEx([In] ref WNDCLASSEX lpWndClass);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
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

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, uint min, uint max, uint remove);

        [DllImport("user32.dll")]
        private static extern bool TranslateMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll")]
        private static extern IntPtr DispatchMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll")]
        private static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern void PostQuitMessage(int exitCode);

        [DllImport("user32.dll")]
        private static extern bool DestroyWindow(IntPtr hWnd);
    }
}
