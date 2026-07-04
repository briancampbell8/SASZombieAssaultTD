/*
File:    ModernUIRenderer_Elements.cs
Folder:  Engine/UI/Rendering/Modern/
Purpose:  Core UI rendering component for SAS Zombie Assault TD.
*/

//============================================================================
//File: ModernUIRenderer_Elements.cs
//Path: Engine/UI/Rendering/Modern/ModernUIRenderer_Elements.cs
//Namespace: SASZombieAssaultTD.Engine.UI.Rendering.Modern
//Program: ModernUIRenderer (Partial) — Element Rendering Subsystem
//
//PURPOSE:
//    Handles per‑element rendering logic, including material acquisition,
//    viewport culling, and command creation.
//
//RESPONSIBILITIES:
//    - Render individual UI elements
//    - Perform viewport culling
//    - Acquire or generate UIMaterial instances
//    - Emit render commands into batching or command buffer
//
//EXECUTION TRIGGERS:
//    - RenderUIElements() iterates elements and calls RenderElement()
//    - IsElementInViewport() used for culling
//    - GetElementMaterial() used for material resolution
//
//DEPENDENCIES:
//    - UIElementBase
//    - UIMaterial
//    - RenderCommand
//    - RenderCommandType
//
//CONTENTS:
//    - RenderElement()
//    - IsElementInViewport()
//    - GetElementMaterial()
//============================================================================

//
using System;
using SASZombieAssaultTD.Engine.UI.Components;
//
using SASZombieAssaultTD.Engine.Rendering;


using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.UI.Rendering.Modern
{
    public partial class ModernUIRenderer
    {
        //--------------------------------------------------------------------
        //RENDER SINGLE ELEMENT
        //--------------------------------------------------------------------
        ///<summary>
        ///Renders a single UI element by creating a RenderCommand and routing
        ///it into batching or direct command buffer execution.
        ///</summary>
        private void RenderElement(UIElementBase element)
        {
            DLogger.Log("PassThru",
                $"ModernUIRenderer: RenderElement invoked for {element?.GetType().Name ?? "NULL"}.");

            if (element == null)
            {
                DLogger.Log("Error",
                    "ModernUIRenderer: RenderElement received NULL element.");
                return;
            }

            try
            {
                //Create render command
                var command = new RenderCommand
                {
                    Type = (int)RenderCommandType.DrawElement,
                    Element = element,
                    Transform = element.Transform,
                    Material = GetElementMaterial(element)
                };

                DLogger.Log("PassThru",
                    $"ModernUIRenderer: RenderCommand created for element {element.Id}.");

                //Route command based on batching state
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
                    $"ModernUIRenderer: RenderElement failed: {ex.Message}");
                throw;
            }
        }

        //--------------------------------------------------------------------
        //VIEWPORT CULLING
        //--------------------------------------------------------------------
        ///<summary>
        ///Determines whether a UI element is inside the current viewport.
        ///</summary>
        //VIEWPORT CULLING
        //---------------------------------------------------------------
        ///<summary>
        ///Determines whether a UI element is inside the current viewport.
        ///</summary>
        private bool IsElementInViewport(UIElementBase element)
        {
            if (element == null)
                return false;

            var bounds = element.Bounds;
            var viewport = _renderContext.ViewportSize;

            bool visible =
                bounds.Right >= 0 &&
                bounds.Left <= viewport.X &&
                bounds.Bottom >= 0 &&
                bounds.Top <= viewport.Y;

            return visible;
        }


        //--------------------------------------------------------------------
        //MATERIAL ACQUISITION
        //--------------------------------------------------------------------
        ///<summary>
        ///Retrieves or constructs a UIMaterial for the given UI element.
        ///</summary>
        //MATERIAL RETRIEVAL
        //---------------------------------------------------------------
        ///<summary>
        ///Retrieves the material assigned to a UI element.
        ///</summary>
        private UIMaterial GetElementMaterial(UIElementBase element)
        {
            if (element == null)
                return null;

            //Modern pipeline: materials are managed by the UITextureAtlasManager.
            //This safely returns a default material if none is registered.
            var material = _textureAtlasManager.GetMaterialForElement(element.Id);

            return (UIMaterial)(material ?? _textureAtlasManager.DefaultMaterial);
        }

    }
}
