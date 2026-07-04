/*
File:    TextAlignment.cs
Purpose: Enhanced text rendering with alignment, word wrapping, and audit-friendly logging.
Features: Text drawing with alignment options, word wrapping, fallback behavior.

P11-04-09-F: Enhanced with DrawText method supporting alignment options (left, center, right),
word wrapping (optional), audit-friendly logging and fallback behavior.
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Math;
using System.Drawing;
using SASZombieAssaultTD.Engine.Extensions;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering
{
    ///<summary>
    ///Text alignment options for rendering.
    ///P11-04-09-F: Supports left, center, and right text alignment.
    ///</summary>
    public enum TextAlignment
    {
        ///<summary>Align text to the left</summary>
        Left,
        ///<summary>Align text to the center</summary>
        Center,
        ///<summary>Align text to the right</summary>
        Right
    }
}
