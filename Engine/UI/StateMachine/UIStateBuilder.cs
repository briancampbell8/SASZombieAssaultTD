// =====================================================================================================
//  FILE: UIStateBuilder.cs
//  PATH: Engine/UI/StateMachine/UIStateBuilder.cs
//  SUBSYSTEM: UI StateMachine
//
//  ROLE:
//      Deterministic construction engine for UIState instances. Responsible for assembling,
//      validating, and flattening hierarchical UI panel graphs into a fully‑resolved,
//      render‑agnostic UIState consumed by the UIStateMachine and HUDManager.
//
//  RESPONSIBILITIES:
//      - Build UIState objects from root UIPanel definitions.
//      - Resolve deterministic layout ordering, visibility, and structural metadata.
//      - Flatten hierarchical UI element trees into a deterministic element list.
//      - Provide a clean, engine-facing API for scene initialization and UI bootstrap.
//
//  NON-RESPONSIBILITIES:
//      - Executing rendering commands or producing Finalizer instructions.
//      - Managing active UI states or performing transitions.
//      - Allocating GPU resources or interacting with hardware subsystems.
//
//  ARCHITECTURAL NOTES:
//      - UIStateBuilder is invoked by UIBootstrap and UIStateMachine during scene initialization.
//      - All layout resolution is deterministic: no async, no global state, no runtime mutation.
//      - HUD panels and UI elements are composed here, but rendered later by HUDPanelBuilder.
// =====================================================================================================

using SASZombieAssaultTD.Engine.UI.Elements;

namespace SASZombieAssaultTD.Engine.UI.StateMachine
{
    internal sealed class UIStateBuilder
    {
        // -------------------------------------------------------------------------------------------------
        // Build a new UIState with deterministic layout resolution
        // -------------------------------------------------------------------------------------------------

        internal UIState Build(string stateId, UIPanel rootPanel)
        {
            var state = new UIState
            {
                StateId = stateId,
                RootPanel = rootPanel,
                IsVisible = true,
                IsDeterministic = true
            };

            ResolveElementsRecursive(rootPanel, state);
            return state;
        }

        // -------------------------------------------------------------------------------------------------
        // Recursive deterministic element resolution
        // -------------------------------------------------------------------------------------------------

        private void ResolveElementsRecursive(UIPanel panel, UIState state)
        {
            if (panel == null)
                return;

            state.Elements.Add(panel);

            foreach (var child in panel.Children)
            {
                state.Elements.Add(child);

                if (child is UIPanel childPanel)
                    ResolveElementsRecursive(childPanel, state);
            }
        }
    }
}
