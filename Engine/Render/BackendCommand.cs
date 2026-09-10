// ====================================================================================================
//  FILE: BackendCommand.cs
//  PATH: Engine/Render
//  MODULE: Render Core – GPU Backend Commands
//
//  ROLE:
//      Immutable, deterministic GPU backend command descriptor.
//      Consumed by IGraphicsDevice to perform low‑level GPU operations.
//
//  RESPONSIBILITIES:
//      - Represent explicit GPU operations (set render target, clear, draw quad, etc.).
//      - Provide immutable, strongly‑typed command data for the GPU submission layer.
//      - Integrate cleanly with RenderCommand, RenderDevice, and IGraphicsDevice.
//      - Support deterministic sorting via SortKey.
//
//  NON-RESPONSIBILITIES:
//      - Resource loading or caching.
//      - UI layout or gameplay logic.
//      - Diagnostics, logging, or fallback behavior.
//      - CPU‑side composition or framebuffer drawing.
//
//  ARCHITECTURAL NOTES:
//      - Commands are immutable and side‑effect‑free.
//      - Enum‑driven command types ensure stable batching and deterministic ordering.
//      - Payload is strongly typed per command type (no dynamic dictionaries).
// ====================================================================================================
using System.Numerics;
using static SASZombieAssaultTD.Engine.Render.RenderEnums;

namespace SASZombieAssaultTD.Engine.Render
{


    /// <summary>
    /// Immutable backend GPU command descriptor.
    /// </summary>
    public sealed class BackendCommand
    {
        public BackendCommandType Type { get; }
        public object Payload { get; }
        public Matrix3x2 Transform { get; }
        public uint SortKey { get; }

        public BackendCommand(
            BackendCommandType type,
            object payload,
            Matrix3x2 transform,
            uint sortKey)
        {
            Type = type;
            Payload = payload;
            Transform = transform;
            SortKey = sortKey;
        }
    }
}
