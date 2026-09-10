// ====================================================================================================
//  FILE: MapM_Transitions.cs
//  PATH: ./Engine/UI/MapMenu/
//  MODULE: UI.MapMenu
//
//  ROLE:
//      Deterministic transition controller for Map Menu UI state changes. Responsible for handling
//      visual transitions such as fade‑in, fade‑out, tile hover transitions, and panel visibility
//      changes. Ensures stable, reproducible behavior across all hardware configurations.
//
//  RESPONSIBILITIES:
//      - Provide fade‑in and fade‑out transitions for Map Menu scene activation.
//      - Handle tile hover and selection transitions.
//      - Manage panel visibility transitions using deterministic Option‑B rules.
//      - Package transition results for UIRenderer and UIManager.
//      - Guarantee stable timing and reproducible animation curves.
//
//  NON‑RESPONSIBILITIES:
//      - Scene switching logic (handled by MapMenuUI).
//      - Input dispatch (UIEventSystem).
//      - Asset loading (UIAssetLoader).
//
//  AUTHORSHIP:
//      Created by the duo developers of Copilot and Brian Campbell respectively
//      Updated or changed by Copilot on 2026‑07‑27
//
// ====================================================================================================

using SASZombieAssaultTD.Engine.UI.Elements;
using SASZombieAssaultTD.Engine.UI.MainMenu;
using SASZombieAssaultTD.Engine.UI.Rendering;

namespace SASZombieAssaultTD.Engine.UI.MapMenu
{
    internal class MapM_Transitions
    {
        // Fade‑in transition for Map Menu
        public void FadeIn(UIState state, float duration)
        {
            foreach (var element in state.Elements)
                element.Opacity = 0f;

            // Fixed: Construct the read-only struct via its constructor rather than passing UITransition directly
            var fadeInPacket = new RegisterTransition(
                TransitionType.FadeIn,
                TransitionMode.Immediate,
                TransitionDiagnostic.None,
                new UITransition
                {
                    Duration = duration,
                    Apply = progress =>
                    {
                        foreach (var element in state.Elements)
                            element.Opacity = (float)progress;
                    }
                },
                0f
            );

            Diagnostics.DLogger.Log($"MapM_Transitions: FadeIn packet queued for duration {duration}s.");
        }

        // Fade‑out transition for Map Menu
        public void FadeOut(UIState state, float duration)
        {
            foreach (var element in state.Elements)
                element.Opacity = 1f;

            // Fixed: Construct the read-only struct via its constructor rather than passing UITransition directly
            var fadeOutPacket = new RegisterTransition(
                TransitionType.FadeOut,
                TransitionMode.Immediate,
                TransitionDiagnostic.None,
                new UITransition
                {
                    Duration = duration,
                    Apply = progress =>
                    {
                        foreach (var element in state.Elements)
                            element.Opacity = 1f - (float)progress;
                    }
                },
                0f
            );

            Diagnostics.DLogger.Log($"MapM_Transitions: FadeOut packet queued for duration {duration}s.");
        }

        // Tile selection transition (deterministic)
        public void SelectTile(UIPanel tile)
        {
            tile.Style.BorderThickness = 4;
            tile.Style.BorderColor = tile.Style.PressedColor;
            tile.Style.ShadowOffset = 1f;
        }

        // Tile deselection transition (deterministic)
        public void DeselectTile(UIPanel tile)
        {
            tile.Style.BorderThickness = 2;
            tile.Style.BorderColor = tile.Style.BackgroundColor;
            tile.Style.ShadowOffset = 3f;
        }

        // Generic panel visibility transition
        public void SetPanelVisible(UIPanel panel, bool visible)
        {
            panel.PanelVisible = visible;
        }
    }
}
