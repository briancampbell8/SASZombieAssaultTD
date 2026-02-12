/*
    File:    Win32Window.cs
    Author:  BDC
    Created: 2026-02-10

    Purpose:
        Manages Win32 window creation, message pumping, and presenting the framebuffer.

    Notes:
        Implements IDisposable to release the native window handle.
        BITMAPINFO struct is private and used internally for Win32 presentation.
*/
using SASZombieAssaultTD.Engine.Rendering;
using System;
using System.Runtime.InteropServices;

namespace SASZombieAssaultTD.Engine.Platform
{
    public sealed class Win32Window : IDisposable
    {
        // BITMAPINFO struct is private and used internally.
        // See Dispose() for resource cleanup.

        private readonly int _width;
        private readonly int _height;
        private readonly string _title;
        private readonly Framebuffer _fb;

        private readonly WndProcDelegate _wndProcDelegate;

        private IntPtr _hwnd;
        private bool _disposed;

        public Win32Window(int width, int height, string title, Framebuffer fb)
        {
            _width = width;
            _height = height;
            _title = title;
            _fb = fb;

            _wndProcDelegate = WndProc; // root delegate to avoid GC
        }

        private const int WS_OVERLAPPEDWINDOW = 0x00CF0000;
        private const int CW_USEDEFAULT = unchecked((int)0x80000000);
        private const int WM_DESTROY = 0x0002;
        private const int WM_QUIT = 0x0012;

        public void Create()
        {
            WNDCLASS wc = new WNDCLASS();
            wc.lpfnWndProc = _wndProcDelegate;
            wc.lpszClassName = "SASWindowClass";

            ushort classAtom = RegisterClass(ref wc);
            if (classAtom == 0)
            {
                int err = Marshal.GetLastWin32Error();
                throw new Exception($"Failed to register window class. Win32Error={err}");
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
                throw new Exception($"Failed to create window. Win32Error={err}");
            }

            ShowWindow(_hwnd, 1);
        }

        /// <summary>
        /// Pumps OS messages. Returns false when a quit message is received.
        /// </summary>
        public bool PumpMessages()
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

        public void Present()
        {
            IntPtr hdc = GetDC(_hwnd);

            BITMAPINFO bmi = new BITMAPINFO();
            bmi.biSize = Marshal.SizeOf(typeof(BITMAPINFO));
            bmi.biWidth = _fb.Width;
            bmi.biHeight = -_fb.Height; // top-down
            bmi.biPlanes = 1;
            bmi.biBitCount = 32;
            bmi.biCompression = 0; // BI_RGB

            StretchDIBits(
                hdc,
                0, 0, _fb.Width, _fb.Height,
                0, 0, _fb.Width, _fb.Height,
                _fb.Pixels,
                ref bmi,
                0, // DIB_RGB_COLORS
                0x00CC0020 // SRCCOPY
            );

            ReleaseDC(_hwnd, hdc);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            if (_hwnd != IntPtr.Zero)
            {
                DestroyWindow(_hwnd);
                _hwnd = IntPtr.Zero;
            }

            _disposed = true;
        }

        private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == WM_DESTROY)
            {
                PostQuitMessage(0);
            }

            return DefWindowProc(hWnd, msg, wParam, lParam);
        }

        // -------------------------
        // Win32 Structs & Imports
        // -------------------------

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

        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFO
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
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

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int StretchDIBits(
            IntPtr hdc,
            int xDest, int yDest, int wDest, int hDest,
            int xSrc, int ySrc, int wSrc, int hSrc,
            byte[] lpBits,
            [In] ref BITMAPINFO lpBitsInfo,
            uint iUsage,
            uint dwRop
        );
    }
}