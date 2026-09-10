// =====================================================================================================
//  FILE: TextEnums.cs
//  PATH: Engine/Render/Text/TextEnums.cs
//  SUBSYSTEM: Render Text
//
//  ROLE:
//      Defines All enumerations used by the Render Text Pipeline
//
//  RESPONSIBILITIES:
//      - Provide a strict, minimal lifecycle surface for program orchestration.
//      - Enforce the structural sequencing contract: Initialize → Run Loop Execution → Shutdown.
//      - Serve as the base contract for any future top-level engine-hosted program modules.
//
//  NON-RESPONSIBILITIES:
//      - Implementing deep frame-level update calculation rules or rendering commands directly.
//      - Managing active systems registration pools, engine assets, or game states.
//      - Handling discrete hardware device allocation boundaries.
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces the legacy GameRoot partial lifecycle methods.
//      - GameRootMain implements this interface and delegates to its subsystems:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
// =====================================================================================================using System;
namespace SASZombieAssaultTD.Engine.TextRendering
{
    public class TextEnums
    {
        //---------------------------------------------
        // Enum: Font Weight
        //---------------------------------------------
        public enum FontWeight
        {
            Thin,
            ExtraLight,
            Light,
            Regular,
            Medium,
            SemiBold,
            Bold,
            ExtraBold,
            Black
        }

        //---------------------------------------------
        // Enum: Font Style
        //---------------------------------------------

        public enum FontStyle
        {
            Normal,
            Italic,
            Oblique
        }

        /// <summary>
        /// Text alignment options for rendering. P11-04-09-F: Supports left, center, and right text alignment.
        /// </summary>
        public enum TextAlignment
        {
            /// <summary>Align text to the left</summary>
            Left,

            /// <summary>Align text to the center</summary>
            Center,

            /// <summary>Align text to the right</summary>
            Right
        }

        /// <summary>
        /// Bitmask enumeration for texture creation flags.
        /// </summary>
        public enum EngineTextureFlags
        {
            None = 0,
            IsSRGB = 1 << 0
        }

        /// <summary>
        /// Supported GPU texture formats.
        /// </summary>
        public enum GpuTextureFormat
        {
            BGRA32,
            RGB24,
            A8,
            RGBA,
            RGBA8
        }
    }
}