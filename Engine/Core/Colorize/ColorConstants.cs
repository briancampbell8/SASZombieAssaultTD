//
//* File:    ColorConstants.cs
//* Purpose: Predefined color palette constants for engine-wide color usage.
//        Provides isolated color definitions to keep core struct clean and allow customization.

//Architecture:
//- Partial struct extension of core Color type
//- Predefined color palette constants for consistent theming
//- Isolated from core color logic for easy customization
//- Fundamental colors used as base values throughout engine
//- Organized by color categories for easy access

//Usage:
//   Color red = ColorConstants.Red;
//   Color transparent = ColorConstants.Transparent;
//Access predefined colors without creating new instances
//

//

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Core.Colorize
{
    public readonly partial struct Color
    {
        //---------------------------------------------------------
        //TRANSPARENT & MONOCHROME
        //---------------------------------------------------------
        //Fundamental colors used as base values throughout the engine.

        ///<summary>Transparent black (all components zero).</summary>
        public static readonly Color Transparent = new(0f, 0f, 0f, 0f);

        ///<summary>Opaque black (RGB 0,0,0).</summary>
        public static readonly Color Black = new(0f, 0f, 0f, 1f);

        ///<summary>Opaque white (RGB 1,1,1).</summary>
        public static readonly Color White = new(1f, 1f, 1f, 1f);

        //---------------------------------------------------------
        //PRIMARY COLORS (RGB)
        //---------------------------------------------------------
        //Pure primary colors at full intensity.

        ///<summary>Pure red (RGB 1,0,0).</summary>
        public static readonly Color Red = new(1f, 0f, 0f, 1f);

        ///<summary>Pure green (RGB 0,1,0).</summary>
        public static readonly Color Green = new(0f, 1f, 0f, 1f);

        ///<summary>Pure blue (RGB 0,0,1).</summary>
        public static readonly Color Blue = new(0f, 0f, 1f, 1f);

        //---------------------------------------------------------
        //SECONDARY COLORS (CMY)
        //---------------------------------------------------------
        //Mixtures of two primary colors at full intensity.

        ///<summary>Yellow (RGB 1,1,0) - Red + Green.</summary>
        public static readonly Color Yellow = new(1f, 1f, 0f, 1f);

        ///<summary>Cyan (RGB 0,1,1) - Green + Blue.</summary>
        public static readonly Color Cyan = new(0f, 1f, 1f, 1f);

        ///<summary>Magenta (RGB 1,0,1) - Red + Blue.</summary>
        public static readonly Color Magenta = new(1f, 0f, 1f, 1f);

        //---------------------------------------------------------
        //GRAYSCALE PALETTE
        //---------------------------------------------------------
        //Common gray values for UI backgrounds, disabled states, and shading.

        ///<summary>Medium gray (RGB 0.5,0.5,0.5).</summary>
        public static readonly Color Gray = new(0.5f, 0.5f, 0.5f, 1f);

        ///<summary>Dark gray (RGB 0.25,0.25,0.25).</summary>
        public static readonly Color DarkGray = new(0.25f, 0.25f, 0.25f, 1f);

        ///<summary>Light gray (RGB ~0.827,~0.827,~0.827).</summary>
        public static readonly Color LightGray = new(0.827f, 0.827f, 0.827f, 1f);

        ///<summary>Very dark gray / near black.</summary>
        public static readonly Color DimGray = new(0.41f, 0.41f, 0.41f, 1f);

        ///<summary>Silver gray (lighter than LightGray).</summary>
        public static readonly Color Silver = new(0.753f, 0.753f, 0.753f, 1f);

        //---------------------------------------------------------
        //COMMON UI COLORS
        //---------------------------------------------------------
        //Frequently used in HUD, buttons, and interface elements.

        ///<summary>Orange (RGB 1,0.5,0) - Warning, selection highlight.</summary>
        public static readonly Color Orange = new(1f, 0.5f, 0f, 1f);

        ///<summary>Gold (RGB 1,0.843,0) - Currency, premium items.</summary>
        public static readonly Color Gold = new(1f, 0.843f, 0f, 1f);

        ///<summary>Purple (RGB 0.5,0,1) - Special/rare items, magic.</summary>
        public static readonly Color Purple = new(0.5f, 0f, 1f, 1f);

        ///<summary>Brown (RGB 0.6,0.3,0.1) - Terrain, wood, earth tones.</summary>
        public static readonly Color Brown = new(0.6f, 0.3f, 0.1f, 1f);

        //---------------------------------------------------------
        //LIGHT PASTELS (UI Highlights)
        //---------------------------------------------------------
        //Softer variants for hover states and backgrounds.

        ///<summary>Light coral (pink-red) for soft warnings.</summary>
        public static readonly Color LightCoral = new(1f, 0.5f, 0.5f, 1f);

        ///<summary>Light green for success/positive feedback.</summary>
        public static readonly Color LightGreen = new(0.5f, 1f, 0.5f, 1f);

        ///<summary>Light blue for information/selection.</summary>
        public static readonly Color LightBlue = new(0.678f, 0.847f, 0.902f, 1f);

        ///<summary>Sky blue (brighter than LightBlue).</summary>
        public static readonly Color SkyBlue = new(0.53f, 0.81f, 0.92f, 1f);

        //---------------------------------------------------------
        //SEMANTIC GAME COLORS
        //---------------------------------------------------------
        //Named for specific game system usage.

        ///<summary>Health/damage color (same as Red).</summary>
        public static readonly Color Health = Red;

        ///<summary>Energy/mana color (same as Blue).</summary>
        public static readonly Color Energy = Blue;

        ///<summary>Experience/progress color (same as Green).</summary>
        public static readonly Color Experience = Green;

        ///<summary>Enemy target color (same as Orange).</summary>
        public static readonly Color Enemy = Orange;

        ///<summary>Ally/friendly color (same as Cyan).</summary>
        public static readonly Color Ally = Cyan;

        //---------------------------------------------------------
        //VALUE CORRECTNESS NOTES
        //---------------------------------------------------------
        //All values are pre-computed at compile time (readonly static).
        //No runtime initialization cost - values baked into metadata.
        //Range verification: All components are in [0.0, 1.0] valid range.
        //Memory layout: 16 bytes per color, stored in static data segment.
    }
}
