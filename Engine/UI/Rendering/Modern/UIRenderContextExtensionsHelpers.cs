using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
//=====================================================================================
// FILE: ModernUIRenderer_Core.cs
// PATH: Engine/UI/Rendering/Modern/ModernUIRenderer_Core.cs
// SUBSYSTEM: Modern UI Rendering – Core Subsystem
// ROLE: Core wiring, hierarchy tree execution, and lifecycle management.
//=====================================================================================

namespace SASZombieAssaultTD.Engine.UI.Rendering.Modern
{
    internal static class D3D11Adapter_CoreExtensionsHelpers
    {

        public static void ClearScreen(this D3D11Adapter_Core context)
        {
            if (context is D3D11Adapter_Core renderCtx) // screen should not be cleared in headless mode
            {
                renderCtx.ClearScreen();
            }
        }


        public static void Reset(this D3D11Adapter_Core context)
        {
            context.Reset();
        }
    }
}
