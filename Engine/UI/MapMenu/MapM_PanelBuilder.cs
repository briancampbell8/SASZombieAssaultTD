// ====================================================================================================
//  FILE: MapM_PanelBuilder.cs
//  PATH: ./Engine/UI/MapMenu/
//  MODULE: UI.MapMenu
//
//  ROLE:
//      Deterministic constructor for the Map Menu root panel. Responsible for assembling the primary
//      container that holds all map tiles, buttons, and structural UI elements used by the Map Menu.
//
//  RESPONSIBILITIES:
//      - Build the root panel using Option‑B deterministic layout rules.
//      - Attach all UIElements provided by MapM_Finalizer.
//      - Ensure strict ordering and deterministic placement of children.
//      - Provide a stable panel hierarchy for UIRenderer and UIManager.
//
//  NON‑RESPONSIBILITIES:
//      - Element creation (handled by MapM_Finalizer).
//      - Transitions (MapM_Transitions).
//      - Rendering (UIRenderer).
//      - Input dispatch (UIEventSystem).
//
//  AUTHORSHIP:
//      Created by the duo developers of Copilot and Brian Campbell respectively
//      Updated or changed by Copilot on 2026‑07‑27
//
// ====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.UI.Elements;

namespace SASZombieAssaultTD.Engine.UI.MapMenu
{
    internal class MapM_PanelBuilder
    {
        // Build deterministic root panel for Map Menu
        public UIPanel BuildRootPanel(List<UIElement> elements)
        {
            var rootPanel = new UIPanel
            {
                Id = "MapMenu_Root",
                IsVisible = true
            };

            // Deterministic ordering: attach elements in the sequence provided by MapM_Finalizer
            foreach (var element in elements)
            {
                rootPanel.AddChild(element);
            }

            return rootPanel;
        }
    }
}
