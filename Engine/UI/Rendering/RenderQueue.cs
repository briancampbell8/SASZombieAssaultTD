// ====================================================================================================
//  FILE: RenderQueue.cs
//  PATH: ./Engine/Rendering/
//  MODULE: Rendering
//
//  ROLE:
//      Provide rendering logic, draw calls, batching, or GPU resource management.
//
//  RESPONSIBILITIES:
//      - Provide Enqueue() behavior for the Rendering subsystem.
//      - Provide Clear() behavior for the Rendering subsystem.
//      - Provide GetItems() behavior for the Rendering subsystem.
//      - Provide SortByLayer() behavior for the Rendering subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.ObjectModel;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    ///<summary>
    ///Rendering queue for ordering draw calls in the rendering pipeline.
    ///Part of the rendering domain, not systems domain.
    ///</summary>
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
            //Simple sorting implementation - would need proper layer comparison in real implementation
            _queue.Sort((x, y) => 0);
        }
    }
}



