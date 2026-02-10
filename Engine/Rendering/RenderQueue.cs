using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Rendering
{
    /// <summary>
    /// Rendering-layer queue frontend. Delegates to the Systems.RenderQueue.
    /// Renamed from RenderQueue to resolve the duplicate class name with
    /// Engine.Systems.RenderQueue.
    /// </summary>
    public sealed class RenderQueueFrontend
    {
        private readonly Systems.RenderQueue _queue = new();

        public void Enqueue(Systems.RenderItem item)
        {
            _queue.Enqueue(item);
        }

        public void Clear()
        {
            _queue.Clear();
        }

        public IReadOnlyList<Systems.RenderItem> Items => _queue.Items;

        public void SortByLayer()
        {
            _queue.SortByLayer();
        }
    }
}