// =====================================================================================================
//  FILE: UIButtonStyle.cs
//  PATH: Engine/UI/MainMenu/UIButtonStyle.cs
//  SUBSYSTEM: UI Framework — Main Menu Button Style
//
//  ROLE:
//      Provides deterministic, engine‑friendly styling metadata for UIButton elements used throughout
//      the Main Menu and other UI subsystems. Encapsulates all visual style properties required for
//      rendering, transitions, and interaction feedback.
//
//  RESPONSIBILITIES:
//      - Store stable color values for background, pressed, and active states.
//      - Store shadow offset for deterministic rendering.
//      - Provide a clean, strongly‑typed style surface for UIButton and UIRenderer.
//      - Support transition controllers (MainM_Transitions) without requiring casting or reflection.
//
//  NON‑RESPONSIBILITIES:
//      - Does NOT perform rendering.
//      - Does NOT manage transitions or animations.
//      - Does NOT handle input or UI events.
//      - Does NOT participate in engine lifecycle or subsystem registration.
//
//  ARCHITECTURE NOTES:
//      - Replaces legacy “object Style” fields.
//      - Ensures deterministic Option‑B UI behavior.
//      - Fully compatible with UIRenderer, UIState, and UIButton.
// =====================================================================================================
using SASZombieAssaultTD.Engine.UI.Styles;

namespace SASZombieAssaultTD.Engine.UI.MainMenu
{
    public sealed class UIButtonStyle : UIStyle
    {
        private Color pressedColor = System.Drawing.Color.FromArgb(64, 64, 64);
        private Color activeColor = System.Drawing.Color.FromArgb(48, 48, 48);

        /// <summary>
        /// The color used when the button is in its normal background state.
        /// </summary>
        public Color BackgroundColor { get; set; } = System.Drawing.Color.FromArgb(32, 32, 32);

        /// <summary>
        /// The color used when the button is actively pressed.
        /// </summary>
        public Color PressedColor
        {
            get { return pressedColor; }
            set { pressedColor = value; }
        }

        /// <summary>
        /// The color used when the button is in its active/selected state.
        /// </summary>
        public Color ActiveColor
        {
            get { return activeColor; }
            set { activeColor = value; }
        }

        /// <summary>
        /// Pixel offset used to render the button’s shadow.
        /// </summary>
        public float ShadowOffset { get; set; } = 2f;

        public UIButtonStyle() { }

        public UIButtonStyle(Color background, Color pressed, Color active, float shadowOffset)
        {
            BackgroundColor = background;
            PressedColor = pressed;
            ActiveColor = active;
            ShadowOffset = shadowOffset;
        }
    }
}
