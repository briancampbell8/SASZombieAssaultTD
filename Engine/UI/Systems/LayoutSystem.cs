// ====================================================================================================
//  FILE: LayoutSystem.cs
//  PATH: Engine/UI/Systems/LayoutSystem.cs
//  SUBSYSTEM: UI Framework — Layout System
//
//  ROLE:
//      Manages UI element layout and update sequencing. The LayoutSystem is responsible for
//      maintaining and updating UI elements but does NOT perform rendering in the modern UI pipeline.
//
//  RESPONSIBILITIES:
//      - Maintain a list of UI elements.
//      - Provide AddElement() for scene/UI composition.
//      - Provide Update() for per‑frame UI element state changes (fade, anchoring, etc.).
//      - Provide compatibility stubs for legacy scene code.
//
//  NON-RESPONSIBILITIES:
//      - Rendering (handled exclusively by ModernUIRenderer_Core).
//      - GPU batching, draw calls, or render queue management.
//      - Input routing (handled by UIInputRouter).
//
//  ARCHITECTURAL NOTES:
//      - Legacy Render() calls are intentionally stubbed out to avoid breaking older scene code.
//      - Modern UI pipeline requires rendering to be centralized in ModernUIRenderer_Core.
// ====================================================================================================

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Render;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.UI.Elements;

namespace SASZombieAssaultTD.Engine.UI
{
    public class LayoutSystem
    {
        // Legacy compatibility stub — no longer used in modern pipeline
        public void Render(UIElementBase element, RenderQueue queue, D3D11Adapter_Core context)
        {
            // Rendering is handled by ModernUIRenderer_Core
        }

        private readonly List<UIElementBase> _elements = new();

        public void AddElement(UIElementBase element)
        {
            if (element != null)
                _elements.Add(element);
        }

        public void Update(float deltaTime)
        {
            foreach (var e in _elements)
                e.Update(deltaTime);
        }

        // Modern pipeline: LayoutSystem does NOT render UI elements
        public void Render(D3D11Adapter_Core context)
        {
            // Rendering is handled by ModernUIRenderer_Core
        }

        // Compatibility overloads for old scene code
        public void Update(UIElementBase _, TimeSpan deltaTime)
        {
            Update((float)deltaTime.TotalSeconds);
        }

        public void Render(D3D11Adapter_Core context, UIElementBase _, int __)
        {
            // Rendering is handled by ModernUIRenderer_Core
        }
    }
}
