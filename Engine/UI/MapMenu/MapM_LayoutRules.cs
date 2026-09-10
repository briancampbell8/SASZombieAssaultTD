// ====================================================================================================
//  FILE: MapM_LayoutRules.cs
//  PATH: ./Engine/UI/MapMenu/
//  MODULE: UI.MapMenu
//
//  ROLE:
//      Deterministic geometry and layout resolver for all Map Menu UI elements. Provides stable,
//      reproducible positioning and sizing rules for buttons, title, background components, and
//      map‑entry thumbnails.
//
//  RESPONSIBILITIES:
//      - Define static layout rules for Map Menu UI.
//      - Provide deterministic geometry for all button identifiers.
//      - Supply title, background, and map‑entry layout definitions.
//      - Guarantee stable output across all hardware configurations.
//      - Serve as the geometry backbone for MapM_Finalizer and MapM_ButtonBuilder.
//
//  NON‑RESPONSIBILITIES:
//      - Rendering (handled by UIRenderer).
//      - Runtime input handling (UIEventSystem).
//      - Scene transitions (MapMenuUI).
//
//  AUTHORSHIP:
//      Created by the duo developers of Copilot and Brian Campbell respectively
//      Updated or changed by Copilot on 2026‑07‑27
//
// ====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Drawing;
using SASZombieAssaultTD.Engine.UI.Styles;

namespace SASZombieAssaultTD.Engine.UI.MapMenu
{
    internal static class MapM_LayoutRules
    {
        // Reference resolution for deterministic geometry
        public static readonly SizeF ReferenceResolution = new SizeF(1280, 720);

        // Title layout
        public static (PointF Position, SizeF Size, UIStyle Style) GetTitleLayout()
        {
            return (
                new PointF(ReferenceResolution.Width / 2f, 140f),
                new SizeF(800f, 100f),
                UIStyleSheet.MapMenu.TitleStyle
            );
        }

        // Background layout
        public static (PointF Position, SizeF Size, UIStyle Style) GetBackgroundLayout()
        {
            return (
                new PointF(0f, 0f),
                new SizeF(ReferenceResolution.Width, ReferenceResolution.Height),
                UIStyleSheet.MapMenu.BackgroundStyle
            );
        }

        // Button layout resolver
        public static (PointF Position, SizeF Size) GetButtonLayout(string id)
        {
            return id switch
            {
                "Back" => (new PointF(200f, 640f), new SizeF(240f, 60f)),
                "Start" => (new PointF(1040f, 640f), new SizeF(240f, 60f)),
                _ => (new PointF(640f, 640f), new SizeF(240f, 60f))
            };
        }

        // Map entry layout definition
        public static List<(string MapName, PointF Position, SizeF Size, UIStyle Style)> GetMapEntryLayouts()
        {
            var entries = new List<(string, PointF, SizeF, UIStyle)>
            {
                ("Mean Streets", new PointF(240f, 260f), new SizeF(300f, 180f), UIStyleSheet.MapMenu.MapEntryStyle),
                ("Sub-Zero",     new PointF(520f, 260f), new SizeF(300f, 180f), UIStyleSheet.MapMenu.MapEntryStyle),
                ("Dead Warehouse", new PointF(800f, 260f), new SizeF(300f, 180f), UIStyleSheet.MapMenu.MapEntryStyle),

                ("Biohazard",    new PointF(240f, 460f), new SizeF(300f, 180f), UIStyleSheet.MapMenu.MapEntryStyle),
                ("Nightfall",    new PointF(520f, 460f), new SizeF(300f, 180f), UIStyleSheet.MapMenu.MapEntryStyle),
                ("Outbreak",     new PointF(800f, 460f), new SizeF(300f, 180f), UIStyleSheet.MapMenu.MapEntryStyle)
            };

            return entries;
        }
    }
}
