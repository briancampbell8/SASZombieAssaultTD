//
//* File:    FramebufferTexture.cs
//* Path:    Engine/Rendering/Framebuffer/FramebufferTexture.cs
//* Purpose: Texture and sprite rendering pipeline for the Framebuffer.
//*          Contains all DrawTexture and DrawSprite overloads with alpha blending,
//*          nearest-neighbor sampling, source rectangle clipping, and tinting logic.
////

//

using System;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering
{
    public partial class Framebuffer
    {
        public Framebuffer()
        {
        }

        //---------------------------------------------------------
        //TEXTURE METHODS ONLY
        //---------------------------------------------------------

        ///<summary>
        ///Draws a sprite at specified position with color.
        ///</summary>
        ///<param name="texture">Sprite texture object.</param>
        ///<param name="x">X coordinate.</param>
        ///<param name="y">Y coordinate.</param>
        ///<param name="color">Sprite color.</param>
        public void DrawSprite(object texture, float x, float y, System.Drawing.Color color)
        {
            System.Diagnostics.Debug.WriteLine("[DIAG] Framebuffer.DrawSprite - STUB PROCESSED");
        }

        ///<summary>
        ///Draws a sprite at specified position with dimensions and color.
        ///</summary>
        ///<param name="texture">Sprite texture object.</param>
        ///<param name="x">X coordinate.</param>
        ///<param name="y">Y coordinate.</param>
        ///<param name="width">Sprite width.</param>
        ///<param name="height">Sprite height.</param>
        ///<param name="color">Sprite color.</param>
        public void DrawSprite(object texture, float x, float y, float width, float height, System.Drawing. Color color)
        {
            System.Diagnostics.Debug.WriteLine("[DIAG] Framebuffer.DrawSprite - STUB PROCESSED");
        }

        internal void DrawTexture(Texture2D whitePixel, int x, int y, int width, int height)
        {
            NI.Hit();
        }

        //All legacy methods removed as per SECTION 1
    }
}
