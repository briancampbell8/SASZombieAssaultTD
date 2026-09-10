// ====================================================================================================
//  FILE: MapMenuLayout.cs
//  PATH: ./Engine/UI/MapMenu/
//  MODULE: UI.MapMenu
//
//  ROLE:
//      High‑level deterministic layout coordinator for the Map Menu UI. This module provides
//      structured access to geometry constants, spacing rules, alignment anchors, and Option‑B
//      deterministic layout definitions used by the Map Menu subsystem.
//
//  RESPONSIBILITIES:
//      - Expose unified layout constants for Map Menu UI.
//      - Provide anchor points for title, buttons, background, and map tiles.
//      - Serve as a stable geometry reference for MapM_LayoutRules and MapM_Finalizer.
//      - Guarantee reproducible layout behavior across all hardware configurations.
//      - Maintain deterministic Option‑B compliance for all layout values.
//
//  NON‑RESPONSIBILITIES:
//      - Rendering (handled by UIRenderer).
//      - Input dispatch (UIEventSystem).
//      - Scene transitions (MapMenuUI).
//
//  AUTHORSHIP:
//      Created by the duo developers of Copilot and Brian Campbell respectively
//      Updated or changed by Copilot on 2026‑07‑27
//
// ====================================================================================================

using System.Drawing;

namespace SASZombieAssaultTD.Engine.UI.MapMenu
{
    internal static class MapMenuLayout
    {
        // Reference resolution for deterministic geometry
        public static readonly SizeF ReferenceResolution = new SizeF(1280f, 720f);

        // Title anchor point
        public static readonly PointF TitleAnchor = new PointF(
            ReferenceResolution.Width / 2f,
            140f
        );

        // Background geometry
        public static readonly PointF BackgroundPosition = new PointF(0f, 0f);
        public static readonly SizeF BackgroundSize = new SizeF(
            ReferenceResolution.Width,
            ReferenceResolution.Height
        );

        // Button anchor baseline
        public static readonly PointF ButtonAnchor = new PointF(
            ReferenceResolution.Width / 2f,
            640f
        );

        // Button spacing (horizontal)
        public const float ButtonSpacing = 420f;

        // Button size (deterministic)
        public static readonly SizeF ButtonSize = new SizeF(240f, 60f);

        // Map tile grid anchors
        public static readonly PointF TileGridOrigin = new PointF(240f, 260f);
        public static readonly SizeF TileSize = new SizeF(300f, 180f);

        // Tile spacing (horizontal and vertical)
        public const float TileSpacingX = 280f;
        public const float TileSpacingY = 200f;

        // Deterministic resolver for tile positions
        public static PointF ResolveTilePosition(int column, int row)
        {
            return new PointF(
                TileGridOrigin.X + (column * TileSpacingX),
                TileGridOrigin.Y + (row * TileSpacingY)
            );
        }
    }
}
