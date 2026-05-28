//
// * File:    IGraphicsDevice.cs
// * Path:    Engine/Rendering/IGraphicsDevice.cs
// * Purpose: Minimal, deterministic GPU abstraction for SAS Zombie Assault TD.
// *
// * Role:    - Defines the contract for GPU-backed rendering devices
// *          - Supports render target creation, command execution, presentation, and resize
// *
// * Notes:   This interface is intentionally minimal and audit-friendly.
// *          It is designed to support ModernUIRenderer and future GPU-backed systems
// *          without introducing unnecessary abstraction layers or complexity.
// //

using SASZombieAssaultTD.Engine.Diagnostics;

using SASZombieAssaultTD.Engine.UI.Rendering;
using System;
using System.Collections.Generic;
using System.Numerics;
using Vortice.Direct3D11;

namespace SASZombieAssaultTD.Engine.Rendering
{
    /// <summary>
    /// Available graphics rendering backends.
    /// </summary>
    public enum RendererBackend
    {
        D3D11,
        BGFX
    }

    /// <summary>
    /// Minimal GPU graphics device abstraction for SAS Zombie Assault TD.
    /// Provides deterministic, audit-friendly access to GPU rendering operations.
    /// </summary>
    /// <remarks>
    /// This interface is designed to:
    /// - Support ModernUIRenderer and other GPU-backed renderers
    /// - Keep the surface area small and explicit
    /// - Avoid hidden state, reflection, or dynamic behavior
    ///
    /// Implementations must be:
    /// - Single-owner of GPU device and context
    /// - Deterministic in command execution order
    /// - Explicit about resource creation and lifetime
    /// </remarks>
    public interface IGraphicsDevice
    {
        /// <summary>
        /// Gets a value indicating whether the graphics device has been fully initialized
        /// and is ready for rendering operations.
        /// </summary>
        bool IsInitialized { get; }

        int Width { get; }
        int Height { get; }

        /// <summary>
        /// Initializes the graphics device with the specified window and dimensions.
        /// </summary>
        /// <param name="windowHandle">Native window handle.</param>
        /// <param name="width">Initial width in pixels.</param>
        /// <param name="height">Initial height in pixels.</param>
        void Initialize(IntPtr windowHandle, int width, int height);

        /// <summary>
        /// Creates a GPU-backed render target with the specified name, dimensions, and pixel format.
        /// </summary>
        IRenderTarget CreateRenderTarget(string name, int width, int height, PixelFormat format);

        /// <summary>
        /// Executes a sequence of render commands on the GPU in deterministic order.
        /// </summary>
        void ExecuteCommands(IEnumerable<RenderCommand> commands);

        /// <summary>
        /// Presents the current backbuffer to the display.
        /// </summary>
        void Present();

        /// <summary>
        /// Uploads CPU framebuffer content to the GPU backbuffer.
        /// </summary>
        void UploadFramebuffer(Framebuffer fb);

        /// <summary>
        /// Resizes the underlying swap chain and any backbuffer-dependent resources.
        /// </summary>
        void Resize(int width, int height);

        // Backend command application
        void ApplySetRenderTarget(BackendCommand op);

        void ApplyClear(BackendCommand op);

        void ApplyDrawQuad(ID3D11DeviceContext context, BackendCommand op);

        void ApplyDrawText(BackendCommand op);

        void ApplySetScissor(BackendCommand op);

        // Shader / resource creation
        ID3D11BlendState GetOrCreateBlendState();

        ID3D11InputLayout GetOrCreateInputLayout(ID3D11VertexShader vs);

        ID3D11PixelShader GetOrCreatePixelShader();

        ID3D11SamplerState GetOrCreateSampler();

        ID3D11ShaderResourceView GetOrCreateShaderResourceView(ITexture2D texture);

        ID3D11VertexShader GetOrCreateVertexShader();

        // Legacy compatibility
        ID3D11DeviceContext Get_context1();

        void PresentFramebuffer(Framebuffer fb, ID3D11DeviceContext context);

        // Cleanup
        void Dispose();
        //object CreateRenderTarget(object x, object y, object color);
        //object CreateRenderTarget(object width, object height, object color);

        ID3D11RenderTargetView CreateRenderTarget(object x, object y, object color);
    }

    public class RenderCommand
    {
        internal object Material;
        internal object Element;
        internal int Type;
        internal Matrix3x2 Transform;
        internal uint SortKey;
    }
}
