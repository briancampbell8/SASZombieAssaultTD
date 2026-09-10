// =====================================================================================================
//  FILE: ModernUIRendererP5.cs
//  PATH: Engine/UI/Rendering/Modern/ModernUIRendererP5.cs
//  SUBSYSTEM: Modern UI Rendering — Command Construction Subsystem (Partial)
//
//  ROLE:
//      Constructs immutable RenderCommand primitives using flattened, allocationless data.
//      Validates commands and routes them into the UIRenderCommandBuffer or active batch buffer.
//
//  RESPONSIBILITIES:
//      - Build RenderCommand structs for UI elements.
//      - Validate material and structural integrity.
//      - Route commands based on batching state.
//      - Maintain deterministic, allocationless command flow.
//
//  NON-RESPONSIBILITIES:
//      - Rendering execution (handled by UIRenderCommandBuffer).
//      - Element tree traversal (handled in ModernUIRendererP3).
//      - Resource initialization (handled in ModernUIRendererP2).
//
//  ARCHITECTURAL NOTES:
//      - Uses 'in' parameters to avoid struct copying.
//      - Must rely on canonical RenderCommand and UIMaterial definitions from engine core.
//      - Must not define engine-level types inside this partial.
//      - This partial MUST own all fields it uses (P5_* naming).
//      - No cross-partial field access is permitted.
// =====================================================================================================

using System.Collections.Generic;
using System.Numerics;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.TextureRendering;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI.Rendering.Modern
{
    public partial class ModernUIRenderer
    {
        // ----------------------------------------------------------------------------------------------
        // P5 OWNED FIELDS — NO CROSS-PARTIAL DEPENDENCIES
        // ----------------------------------------------------------------------------------------------
        private bool P5_isBatching;
        private readonly List<RenderCommand> P5_currentBatch = new();
        private UIRenderCommandBuffer P5_commandBuffer;

        // ----------------------------------------------------------------------------------------------
        // BINDING API
        // ----------------------------------------------------------------------------------------------
        public void P5_SetCommandContext(UIRenderCommandBuffer commandBuffer)
        {
            P5_commandBuffer = commandBuffer;
        }

        // ----------------------------------------------------------------------------------------------
        // COMMAND CREATION
        // ----------------------------------------------------------------------------------------------
        private RenderCommand P5_CreateCommandForElement(
            uint elementId,
            Vector3 position,
            Vector2 size,
            UIMaterial material)
        {
            if (material == null)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "Error",
                    "[ModernUIRendererP5] CreateCommandForElement aborted — NULL material.");
                return default;
            }

            return new RenderCommand(
                RenderCommandType.Rectangle,
                elementId,
                position,
                size,
                material,
                0.0f,            // depth layer
                Vector4.One,     // default color payload
                Vector2.Zero     // secondary payload
            );
        }

        // ----------------------------------------------------------------------------------------------
        // VALIDATION
        // ----------------------------------------------------------------------------------------------
        private bool P5_ValidateCommand(in RenderCommand command)
        {
            if (command.Material == null)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "Error",
                    "[ModernUIRendererP5] Validation failed — NULL material.");
                return false;
            }

            return true;
        }

        // ----------------------------------------------------------------------------------------------
        // SUBMISSION
        // ----------------------------------------------------------------------------------------------
        private void P5_SubmitCommand(in RenderCommand command)
        {
            if (!P5_ValidateCommand(in command))
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "Error",
                    "[ModernUIRendererP5] SubmitCommand aborted — validation failure.");
                return;
            }

            if (P5_isBatching)
            {
                // add a copy into the batch (safe)
                P5_currentBatch.Add(command);
                DLogger.Log(LogSubsystems.ResourcesPipeline, "Telemetry",
                    "[ModernUIRendererP5] Command cached in active batch.");
            }
            else
            {
                // create a local copy to capture in the lambda (avoids capturing the 'in' parameter)
                var cmd = command;
                P5_commandBuffer.Add(ctx => ctx.Render(cmd));
            }
        }

        // ----------------------------------------------------------------------------------------------
        // BATCH CONTROL (CALLED BY P4)
        // ----------------------------------------------------------------------------------------------
        public void P5_BeginBatch()
        {
            P5_isBatching = true;
            P5_currentBatch.Clear();
        }

        public void P5_EndBatch()
        {
            if (!P5_isBatching)
                return;

            for (int i = 0; i < P5_currentBatch.Count; i++)
            {
                // copy element to local before capturing in lambda
                var cmd = P5_currentBatch[i];
                P5_commandBuffer.Add(ctx => ctx.Render(cmd));
            }

            P5_currentBatch.Clear();
            P5_isBatching = false;
        }
    }
}
