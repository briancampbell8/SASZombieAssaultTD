// ====================================================================================================
//  FILE: D3D11Adapter_Core.cs
//  PATH: Engine/TextureRendering/UI/D3D11Adapter_Core.cs
//  MODULE: TextureRendering
//
//  ROLE:
//      Adapt high-level UI drawing calls to the core Renderer using deterministic, engine-native types.
//      Provides unified drawing APIs for ModernUIRenderer and HUD/UI subsystems.
//
//  RESPONSIBILITIES:
//      - Draw rectangles, circles, lines, text, and sprites.
//      - Provide batching hooks.
//      - Expose viewport size for UI layout.
//      - Use engine-native Rect instead of System.Drawing.Rectangle.
//
//  NON-RESPONSIBILITIES:
//      - GPU resource management.
//      - Swap chain ownership.
//      - Game logic or state management.
// ====================================================================================================
using System;
using Vortice.Mathematics;
namespace SASZombieAssaultTD.Engine.TextureRendering.UI
{
    internal static class D3D11Adapter_CoreBaseHelpers
    {

        public static void FillRectangle(Rect rect, Color color)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            DrawFilledRectangle(rect.X, rect.Y, rect.Width, rect.Height, color._backingColor);
        }

        private static void DrawFilledRectangle(float x, float y, float width, float height, System.Drawing.Color backingColor)
        {
            throw new NotImplementedException();
        }
    }
}
