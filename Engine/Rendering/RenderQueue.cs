using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SASZombieAssaultTD.Engine.Rendering
{
    /// <summary>
    /// Rendering queue for ordering draw calls in the rendering pipeline.
    /// Part of the rendering domain, not systems domain.
    /// </summary>
    public sealed class RenderQueue
    {
        private readonly List<object> _queue = new();

        public void Enqueue(object item)
        {
            _queue.Add(item);
        }

        public void Clear()
        {
            _queue.Clear();
        }

        public IReadOnlyList<object> GetItems()
        {
            return new ReadOnlyCollection<object>(_queue);
        }

        public void SortByLayer()
        {
            // Simple sorting implementation - would need proper layer comparison in real implementation
            _queue.Sort((x, y) => 0);
        }
    }
}


