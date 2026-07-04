//
//* File:    BGFXFramebufferUploader.cs
//* Path:    Engine/Rendering/BGFX/BGFXFramebufferUploader.cs
//* Purpose: CPU framebuffer to BGFX texture upload.
//*          Parallel implementation to BGFXFramebufferUploader.
////
using SASZombieAssaultTD.Engine.Diagnostics;

using System;

namespace SASZombieAssaultTD.Engine.Rendering.BGFX
{
    ///<summary>
    ///BGFX CPU framebuffer upload handler.
    ///Uploads CPU-rendered framebuffer to GPU via BGFX texture.
    ///</summary>
    internal sealed class BGFXFramebufferUploader : IDisposable
    {
        ///<summary>
        ///   private BGFXDeviceCore _core; needed for future BGFX texture management and upload operations.
        ///</summary>
        ///<param name="core"></param>

        //FUTURE IMPLEMENTATION: exception handling removed during BGFX bypass *************************************
        //public BGFXFramebufferUploader(BGFXDeviceCore core)
        //{
        //    _core = core ?? throw new ArgumentNullException(nameof(core));
        //}
        //FUTURE IMPLEMENTATION: exception handling removed during BGFX bypass *************************************

        ///  public BGFXFramebufferUploader(BGFXDeviceCore core)
        ///  {
        ///      _core = core; //Null check bypassed for BGFX stub implementation
        ///   }

        //FUTURE IMPLEMENTATION: exception handling removed during BGFX bypass *************************************
        //public void Upload(Framebuffer fb)
        //{
        //    if (fb == null) throw new ArgumentNullException(nameof(fb));
        //    if (fb.Pixels == null) throw new ArgumentException("Framebuffer pixel array cannot be null", nameof(fb));
        //
        //    //BGFX texture upload placeholder
        //    System.Diagnostics.Debug.WriteLine("[BGFX] BGFXFramebufferUploader.Upload() - {0}x{1}", fb.Width, fb.Height);
        //}
        //FUTURE IMPLEMENTATION: exception handling removed during BGFX bypass *************************************

        public void Upload(Framebuffer fb)
        {
            //BGFX texture upload placeholder
            System.Diagnostics.Debug.WriteLine("[BGFX] BGFXFramebufferUploader.Upload() - {0}x{1}", fb?.Width ?? 0, fb?.Height ?? 0);
        }

        public void Dispose()
        {
            System.Diagnostics.Debug.WriteLine("[BGFX] BGFXFramebufferUploader.Dispose()");
        }
    }
}
