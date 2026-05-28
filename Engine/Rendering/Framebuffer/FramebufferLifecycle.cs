//
// * File:    FramebufferLifecycle.cs
// * Path:    Engine/Rendering/Framebuffer/FramebufferLifecycle.cs
// * Purpose: Lifecycle management for Framebuffer operations.
// *          Contains methods for batch rendering and presentation.
// //
using SASZombieAssaultTD.Engine.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing; // Re-added to support Color hooks smoothly

namespace SASZombieAssaultTD.Engine.Rendering
{
    public partial class Framebuffer
    {
        // ---------------------------------------------------------
        // LIFECYCLE METHODS ONLY
        // ---------------------------------------------------------

        /// <summary>
        /// Gets or sets whether first frame has been rendered.
        /// Used to prevent capturing framebuffer before game is ready.
        /// </summary>
        public bool HasRenderedFirstFrame
        {
            get => _hasRenderedFirstFrame;
            set => _hasRenderedFirstFrame = value;
        }
    }
}
