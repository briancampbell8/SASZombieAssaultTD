using UIButton = SASZombieAssaultTD.Engine.Systems.UI.Button;
using UIPanel = SASZombieAssaultTD.Engine.Systems.UI.Panel;
using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Systems.UI;
using SASZombieAssaultTD.Engine.Rendering;

namespace SASZombieAssaultTD.Engine.Systems.UI
{
    public sealed class HUD
    {
        private readonly List<UIElementBase> _elements = new List<UIElementBase>();

        public void Add(UIElementBase element)
        {
            if (element != null)
                _elements.Add(element);
        }

        public void Update(TimeSpan deltaTime)
        {
            foreach (var element in _elements)
                element.Update(deltaTime);
        }

        public void Render(IRenderContext context)
        {
            foreach (var element in _elements)
                element.Render(context);
        }
    }
}