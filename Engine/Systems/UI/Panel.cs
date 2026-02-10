
using UIButton = SASZombieAssaultTD.Engine.Systems.UI.Button;
using UIPanel = SASZombieAssaultTD.Engine.Systems.UI.Panel;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Rendering;

namespace SASZombieAssaultTD.Engine.Systems.UI
{
    public class Panel : UIElementBase
    {
        public string Id { get; set; } = string.Empty;
        public bool IsVisible { get; set; } = true;

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