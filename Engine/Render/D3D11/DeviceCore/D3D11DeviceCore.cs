// =====================================================================================================
//  FILE: D3D11DeviceCore.cs
//  PATH: Engine/Render/D3D11/DeviceCore/D3D11DeviceCore.cs
//  SUBSYSTEM: DDCOps Hardware Core (Direct3D11 HAL)
//
//  ROLE:
//      Pure hardware abstraction for Direct3D 11 device creation, swap chain management,
//      backbuffer RTV creation, resizing, and texture/SRV creation.
//
//  RESPONSIBILITIES:
//      - Create and own ID3D11Device and ID3D11DeviceContext.
//      - Create and manage IDXGISwapChain1.
//      - Create and manage backbuffer RTV and depth-stencil targets.
//      - Provide deterministic resize behavior.
//      - Provide texture and SRV creation helpers.
//      - Expose hardware state to DGDOps pipeline manager.
//
//  NON-RESPONSIBILITIES:
//      - Pipeline binding (DGDOps_PipelineState).
//      - Rendering lifecycle (D3D11GraphicsDevice).
//      - UI/HUD rendering.
//      - Any draw calls.
// =====================================================================================================

using System;
using System.Numerics;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore
{
    public sealed class D3D11DeviceCore : IDisposable
    {
        // -------------------------------------------------------------------------------------------------
        //  PUBLIC HARDWARE STATE
        // -------------------------------------------------------------------------------------------------

        public ID3D11Device Device { get; private set; }
        public ID3D11DeviceContext Context { get; private set; }
        public IDXGISwapChain1 SwapChain { get; private set; }

        public ID3D11RenderTargetView BackbufferRtv { get; internal set; }
        public ID3D11Texture2D DepthStencilTexture { get; internal set; }
        public ID3D11DepthStencilView DepthStencilView { get; internal set; }

        public int Width { get; internal set; }
        public int Height { get; internal set; }

        private Vector2 _viewportSize;

        public Vector2 ViewportSize
        {
            get => _viewportSize;
            set
            {
                _viewportSize = value;
                Width = (int)value.X;
                Height = (int)value.Y;
            }
        }

        public bool IsInitialized { get; internal set; }
        public FeatureLevel FeatureLevel { get; internal set; }
        public DeviceCreationFlags DeviceFlags { get; internal set; }
        public int BackbufferWidth { get; internal set; }
        public int BackbufferHeight { get; internal set; }
        public int ScreenHeight { get; internal set; }
        public int ScreenWidth { get; internal set; }
        public D3D11RenderContext RenderContext { get; internal set; }

        private bool _disposed;
        private object adapter;
        private nint hwnd;
        private readonly bool _vsync;

        // -------------------------------------------------------------------------------------------------
        //  TRACE HELPER
        // -------------------------------------------------------------------------------------------------

        private void Trace(string msg)
        {
            System.Diagnostics.Debug.WriteLine("[D3D11DeviceCore] " + msg);
        }

        // -------------------------------------------------------------------------------------------------
        //  CONSTRUCTORS — VORTICE-ONLY, DETERMINISTIC
        // -------------------------------------------------------------------------------------------------

        public D3D11DeviceCore(
            IDXGIFactory2 factory,
            IntPtr windowHandle,
            int width,
            int height,
            bool vsync)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            if (windowHandle == IntPtr.Zero)
                throw new ArgumentException("Invalid window handle.", nameof(windowHandle));
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("Invalid swap chain dimensions.");

            Width = width;
            Height = height;
            ViewportSize = new Vector2(width, height);
            _vsync = vsync;

            Trace($"Ctor(factory): hwnd={windowHandle}, width={width}, height={height}, vsync={vsync}");

            CreateDeviceAndSwapChain(factory, windowHandle);
            CreateBackbufferTargets();
            IsInitialized = true;

            Trace("Ctor(factory): Initialization complete.");
        }

        public D3D11DeviceCore(
            IntPtr windowHandle,
            int width,
            int height,
            bool vsync)
        {
            if (windowHandle == IntPtr.Zero)
                throw new ArgumentException("Invalid window handle.", nameof(windowHandle));
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("Invalid swap chain dimensions.");

            Width = width;
            Height = height;
            ViewportSize = new Vector2(width, height);
            _vsync = vsync;

            Trace($"Ctor(factory-from-DXGI): hwnd={windowHandle}, width={width}, height={height}, vsync={vsync}");

            var factory = DXGI.CreateDXGIFactory2<IDXGIFactory2>(false);
            CreateDeviceAndSwapChain(factory, windowHandle);
            CreateBackbufferTargets();
            IsInitialized = true;

            Trace("Ctor(factory-from-DXGI): Initialization complete.");
        }

        public D3D11DeviceCore(nint hwnd)
        {
            this.hwnd = hwnd;
            Trace($"Ctor(hwnd-only): hwnd={hwnd}");
        }

        // -------------------------------------------------------------------------------------------------
        //  DEVICE + SWAP CHAIN CREATION (VORTICE)
        // -------------------------------------------------------------------------------------------------

        private void CreateDeviceAndSwapChain(IDXGIFactory2 factory, IntPtr windowHandle)
        {
            Trace($"CreateDeviceAndSwapChain: Begin. hwnd={windowHandle}, width={Width}, height={Height}");

            DeviceCreationFlags flags = DeviceCreationFlags.BgraSupport;
#if DEBUG
            flags |= DeviceCreationFlags.Debug;
#endif
            DeviceFlags = flags;

            Trace($"CreateDeviceAndSwapChain: DeviceFlags={flags}");

            D3D11CreateDevice(
                null,
                DriverType.Hardware,
                flags,
                null,
                out ID3D11Device device,
                out FeatureLevel featureLevel,
                out ID3D11DeviceContext context);

            Trace($"CreateDeviceAndSwapChain: D3D11CreateDevice returned. Device={(device != null)}, Context={(context != null)}, FeatureLevel={featureLevel}");

            if (device == null || context == null)
                throw new InvalidOperationException("D3D11DeviceCore: Device or context creation failed.");

            Device = device;
            Context = context;
            FeatureLevel = featureLevel;

            var swapDesc = new SwapChainDescription1
            {
                Width = (uint)Width,
                Height = (uint)Height,
                Format = Format.B8G8R8A8_UNorm,
                Stereo = false,
                SampleDescription = new SampleDescription(1, 0),
                BufferUsage = Usage.RenderTargetOutput,
                BufferCount = 2,
                Scaling = Scaling.Stretch,
                SwapEffect = SwapEffect.FlipDiscard,
                AlphaMode = AlphaMode.Ignore
            };

            Trace($"CreateDeviceAndSwapChain: Creating swap chain. Width={swapDesc.Width}, Height={swapDesc.Height}, Format={swapDesc.Format}, BufferCount={swapDesc.BufferCount}, SwapEffect={swapDesc.SwapEffect}");

            SwapChain = factory.CreateSwapChainForHwnd(Device, windowHandle, swapDesc);

            Trace($"CreateDeviceAndSwapChain: SwapChain created. SwapChain={(SwapChain != null)}");

            if (SwapChain == null)
                throw new InvalidOperationException("D3D11DeviceCore: Swap chain creation failed.");
        }

        private void D3D11CreateDevice(
            object value1,
            DriverType hardware,
            DeviceCreationFlags flags,
            object value2,
            out ID3D11Device device,
            out FeatureLevel featureLevel,
            out ID3D11DeviceContext context)
        {
            device = null;
            context = null;
            featureLevel = FeatureLevel.Level_11_0;

            var featureLevels = new[]
            {
                FeatureLevel.Level_11_1,
                FeatureLevel.Level_11_0,
                FeatureLevel.Level_10_1,
                FeatureLevel.Level_10_0
            };

            IDXGIAdapter dxgiAdapter = value1 as IDXGIAdapter;

            Trace($"D3D11CreateDevice(core): Adapter={(dxgiAdapter != null ? "IDXGIAdapter" : "null")}, Flags={flags}, Levels=[{string.Join(",", featureLevels)}]");

            Vortice.Direct3D11.D3D11.D3D11CreateDevice(
                dxgiAdapter,
                hardware,
                flags,
                featureLevels,
                out device,
                out featureLevel,
                out context);

            Trace($"D3D11CreateDevice(core): Result Device={(device != null)}, Context={(context != null)}, FeatureLevel={featureLevel}");
        }

        internal void InitializeDeviceAndSwapChain(nint hwnd)
        {
            Trace($"InitializeDeviceAndSwapChain: Begin. hwnd={hwnd}, IsInitialized={IsInitialized}, Width={Width}, Height={Height}");

            if (IsInitialized)
            {
                Trace("InitializeDeviceAndSwapChain: Already initialized. Skipping.");
                return;
            }

            if (hwnd == IntPtr.Zero)
                throw new ArgumentException("Invalid window handle.", nameof(hwnd));
            if (Width <= 0 || Height <= 0)
                throw new InvalidOperationException("Width/Height must be set before InitializeDeviceAndSwapChain.");

            var factory = DXGI.CreateDXGIFactory2<IDXGIFactory2>(false);
            CreateDeviceAndSwapChain(factory, hwnd);
            CreateBackbufferTargets();
            IsInitialized = true;

            Trace("InitializeDeviceAndSwapChain: Initialization complete.");
        }

        // -------------------------------------------------------------------------------------------------
        //  BACKBUFFER RTV + DEPTH-STENCIL TARGETS
        // -------------------------------------------------------------------------------------------------

        private void CreateBackbufferRtv()
        {
            Trace("CreateBackbufferRtv: Begin.");

            BackbufferRtv?.Dispose();

            using var backbuffer = SwapChain.GetBuffer<ID3D11Texture2D>(0);
            Trace($"CreateBackbufferRtv: Got backbuffer. Backbuffer={(backbuffer != null)}");

            BackbufferRtv = Device.CreateRenderTargetView(backbuffer);
            Trace($"CreateBackbufferRtv: RTV created. BackbufferRtv={(BackbufferRtv != null)}");
        }

        internal void CreateBackbufferTargets()
        {
            Trace("CreateBackbufferTargets: Begin.");

            if (SwapChain == null)
                throw new InvalidOperationException("SwapChain must be initialized before creating backbuffer targets.");

            BackbufferRtv?.Dispose();
            DepthStencilView?.Dispose();
            DepthStencilTexture?.Dispose();

            Trace("CreateBackbufferTargets: Old targets disposed.");

            CreateBackbufferRtv();

            DepthStencilTexture = CreateTexture(
                Width,
                Height,
                Format.D24_UNorm_S8_UInt,
                BindFlags.DepthStencil);

            Trace($"CreateBackbufferTargets: DepthStencilTexture created. Texture={(DepthStencilTexture != null)}");

            DepthStencilView = Device.CreateDepthStencilView(DepthStencilTexture);
            Trace($"CreateBackbufferTargets: DepthStencilView created. View={(DepthStencilView != null)}");

            ViewportSize = new Vector2(Width, Height);
            Trace($"CreateBackbufferTargets: ViewportSize set to {ViewportSize}.");

            Context.OMSetRenderTargets(BackbufferRtv, DepthStencilView);
            Trace("CreateBackbufferTargets: OMSetRenderTargets applied.");
        }

        // -------------------------------------------------------------------------------------------------
        //  RESIZE
        // -------------------------------------------------------------------------------------------------

        public void Resize(int width, int height)
        {
            Trace($"Resize: Requested width={width}, height={height}");

            if (width <= 0 || height <= 0)
            {
                Trace("Resize: Invalid dimensions. Ignoring.");
                return;
            }

            Width = width;
            Height = height;
            ViewportSize = new Vector2(width, height);

            Trace($"Resize: Updated Width={Width}, Height={Height}, ViewportSize={ViewportSize}");

            if (SwapChain == null || Device == null || Context == null)
            {
                Trace("Resize: SwapChain/Device/Context not ready. Skipping buffer resize.");
                return;
            }

            Context.ClearState();
            Trace("Resize: Context.ClearState called.");

            BackbufferRtv?.Dispose();
            DepthStencilView?.Dispose();
            DepthStencilTexture?.Dispose();

            Trace("Resize: Old targets disposed.");

            SwapChain.ResizeBuffers(
                2,
                (uint)width,
                (uint)height,
                Format.B8G8R8A8_UNorm,
                SwapChainFlags.None);

            Trace("Resize: SwapChain.ResizeBuffers completed.");

            CreateBackbufferTargets();
            Trace("Resize: Backbuffer targets recreated.");
        }

        // -------------------------------------------------------------------------------------------------
        //  TEXTURE CREATION
        // -------------------------------------------------------------------------------------------------

        public ID3D11Texture2D CreateTexture(int width, int height, Format format, BindFlags bindFlags)
        {
            Trace($"CreateTexture: width={width}, height={height}, format={format}, bindFlags={bindFlags}");

            var desc = new Texture2DDescription
            {
                Width = (uint)width,
                Height = (uint)height,
                MipLevels = 1,
                ArraySize = 1,
                Format = format,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = bindFlags,
                CPUAccessFlags = CpuAccessFlags.None,
                MiscFlags = ResourceOptionFlags.None
            };

            var texture = Device.CreateTexture2D(desc);
            Trace($"CreateTexture: Texture created. Texture={(texture != null)}");
            return texture;
        }

        public ID3D11ShaderResourceView CreateTextureView(ID3D11Texture2D texture)
        {
            Trace($"CreateTextureView: Texture={(texture != null)}");

            var srv = Device.CreateShaderResourceView(texture);
            Trace($"CreateTextureView: SRV created. SRV={(srv != null)}");
            return srv;
        }

        // -------------------------------------------------------------------------------------------------
        //  DISPOSE
        // -------------------------------------------------------------------------------------------------

        public void Dispose()
        {
            Trace($"Dispose: Begin. _disposed={_disposed}");

            if (_disposed)
            {
                Trace("Dispose: Already disposed. Skipping.");
                return;
            }

            DepthStencilView?.Dispose();
            Trace("Dispose: DepthStencilView disposed.");

            DepthStencilTexture?.Dispose();
            Trace("Dispose: DepthStencilTexture disposed.");

            BackbufferRtv?.Dispose();
            Trace("Dispose: BackbufferRtv disposed.");

            SwapChain?.Dispose();
            Trace("Dispose: SwapChain disposed.");

            Device?.Dispose();
            Trace("Dispose: Device disposed.");

            Context?.Dispose();
            Trace("Dispose: Context disposed.");

            _disposed = true;
            Trace("Dispose: Completed.");
        }
    }
}
