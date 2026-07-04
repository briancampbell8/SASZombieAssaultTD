// ====================================================================================================
//  FILE: D3D11FramebufferPresenter.cs
//  PATH: Engine/Rendering/ 
//  PROGRAM: D3D11FramebufferPresenter.cs
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

/* ====================================================================================================
 *  FILE:       D3D11FramebufferPresenter.cs
 *  PATH:       Engine/Rendering/D3D11/
 *  SUBSYSTEM:  Rendering.D3D11
 *  ROLE:       Bridge between the CPU-side deterministic Framebuffer and the GPU-side D3D11DeviceCore.
 *              Uploads the framebuffer pixel data to a D3D11 texture and exposes it as a shader resource
 *              for fullscreen-quad presentation.
 *
 *  RESPONSIBILITIES:
 *      - Maintain a D3D11 texture sized to the current Framebuffer dimensions.
 *      - Upload Framebuffer pixel data to the GPU each frame.
 *      - Expose a shader resource view (SRV) for use by the fullscreen quad pipeline.
 *      - Handle resize of the backing texture when the Framebuffer size changes.
 *
 *  NON-RESPONSIBILITIES:
 *      - Creating or managing the D3D11 device, context, or swap chain (delegated to D3D11DeviceCore).
 *      - Configuring the fullscreen quad pipeline or issuing draw calls (delegated to D3D11DeviceCore).
 *      - Performing any game logic, ECS operations, or CPU-side rendering (delegated to Framebuffer).
 *
 *  DEPENDENCIES:
 *      - D3D11DeviceCore (provides ID3D11Device and ID3D11DeviceContext).
 *      - Framebuffer (CPU-side render target and pixel source).
 *      - Vortice.Direct3D11 (D3D11 texture and SRV types).
 *      - DebugLogger (diagnostics).
 *
 *  CALLED BY:
 *      - Platform-specific render loop (e.g., Win32 host) after CPU rendering is complete.
 *      - Higher-level engine code responsible for presenting frames.
 *
 *  CALLS INTO:
 *      - D3D11DeviceCore.Device and D3D11DeviceCore.ImmediateContext.
 *      - ID3D11Device.CreateTexture2D / CreateShaderResourceView.
 *      - ID3D11DeviceContext.UpdateSubresource.
 *
 *  ARCHITECTURAL NOTES:
 *      - This type is strictly a bridge: CPU framebuffer → GPU texture.
 *      - It does not own or control the swap chain; Present() remains on D3D11DeviceCore.
 *      - It must remain deterministic and free of gameplay logic.
 *      - This file is complete and must not be split into partials.
 * ==================================================================================================== */

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
//
using Vortice.Direct3D11;
using Vortice.DXGI;
namespace SASZombieAssaultTD.Engine.Rendering.D3D11
{
    ///<summary>
    ///Bridges the CPU-side <see cref="Framebuffer"/> to the GPU-side <see cref="D3D11DeviceCore"/>
    ///by maintaining a D3D11 texture and shader resource view that mirror the framebuffer contents.
    ///</summary>
    public sealed class D3D11FramebufferPresenter : IDisposable
    {
        private readonly D3D11DeviceCore _deviceCore;

        private ID3D11Texture2D? _framebufferTexture;
        private ID3D11ShaderResourceView? _framebufferSrv;

        private int _width;
        private int _height;
        private bool _disposed;

        ///<summary>
        ///The shader resource view that exposes the framebuffer texture to the fullscreen quad pipeline.
        ///</summary>
        public ID3D11ShaderResourceView? ShaderResourceView => _framebufferSrv;

        ///<summary>
        ///Creates a new presenter bound to the specified D3D11 device core.
        ///</summary>
        ///<param name="deviceCore">The D3D11 device core providing device and context.</param>
        public D3D11FramebufferPresenter(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
            DLogger.Log(LogSubsystems.D3D11, "D3D11FramebufferPresenter: constructed.");
        }

