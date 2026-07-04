//
//* Path:    Engine/Rendering/BGFX/BGFXCallbackHandler.cs
//* Purpose: BGFX callback implementation for runtime debugging and diagnostics.
////

//

using System;
using System.Runtime.InteropServices;

//BGFX Diagnostic Types
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering.BGFX
{
    public enum BGFXFatalCode
    {
        None = 0,
        InvalidShader = 1,
        UnableToInitialize = 2,
        UnableToCreateTexture = 3
    }

    public struct BGFXCallbackVtbl
    {
        public IntPtr Fatal;
        public IntPtr Trace;
        public IntPtr ProfilerBegin;
        public IntPtr ProfilerEnd;
    }
}

//using BGFXCallbackVtbl = SASZombieAssaultTD.Engine.Rendering.BGFX.BGFXCallbackVtbl; Not implemented yet, future process

namespace SASZombieAssaultTD.Engine.Rendering.BGFX.Callbacks
{
    ///<summary>
    ///BGFX callback handler for runtime debugging.
    ///Captures BGFX internal messages and routes them to System.Diagnostics.Debug.WriteLine.
    ///</summary>
    public static class BGFXCallbackHandler
    {
        static BGFXCallbackHandler()
        {
            System.Diagnostics.Debug.WriteLine($"[FORENSIC:BGFX PROCESS] BGFX future implementation BGFXCallbackHandler");
        }

        private static BGFX.BGFXCallbackVtbl _callbackVtbl;

        //private static BGFXCallbackDelegates _delegates; Future process,not yet implemented
        //private static BGFXCallbackVtbl _callbackVtbl; Future process,not yet implemented
        private static IntPtr _callbackVtblPtr;

        private static IntPtr _callbackInterfacePtr;
        private static Action<nint, nint, ushort, BGFXFatalCode, nint> Fatal;
        private static Action<nint, nint, ushort, nint, nint> TraceVargs;
        private static Action<nint, nint, uint, nint, ushort> ProfilerBegin;
        private static Action<nint, nint, uint, nint, ushort> ProfilerBeginLiteral;
        private static Action<nint> ProfilerEnd;
        private static Action<nint, uint> CacheReadSize;
        private static Action<nint, uint, nint, uint> CacheRead;
        private static Action<nint, uint, nint, uint> CacheWrite;
        private static Action<nint, nint, uint, uint, uint, uint, nint> ScreenShot;
        private static Action<nint, uint, uint, uint, nint, bool> CaptureBegin;
        private static Action<nint> CaptureEnd;
        private static Action<nint, nint, uint> CaptureFrame;
        private static BGFXCallbackDelegates _delegates;

        //private static BGFXCallbacks.FatalDelegate Fatal; Future process,not yet implemented
        //private static BGFXCallbacks.TraceVargsDelegate TraceVargs;
        //private static BGFXCallbacks.ProfilerBeginDelegate ProfilerBegin; Future process,not yet implemented
        //private static BGFXCallbacks.ProfilerBeginDelegate ProfilerBeginLiteral; Future process,not yet implemented
        //private static BGFXCallbacks.ProfilerEndDelegate ProfilerEnd; Future process,not yet implemented
        //private static BGFXCallbacks.CacheReadSizeDelegate CacheReadSize; Future process,not yet implemented
        //private static BGFXCallbacks.CacheReadDelegate CacheRead; Future process,not yet implemented
        //private static BGFXCallbacks.CacheWriteDelegate CacheWrite; Future process,not yet implemented
        //private static BGFXCallbacks.ScreenShotDelegate ScreenShot; Future process,not yet implemented
        //private static BGFXCallbacks.CaptureBeginDelegate CaptureBegin; Future process,not yet implemented
        //private static BGFXCallbacks.CaptureEndDelegate CaptureEnd; Future process,not yet implemented
        //private static BGFXCallbacks.CaptureFrameDelegate CaptureFrame; Future process,not yet implemented

