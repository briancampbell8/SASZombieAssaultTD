using UIButton = SASZombieAssaultTD.Engine.Systems.UI.Button;
using UIPanel = SASZombieAssaultTD.Engine.Systems.UI.Panel;
using System;

namespace SASZombieAssaultTD.Engine.Systems.UI
{
    public abstract class UIElementBase
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public virtual void Update(TimeSpan deltaTime) { }
        public virtual void Render(SASZombieAssaultTD.Engine.Rendering.IRenderContext context) { }
    }
}