// =====================================================================================================
//  FILE: UIButton.cs
//  PATH: Engine/UI/MainMenu/UIButton.cs
//  SUBSYSTEM: Main Menu UI — Interactive Button Component
//
//  ROLE:
//      Defines the minimal deterministic lifecycle contract for any engine-hosted program. Implemented
//      by GameRootMain to provide a clean, engine-facing boundary for initialization, execution, update,
//      render dispatch, ticking, and shutdown.
//
//  RESPONSIBILITIES:
//      - Provide a strict lifecycle surface: Initialize → Run → Update → Render → Shutdown.
//      - Allow engine hosts (e.g., GameRootMain) to expose deterministic lifecycle entry points.
//      - Serve as the base contract for any future top-level engine program modules.
//      - Support both GPU-context rendering and generic object-based render forwarding.
//
//  NON-RESPONSIBILITIES:
//      - Implementing update or render logic internally (delegated to subsystems).
//      - Managing system registration, asset loading, or state-machine orchestration.
//      - Handling GPU device creation, swap-chain management, or windowing.
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces legacy partial lifecycle methods.
//      - GameRootMain implements this interface and delegates lifecycle operations to:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
//      - Includes legacy compatibility signatures (Render(object), Tick(object,...)) for transitional
//        subsystem support, though the GPU-only pipeline uses Render(D3D11Adapter_Core).
// =====================================================================================================


using System;
using System.Drawing;
using SASZombieAssaultTD.Engine.UI.Elements;
using SASZombieAssaultTD.Engine.UI.Styles;

namespace SASZombieAssaultTD.Engine.UI.MainMenu
{
    public sealed class UIButton : UIElement
    {
        public string ButtonId { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public Action? OnClick { get; set; }
        public bool ButtonVisible { get; set; } = true;
        public float Opacity { get; set; } = 1f;

        public Components.PointF Position { get; set; } = new Components.PointF(0f, 0f);
        public SizeF Size { get; set; } = new SizeF(120f, 40f);

        /// <summary>
        /// MapMenu + MainMenu compatible style surface.
        /// </summary>
        public UIStyle Style { get; internal set; } = new UIStyle();

        public UIButton() { }

        internal UIButtonStyle GetStyle()
        {
            // Prefer the concrete UIButtonStyle if the current Style already is one.
            if (Style is UIButtonStyle buttonStyle)
            {
                return buttonStyle;
            }

            // If no style is set, return a new default UIButtonStyle.
            // This keeps callers safe from null and provides a reasonable fallback.
            return new UIButtonStyle();
        }
    }
}
