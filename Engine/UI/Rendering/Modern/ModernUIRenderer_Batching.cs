/*
File:    ModernUIRenderer_Batching.cs
Folder:  Engine/UI/Rendering/Modern/
Purpose:  Core UI rendering component for SAS Zombie Assault TD.
*/

//============================================================================
//File: ModernUIRenderer_Batching.cs
//Path: Engine/UI/Rendering/Modern/ModernUIRenderer_Batching.cs
//Namespace: SASZombieAssaultTD.Engine.UI.Rendering.Modern
//Program: ModernUIRenderer (Partial) — Batching Subsystem
//
//PURPOSE:
//    Implements GPU‑optimized batching for UI rendering. Accumulates render
//    commands, sorts them to minimize GPU state changes, and commits them to
//    the command buffer.
//
//RESPONSIBILITIES:
//    - Begin and end batching sessions
//    - Accumulate render commands during batching
//    - Sort commands by material/element to reduce GPU state switches
//    - Commit optimized commands to the RenderCommandBuffer
//
//EXECUTION TRIGGERS:
//    - RenderUIElements() calls BeginBatch() and EndBatch()
//    - RenderElement() adds commands to the batch when batching is active
//
//DEPENDENCIES:
//    - RenderCommandBuffer
//    - RenderCommand
//    - UIMaterial
//    - UIElementBase
//
//CONTENTS:
//    - BeginBatch()
//    - EndBatch()
//    - OptimizeBatchCommands()
//============================================================================

//
//
using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Scenes.Battlefields;
namespace SASZombieAssaultTD.Engine.UI.Rendering.Modern
{
    public partial class ModernUIRenderer
    {
        //--------------------------------------------------------------------
        //PRIVATE FIELDS — BATCHING STATE
        //--------------------------------------------------------------------
        private bool _isBatching = false;
        private readonly List<RenderCommand> _currentBatch = new();

        //--------------------------------------------------------------------
        //BEGIN BATCHING
        //--------------------------------------------------------------------
        ///<summary>
        ///Begins batch rendering mode. Clears previous batch and prepares to
        ///accumulate render commands for GPU‑optimized execution.
        ///</summary>
        private void BeginBatch()
        {
            if (!_isBatching)
            {
                _isBatching = true;
                _currentBatch.Clear();

                Dlogger.Log("Debug",
                    "ModernUIRenderer: Batch started.");
            }
        }

        //--------------------------------------------------------------------
        //END BATCHING
        //--------------------------------------------------------------------
        ///<summary>
        ///Ends batching mode. Sorts accumulated commands to minimize GPU state
        ///changes, then commits them to the command buffer.
        ///</summary>
        private void EndBatch()
        {
            if (_isBatching)
            {
                try
                {
                    OptimizeBatchCommands();

                    //Commit optimized commands to the command buffer
                    foreach (var command in _currentBatch)
                        _commandBuffer.AddCommand(command);

                    Dlogger.Log("Debug",
                        $"ModernUIRenderer: Batch executed with {_currentBatch.Count} commands.");
                }
                catch (Exception ex)
                {
                    Dlogger.Log("Error",
                        $"ModernUIRenderer: Batch execution failed: {ex.Message}");
                    throw;
                }
                finally
                {
                    _isBatching = false;
                    _currentBatch.Clear();
                }
            }
        }

        //--------------------------------------------------------------------
        //OPTIMIZE BATCH COMMANDS
        //--------------------------------------------------------------------
        ///<summary>
        ///Sorts batched commands by material and element identity to reduce
        ///GPU state changes and improve rendering throughput.
        ///</summary>
        private void OptimizeBatchCommands()
        {
            try
            {
                _currentBatch.Sort((a, b) =>
                {
                    //Primary sort: material identity
                    int materialCompare =
                        a.Material.GetHashCode().CompareTo(b.Material.GetHashCode());

                    if (materialCompare != 0)
                        return materialCompare;

                    //Secondary sort: element identity
                    return a.Element.GetHashCode().CompareTo(b.Element.GetHashCode());
                });

                Dlogger.Log("Debug",
                    "ModernUIRenderer: Batch optimized.");
            }
            catch (Exception ex)
            {
                Dlogger.Log("Error",
                    $"ModernUIRenderer: Batch optimization failed: {ex.Message}");
                throw;
            }
        }
    }
}
