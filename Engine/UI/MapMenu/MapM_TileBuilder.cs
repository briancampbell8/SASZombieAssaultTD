// ====================================================================================================
//  FILE: MapM_TileBuilder.cs
//  PATH: ./Engine/UI/MapMenu/
//  MODULE: UI.MapMenu
//
//  ROLE:
//      Deterministic builder for map‑selection tiles. Responsible for constructing the visual
//      thumbnail elements representing each playable map, including geometry, style, and
//      interaction behavior.
//
//  RESPONSIBILITIES:
//      - Build map tiles using Option‑B deterministic rules.
//      - Apply geometry from MapM_LayoutRules.
//      - Package tile elements for UIManager and UIRenderer.
//      - Provide hover and selection state transitions.
//      - Ensure stable, reproducible layout across all hardware configurations.
//
//  NON‑RESPONSIBILITIES:
//      - Scene transitions (handled by MapMenuUI).
//      - Asset loading (UIAssetLoader).
//      - Runtime input dispatch (handled by UIEventSystem).
//
//  AUTHORSHIP:
//      Created by the duo developers of Copilot and Brian Campbell respectively
//      Updated or changed by Copilot on 2026‑07‑27
//
//  CHANGE LOG ENTRY (2026‑07‑28):
//      • Removed illegal assignment to UIElement.Children (read‑only).
//      • Replaced Children.Add(...) with deterministic AddChild(...).
//      • Corrected UILabel namespace after subsystem merge.
//      • Stabilized UIElement tree construction under unified UI.Elements pipeline.
//
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.UI.Elements;
using SASZombieAssaultTD.Engine.UI.MainMenu;
using SASZombieAssaultTD.Engine.UI.Styles;

namespace SASZombieAssaultTD.Engine.UI.MapMenu
{
    internal class MapM_TileBuilder
    {
        // Build a single map tile
        public UIPanel BuildTile(string mapName, Action? onSelect)
        {
            var layout = MapM_LayoutRules
                .GetMapEntryLayouts()
                .Find(e => e.MapName == mapName);

            var tile = new UIPanel
            {
                PanelId = "MapTile_" + mapName.Replace(" ", ""),
                // Fixed: Map System.Drawing.PointF to Engine Components.PointF
                Position = new Components.PointF(layout.Position.X, layout.Position.Y),
                // Fixed: Map System.Drawing.SizeF to Engine Components.SizeF
                Size = new System.Drawing.SizeF(layout.Size.Width, layout.Size.Height),
                Style = layout.Style,
                PanelVisible = true
            };

            BuildLabel(tile, mapName);
            BuildInteraction(tile, onSelect);

            return tile;
        }

        // Add map name label to tile
        private void BuildLabel(UIPanel tile, string mapName)
        {
            var label = new UILabel
            {
                Text = mapName,
                // UILabel expects standard System.Drawing types
                Position = new System.Drawing.PointF(
                    tile.Position.X + 20f,
                    tile.Position.Y + tile.Size.Height - 50f
                ),
                Size = new System.Drawing.SizeF(tile.Size.Width - 40f, 40f),
                Style = (UIStyle)UIStyleSheet.MapMenu.MapEntryLabelStyle,
                LabelVisible = true
            };

            tile.AddChild(label);
        }


        // Add deterministic interaction behavior
        private void BuildInteraction(UIPanel tile, Action? onSelect)
        {
            tile.OnClick = onSelect;

            tile.OnHover = () =>
            {
                tile.Style.BackgroundColor = UIStyleSheet.MapMenu.MapEntryHover;
                tile.Style.ShadowOffset = 2f; // Fixed: Changed from PointF object to float literal
            };

            tile.OnHoverExit = () =>
            {
                tile.Style.BackgroundColor = UIStyleSheet.MapMenu.MapEntryStyle.BackgroundColor;
                tile.Style.ShadowOffset = 3f; // Fixed: Changed from PointF object to float literal
            };
        }
    }
}
