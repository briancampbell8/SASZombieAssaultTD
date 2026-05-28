// FUTURE IMPLEMENTATION: entire BGFXNative.cs file commented out due to unresolved compilation issues *****************************
/// FUTURE IMPLEMENTATION: entire BGFXNative.cs file commented out due to unresolved compilation issues *****************************
using SASZombieAssaultTD.Engine.Diagnostics;

// FUTURE IMPLEMENTATION: entire BGFXNative.cs file commented out due to unresolved compilation issues *****************************

// * File:    BGFXNative.cs
// * Path:    Engine/Rendering/BGFX/BGFXNative.cs
// * Purpose: BGFX native interop surface - minimal P/Invoke declarations for BGFX API.
// //
//using System;
//using System.Runtime.InteropServices;

//namespace SASZombieAssaultTD.Engine.Rendering.BGFX
//{
//    /// <summary>
//    /// BGFX callback delegates for runtime debugging.
//    /// </summary>
//    public static class BGFXCallbacks
//    {
//        public delegate void FatalDelegate(IntPtr callbackInterface, IntPtr filePath, ushort line, BGFXFatalCode code, IntPtr str);
//        public delegate void TraceVargsDelegate(IntPtr callbackInterface, IntPtr filePath, ushort line, IntPtr format, IntPtr argList);
//        public delegate void ProfilerBeginDelegate(IntPtr callbackInterface, IntPtr name, uint abgr, IntPtr filePath, ushort line);
//        public delegate void ProfilerEndDelegate(IntPtr callbackInterface);
//        public delegate void CacheReadSizeDelegate(IntPtr callbackInterface, uint id);
//        public delegate void CacheReadDelegate(IntPtr callbackInterface, uint id, IntPtr data, uint size);
//        public delegate void CacheWriteDelegate(IntPtr callbackInterface, uint id, IntPtr data, uint size);
//        public delegate void ScreenShotDelegate(IntPtr callbackInterface, IntPtr filePath, uint width, uint height, uint pitch, uint size, IntPtr data);
//        public delegate void CaptureBeginDelegate(IntPtr callbackInterface, uint width, uint height, uint pitch, IntPtr format, [MarshalAs(UnmanagedType.U1)] bool yflip);
//        public delegate void CaptureEndDelegate(IntPtr callbackInterface);
//        public delegate void CaptureFrameDelegate(IntPtr callbackInterface, IntPtr data, uint size);
//    }

//    /// <summary>
//    /// BGFX fatal error codes.
//    /// </summary>
//    public enum BGFXFatalCode : byte
//    {
//        DebugCheck = 0,
//        InvalidShader = 1,
//        UnableToInitialize = 2,
//        UnableToCreateTexture = 3,
//        DeviceLost = 4,
//        Count = 5
//    }

//    /// <summary>
//    /// BGFX callback virtual table structure.
//    /// </summary>
//    [StructLayout(LayoutKind.Sequential)]
//    public struct BGFXCallbackVtbl
//    {
//        public IntPtr fatal;
//        public IntPtr trace_vargs;
//        public IntPtr profiler_begin;
//        public IntPtr profiler_begin_literal;
//        public IntPtr profiler_end;
//        public IntPtr cache_read_size;
//        public IntPtr cache_read;
//        public IntPtr cache_write;
//        public IntPtr screen_shot;
//        public IntPtr capture_begin;
//        public IntPtr capture_end;
//        public IntPtr capture_frame;
//    }

//    /// <summary>
//    /// BGFX callback interface structure.
//    /// </summary>
//    [StructLayout(LayoutKind.Sequential)]
//    public struct BGFXCallbackInterface
//    {
//        public IntPtr vtbl; // Pointer to BGFXCallbackVtbl
//    }

