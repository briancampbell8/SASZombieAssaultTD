/*
File:    RenderContextD3D11Adapter.cs
Folder:  Engine/Systems/
Purpose: Minimal adapter so RenderManager + StateMachine can render safely.
*/

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Systems
{
    public class RenderContextD3D11Adapter
    {
        public RenderContextD3D11Adapter()
        {
            DebugLogger.Log("RenderContext",
                "RenderContextD3D11Adapter created");
        }
    }
}
