// ====================================================================================================
//  FILE: D3D11DeviceCore.cs
//  PATH: Engine/Rendering/ 
//  PROGRAM: D3D11DeviceCore.cs
//  MODULE: Resource Management Framework
//  ROLE:
//      Defines the structures, loaders, and integration points responsible for discovering, validating, and providing engine resources in a deterministic manner.
//
//  RESPONSIBILITIES:
//      - Provide a unified API for loading, caching, and resolving engine resources.
//      - Enforce deterministic resource lookup and lifecycle rules.
//      - Abstract file formats, storage locations, and integration layers behind a stable interface.
//      - Ensure resource availability for all engine subsystems (Rendering, Audio, Gameplay, UI).
//
//  NON-RESPONSIBILITIES:
//      - Performing rendering or GPU upload operations.
//      - Managing gameplay logic or scene entities.
//      - Handling diagnostics, logging, or performance metrics.
//      - Encoding or authoring resource files.
//
//  ARCHITECTURAL NOTES:
//      - The Resource Management Framework acts as the central authority for all asset retrieval.
//      - Resource modules must remain pure: no side effects outside resource acquisition and validation.
//      - All resource types (textures, data files, definitions, metadata) must follow deterministic load rules.
//  ====================================================================================================

//
using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SharpGen.Runtime;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using static Vortice.DXGI.DXGI;
namespace SASZombieAssaultTD.Engine.Rendering.D3D11
{
    public sealed partial class D3D11DeviceCore : IDisposable
    {
        private static readonly byte[] QuadVS_Bytecode = new byte[]
        {
            0x44,0x58,0x42,0x43,0x8F,0xE3,0xA1,0xC4,0xC1,0x7E,0xC2,0xE9,0xF8,0xA1,0xC3,0xE4,
            0x7C,0xE7,0xF7,0x0B,0x01,0x00,0x00,0x00,0xC0,0x04,0x00,0x00,0x07,0x00,0x00,0x00,
            0x3C,0x00,0x00,0x00,0x5C,0x00,0x00,0x00,0xBC,0x00,0x00,0x00,0x20,0x01,0x00,0x00,
            0xE0,0x01,0x00,0x00,0xF4,0x03,0x00,0x00,0x52,0x44,0x45,0x46,0xFC,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x1C,0x00,0x00,0x00,
            0x00,0x05,0xFE,0xFF,0x00,0x01,0x00,0x00,0x5C,0x00,0x00,0x00,0x52,0x44,0x45,0x46,
            0x2C,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x1C,0x00,0x00,0x00,0x00,0x05,0xFE,0xFF,0x00,0x01,0x00,0x00
        };

        private static readonly byte[] QuadPS_Bytecode = new byte[]
        {
            0x44,0x58,0x42,0x43,0xA1,0xB2,0xC4,0xD3,0xE4,0xF5,0xA6,0xB7,0xC8,0xD9,0xEA,0xFB,
            0x01,0x00,0x00,0x00,0xA0,0x03,0x00,0x00,0x07,0x00,0x00,0x00,0x3C,0x00,0x00,0x00,
            0x5C,0x00,0x00,0x00,0xBC,0x00,0x00,0x00,0x20,0x01,0x00,0x00,0xC0,0x01,0x00,0x00,
            0xF4,0x02,0x00,0x00,0x52,0x44,0x45,0x46,0xFC,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x1C,0x00,0x00,0x00,0x00,0x05,0xFE,0xFF,
            0x00,0x01,0x00,0x00,0x5C,0x00,0x00,0x00,0x52,0x44,0x45,0x46,0x2C,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x1C,0x00,0x00,0x00,
            0x00,0x05,0xFE,0xFF,0x00,0x01,0x00,0x00
        };

        public ID3D11Device Device { get; }
        public ID3D11DeviceContext ImmediateContext { get; }
        public IDXGISwapChain1 SwapChain { get; }
        public bool VSyncEnabled { get; }

        private int _width;
        private int _height;
        private bool _disposed;

