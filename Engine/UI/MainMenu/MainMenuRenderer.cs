// =====================================================================================================
//  FILE: MainMenuRenderer.cs
//  PATH: Engine/UI/MainMenu/MainMenuRenderer.cs
//  SUBSYSTEM: UI.MainMenu
//
//  ROLE:
//      Dedicated renderer for the Main Menu subsystem. Clears the frame, applies any menu‑specific
//      visual effects, and dispatches rendering of the UIRoot tree owned by MainMenuUI.
//
//  RESPONSIBILITIES:
//      - Clear the screen using the engine’s D3D11 adapter.
//      - Render all UIElement instances attached to MainMenuUI.Root.
//      - Provide a clean boundary between the UI subsystem and the GPU rendering pipeline.
//
//  NON‑RESPONSIBILITIES:
//      - Update logic (delegated to MainMenuUI).
//      - Asset loading (UIAssetLoader).
//      - Input dispatch (UIEventSystem).
//      - Scene transitions (SceneManager).
//
//  ARCHITECTURAL NOTES:
//      - Works directly with UIRoot.Render(), which handles layout updates and element rendering.
//      - Integrates with the engine’s D3D11Adapter_Core for deterministic GPU‑side rendering.
//      - Matches the Option‑B deterministic UI pipeline used across all UI subsystems.
//
//  AUTHORSHIP:
//      Modernized by Copilot and Brian Campbell — 2026‑09‑10
// =====================================================================================================

using SASZombieAssaultTD.Engine.UI.Rendering;

namespace SASZombieAssaultTD.Engine.UI.MainMenu
{
    public sealed class MainMenuRenderer
    {
        public void Render(MainMenuUI ui)
        {
            // Clear the screen (your engine’s UIRenderer wrapper)
            UIRenderer.Clear(Color.Black);

            // Render the entire UI tree
            ui.Root.Render();
        }
    }
}