//    /// <summary>
//    /// Managed delegate holder for BGFX callbacks.
//    /// </summary>
//    public sealed class BGFXCallbackDelegates
//    {
//        public BGFXCallbacks.FatalDelegate Fatal;
//        public BGFXCallbacks.TraceVargsDelegate TraceVargs;
//        public BGFXCallbacks.ProfilerBeginDelegate ProfilerBegin;
//        public BGFXCallbacks.ProfilerBeginDelegate ProfilerBeginLiteral;
//        public BGFXCallbacks.ProfilerEndDelegate ProfilerEnd;
//        public BGFXCallbacks.CacheReadSizeDelegate CacheReadSize;
//        public BGFXCallbacks.CacheReadDelegate CacheRead;
//        public BGFXCallbacks.CacheWriteDelegate CacheWrite;
//        public BGFXCallbacks.ScreenShotDelegate ScreenShot;
//        public BGFXCallbacks.CaptureBeginDelegate CaptureBegin;
//        public BGFXCallbacks.CaptureEndDelegate CaptureEnd;
//        public BGFXCallbacks.CaptureFrameDelegate CaptureFrame;
//    }

//    /// <summary>
//    /// BGFX native interop surface - minimal P/Invoke declarations for BGFX API.
//    /// This class contains only the essential native declarations needed for Phase 10.
//    /// </summary>
//    internal static class BGFXNative
//    {
//        /// <summary>
//        /// BGFX renderer type enumeration.
//        /// </summary>
//        public enum RendererType : uint
//        {
//            Noop = 0,
//            Direct3D11 = 1,
//            Direct3D12 = 2,
//            OpenGL = 3,
//            Vulkan = 4,
//            Metal = 5,
//            Gnm = 6,
//            Nvn = 7,
//            WebGPU = 8,
//            Count = 9
//        }

//        /// <summary>
//        /// BGFX reset flags enumeration.
//        /// </summary>
//        [Flags]
//        public enum ResetFlags : uint
//        {
//            Vsync = 0x00000080,
//            MaxAnisotropy = 0x00000100,
//            FlushAfterRender = 0x00000200,
//            SrgbBackbuffer = 0x00000400,
//            MsaaX2 = 0x00000010,
//            MsaaX4 = 0x00000020,
//            MsaaX8 = 0x00000030,
//            MsaaX16 = 0x00000040
//        }

//        /// <summary>
//        /// BGFX clear flags enumeration.
//        /// </summary>
//        [Flags]
//        public enum ClearFlags : ushort
//        {
//            Color = 0x0001,
//            Depth = 0x0002,
//            Stencil = 0x0004
//        }

//        /// <summary>
//        /// BGFX initialization structure.
//        /// </summary>
//        [StructLayout(LayoutKind.Sequential, Pack = 8)]
//        public struct Init
//        {
//            public RendererType type;
//            public ushort vendorId;
//            public ushort deviceId;
//            public ulong capabilities;
//            // BGFX uses 1-byte bools; represent them as bytes in C#
//            public byte debug;
//            public byte profile;
//            public byte fallback;
//            public byte _pad0; // padding to align next field
//            public PlatformData platformData;
//            public Resolution resolution;
//            public InitLimits limits;
//            public IntPtr callback; // bgfx_callback_interface_t*
//            public IntPtr allocator; // bgfx_allocator_interface_t*
//        }

//        /// <summary>
//        /// BGFX platform data structure.
//        /// </summary>
//        [StructLayout(LayoutKind.Sequential, Pack = 8)]
//        public struct PlatformData
//        {
//            public IntPtr ndt;          // Native display type (void*)
//            public IntPtr nwh;          // Native window handle (void*)
//            public IntPtr context;      // GL context, D3D device, or Vulkan device (void*)
//            public IntPtr queue;        // D3D12 Queue (void*)
//            public IntPtr backBuffer;   // GL back-buffer, or D3D render target view (void*)
//            public IntPtr backBufferDS; // Backbuffer depth/stencil (void*)
//            public int type;            // bgfx_native_window_handle_type_t (enum = 4 bytes)
//        }

