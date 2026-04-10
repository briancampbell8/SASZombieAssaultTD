using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.UI.Components;
using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.UI
{
    public class LayoutSystem
    {
        public void Render(UIElementBase element, RenderQueue queue, IRenderContext context)
        {
            Render(context);
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

        public void Render(IRenderContext context)
        {
            foreach (var e in _elements)
                e.Render(context);
        }

        // Compatibility overloads for old scene code
        public void Update(UIElementBase _, TimeSpan deltaTime)
        {
            Update((float)deltaTime.TotalSeconds);
        }

        public void Render(IRenderContext context, UIElementBase _, int __)
        {
            Render(context);
        }
    }
}



