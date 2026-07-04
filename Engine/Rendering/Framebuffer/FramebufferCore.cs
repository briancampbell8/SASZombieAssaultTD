//File:    FramebufferCore.cs
//Purpose: Core memory and pixel infrastructure for the Framebuffer.
//         Contains additional framebuffer methods and core functionality.
//Author:   BDC
//Created: 2026-02-10
//Dependencies: VectorMath, Core
//Thread Safety: Instance members require external synchronization for thread safety.
//Notes:
//- Partial class extending Framebuffer functionality.
//- Contains core memory and pixel management methods.
//- Works with other Framebuffer partial classes for complete implementation.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

//
using SASZombieAssaultTD.Engine.VectorMath;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering
{
    ///<summary>
    ///Main Framebuffer class containing pixel array and core properties.
    ///</summary>
    public partial class Framebuffer
    {
        private uint[] _pixels;

        private bool _hasRenderedFirstFrame = false;

        private int i;

        private void ClearTransparent()
        {
            Array.Fill(_pixels, 0u);
        }

        public void ClearRaw(int x, int y, int width, int height, Color color, bool filled)
        {
            for (int i = 0; i < _pixels.Length; i += 4)
            {
                _pixels[i + 0] = 0;
                _pixels[i + 1] = 0;
                _pixels[i + 2] = 0;
                _pixels[i + 3] = 0;
            }
        }

    }
}