//        /// <summary>
//        /// BGFX resolution structure.
//        /// </summary>
//        [StructLayout(LayoutKind.Sequential, Pack = 4)]
//        public struct Resolution
//        {
//            public int formatColor;         // bgfx_texture_format_t (enum = 4 bytes)
//            public int formatDepthStencil;  // bgfx_texture_format_t (enum = 4 bytes)
//            public uint width;              // Backbuffer width (4 bytes)
//            public uint height;             // Backbuffer height (4 bytes)
//            public uint reset;              // Reset parameters (4 bytes)
//            public byte numBackBuffers;     // Number of back buffers (1 byte)
//            public byte maxFrameLatency;    // Maximum frame latency (1 byte)
//            public byte debugTextScale;     // Scale factor for debug text (1 byte)
//            private byte _padding;          // Alignment padding (1 byte)
//        }

//        /// <summary>
//        /// BGFX initialization limits structure.
//        /// </summary>
//        [StructLayout(LayoutKind.Sequential, Pack = 4)]
//        public struct InitLimits
//        {
//            public ushort maxEncoders;          // Maximum number of encoder threads (2 bytes)
//            public ushort _padding0;            // Padding to align to 4 bytes
//            public uint minResourceCbSize;      // Minimum resource command buffer size (4 bytes)
//            public uint maxTransientVbSize;     // Maximum transient vertex buffer size (4 bytes)
//            public uint maxTransientIbSize;      // Maximum transient index buffer size (4 bytes)
//            public uint minUniformBufferSize;    // Minimum uniform buffer size (4 bytes)
//        }

//        /// <summary>
//        /// Initialize BGFX library.
//        /// </summary>
//        /// <param name="init">Initialization parameters.</param>
//        /// <returns>True if initialization succeeded, false otherwise.</returns>
//        [DllImport("bgfx", CallingConvention = CallingConvention.Cdecl)]
//        public static extern bool bgfx_init(ref Init init);

//        /// <summary>
//        /// Shutdown BGFX library.
//        /// </summary>
//        [DllImport("bgfx", CallingConvention = CallingConvention.Cdecl)]
//        public static extern void bgfx_shutdown();

//        /// <summary>
//        /// Reset BGFX with specified parameters.
//        /// </summary>
//        /// <param name="width">Width in pixels.</param>
//        /// <param name="height">Height in pixels.</param>
//        /// <param name="flags">Reset flags.</param>
//        /// <param name="init">Optional init structure (nullptr if not needed).</param>
//        [DllImport("bgfx", CallingConvention = CallingConvention.Cdecl)]
//        public static extern void bgfx_reset(uint width, uint height, ResetFlags flags, IntPtr init);

//        /// <summary>
//        /// Set view rectangle for specified view.
//        /// </summary>
//        /// <param name="id">View ID.</param>
//        /// <param name="x">X offset in pixels.</param>
//        /// <param name="y">Y offset in pixels.</param>
//        /// <param name="width">Width in pixels.</param>
//        /// <param name="height">Height in pixels.</param>
//        [DllImport("bgfx", CallingConvention = CallingConvention.Cdecl)]
//        public static extern void bgfx_set_view_rect(ushort id, ushort x, ushort y, ushort width, ushort height);

//        /// <summary>
//        /// Set view clear parameters.
//        /// </summary>
//        /// <param name="id">View ID.</param>
//        /// <param name="flags">Clear flags.</param>
//        /// <param name="rgba">Clear color (0xRRGGBBAA).</param>
//        /// <param name="depth">Clear depth value.</param>
//        /// <param name="stencil">Clear stencil value.</param>
//        [DllImport("bgfx", CallingConvention = CallingConvention.Cdecl)]
//        public static extern void bgfx_set_view_clear(ushort id, ushort flags, uint rgba, float depth, byte stencil);

//        /// <summary>
//        /// Advance to next frame.
//        /// </summary>
//        [DllImport("bgfx", CallingConvention = CallingConvention.Cdecl)]
//        public static extern uint bgfx_frame();

