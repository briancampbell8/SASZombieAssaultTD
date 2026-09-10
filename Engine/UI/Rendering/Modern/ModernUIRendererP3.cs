// =====================================================================================================
//  FILE: ModernUIRendererP3.cs
//  PATH: Engine/UI/Rendering/Modern/ModernUIRendererP3.cs
//  SUBSYSTEM: Modern UI Rendering — UI Command Submission Only (NO FRAME OWNERSHIP)
//
//  ROLE:
//      Traverses the UI element tree and submits render commands to the UI command buffer.
//      THIS PARTIAL MUST NOT OWN THE GPU FRAME.
//
//  ARCHITECTURAL RULES:
//      - NO BeginFrame()
//      - NO EndFrame()
//      - NO Present()
//      - NO Clear()
//      - NO render-target binding
//      - NO HUD/dynamic text/dynamic icon passes
//      - ONLY UI command submission
// =====================================================================================================

using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.UI.Elements;

namespace SASZombieAssaultTD.Engine.UI.Rendering.Modern
{
    public partial class ModernUIRenderer
    {
        private D3D11Adapter_Core P3_uiContext;
        private UIRenderCommandBuffer P3_commandBuffer;

        private UIElementBase? P3_rootElement;
        private System.Numerics.Vector2 P3_viewportSize = new System.Numerics.Vector2(1280, 720);
        private int P3_frameIndex;

        public void P3_SetContext(D3D11Adapter_Core uiContext, UIRenderCommandBuffer commandBuffer)
        {
            P3_uiContext = uiContext;
            P3_commandBuffer = commandBuffer;
        }

        public void P3_SetRoot(UIElementBase? root)
        {
            P3_rootElement = root;
        }

        // ----------------------------------------------------------------------------------------------
        // UI RENDER PASS — PURE COMMAND SUBMISSION (NO GPU FRAME OWNERSHIP)
        // ----------------------------------------------------------------------------------------------
        public void P3_RenderFrame(float deltaTime)
        {
            if (P3_rootElement == null)
                return;

            P3_frameIndex++;

            // Reset UI command buffer and viewport state
            P3_commandBuffer.Clear();
            P3_uiContext.Reset();

            // Traverse UI tree and submit commands
            P3_RenderElementTree(P3_rootElement, deltaTime);

            // Execute UI commands against the CURRENT frame owned by GameRootUpdateLoop
            P3_commandBuffer.ExecuteAll(P3_uiContext);
        }

        private void P3_RenderElementTree(UIElementBase root, float deltaTime)
        {
            void Traverse(UIElementBase element)
            {
                if (element == null)
                    return;

                // Skip invisible elements
                if (element.IsVisible == false)
                    return;

                // Submit render command if in viewport
                if (P6_IsElementInViewport(element))
                    P6_RenderElement(element);

                // Traverse children
                var type = element.GetType();

                object childrenObj = null;

                var prop = type.GetProperty("Children",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic);

                if (prop != null)
                    childrenObj = prop.GetValue(element);

                if (childrenObj == null)
                {
                    var method = type.GetMethod("GetChildren",
                        System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.NonPublic);

                    if (method != null)
                        childrenObj = method.Invoke(element, null);
                }

                if (childrenObj is System.Collections.IEnumerable enumerable)
                {
                    foreach (var child in enumerable)
                        if (child is UIElementBase childElement)
                            Traverse(childElement);
                }
            }

            Traverse(root);
        }

        // ----------------------------------------------------------------------------------------------
        // NO FRAME OWNERSHIP — REMOVE LEGACY RENDER LOOP
        // ----------------------------------------------------------------------------------------------
        // The old Render(D3D11Adapter_Core adapter, float v) is intentionally removed.
        // ModernUIRenderer MUST NOT call BeginFrame/EndFrame/Present.
        // ----------------------------------------------------------------------------------------------

        public void P3_SetViewport(int width, int height)
        {
            P3_viewportSize = new System.Numerics.Vector2(width, height);
            P3_uiContext.SetScreenSize(width, height);
        }
    }

    internal interface IUIComposite
    {
        IEnumerable<object> Children { get; }
    }
}