        public D3D11DeviceCore(IntPtr windowHandle, int width, int height, bool vsync)
        {
            _width = width;
            _height = height;
            VSyncEnabled = vsync;

            DLogger.Log(
                LogSubsystems.D3D11,
                LogLevel.Debug,
                "D3D11DeviceCore: initialization started.");

            Result factoryResult = CreateDXGIFactory1(out IDXGIFactory2 factory);
            if (factoryResult.Failure || factory is null)
            {
                DLogger.Log(
                    LogSubsystems.D3D11,
                    LogLevel.Error,
                    $"DXGI factory creation failed: {factoryResult.Code}");
                throw new InvalidOperationException("DXGI factory creation failed: " + factoryResult.Code);
            }

            try
            {
                FeatureLevel[] featureLevels =
                {
                    FeatureLevel.Level_11_1,
                    FeatureLevel.Level_11_0,
                    FeatureLevel.Level_10_1,
                    FeatureLevel.Level_10_0
                };

                DLogger.Log(
                    LogSubsystems.D3D11,
                    LogLevel.Debug,
                    "D3D11DeviceCore: creating D3D11 device.");

                Result deviceResult = D3D11CreateDevice(
                    DriverType.Hardware,
                    DeviceCreationFlags.BgraSupport,
                    featureLevels,
                    out ID3D11Device device,
                    out ID3D11DeviceContext context
                );

                if (deviceResult.Failure)
                {
                    DLogger.Log(
                        LogSubsystems.D3D11,
                        LogLevel.Error,
                        $"Failed to create D3D11 device: {deviceResult.Code}");
                    throw new InvalidOperationException(
                        $"Failed to create D3D11 device: {deviceResult.Code}"
                    );
                }

                Device = device;
                ImmediateContext = context;

                DLogger.Log(
                    LogSubsystems.D3D11,
                    LogLevel.Info,
                    "D3D11DeviceCore: D3D11 device and immediate context created.");

                var swapDesc = new SwapChainDescription1
                {
                    Width = (uint)_width,
                    Height = (uint)_height,
                    Format = Format.B8G8R8A8_UNorm,
                    Stereo = false,
                    SampleDescription = new SampleDescription(1, 0),
                    BufferUsage = Usage.RenderTargetOutput,
                    BufferCount = 2,
                    Scaling = Scaling.Stretch,
                    SwapEffect = SwapEffect.Discard,
                    AlphaMode = AlphaMode.Ignore,
                    Flags = SwapChainFlags.None
                };

                var fullscreenDescription = new SwapChainFullscreenDescription
                {
                    Windowed = true,
                    RefreshRate = new Rational(60, 1)
                };

                DLogger.Log(
                    LogSubsystems.D3D11,
                    LogLevel.Debug,
                    "D3D11DeviceCore: creating swap chain for HWND.");

                SwapChain = factory
                    .CreateSwapChainForHwnd((IUnknown)Device, windowHandle, swapDesc, fullscreenDescription)
                    .As<IDXGISwapChain1>();

                DLogger.Log(
                    LogSubsystems.D3D11,
                    LogLevel.Info,
                    "D3D11DeviceCore: swap chain created successfully.");

                InitializeRenderTargets(_width, _height);
                InitializeSampler();
                InitializeFullscreenQuadPipeline();
            }
            finally
            {
                factory.Dispose();
                DLogger.Log(
                    LogSubsystems.D3D11,
                    LogLevel.Debug,
                    "D3D11DeviceCore: DXGI factory disposed.");
            }
        }

        private static Result D3D11CreateDevice(
            DriverType driverType,
            DeviceCreationFlags creationFlags,
            FeatureLevel[] featureLevels,
            out ID3D11Device device,
            out ID3D11DeviceContext context)
        {
            return Vortice.Direct3D11.D3D11.D3D11CreateDevice(
                null,              //default adapter
                driverType,
                creationFlags,
                featureLevels,
                out device,
                out context
            );
        }

        public void Resize(int width, int height)
        {
            if (width <= 0 || height <= 0)
                return;

            if (width == _width && height == _height)
                return;

            _width = width;
            _height = height;

            DLogger.Log(
                LogSubsystems.D3D11,
                LogLevel.Debug,
                $"D3D11DeviceCore: resizing swap chain to {_width}x{_height}.");

            SwapChain.ResizeBuffers(
                2,
                (uint)width,
                (uint)height,
                Format.B8G8R8A8_UNorm,
                SwapChainFlags.None
            );

            InitializeRenderTargets(_width, _height);
        }

        public void Present()
        {
            uint syncInterval = VSyncEnabled ? 1u : 0u;
            SwapChain.Present(syncInterval, PresentFlags.None);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            DLogger.Log(
                LogSubsystems.D3D11,
                LogLevel.Debug,
                "D3D11DeviceCore: disposing resources.");

            DisposeRtvResources();
            DisposeSampler();
            DisposeQuadResources();

            SwapChain?.Dispose();
            ImmediateContext?.Dispose();
            Device?.Dispose();

            DLogger.Log(
                LogSubsystems.D3D11,
                LogLevel.Info,
                "D3D11DeviceCore: disposed.");
        }
    }
}
