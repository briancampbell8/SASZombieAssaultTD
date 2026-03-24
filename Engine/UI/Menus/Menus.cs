using SASZombieAssaultTD.Engine.Rendering;
using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.UI
{
    public sealed class Menus
    {
        private readonly List<UIElementBase> _elements = new List<UIElementBase>();

        public void Add(UIElementBase element)
        {
            if (element != null)
                _elements.Add(element);
        }

        public void Update(float deltaTime)
        {
            foreach (var element in _elements)
                element.Update(deltaTime);
        }

        public void Render(IRenderContext context)
        {
            foreach (var element in _elements)
                element.Render(context);
        }

        // Compatibility overloads for old scene code
        public void Update(UIElementBase _, TimeSpan deltaTime)
        {
            Update((float)deltaTime.TotalSeconds);
        }
    }
}


