/*
File:    ModernUIRenderer_Commands.cs
Folder:  Engine/UI/Rendering/Modern/
Purpose:  Core UI rendering component for SAS Zombie Assault TD.
*/

//============================================================================
//File: ModernUIRenderer_Commands.cs
//Path: Engine/UI/Rendering/Modern/ModernUIRenderer_Commands.cs
//Namespace: SASZombieAssaultTD.Engine.UI.Rendering.Modern
//Program: ModernUIRenderer (Partial) — Command Construction Subsystem
//
//PURPOSE:
//    Provides helper methods for constructing, validating, and routing render
//    commands into the batching system or directly into the command buffer.
//
//RESPONSIBILITIES:
//    - Create RenderCommand instances
//    - Validate command data
//    - Route commands to batch or buffer
//    - Emit pass‑thru diagnostics for every command operation
//
//EXECUTION TRIGGERS:
//    - RenderElement() calls CreateCommandForElement()
//    - Internal systems may request direct command creation
//
//DEPENDENCIES:
//    - RenderCommand
//    - RenderCommandType
//    - UIElementBase
//    - UIMaterial
//
//CONTENTS:
//    - CreateCommandForElement()
//    - ValidateCommand()
//    - SubmitCommand()
//============================================================================

//
//
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.UI.Components;
using System;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.UI.Rendering.Modern
{
    public partial class ModernUIRenderer
    {
        //--------------------------------------------------------------------
        //CREATE COMMAND FOR ELEMENT
        //--------------------------------------------------------------------
        ///<summary>
        ///Creates a RenderCommand for a UI element with full diagnostics.
        ///</summary>
        private RenderCommand CreateCommandForElement(UIElementBase element, UIMaterial material)
        {
            DLogger.Log("ModernUIRenderer", $"CreateCommandForElement invoked for element " +
                $"{(element == null ? "null" : element.Id.ToString())}."
);


            if (element == null)
            {
                DLogger.Log("Error",
                    "ModernUIRenderer: CreateCommandForElement received NULL element.");
                return null;
            }

            if (material == null)
            {
                DLogger.Log("Error",
                    $"ModernUIRenderer: Element {element.Id} has NULL material.");
                return null;
            }

            try
            {
                var command = new RenderCommand
                {
                    Type = (int)RenderCommandType.DrawElement,
                    Element = element,
                    Material = material,
                    Transform = element.Transform
                };

                DLogger.Log("PassThru",
                    $"ModernUIRenderer: RenderCommand created for element {element.Id}.");

                return command;
            }
            catch (Exception ex)
            {
                DLogger.Log("Error",
                    $"ModernUIRenderer: CreateCommandForElement failed: {ex.Message}");
                throw;
            }
        }

        //--------------------------------------------------------------------
        //VALIDATE COMMAND
        //--------------------------------------------------------------------
        ///<summary>
        ///Ensures a RenderCommand contains valid data before submission.
        ///</summary>
        private bool ValidateCommand(RenderCommand command)
        {
            DLogger.Log("PassThru",
                "ModernUIRenderer: ValidateCommand invoked.");

            if (command == null)
            {
                DLogger.Log("Error",
                    "ModernUIRenderer: ValidateCommand received NULL command.");
                return false;
            }

            if (command.Element == null)
            {
                DLogger.Log("Error",
                    "ModernUIRenderer: Command has NULL element.");
                return false;
            }

            if (command.Material == null)
            {
                DLogger.Log("Error",
                    $"ModernUIRenderer: Command for element {command.Element} has NULL material.");
                return false;
            }

            DLogger.Log("PassThru",
                $"ModernUIRenderer: Command for element {command.Element} validated successfully.");

            return true;
        }

        //--------------------------------------------------------------------
        //SUBMIT COMMAND
        //--------------------------------------------------------------------
        ///<summary>
        ///Routes a validated RenderCommand into batching or directly into the
        ///command buffer.
        ///</summary>
        private void SubmitCommand(RenderCommand command)
        {
            DLogger.Log("PassThru",
                "ModernUIRenderer: SubmitCommand invoked.");

            if (!ValidateCommand(command))
            {
                DLogger.Log("Error",
                    "ModernUIRenderer: SubmitCommand aborted — command invalid.");
                return;
            }

            try
            {
                if (_isBatching)
                {
                    _currentBatch.Add(command);

                    DLogger.Log("PassThru",
                        $"ModernUIRenderer: Command added to batch. Batch size now {_currentBatch.Count}.");
                }
                else
                {
                    _commandBuffer.AddCommand(command);

                    DLogger.Log("PassThru",
                        "ModernUIRenderer: Command added directly to command buffer.");
                }
            }
            catch (Exception ex)
            {
                DLogger.Log("Error",
                    $"ModernUIRenderer: SubmitCommand failed: {ex.Message}");
                throw;
            }
        }
    }
}