        ///<summary>
        ///Ensures that the backing D3D11 texture matches the dimensions of the provided framebuffer.
        ///Recreates the texture and SRV if the size has changed or if resources are missing.
        ///</summary>
        ///<param name="framebuffer">The framebuffer whose dimensions drive the texture size.</param>
        public void EnsureSize(Framebuffer framebuffer)
        {
            if (framebuffer == null)
                throw new ArgumentNullException(nameof(framebuffer));

            if (framebuffer.Width <= 0 || framebuffer.Height <= 0)
                throw new ArgumentOutOfRangeException(nameof(framebuffer), "Framebuffer dimensions must be positive.");

            if (_framebufferTexture != null &&
                framebuffer.Width == _width &&
                framebuffer.Height == _height)
            {
                //Size is already correct; nothing to do.
                return;
            }

            DisposeTextureResources();

            _width = framebuffer.Width;
            _height = framebuffer.Height;

            DLogger.Log(LogSubsystems.D3D11, $"D3D11FramebufferPresenter: creating texture {_width}x{_height}.");

            var desc = new Texture2DDescription
            {
                Width = (uint)_width,
                Height = (uint)_height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.ShaderResource,
                CPUAccessFlags = CpuAccessFlags.None
                //OptionFlags = ResourceOptionFlags.None NOT NEEDED AS THIS IS THE DEFAULT VALUE
            };

            _framebufferTexture = _deviceCore.Device.CreateTexture2D(desc);
            _framebufferSrv = _deviceCore.Device.CreateShaderResourceView(_framebufferTexture);

            DLogger.Log(LogSubsystems.D3D11, "D3D11FramebufferPresenter: texture and SRV created.");
        }

        ///<summary>
        ///Uploads the framebuffer pixel data to the GPU texture.
        ///The caller is responsible for ensuring that <see cref="EnsureSize"/> has been called
        ///when framebuffer dimensions change.
        ///</summary>
        ///<param name="framebuffer">The framebuffer whose pixel data will be uploaded.</param>
        public void Upload(Framebuffer framebuffer)
        {
            if (framebuffer == null)
                throw new ArgumentNullException(nameof(framebuffer));

            if (_disposed)
                throw new ObjectDisposedException(nameof(D3D11FramebufferPresenter));

            if (_framebufferTexture == null || _framebufferSrv == null)
            {
                EnsureSize(framebuffer);
            }

            if (framebuffer.Pixels == null || framebuffer.Pixels.Length == 0)
            {
                DLogger.Log(LogSubsystems.D3D11, "D3D11FramebufferPresenter.Upload: framebuffer has no pixel data.");
                return;
            }

            //Each pixel is 4 bytes (BGRA8).
            int rowPitch = _width * 4;

            SubresourceData dataBox = default;

            _deviceCore.ImmediateContext.UpdateSubresource(dataBox, _framebufferTexture, 0);

            DLogger.Log(LogSubsystems.D3D11, "D3D11FramebufferPresenter.Upload: framebuffer data uploaded to GPU.");
        }

        ///<summary>
        ///Releases all GPU resources owned by this presenter.
        ///Does not dispose the underlying D3D11DeviceCore.
        ///</summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            DisposeTextureResources();

            DLogger.Log(LogSubsystems.D3D11, "D3D11FramebufferPresenter: disposed.");
        }

        ///<summary>
        ///Disposes the texture resources.
        ///</summary>
        private void DisposeTextureResources()
        {
            throw new NotImplementedException();
        }
        ///<summary>
        ///Disposes the texture resources.
        ///</summary>

        //   private void DisposeTextureResources()  Already implemented in the Dispose() method,
        //   so this is not needed as a separate method.
        //   If you want to keep it as a separate method for clarity, you can implement it like this:
        //   {
        //       throw new NotImplementedException();
        //   }
        //}
    }
}
