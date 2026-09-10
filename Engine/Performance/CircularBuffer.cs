// =====================================================================================================
//  FILE: CircularBuffer.cs
//  PATH: Engine/Performance/CircularBuffer.cs
//  SUBSYSTEM: Performance
//
//  ROLE:
//      Defines the minimal deterministic lifecycle contract for any engine-hosted program.
//      This interface is implemented by engine hosts (e.g., GameRootMain) to provide a clean,
//      engine-facing API for startup, execution entry, and deterministic shutdown operations.
//
//  RESPONSIBILITIES:
//      - Provide a strict, minimal lifecycle surface for program orchestration.
//      - Enforce the structural sequencing contract: Initialize → Run Loop Execution → Shutdown.
//      - Serve as the base contract for any future top-level engine-hosted program modules.
//
//  NON-RESPONSIBILITIES:
//      - Implementing deep frame-level update calculation rules or rendering commands directly.
//      - Managing active systems registration pools, engine assets, or game states.
//      - Handling discrete hardware device allocation boundaries.
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces the legacy GameRoot partial lifecycle methods.
//      - GameRootMain implements this interface and delegates to its subsystems:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Performance
{
    // <summary>
    /// Circular buffer for performance metrics.
    /// </summary>
    public class CircularBuffer<T>
    {
        private readonly T[] _buffer;
        private int _head;
        private int _tail;
        private int _count;

        public CircularBuffer(int capacity)
        {
            _buffer = new T[capacity];
            _head = 0;
            _tail = 0;
            _count = 0;
        }

        public void Add(T item)
        {
            _buffer[_tail] = item;
            _tail = (_tail + 1) % _buffer.Length;
            if (_count < _buffer.Length)
                _count++;
        }

        public T[] GetItems()
        {
            var result = new T[_count];
            for (int i = 0; i < _count; i++)
            {
                result[i] = _buffer[(_head + i) % _buffer.Length];
            }
            return result;
        }
    }
}
