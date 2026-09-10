// =====================================================================================================
//  FILE: UIRoot.cs
//  PATH: Engine/UI/Components/UIRoot.cs
//  SUBSYSTEM: UI Framework — Root Container
//
//  ROLE:
//      Serves as the deterministic root container for all UIElement instances within a UI subsystem.
//      Provides initialization, update propagation, layout invalidation, and render dispatch for the
//      entire UI tree. All UI subsystems (MainMenu, MapMenu, HUD, PauseMenu) anchor their elements
//      under a UIRoot instance.
//
//  RESPONSIBILITIES:
//      - Maintain the authoritative collection of UIElement children.
//      - Provide deterministic initialization, update, layout, and render entry points.
//      - Propagate Update(), Render(), and UpdateLayout() calls to all child elements.
//      - Track and invalidate layout state using NeedsLayoutUpdate flags.
//      - Manage parent/child relationships for UIElement instances.
//      - Serve as the root node for UIRenderer, UIEventSystem, and UIState pipelines.
//
//  NON-RESPONSIBILITIES:
//      - Does NOT perform element-specific rendering logic (delegated to UIElement subclasses).
//      - Does NOT manage input routing (handled by UIEventSystem).
//      - Does NOT manage scene transitions or UI subsystem switching.
//      - Does NOT handle asset loading or texture management.
//
//  ARCHITECTURAL NOTES:
//      - UIRoot inherits from UIElement to maintain a unified tree structure.
//      - All UIElement instances must be attached to a UIRoot to participate in rendering and input.
//      - Layout invalidation follows deterministic Option‑B rules: any child invalidation bubbles up.
//      - UIRoot is required by MapMenu, MainMenu, HUD, and all future UI subsystems.
//
//  AUTHORSHIP:
//      Modernized by Copilot and Brian Campbell — 2026‑09‑10
// =====================================================================================================

using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.UI.Elements;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI
{
    public sealed class UIRoot : UIElement
    {
        private readonly List<UIElement> _elements = new List<UIElement>();
        private bool _isInitialized = false;
        private bool _needsLayoutUpdate = true;

        public bool IsInitialized => _isInitialized;
        public IReadOnlyList<UIElement> Elements => _elements.AsReadOnly();

        public Color BackgroundColor { get; internal set; } = Color.Transparent;

        public bool GetNeedsLayoutUpdate()
        {
            return _needsLayoutUpdate;
        }

        private void SetNeedsLayoutUpdate(bool value)
        {
            _needsLayoutUpdate = value;
        }

        // ---------------------------------------------------------------------------------------------
        // Initialization
        // ---------------------------------------------------------------------------------------------
        public void Initialize()
        {
            if (_isInitialized)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "UIRoot: Already initialized");
                return;
            }

            _elements.Clear();
            SetNeedsLayoutUpdate(true);
            _isInitialized = true;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "UIRoot: Initialized");
        }

        // ---------------------------------------------------------------------------------------------
        // Update
        // ---------------------------------------------------------------------------------------------
        public override void Update(float deltaTime)
        {
            if (!_isInitialized)
                return;

            foreach (var element in _elements)
            {
                element.Update(deltaTime);

                if (element.NeedsLayoutUpdate)
                    SetNeedsLayoutUpdate(true);
            }
        }

        // ---------------------------------------------------------------------------------------------
        // Render
        // ---------------------------------------------------------------------------------------------
        public void Render()
        {
            if (!_isInitialized)
                return;

            if (GetNeedsLayoutUpdate())
                UpdateLayout();

            foreach (var element in _elements)
                element.Render();
        }

        // ---------------------------------------------------------------------------------------------
        // Layout
        // ---------------------------------------------------------------------------------------------
        public override void UpdateLayout()
        {
            foreach (var element in _elements)
                element.UpdateLayout();

            SetNeedsLayoutUpdate(false);
        }

        // ---------------------------------------------------------------------------------------------
        // Element Management
        // ---------------------------------------------------------------------------------------------
        public void AddElement(UIElement element)
        {
            if (element == null)
                return;

            if (_elements.Contains(element))
                return;

            element.Parent = this;
            _elements.Add(element);
            SetNeedsLayoutUpdate(true);
        }

        public void RemoveElement(UIElement element)
        {
            if (element == null)
                return;

            if (_elements.Remove(element))
            {
                element.Parent = null;
                SetNeedsLayoutUpdate(true);
            }
        }

        // ---------------------------------------------------------------------------------------------
        // Shutdown
        // ---------------------------------------------------------------------------------------------
        public void Shutdown()
        {
            if (!_isInitialized)
                return;

            foreach (var element in _elements)
                element.Parent = null;

            _elements.Clear();
            SetNeedsLayoutUpdate(false);
            _isInitialized = false;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "UIRoot: Shutdown complete");
        }
    }
}