        //FUTURE IMPLEMENTATION: exception handling removed during BGFX bypass *****************************
        //public static IntPtr Initialize()
        //{
        //    System.Diagnostics.Debug.WriteLine("[BOOT] BGFXCallbackHandler.Initialize() starting");
        //
        //    //Assign to static fields first to prevent GC of delegates
        //    Fatal = FatalCallback;
        //    TraceVargs = TraceVargsCallback;
        //    ProfilerBegin = ProfilerBeginCallback;
        //    ProfilerBeginLiteral = ProfilerBeginCallback;
        //    ProfilerEnd = ProfilerEndCallback;
        //    CacheReadSize = OnCacheReadSize;
        //    CacheRead = OnCacheRead;
        //    CacheWrite = OnCacheWrite;
        //    ScreenShot = OnScreenShot;
        //    CaptureBegin = OnCaptureBegin;
        //    CaptureEnd = OnCaptureEnd;
        //    CaptureFrame = OnCaptureFrame;
        //
        //    _delegates = new BGFXCallbackDelegates
        //    {
        //        Fatal = Fatal,
        //        TraceVargs = TraceVargs,
        //        ProfilerBegin = ProfilerBegin,
        //        ProfilerBeginLiteral = ProfilerBeginLiteral,
        //        ProfilerEnd = ProfilerEnd,
        //        CacheReadSize = CacheReadSize,
        //        CacheRead = CacheRead,
        //        CacheWrite = CacheWrite,
        //        ScreenShot = ScreenShot,
        //        CaptureBegin = CaptureBegin,
        //        CaptureEnd = CaptureEnd,
        //        CaptureFrame = CaptureFrame
        //    };
        //
        //    //Runtime null-guard before marshaling
        //    if (_delegates.Fatal == null ||
        //        _delegates.TraceVargs == null ||
        //        _delegates.ProfilerBegin == null ||
        //        _delegates.ProfilerBeginLiteral == null ||
        //        _delegates.ProfilerEnd == null ||
        //        _delegates.CacheReadSize == null ||
        //        _delegates.CacheRead == null ||
        //        _delegates.CacheWrite == null ||
        //        _delegates.ScreenShot == null ||
        //        _delegates.CaptureBegin == null ||
        //        _delegates.CaptureEnd == null ||
        //        _delegates.CaptureFrame == null)
        //    {
        //        throw new InvalidOperationException("[BGFX] One or more callback delegates are null before vtable creation.");
        //    }
        //
        //    _callbackVtbl = new BGFXCallbackVtbl
        //    {
        //        fatal = Marshal.GetFunctionPointerForDelegate(_delegates.Fatal),
        //        trace_vargs = Marshal.GetFunctionPointerForDelegate(_delegates.TraceVargs),
        //        profiler_begin = Marshal.GetFunctionPointerForDelegate(_delegates.ProfilerBegin),
        //        profiler_begin_literal = Marshal.GetFunctionPointerForDelegate(_delegates.ProfilerBeginLiteral),
        //        profiler_end = Marshal.GetFunctionPointerForDelegate(_delegates.ProfilerEnd),
        //        cache_read_size = Marshal.GetFunctionPointerForDelegate<BGFXCallbacks.CacheReadSizeDelegate>(_delegates.CacheReadSize),
        //        cache_read = Marshal.GetFunctionPointerForDelegate<BGFXCallbacks.CacheReadDelegate>(_delegates.CacheRead),
        //        cache_write = Marshal.GetFunctionPointerForDelegate<BGFXCallbacks.CacheWriteDelegate>(_delegates.CacheWrite),
        //        screen_shot = Marshal.GetFunctionPointerForDelegate<BGFXCallbacks.ScreenShotDelegate>(_delegates.ScreenShot),
        //        capture_begin = Marshal.GetFunctionPointerForDelegate<BGFXCallbacks.CaptureBeginDelegate>(_delegates.CaptureBegin),
        //        capture_end = Marshal.GetFunctionPointerForDelegate<BGFXCallbacks.CaptureEndDelegate>(_delegates.CaptureEnd),
        //        capture_frame = Marshal.GetFunctionPointerForDelegate<BGFXCallbacks.CaptureFrameDelegate>(_delegates.CaptureFrame)
        //    };
        //
        //    _callbackVtblPtr = Marshal.AllocHGlobal(Marshal.SizeOf<BGFXCallbackVtbl>());
        //    Marshal.StructureToPtr(_callbackVtbl, _callbackVtblPtr, false);
        //
        //    //Create callback interface that points to vtable
        //    var callbackInterface = new BGFXCallbackInterface { vtbl = _callbackVtblPtr };
        //    _callbackInterfacePtr = Marshal.AllocHGlobal(Marshal.SizeOf<BGFXCallbackInterface>());
        //    Marshal.StructureToPtr(callbackInterface, _callbackInterfacePtr, false);
        //
        //    System.Diagnostics.Debug.WriteLine("[BGFX] Callback handler initialized");
        //
        //    return _callbackInterfacePtr;
        //}
        //FUTURE IMPLEMENTATION: exception handling removed during BGFX bypass *****************************

        public static IntPtr Initialize()
        {
            System.Diagnostics.Debug.WriteLine("[BOOT] BGFXCallbackHandler.Initialize() STUB - bypassed for BGFX implementation");
            return IntPtr.Zero; //Stub implementation
        }

        ///<summary>
        ///Cleanup callback system.
        ///</summary>
        public static void Shutdown()
        {
            if (_callbackInterfacePtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_callbackInterfacePtr);
                _callbackInterfacePtr = IntPtr.Zero;
            }

            if (_callbackVtblPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_callbackVtblPtr);
                _callbackVtblPtr = IntPtr.Zero;
            }

