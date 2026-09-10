// ====================================================================================================
//  FILE: IGraphicsDevice.cs
//  PATH: Engine/Interfaces/IGraphicsDevice.cs
//  MODULE: Render Core – GPU Device Abstraction
//  SUBSYSTEMS: Interfaces
//  ROLE:
//      Deterministic, low‑level GPU device interface.
//      Owns the GPU context and executes RenderCommand sequences emitted by the Render subsystem.
//
//  RESPONSIBILITIES:
//      - Own and initialize the GPU device and immediate context.
//      - Create GPU-backed render targets and shader resources.
//      - Execute RenderCommand sequences in deterministic order.
//      - Upload CPU framebuffer data to the GPU when required.
//      - Present the final backbuffer to the display.
//      - Provide backend-specific helpers for D3D11 resource creation.
//
//  NON-RESPONSIBILITIES:
//      - Resource loading or caching (handled elsewhere).
//      - Gameplay logic, UI layout, or scene management.
//      - CPU-side rasterization (handled by FramebufferDrawing).
//      - Composite operations (handled by CompositePipeline).
//      - Diagnostics, logging, or performance metrics.
//
//  ARCHITECTURAL NOTES:
//      - All operations must be deterministic and side‑effect‑free beyond GPU state changes.
//      - RenderCommand and BackendCommand define the only legal GPU operations.
//      - Backend-specific helpers (D3D11) must remain isolated and explicit.
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Numerics;
using SASZombieAssaultTD.Engine.Graphics.Software;
using Vortice.Direct3D11;
using static SASZombieAssaultTD.Engine.Render.RenderEnums;

namespace SASZombieAssaultTD.Engine.Interfaces
{


    public sealed class RenderCommand
    {
        public RenderCommandType Type { get; }
        public object Material { get; set; }
        public object Element { get; set; }
        public Matrix3x2 Transform { get; }
        public uint SortKey { get; }

        public RenderCommand(
            RenderCommandType type,
            object material,
            object element,
            Matrix3x2 transform,
            uint sortKey)
        {
            Type = type;
            Material = material;
            Element = element;
            Transform = transform;
            SortKey = sortKey;
        }
    }

    public interface IGraphicsDevice : IDisposable
    {
        bool IsInitialized { get; }

        int Width { get; }
        int Height { get; }

        ID3D11RenderTargetView MainRenderTargetView { get; }

        void Initialize(IntPtr windowHandle, int width, int height);

        IRenderTarget CreateRenderTarget(string name, int width, int height, PixelFormat format);

        void ExecuteCommands(IEnumerable<RenderCommand> commands);

        void Present();

        void UploadFramebufferDrawing(FramebufferDrawing fb);

        void Resize(int width, int height);

        // Backend-specific helpers (D3D11)
        ID3D11BlendState GetOrCreateBlendState();
        ID3D11InputLayout GetOrCreateInputLayout(ID3D11VertexShader vs);
        ID3D11PixelShader GetOrCreatePixelShader();
        ID3D11SamplerState GetOrCreateSampler();
        ID3D11ShaderResourceView GetOrCreateShaderResourceView(ITexture2D texture);
        ID3D11VertexShader GetOrCreateVertexShader();

        ID3D11DeviceContext GetDeviceContext();

        void PresentFramebufferDrawing(FramebufferDrawing fb, ID3D11DeviceContext context);

    }
}
