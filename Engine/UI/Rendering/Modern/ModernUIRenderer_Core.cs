/*
File:    ModernUIRenderer_Core.cs
Folder:  Engine/Modern/
Purpose:  Core engine component for SAS Zombie Assault TD.
*/

// ============================================================================
// Program: ModernUIRenderer_Core.cs
// Project: SASZombieAssaultTD
// Role:    Core wiring for the Modern UI renderer
// ============================================================================

using SASZombieAssaultTD.Engine.Diagnostics;

using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Rendering.D3D11;
using SASZombieAssaultTD.Engine.UI.Components;
using SASZombieAssaultTD.Engine.UI.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;



namespace SASZombieAssaultTD.Engine.UI.Rendering.Modern
{
    /// <summary>
    /// Core wiring and lifecycle management for the modern UI renderer.
    /// Other behavior is implemented in the sibling partials:
    ///   - ModernUIRenderer_Batching
    ///   - ModernUIRenderer_Commands
    ///   - ModernUIRenderer_Elements
    ///   - ModernUIRenderer_Initialization
    ///   - ModernUIRenderer_Performance
    ///   - ModernUIRenderer_Resources
    /// </summary>
    public partial class ModernUIRenderer
    {
        // --------------------------------------------------------------------
        // Core dependencies (canonical, existing types)
        // --------------------------------------------------------------------

        private readonly IGraphicsDevice _graphicsDevice;
        private readonly UIRenderContext _renderContext;
        private readonly RenderCommandBuffer _commandBuffer;
        private readonly RenderPerformanceMonitor _performanceMonitor;
        private readonly UITextureAtlasManager _textureAtlasManager;
        // Stores loaded shader effects by name
        private readonly Dictionary<string, IRenderEffect> _effects =
            new Dictionary<string, IRenderEffect>();

        private UIElementBase _rootElement;
        // ================================================================
        // ModernUIRenderer core fields
        // ================================================================

        // Viewport size (matches swap chain dimensions)
        private Size2 _viewportSize = new Size2(1280, 720); // default; updated dynamically

        // Render target format (matches swap chain format)
        private const Format RenderTargetFormat = Format.B8G8R8A8_UNorm;

        // Render target collection
        private readonly Dictionary<string, ID3D11RenderTargetView> _renderTargets =
            new Dictionary<string, ID3D11RenderTargetView>();

        // --------------------------------------------------------------------
        // Construction
        // --------------------------------------------------------------------

        internal ModernUIRenderer(
            IGraphicsDevice graphicsDevice,
            UIRenderContext renderContext,
            RenderCommandBuffer commandBuffer,
            RenderPerformanceMonitor performanceMonitor,
            UITextureAtlasManager textureAtlasManager)
        {
            if (graphicsDevice == null) throw new ArgumentNullException(nameof(graphicsDevice));
            if (renderContext == null) throw new ArgumentNullException(nameof(renderContext));
            if (commandBuffer == null) throw new ArgumentNullException(nameof(commandBuffer));
            if (performanceMonitor == null) throw new ArgumentNullException(nameof(performanceMonitor));
            if (textureAtlasManager == null) throw new ArgumentNullException(nameof(textureAtlasManager));

            _graphicsDevice = graphicsDevice;
            _renderContext = renderContext;
            _commandBuffer = commandBuffer;
            _performanceMonitor = performanceMonitor;
            _textureAtlasManager = textureAtlasManager;   // ← REQUIRED
        }

        // Removed: implicit conversion operator to IRenderContext.
        // C# does not allow user-defined conversions to or from interface types.
        // Access the render context explicitly via a property or method instead.

        //   public ModernUIRenderer(D3D11DeviceCore graphicsDevice, 
        //       UIContext uiContext, 
        //       UICommandBuffer commandBuffer, 
        //       PerformanceMonitor performanceMonitor, 
        //       AtlasManager atlasManager)
        //   {
        //    }
        public IDrawingContext AsRenderContext => (IDrawingContext)_renderContext;

        //public ModernUIRenderer(D3D11UIRenderBackend uiBackend)
        // { 
        // }

        // --------------------------------------------------------------------
        // Public API
        // --------------------------------------------------------------------

        public void SetRoot(UIElementBase root)
        {
            _rootElement = root;
        }

        public void RenderFrame(float deltaTime)
        {
            if (_rootElement == null)
                return;

            BeginFrame(deltaTime);
            RenderRootElement(_rootElement, deltaTime);
            EndFrame(deltaTime);
        }

        // --------------------------------------------------------------------
        // Frame lifecycle
        // --------------------------------------------------------------------

        protected virtual void BeginFrame(float deltaTime)
        {
            _commandBuffer.Clear();
            _performanceMonitor.BeginFrame();
        }

        protected virtual void EndFrame(float deltaTime)
        {
            // Do NOT call a non‑existent Flush() here.
            _performanceMonitor.EndFrame();
        }

        protected virtual void RenderRootElement(UIElementBase root, float deltaTime)
        {
            if (root == null)
                return;

            RenderElementTree(root, deltaTime);
        }

        // --------------------------------------------------------------------
        // Extension point for element-tree rendering
        // --------------------------------------------------------------------

        // Partial method: if no implementation exists in other partials,
        // calls are compiled away with no runtime cost.
        partial void RenderElementTree(UIElementBase element, float deltaTime);

        internal void BeginFrame(D3D11RenderContextBridge d3D11RenderContextBridge)
        {
            NI.Hit();
        }

        internal void EndFrame()
        {
            NI.Hit();
        }

        internal void SetViewport(int width, int height)
        {
            NI.Hit();
        }
    }

    internal class UITextureAtlasManager
    {
        internal object DefaultMaterial;

        internal object GetMaterialForElement(string id)
        {
            return NI.Hit<object>();
        }

        internal Texture GetTexture(string name)
        {
            return NI.Hit<Texture>();
        }
    }

    internal class RenderPerformanceMonitor
    {
        internal void BeginFrame()
        {
            NI.Hit();
        }

        internal void EndFrame()
        {
            NI.Hit();
        }

        internal RenderStats GetStats()
        {
            return NI.Hit<RenderStats>();
        }
    }

    internal class RenderCommandBuffer
    {
        internal void AddCommand(RenderCommand command)
        {
            NI.Hit();
        }

        internal void Clear()
        {
            NI.Hit();
        }
    }

    internal interface IRenderEffect
    {
    }

    internal class Size2
    {
        internal object Width;
        internal object Height;

        public Size2(int v1, int v2)
        {
        }
    }
}
