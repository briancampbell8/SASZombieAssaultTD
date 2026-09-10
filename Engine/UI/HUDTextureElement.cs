// =====================================================================================================
//  FILE: HUDTextureElement.cs
//  PATH: Engine/UI/HUDTextureElement.cs
//  SUBSYSTEM: UI Subsystem / Core Components
//
//  ROLE:
//      Specialized texture layout control representing a physical image card within the HUD. 
//      Inherits directly from UIElement to support structural node hierarchies and render loop tracking.
//
//  RESPONSIBILITIES:
//      - Expose string paths, bounding rectangles, and layer ranks for serialization mapping.
//      - Track native abstract rendering texture object handles.
//      - Override the virtual Draw method to draw pixel layouts via the active presentation context.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.UI.Elements;

namespace SASZombieAssaultTD.Engine.UI
{
    /// <summary>
    /// Specialized graphic node mapping texture buffers into core engine render paths.
    /// </summary>
    public sealed class HUDTextureElement : UIElement
    {
        public string Path { get; set; } = string.Empty;
        public object? Texture { get; set; }

        public void Draw(D3D11Adapter_Core adapter_Core)
        {
            if (adapter_Core == null || !IsVisible)
                return;

            var absPos = AbsolutePosition;
            int finalX = (int)absPos.X;
            int finalY = (int)absPos.Y;

            // Optional structural binding call for textures if your D3D11Adapter_Core uses direct imagery parameters:
            adapter_Core.DrawTexture(
                Texture,
                finalX,
                finalY,
                (int)Size.Width,
                (int)Size.Height);

            base.Draw(adapter_Core);
        }
    }
}
