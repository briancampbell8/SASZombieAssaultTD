// =====================================================================================================
//  FILE: ModernUIRendererP4.cs
//  PATH: Engine/UI/Rendering/Modern/ModernUIRendererP4.cs
//  SUBSYSTEM: Modern UI Rendering — GPU Batching (Partial)
//
//  ROLE:
//      Provides the deterministic batching subsystem for ModernUIRenderer.
//      Accumulates UI render primitives, sorts them to minimize GPU state changes,
//      and commits them to the UI command buffer.
//
//  RESPONSIBILITIES:
//      - Begin batching sessions without generating garbage.
//      - Accumulate batched UI primitives deterministically.
//      - Sort batched commands by material and element identity.
//      - Commit optimized batch commands to the UIRenderCommandBuffer.
//      - Maintain stable batching order.
//
//  NON-RESPONSIBILITIES:
//      - Initialization logic (handled in ModernUIRendererP2).
//      - Frame scheduling or UI tree traversal (handled in ModernUIRendererP3).
//      - Dynamic overlay rendering (handled in ModernUIRendererP1).
//      - GPU resource creation or shader validation.
//
//  ARCHITECTURAL NOTES:
//      - This partial MUST NOT define constructors.
//      - This partial MUST own all fields it uses (P4_* naming).
//      - No cross-partial field access is permitted.
//      - Batching is synchronous and allocation-free.
// =====================================================================================================

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI.Rendering.Modern
{
    public partial class ModernUIRenderer
    {
        // ----------------------------------------------------------------------------------------------
        // P4 OWNED FIELDS — NO CROSS-PARTIAL DEPENDENCIES
        // ----------------------------------------------------------------------------------------------
        private bool P4_isBatching;
        private readonly List<UIBatchCommand> P4_currentBatch = new();
        private UIRenderCommandBuffer P4_commandBuffer;

        // ----------------------------------------------------------------------------------------------
        // BINDING API
        // ----------------------------------------------------------------------------------------------
        public void P4_SetBatchContext(UIRenderCommandBuffer commandBuffer)
        {
            P4_commandBuffer = commandBuffer ?? throw new ArgumentNullException(nameof(commandBuffer));
        }

        // ----------------------------------------------------------------------------------------------
        // BEGIN BATCHING
        // ----------------------------------------------------------------------------------------------
        private void P4_BeginBatch()
        {
            if (P4_isBatching)
                return;

            P4_isBatching = true;
            P4_currentBatch.Clear();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Telemetry",
                "[ModernUIRendererP4] Batch session opened.");
        }

        // ----------------------------------------------------------------------------------------------
        // END BATCHING
        // ----------------------------------------------------------------------------------------------
        private void P4_EndBatch()
        {
            if (!P4_isBatching)
                return;

            if (P4_commandBuffer == null)
                return;

            try
            {
                P4_OptimizeBatchCommands();

                for (int i = 0; i < P4_currentBatch.Count; i++)
                {
                    P4_commandBuffer.Add((
                        Action<Render.D3D11.Adapter.D3D11Adapter_Core>)P4_currentBatch[i].Execute);
                }

                DLogger.Log(LogSubsystems.ResourcesPipeline, "Telemetry",
                    "[ModernUIRendererP4] Batch flushed to pipeline.");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "Error",
                    $"[ModernUIRendererP4] EndBatch failure: {ex.Message}");
                throw;
            }
            finally
            {
                P4_isBatching = false;
                P4_currentBatch.Clear();
            }
        }

        // ----------------------------------------------------------------------------------------------
        // OPTIMIZE BATCH COMMANDS
        // ----------------------------------------------------------------------------------------------
        private void P4_OptimizeBatchCommands()
        {
            try
            {
                if (P4_currentBatch.Count <= 1)
                    return;

                P4_currentBatch.Sort((a, b) =>
                {
                    int materialCompare = a.MaterialId.CompareTo(b.MaterialId);
                    if (materialCompare != 0)
                        return materialCompare;

                    return a.ElementId.CompareTo(b.ElementId);
                });

                DLogger.Log(LogSubsystems.ResourcesPipeline, "Telemetry",
                    "[ModernUIRendererP4] Batch optimized.");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "Error",
                    $"[ModernUIRendererP4] OptimizeBatchCommands aborted: {ex.Message}");
                throw;
            }
        }
    }

    // ----------------------------------------------------------------------------------------------
    // BATCH COMMAND STRUCT — SELF-CONTAINED
    // ----------------------------------------------------------------------------------------------
    public readonly struct UIBatchCommand
    {
        public readonly int MaterialId;
        public readonly int ElementId;
        public readonly Action<D3D11Adapter_Core> Execute;

        public UIBatchCommand(int materialId, int elementId, Action<D3D11Adapter_Core> execute)
        {
            MaterialId = materialId;
            ElementId = elementId;
            Execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }
    }
}
