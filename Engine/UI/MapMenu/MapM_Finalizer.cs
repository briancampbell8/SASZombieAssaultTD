// ====================================================================================================
//  FILE: MapM_Finalizer.cs
//  PATH: ./Engine/UI/MapMenu/
//  MODULE: UI.MapMenu
//
//  ROLE:
//      Deterministic compiler for the Map Menu UI panel. Responsible for assembling all map‑selection
//      components (buttons, title, background elements, map entries) into a fully resolved,
//      render‑ready UIState package consumed by the UIManager and UIRenderer.
//
//  RESPONSIBILITIES:
//      - Construct the Map Menu panel using Option‑B deterministic rules.
//      - Invoke MapM_ButtonBuilder for all button elements.
//      - Apply MapM_LayoutRules for geometry, spacing, and alignment.
//      - Produce a sealed UIState object for the Map Menu scene.
//      - Guarantee stable, reproducible output across all hardware configurations.
//
//  NON‑RESPONSIBILITIES:
//      - Scene transitions (handled by MapMenuUI).
//      - Texture loading (handled by UIAssetLoader).
//      - Runtime input dispatch (UIEventSystem).
//
//  AUTHORSHIP:
//      Created by the duo developers of Copilot and Brian Campbell respectively
//      Updated or changed by Copilot on 2026‑07‑27
//
// ====================================================================================================

using System.Collections.Generic;
using SASZombieAssaultTD.Engine.UI.Elements;
using SASZombieAssaultTD.Engine.UI.MainMenu;

namespace SASZombieAssaultTD.Engine.UI.MapMenu
{
    internal class MapM_Finalizer
    {
        private readonly MapM_ButtonBuilder _buttonBuilder = new MapM_ButtonBuilder();

        // Entry point: build the entire Map Menu UIState
        public UIState Build()
        {
            var state = new UIState
            {
                Elements = new List<UIElement>(),
                StateId = "MapMenu",
                IsVisible = true
            };

            BuildButtons(state);
            BuildTitle(state);
            BuildBackground(state);
            BuildMapEntries(state);

            return state;
        }

        // Build all Map Menu buttons
        private void BuildButtons(UIState state)
        {
            var backButton = _buttonBuilder.Build("Back", "Back", () => { });
            var startButton = _buttonBuilder.Build("Start", "Start", () => { });

            state.Elements.Add(backButton);
            state.Elements.Add(startButton);
        }

        // Build Map Menu title element
        private void BuildTitle(UIState state)
        {
            var layout = MapM_LayoutRules.GetTitleLayout();

            var title = new UILabel
            {
                Text = "Select a Map",
                Position = layout.Position,
                Size = layout.Size,
                Style = layout.Style,
                LabelVisible = true
            };

            state.Elements.Add(title);
        }

        // Build background panel
        private void BuildBackground(UIState state)
        {
            var layout = MapM_LayoutRules.GetBackgroundLayout();
            var background = new UIPanel
            {
                PanelId = "MapMenuBackground",
                // Explicitly map System.Drawing.PointF to your engine's PointF
                Position = new Components.PointF(layout.Position.X, layout.Position.Y),
                // Explicitly map System.Drawing.SizeF to your engine's SizeF
                Size = new System.Drawing.SizeF(layout.Size.Width, layout.Size.Height),
                Style = layout.Style,
                PanelVisible = true
            };


            state.Elements.Add(background);
        }

        // Build map entries (deterministic)
        private void BuildMapEntries(UIState state)
        {
            var entries = MapM_LayoutRules.GetMapEntryLayouts();

            foreach (var entry in entries)
            {
                var mapLabel = new UILabel
                {
                    Text = entry.MapName,
                    Position = entry.Position,
                    Size = entry.Size,
                    Style = entry.Style,
                    LabelVisible = true
                };

                state.Elements.Add(mapLabel);
            }
        }
    }
}
