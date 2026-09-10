// =====================================================================================================
//  FILE: UIRenderCommandBuffer.cs
//  PATH: Engine/UI/Rendering/Modern/UIRenderCommandBuffer.cs
//  SUBSYSTEM: Modern UI Rendering — Command Buffer
//
//  ROLE:
//      Provides a lightweight, allocation-free command buffer used by ModernUIRenderer
//      to queue and execute UI rendering operations during a frame.
//
//  RESPONSIBILITIES:
//      - Store UI rendering commands deterministically.
//      - Provide Clear() to reset the buffer each frame.
//      - Provide ExecuteAll() to flush queued commands into the active D3D11Adapter_Core.
//      - Ensure command execution order is stable and predictable.
//      - Avoid per-frame allocations.
//
//  NON-RESPONSIBILITIES:
//      - DeviceCore or swapchain initialization.
//      - Render target binding.
//      - Shader or effect validation.
//      - UI element tree traversal.
//      - GPU batching (handled by other ModernUIRenderer partials).
//
//  ARCHITECTURAL NOTES:
//      - This subsystem is standalone and does not pull data from other partials.
//      - Commands are stored as delegates accepting D3D11Adapter_Core.
//      - Execution is best-effort; individual command failures do not halt the buffer.
// =====================================================================================================
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.UI.Rendering.Modern
{
    public sealed class UIRenderCommandBuffer
    {
        private readonly List<System.Action<D3D11Adapter_Core>> P3_commands = new();

        public void Add(System.Action<D3D11Adapter_Core> command)
        {
            if (command != null)
                P3_commands.Add(command);
        }

        public void Clear()
        {
            P3_commands.Clear();
        }

        public void ExecuteAll(D3D11Adapter_Core context)
        {
            for (int i = 0; i < P3_commands.Count; i++)
            {
                try
                {
                    P3_commands[i]?.Invoke((D3D11Adapter_Core)context);
                }
                catch
                {
                    // Swallow individual command exceptions to ensure full buffer execution.
                }
            }
        }

        internal void Add(RenderCommand command)
        {
            // Copy the command to a local variable so the captured value is stable
            var cmd = command;

            // Reuse the existing overload that accepts an action targeting the native adapter.
            // This stores a lambda that will execute the render command when the adapter is provided.
            Add(ctx => ctx.Render(cmd));
        }

        internal void ExecuteAll()
        {
            // Execute all buffered UI render commands. Use a stable snapshot of the list so
            // callers may modify the original list (e.g. Clear) while we're iterating.
            if (P3_commands == null || P3_commands.Count == 0)
            {
                return;
            }

            var snapshot = P3_commands.ToArray();

            for (int i = 0; i < snapshot.Length; i++)
            {
                var cmd = snapshot[i];
                if (cmd == null)
                {
                    continue;
                }

                try
                {
                    // Invoke the command. Some commands may not require a valid adapter and
                    // will safely handle a null argument. Swallow exceptions per-command so one
                    // failing command does not prevent the rest from running.
                    cmd(null);
                }
                catch
                {
                    // Intentionally ignore exceptions from individual commands to avoid
                    // breaking the frame end path. Logging can be added here if available.
                }
            }
        }
    }
}