            System.Diagnostics.Debug.WriteLine("[BGFX] Callback handler shutdown");
        }

        ///<summary>
        ///BGFX fatal error callback.
        ///</summary>
        private static void FatalCallback(IntPtr callbackInterface, IntPtr filePath, ushort line, BGFXFatalCode code, IntPtr str)
        {
            string filePathStr = Marshal.PtrToStringAnsi(filePath) ?? "unknown";
            string messageStr = Marshal.PtrToStringAnsi(str) ?? "no message";

            System.Diagnostics.Debug.WriteLine($"[BGFX FATAL] {filePathStr}:{line} - {code} - {messageStr}");
        }

        ///<summary>
        ///BGFX trace callback with varargs.
        ///</summary>
        private static void TraceVargsCallback(IntPtr callbackInterface, IntPtr filePath, ushort line, IntPtr format, IntPtr argList)
        {
            string filePathStr = Marshal.PtrToStringAnsi(filePath) ?? "unknown";
            string formatStr = Marshal.PtrToStringAnsi(format) ?? "";

            //For simplicity, we'll just log the format string without processing varargs
            //In a full implementation, you'd need to handle varargs properly
            System.Diagnostics.Debug.WriteLine($"[BGFX TRACE] {filePathStr}:{line} - {formatStr}");
        }

        ///<summary>
        ///BGFX profiler begin callback.
        ///</summary>
        private static void ProfilerBeginCallback(IntPtr callbackInterface, IntPtr name, uint abgr, IntPtr filePath, ushort line)
        {
            string nameStr = Marshal.PtrToStringAnsi(name) ?? "unknown";
            string filePathStr = Marshal.PtrToStringAnsi(filePath) ?? "unknown";

            System.Diagnostics.Debug.WriteLine($"[BGFX PROFILER] BEGIN: {nameStr} (0x{abgr:X8}) at {filePathStr}:{line}");
        }

        ///<summary>
        ///BGFX profiler end callback.
        ///</summary>
        private static void ProfilerEndCallback(IntPtr callbackInterface)
        {
            System.Diagnostics.Debug.WriteLine("[BGFX PROFILER] END");
        }

        ///<summary>
        ///BGFX cache read size callback.
        ///</summary>
        private static void OnCacheReadSize(IntPtr callbackInterface, uint id)
        {
            System.Diagnostics.Debug.WriteLine($"[BGFX] Cache read size: {id}");
        }

        ///<summary>
        ///BGFX cache read callback.
        ///</summary>
        private static void OnCacheRead(IntPtr callbackInterface, uint id, IntPtr data, uint size)
        {
            System.Diagnostics.Debug.WriteLine($"[BGFX] Cache read: {id} ({size} bytes)");
        }

        ///<summary>
        ///BGFX cache write callback.
        ///</summary>
        private static void OnCacheWrite(IntPtr callbackInterface, uint id, IntPtr data, uint size)
        {
            System.Diagnostics.Debug.WriteLine($"[BGFX] Cache write: {id} ({size} bytes)");
        }

        ///<summary>
        ///BGFX screen shot callback.
        ///</summary>
        private static void OnScreenShot(IntPtr callbackInterface, IntPtr filePath, uint width, uint height, uint pitch, uint size, IntPtr data)
        {
            string path = Marshal.PtrToStringAnsi(filePath) ?? "unknown";
            System.Diagnostics.Debug.WriteLine($"[BGFX] Screenshot: {path} ({width}x{height})");
        }

        ///<summary>
        ///BGFX capture begin callback.
        ///</summary>
        private static void OnCaptureBegin(IntPtr callbackInterface, uint width, uint height, uint pitch, IntPtr format, bool yflip)
        {
            System.Diagnostics.Debug.WriteLine($"[BGFX] Capture begin: {width}x{height}");
        }

        ///<summary>
        ///BGFX capture end callback.
        ///</summary>
        private static void OnCaptureEnd(IntPtr callbackInterface)
        {
            System.Diagnostics.Debug.WriteLine("[BGFX] Capture end");
        }

        ///<summary>
        ///BGFX capture frame callback.
        ///</summary>
        private static void OnCaptureFrame(IntPtr callbackInterface, IntPtr data, uint size)
        {
            System.Diagnostics.Debug.WriteLine($"[BGFX] Capture frame: {size} bytes");
        }
    }

    internal class BGFXCallbackDelegates
    {
        public Action<nint, nint, ushort, BGFXFatalCode, nint> Fatal { get; set; }
        public Action<nint, nint, ushort, nint, nint> TraceVargs { get; set; }
        public Action<nint, nint, uint, nint, ushort> ProfilerBegin { get; set; }
        public Action<nint, nint, uint, nint, ushort> ProfilerBeginLiteral { get; set; }
        public Action<nint> ProfilerEnd { get; set; }
        public Action<nint, uint> CacheReadSize { get; set; }
        public Action<nint, uint, nint, uint> CacheRead { get; set; }
        public Action<nint, uint, nint, uint> CacheWrite { get; set; }
        public Action<nint, nint, uint, uint, uint, uint, nint> ScreenShot { get; set; }
        public Action<nint, uint, uint, uint, nint, bool> CaptureBegin { get; set; }
        public Action<nint> CaptureEnd { get; set; }
        public Action<nint, nint, uint> CaptureFrame { get; set; }
    }
}
