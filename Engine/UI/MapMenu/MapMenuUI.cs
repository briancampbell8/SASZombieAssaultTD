// ====================================================================================================
//  FILE: MapMenuUI.cs
//  PATH: ./Engine/UI/MapMenu/
//  MODULE: UI.MapMenu
//
//  ROLE:
//      High‑level deterministic controller for the Map Menu UI subsystem. Responsible for orchestrating
//      layout assembly, tile construction, transitions, and delivery of a fully resolved UIState to
//      the SceneManager and UIManager.
//
//  RESPONSIBILITIES:
//      - Initialize Map Menu UI pipeline using Option‑B deterministic rules.
//      - Invoke MapM_Finalizer to construct the UIState.
//      - Build root panel via MapM_PanelBuilder.
//      - Apply fade‑in transitions on scene entry.
//      - Provide deterministic hooks for button actions (Back, Start).
//      - Deliver final UIState to UIManager for rendering.
//
//  NON‑RESPONSIBILITIES:
//      - Rendering (handled by UIRenderer).
//      - Asset loading (UIAssetLoader).
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
    internal class MapMenuUI
    {
        private readonly MapM_Finalizer _finalizer = new MapM_Finalizer();
        private readonly MapM_PanelBuilder _panelBuilder = new MapM_PanelBuilder();
        private readonly MapM_Transitions _transitions = new MapM_Transitions();

        private UIState? _state;

        // Initialize Map Menu UI
        public void Initialize()
        {
            _state = _finalizer.Build();

            var rootPanel = _panelBuilder.BuildRootPanel(_state.Elements);
            _state.RootPanel = rootPanel;

            ApplySceneEntryTransition();
        }

        // Fade‑in transition on scene entry
        private void ApplySceneEntryTransition()
        {
            if (_state == null)
                return;

            _transitions.FadeIn(_state, duration: 0.45f);
        }

        // Deliver final UIState to UIManager
        public UIState GetState()
        {
            return _state ?? new UIState
            {
                Elements = new List<UIElement>(),
                StateId = "MapMenu",
                IsVisible = true
            };
        }

        // Deterministic button action hooks
        public void OnBack()
        {
            // Scene transition handled externally
        }

        public void OnStart()
        {
            // Map start logic handled externally
        }
    }
}
