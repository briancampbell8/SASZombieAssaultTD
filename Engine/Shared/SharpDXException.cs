/*
//*File:    D3D11Presentation.cs
//* Path:    Engine / Rendering / D3D11 / D3D11Presentation.cs
//* Purpose: Swap chain presentation and frame management.
//*          Handles the final step of presenting rendered content to the display.
//*
//* Role:    -Flushes pending GPU commands
//* -Calls Present() on the swap chain
//*          - Handles presentation errors and recovery
//*          - Provides diagnostics for presentation debugging
//*
//* Notes:   This is the final step in the rendering pipeline.
//*          All rendering must be complete before calling Present().
//

 */

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Shared
{
    [Serializable]
    internal class SharpDXException : Exception
    {
        internal object ResultCode;

        public SharpDXException()
        {
        }

        public SharpDXException(string message) : base(message)
        {
        }

        public SharpDXException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
