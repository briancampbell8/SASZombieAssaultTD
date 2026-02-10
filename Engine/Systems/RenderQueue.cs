using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Systems
{
    /// <summary>
    /// A simple ordered render queue for batching draw calls.
    /// </summary>
    public sealed class RenderQueue
    {
        private readonly List<RenderItem> _items = new();

        public void Enqueue(RenderItem item)
        {
            _items.Add(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public IReadOnlyList<RenderItem> Items => _items;

        public void SortByLayer()
        {
            _items.Sort((a, b) => a.Layer.CompareTo(b.Layer));
        }
    }

    public sealed class RenderItem
    {
        public int Layer { get; }
        public string TextureName { get; }
        public float X { get; }
        public float Y { get; }

        public RenderItem(int layer, string textureName, float x, float y)
        {
            Layer = layer;
            TextureName = textureName ?? throw new ArgumentNullException(nameof(textureName));
            X = x;
            Y = y;
        }
    }
}