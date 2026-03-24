using System;
using System.Collections.Generic;

namespace Engine.Systems.Gameplay
{
    public class PriorityQueue<T>
    {
        private readonly List<(T Item, float Priority)> _elements = new();
        private readonly HashSet<T> _set = new(); // For efficient Contains checks

        public int Count => _elements.Count;

        public void Enqueue(T item, float priority)
        {
            if (_set.Contains(item))
                throw new InvalidOperationException("Item already exists in the priority queue.");

            _elements.Add((item, priority));
            _set.Add(item);
            _elements.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        public T Dequeue()
        {
            if (_elements.Count == 0)
                throw new InvalidOperationException("The priority queue is empty.");

            var item = _elements[0].Item;
            _elements.RemoveAt(0);
            _set.Remove(item);
            return item;
        }

        public bool Contains(T item)
        {
            return _set.Contains(item);
        }

        public bool IsEmpty()
        {
            return _elements.Count == 0;
        }
    }
}
