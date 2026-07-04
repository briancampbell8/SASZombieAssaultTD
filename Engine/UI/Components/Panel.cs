using SASZombieAssaultTD.Engine.Rendering;
using System.Collections.Generic;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI
{
    public class Panel : UIElementBase
    {
        public string PanelId { get; set; } = string.Empty;
        public bool PanelVisible { get; set; } = true;

        private readonly List<UIElementBase> _children = new();

        public void AddChild(UIElementBase child)
        {
            if (child != null)
                _children.Add(child);
        }

        public override void Render(IRenderContext context)
        {
            if (!IsVisible)
                return;

            foreach (var child in _children)
                child.Render(context);
        }
    }
}


