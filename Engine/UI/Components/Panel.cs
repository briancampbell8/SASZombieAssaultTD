// =====================================================================================================
//  FILE: Panel.cs
//  PATH: Engine/UI/Components/Panel.cs
//  SUBSYSTEM: UI Framework — Container Element Component
//
//  ROLE:
//      Deterministic UI container element that groups and manages child UI elements. Provides structural
//      hierarchy, visibility control, and delegated rendering for composite UI layouts.
//
//  RESPONSIBILITIES:
//      - Maintain an ordered list of child UIElementBase instances.
//      - Provide AddChild() for deterministic UI hierarchy construction.
//      - Forward Update() and Render() calls to visible child elements.
//      - Act as a non-visual layout container (no GPU drawing performed directly).
//
//  NON-RESPONSIBILITIES:
//      - Performing any direct rendering (delegated to child elements).
//      - Managing GPU resources or issuing RenderSystem commands.
//      - Handling input routing or UI event bubbling.
//      - Performing layout, anchoring, or automatic positioning logic.
//
//  ARCHITECTURAL NOTES:
//      - Panel is a structural node in the UIElementBase hierarchy.
//      - Rendering is delegated entirely to child elements, preserving deterministic ordering.
//      - Panel remains render-agnostic and does not participate directly in the GPU pipeline.
//      - Compatible with the modern RenderSystem → IDrawingContext rendering architecture.
//
//  CHANGE HISTORY:
//      2026-09-10 — Header modernized to match engine documentation standards.
// =====================================================================================================



using System;
using System.Collections.Generic;
using System.Drawing;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.UI.Elements;

namespace SASZombieAssaultTD.Engine.UI
{
    public class Panel : UIElementBase
    {
        public string PanelId { get; set; } = string.Empty;
        public bool PanelVisible { get; set; } = true;

        private readonly List<UIElementBase> _children = new();

        public Panel()
        {
        }

        public Panel(Rectangle bounds) : base(bounds)
        {
        }

        public void AddChild(UIElementBase child)
        {
            if (child != null)
                _children.Add(child);
        }

        public override void Render(D3D11Adapter_Core adapter, float deltaTime)
        {
            if (!PanelVisible || adapter == null)
                return;

            foreach (var child in _children)
            {
                if (child == null)
                    continue;

                if (!child.IsVisible)
                    continue;

                try
                {
                    child.Render(adapter, deltaTime);
                }
                catch (Exception)
                {
                    // Swallow per-child exceptions to keep UI rendering stable
                }
            }
        }
    }
}
