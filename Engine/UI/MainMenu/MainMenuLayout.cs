// ====================================================================================================
//  FILE: MainMenuLayout.cs
//  PATH: ./Engine/UI/MainMenu/
//  MODULE: UI.MainMenu
//
//  ROLE:
//      High‑level deterministic layout coordinator for the Main Menu UI. This module provides
//      structured access to all geometry rules, spacing constants, alignment anchors, and
//      deterministic Option‑B layout definitions used by the Main Menu subsystem.
//
//  RESPONSIBILITIES:
//      - Expose unified layout constants for Main Menu UI.
//      - Provide anchor points for title, buttons, and background.
//      - Serve as a stable geometry reference for MainM_LayoutRules and MainM_Finalizer.
//      - Guarantee reproducible layout behavior across all hardware configurations.
//      - Maintain deterministic Option‑B compliance for all layout values.
//
//  NON‑RESPONSIBILITIES:
//      - Rendering (handled by UIRenderer).
//      - Input dispatch (UIEventSystem).
//      - Scene transitions (MainMenuUI).
//
//  AUTHORSHIP:
//      Created by the duo developers of Copilot and Brian Campbell respectively
//      Updated or changed by Copilot on 2026‑07‑27
//
// ====================================================================================================

using System.Drawing;

namespace SASZombieAssaultTD.Engine.UI.MainMenu
{
    internal static class MainMenuLayout
    {
        // Screen reference resolution (deterministic)
        public static readonly SizeF ReferenceResolution = new SizeF(1280, 720);

        // Title anchor point
        public static readonly PointF TitleAnchor = new PointF(
            ReferenceResolution.Width / 2f,
            180f
        );

        // Button anchor baseline
        public static readonly PointF ButtonAnchor = new PointF(
            ReferenceResolution.Width / 2f,
            420f
        );

        // Button spacing (vertical)
        public const float ButtonSpacing = 90f;

        // Button size (deterministic)
        public static readonly SizeF ButtonSize = new SizeF(300f, 70f);

        // Background panel geometry
        public static readonly PointF BackgroundPosition = new PointF(0f, 0f);
        public static readonly SizeF BackgroundSize = new SizeF(
            ReferenceResolution.Width,
            ReferenceResolution.Height
        );

        // Deterministic resolver for button Y‑offsets
        public static float ResolveButtonY(int index)
        {
            return ButtonAnchor.Y + (index * ButtonSpacing);
        }
    }
}
