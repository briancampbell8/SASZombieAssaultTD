// ====================================================================================================
//  FILE: UILabel.cs
//  PATH: ./Engine/UI/MainMenu/
//  MODULE: UI.MainMenu
//
//  ROLE:
//      Deterministic label element for the Main Menu UI. Provides text rendering metadata,
//      geometry, and style configuration for static and dynamic labels within the Option‑B
//      deterministic UI pipeline.
//
//  RESPONSIBILITIES:
//      - Define label text, position, size, and visibility.
//      - Apply deterministic style rules for font, color, and alignment.
//      - Serve as a render‑ready component consumed by UIState and UIRenderer.
//      - Guarantee reproducible visual output across all hardware configurations.
//
//  NON‑RESPONSIBILITIES:
//      - Runtime input handling (UIEventSystem).
//      - Scene transitions (MainMenuUI).
//      - Asset loading (UIAssetLoader).
//
//  NOTES:
//      Labels are passive visual elements. They do not handle interaction or state changes.
//      All geometry and style data are resolved deterministically through MainM_LayoutRules.
//
//  AUTHORSHIP:
//      Created by the duo developers of Copilot and Brian Campbell respectively
//      Updated or changed by Copilot on 2026‑07‑27
//
// ====================================================================================================


using System.Drawing;
using SASZombieAssaultTD.Engine.UI.Elements;
using SASZombieAssaultTD.Engine.UI.Styles;

namespace SASZombieAssaultTD.Engine.UI.MainMenu
{
    public class UILabel : UIElement
    {
        public string Text { get; set; }
        public PointF Position { get; set; }
        public SizeF Size { get; set; }
        public UIStyle Style { get; set; }
        public bool LabelVisible { get; set; }
    }
}