//        /// <summary>
//        /// Create vertex buffer.
//        /// </summary>
//        /// <param name="data">Vertex data pointer.</param>
//        /// <param name="size">Size of vertex data in bytes.</param>
//        /// <param name="layout">Vertex layout handle.</param>
//        /// <param name="flags">Buffer creation flags.</param>
//        /// <returns>Vertex buffer handle.</returns>
//        [DllImport("bgfx", CallingConvention = CallingConvention.Cdecl)]
//        public static extern IntPtr bgfx_create_vertex_buffer(IntPtr data, uint size, IntPtr layout, ushort flags);

//        /// <summary>
//        /// Create index buffer.
//        /// </summary>
//        /// <param name="data">Index data pointer.</param>
//        /// <param name="size">Size of index data in bytes.</param>
//        /// <param name="flags">Buffer creation flags.</param>
//        /// <returns>Index buffer handle.</returns>
//        [DllImport("bgfx", CallingConvention = CallingConvention.Cdecl)]
//        public static extern IntPtr bgfx_create_index_buffer(IntPtr data, uint size, ushort flags);

//        /// <summary>
//        /// Create shader program.
//        /// </summary>
//        /// <param name="vsh">Vertex shader handle.</param>
//        /// <param name="fsh">Fragment shader handle.</param>
//        /// <returns>Program handle.</returns>
//        [DllImport("bgfx", CallingConvention = CallingConvention.Cdecl)]
//        public static extern IntPtr bgfx_create_program(IntPtr vsh, IntPtr fsh);

//        /// <summary>
//        /// Set vertex buffer.
//        /// </summary>
//        /// <param name="stream">Stream index.</param>
//        /// <param name="vertexBuffer">Vertex buffer handle.</param>
//        /// <param name="startVertex">Start vertex index.</param>
//        /// <param name="numVertices">Number of vertices.</param>
//        [DllImport("bgfx", CallingConvention = CallingConvention.Cdecl)]
//        public static extern void bgfx_set_vertex_buffer(byte stream, IntPtr vertexBuffer, uint startVertex, uint numVertices);

//        /// <summary>
//        /// Set index buffer.
//        /// </summary>
//        /// <param name="indexBuffer">Index buffer handle.</param>
//        /// <param name="firstIndex">First index.</param>
//        /// <param name="numIndices">Number of indices.</param>
//        [DllImport("bgfx", CallingConvention = CallingConvention.Cdecl)]
//        public static extern void bgfx_set_index_buffer(IntPtr indexBuffer, uint firstIndex, uint numIndices);

//        /// <summary>
//        /// Submit draw call.
//        /// </summary>
//        /// <param name="view">View ID.</param>
//        /// <param name="program">Program handle.</param>
//        /// <param name="depth">Depth value.</param>
//        /// <param name="preserveState">Whether to preserve state.</param>
//        [DllImport("bgfx", CallingConvention = CallingConvention.Cdecl)]
//        public static extern void bgfx_submit(ushort view, IntPtr program, int depth, bool preserveState);

//        /// <summary>
//        /// Create shader from compiled data.
//        /// </summary>
//        /// <param name="data">Compiled shader data.</param>
//        /// <returns>Shader handle.</returns>
//        [DllImport("bgfx", CallingConvention = CallingConvention.Cdecl)]
//        public static extern IntPtr bgfx_create_shader(byte[] data);

//        /// <summary>
//        /// Create program from vertex and fragment shaders.
//        /// </summary>
//        /// <param name="vs">Vertex shader handle.</param>
//        /// <param name="fs">Fragment shader handle.</param>
//        /// <param name="destroyShaders">Whether to destroy shaders after linking.</param>
//        /// <returns>Program handle.</returns>
//        [DllImport("bgfx", CallingConvention = CallingConvention.Cdecl)]
//        public static extern IntPtr bgfx_create_program(IntPtr vs, IntPtr fs, bool destroyShaders);

//        /// <summary>
//        /// Create uniform for shader parameter binding.
//        /// </summary>
//        /// <param name="name">Uniform name.</param>
//        /// <param name="type">Uniform type.</param>
//        /// <param name="num">Number of elements (for arrays).</param>
//        /// <returns>Uniform handle.</returns>
//        [DllImport("bgfx", CallingConvention = CallingConvention.Cdecl)]
//        public static extern IntPtr bgfx_create_uniform(string name, UniformType type, ushort num);

