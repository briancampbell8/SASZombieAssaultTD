//
//* File:    BGFXFramebuffer.cs
//* Path:    Engine/Rendering/BGFX/BGFXFramebuffer.cs
//* Purpose: BGFX framebuffer scaffolding - placeholder fields for future BGFX framebuffer.
////

//

using System.Drawing.Imaging;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering.BGFX
{
    ///<summary>
    ///BGFX framebuffer scaffolding - placeholder fields for future BGFX framebuffer.
    ///This class contains only structural fields for BGFX framebuffer management.
    ///</summary>
    internal class BGFXFramebuffer : IRenderTarget
    {
        ///<summary>
        ///Framebuffer width in pixels.
        ///</summary>
        public int Width { get; set; }

        ///<summary>
        ///Framebuffer height in pixels.
        ///</summary>
        public int Height { get; set; }

        ///<summary>
        ///Gets the logical name of the render target.
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        ///Pixel format of the framebuffer.
        ///</summary>
        public PixelFormat Format;

        ///<summary>
        ///Placeholder for BGFX framebuffer handle.
        ///</summary>
        public object Handle;

        ///<summary>
        ///Placeholder for BGFX texture handle.
        ///</summary>
        public object TextureHandle;

        ///<summary>
        ///True if this is the default backbuffer framebuffer.
        ///</summary>
        public bool IsDefault;

        public void Dispose()
        {
            System.Diagnostics.Debug.WriteLine("[DIAG] BGFXFramebuffer.Dispose - STUB PROCESSED");
            //TODO BGFX: Dispose framebuffer resources once BGFX bindings are available.
            //This method intentionally left blank.
        }
    }
}
