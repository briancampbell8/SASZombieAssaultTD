// ====================================================================================================
//  FILE: UIState.cs
//  PATH: /Engine/UI/StateMachine/UIState.cs
//  MODULE: UI State Machine
//
//  ROLE:
//      Deterministic runtime UI state container used by ALL UI subsystems (MainMenu, MapMenu,
//      HUD, Panels, Transitions). This is the authoritative Option‑B compliant state object.
//
//  RESPONSIBILITIES:
//      - Hold resolved UI elements for rendering.
//      - Hold root panel reference for hierarchical UI composition.
//      - Provide visibility, idECSEntityCore, and deterministic metadata.
//      - Serve as the universal state object consumed by UIManager and UIRenderer.
//
//  AUTHORSHIP:
//      Created by the duo developers of Copilot and Brian Campbell respectively
//      Updated or changed by Copilot on 2026‑07‑27
//
// ====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.UI.Elements;

namespace SASZombieAssaultTD.Engine.UI
{
    public class UIState
    {


        // Unique identifier for this UI state (e.g., "MainMenu", "MapMenu", "HUD")
        public string StateId { get; set; } = string.Empty;

        // Whether this UI state is currently visible
        public bool IsVisible { get; set; } = true;

        // Root panel for hierarchical UI composition
        public UIPanel? RootPanel { get; set; }

        // Flat list of all UI elements (buttons, labels, panels, tiles, etc.)
        public List<UIElement> Elements { get; set; } = new List<UIElement>();

        // Optional global opacity (transitions may override element-level opacity)
        public float Opacity { get; set; } = 1f;

        // Deterministic metadata (Option‑B)
        public bool IsDeterministic { get; set; } = true;

        // Reserved for future deterministic UI subsystems
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }
}