//        /// <summary>
//        /// Set uniform value for shader.
//        /// </summary>
//        /// <param name="uniform">Uniform handle.</param>
//        /// <param name="data">Pointer to uniform data.</param>
//        /// <param name="num">Number of elements to set.</param>
//        [DllImport("bgfx", CallingConvention = CallingConvention.Cdecl)]
//        public static extern void bgfx_set_uniform(IntPtr uniform, IntPtr data, ushort num);

//        /// <summary>
//        /// BGFX uniform types enumeration.
//        /// </summary>
//        public enum UniformType : byte
//        {
//            Vec4 = 1,
//            Mat3 = 2,
//            Mat4 = 3
//        }

//        /// <summary>
//        /// Create 2D texture.
//        /// </summary>
//        /// <param name="width">Texture width.</param>
//        /// <param name="height">Texture height.</param>
//        /// <param name="hasMips">Whether texture has mipmaps.</param>
//        /// <param name="format">Texture format.</param>
//        /// <param name="flags">Texture creation flags.</param>
//        /// <param name="data">Texture data.</param>
//        /// <returns>Texture handle.</returns>
//        [DllImport("bgfx", CallingConvention = CallingConvention.Cdecl)]
//        public static extern IntPtr bgfx_create_texture_2d(ushort width, ushort height, bool hasMips, byte format, ushort flags, byte[] data);

//        /// <summary>
//        /// Create sampler.
//        /// </summary>
//        /// <param name="name">Sampler name.</param>
//        /// <returns>Sampler handle.</returns>
//        [DllImport("bgfx", CallingConvention = CallingConvention.Cdecl)]
//        public static extern IntPtr bgfx_create_sampler(string name);

//        /// <summary>
//        /// Set texture for shader stage.
//        /// </summary>
//        /// <param name="stage">Shader stage.</param>
//        /// <param name="sampler">Sampler handle.</param>
//        /// <param name="texture">Texture handle.</param>
//        [DllImport("bgfx", CallingConvention = CallingConvention.Cdecl)]
//        public static extern void bgfx_set_texture(byte stage, IntPtr sampler, IntPtr texture);

//        /// <summary>
//        /// Create framebuffer from texture.
//        /// </summary>
//        /// <param name="texture">Texture handle.</param>
//        /// <returns>Framebuffer handle.</returns>
//        [DllImport("bgfx", CallingConvention = CallingConvention.Cdecl)]
//        public static extern IntPtr bgfx_create_frame_buffer(IntPtr texture);

//        /// <summary>
//        /// Set framebuffer for view.
//        /// </summary>
//        /// <param name="viewId">View ID.</param>
//        /// <param name="frameBuffer">Framebuffer handle.</param>
//        [DllImport("bgfx", CallingConvention = CallingConvention.Cdecl)]
//        public static extern void bgfx_set_view_frame_buffer(ushort viewId, IntPtr frameBuffer);

//    }
//}

// Duplicate BGFXNative class removed - see earlier definition in file
/// </summary>
/// <param name="viewId"></param>
/// <param name="flags"></param>
/// <param name="rgba"></param>
/// <param name="depth"></param>
/// <param name="stencil"></param>("[DIAG] BGFXNative.bgfx_reset - STUB"); }
///    public static void bgfx_set_view_clear(ushort viewId, ushort flags, uint rgba, float depth, byte stencil) { System.Diagnostics.Debug.WriteLine("[DIAG] BGFXNative.bgfx_set_view_clear - STUB"); }
///    public static void bgfx_submit(ushort viewId, IntPtr program, int depth, bool preserveState) { System.Diagnostics.Debug.WriteLine("[DIAG] BGFXNative.bgfx_submit - STUB"); }
/// }
/// }

/// FUTURE IMPLEMENTATION: entire BGFXNative.cs file commented out due to unresolved compilation issues *****************************
