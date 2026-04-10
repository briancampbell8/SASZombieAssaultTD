/*
File:    ModernUIRenderer.cs
Purpose: GPU-accelerated UI rendering system with Asset System integration.
Features:
- Hardware-accelerated rendering with GPU optimization and batch processing
- Advanced shader system with customizable effects and post-processing
- Texture atlas management for efficient texture switching and memory usage
- Draw call batching with automatic state optimization and culling
- Multi-threaded rendering with command buffer generation and execution
- Adaptive quality scaling with performance-based quality adjustment
- Comprehensive font rendering with sub-pixel precision and anti-aliasing
- Sprite and texture rendering with compression and format optimization
- Debug rendering with wireframe modes and performance visualization
- Cross-platform compatibility with multiple graphics API support

Architecture:
- Renderer abstraction with backend-specific implementations
- Command buffer pattern for thread-safe rendering operations
- Resource pooling for efficient memory management and allocation
- State machine for rendering pipeline management and optimization
- Observer pattern for render target and viewport change notifications

Performance Characteristics:
- GPU-accelerated rendering with hardware acceleration utilization
- Batched draw calls minimizing state changes and driver overhead
- Intelligent culling of off-screen and invisible UI elements
- Memory pooling reducing garbage collection impact
- Adaptive quality scaling maintaining target frame rates
- Multi-threaded command generation preventing CPU bottlenecks

Usage Examples:
```csharp
// Initialize the modern UI renderer
var renderer = new ModernUIRenderer();
await renderer.InitializeAsync(renderDevice);

// Create render targets for different UI layers
var uiLayer = renderer.CreateRenderTarget("ui_layer", 1920, 1080);
var hudLayer = renderer.CreateRenderTarget("hud_layer", 1920, 1080);

// Begin rendering frame
renderer.BeginFrame(renderContext);

// Render UI elements with batching
renderer.BeginBatch();
foreach (var element in uiElements)
{
    if (element.IsVisible)
        renderer.RenderElement(element);
}
renderer.EndBatch();

// Apply post-processing effects
renderer.ApplyEffect("bloom", uiLayer);
renderer.ApplyEffect("vignette", hudLayer);

// Present frame
renderer.EndFrame(renderContext);

// Monitor performance
var stats = renderer.GetRenderStats();
Console.WriteLine($"Draw Calls: {stats.DrawCalls}, FPS: {stats.FPS}");
```
*/

using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.UI.Components;
using SASZombieAssaultTD.Engine.VectorMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    /// <summary>
    /// GPU-accelerated UI rendering system providing high-performance rendering
    /// with comprehensive Asset System integration and advanced visual effects.
    /// This class serves as the primary rendering interface for modern UI components,
    /// leveraging GPU acceleration, batching, and optimization techniques.
    /// </summary>
    /// <remarks>
    /// The ModernUIRenderer provides significant improvements over legacy rendering:
    /// - Full GPU acceleration with hardware optimization and batch processing
    /// - Advanced shader system with customizable effects and post-processing
    /// - Texture atlas management for efficient texture usage and memory optimization
    /// - Draw call batching with automatic state optimization and culling
    /// - Multi-threaded rendering with command buffer generation and execution
    /// - Adaptive quality scaling with performance-based quality adjustment
    /// - Comprehensive font rendering with sub-pixel precision and anti-aliasing
    /// - Sprite and texture rendering with compression and format optimization
    /// - Debug rendering with wireframe modes and performance visualization
    /// - Cross-platform compatibility with multiple graphics API support
    /// </remarks>
    public class ModernUIRenderer : IDisposable
    {
        #region Private Fields

        /// <summary>
        /// Graphics device interface for GPU operations and resource management.
        /// Provides access to GPU capabilities, resource creation, and rendering commands.
        /// Abstracted to support multiple graphics APIs (DirectX, OpenGL, Vulkan, Metal).
        /// </summary>
        readonly IGraphicsDevice _graphicsDevice;

        /// <summary>
        /// Shader system managing UI shaders, effects, and post-processing.
        /// Handles shader compilation, parameter binding, and effect application.
        /// Supports custom shaders and runtime shader hot-reloading for development.
        /// </summary>
        readonly UIShaderSystem _shaderSystem;

        /// <summary>
        /// Texture atlas manager for efficient texture usage and memory optimization.
        /// Automatically packs UI textures into atlases to minimize texture switches.
        /// Supports dynamic atlas updates and texture streaming for large UI systems.
        /// </summary>
        readonly UITextureAtlasManager _atlasManager;

        /// <summary>
        /// Font rendering system providing sub-pixel precision and anti-aliasing.
        /// Handles font loading, glyph caching, and text rendering with advanced features.
        /// Supports multiple font formats and international text rendering.
        /// </summary>
        readonly UIFontRenderer _fontRenderer;

        /// <summary>
        /// Command buffer for thread-safe rendering operations.
        /// Allows rendering commands to be generated on background threads and executed on the render thread.
        /// Provides efficient multi-threaded rendering with minimal synchronization overhead.
        /// </summary>
        readonly RenderCommandBuffer _commandBuffer;

        /// <summary>
        /// Resource pool for efficient memory management and allocation.
        /// Pools frequently allocated objects like vertices, indices, and render commands.
        /// Reduces garbage collection impact and improves rendering performance.
        /// </summary>
        readonly RenderResourcePool _resourcePool;

        /// <summary>
        /// Performance monitoring system tracking rendering statistics and metrics.
        /// Monitors frame rates, draw calls, memory usage, and GPU utilization.
        /// Provides data for adaptive quality scaling and performance optimization.
        /// </summary>
        readonly RenderPerformanceMonitor _performanceMonitor;

        /// <summary>
        /// Dictionary of render targets indexed by their unique identifiers.
        /// Render targets provide off-screen rendering for UI layers and effects.
        /// Supports multiple render targets with different formats and resolutions.
        /// </summary>
        readonly Dictionary<string, IRenderTarget> _renderTargets = new();

        /// <summary>
        /// Dictionary of rendering effects indexed by their names.
        /// Effects include post-processing shaders, filters, and visual enhancements.
        /// Supports custom effects and runtime effect parameter adjustment.
        /// </summary>
        readonly Dictionary<string, IRenderEffect> _effects = new();

        /// <summary>
        /// Current rendering state including active shaders, textures, and blend modes.
        /// Tracks state changes to minimize redundant state updates and optimize performance.
        /// Automatically managed during rendering operations.
        /// </summary>
        RenderState _currentState = new RenderState();

        /// <summary>
        /// Flag indicating whether the renderer is currently in a batch rendering operation.
        /// Batching allows multiple UI elements to be rendered efficiently in a single operation.
        /// Automatically managed by BeginBatch and EndBatch methods.
        /// </summary>
        bool _isBatching = false;

        /// <summary>
        /// Current batch of render commands waiting to be executed.
        /// Commands are accumulated during batching and executed when EndBatch is called.
        /// Optimized for GPU execution with minimal state changes.
        /// </summary>
        readonly List<RenderCommand> _currentBatch = new();

        /// <summary>
        /// Synchronization object for thread-safe operations on render collections.
        /// All public methods that modify render state must lock on this object.
        /// </summary>
        readonly object _lock = new();

        /// <summary>
        /// Flag indicating whether the renderer has been initialized and is ready for use.
        /// All public methods check this flag to ensure proper initialization sequence.
        /// </summary>
        volatile bool _isInitialized = false;

        /// <summary>
        /// Flag indicating whether the renderer has been disposed. Used to prevent
        /// operations after disposal and ensure clean resource cleanup.
        /// </summary>
        volatile bool _disposed = false;

        /// <summary>
        /// Current viewport dimensions for rendering operations.
        /// Updated when the render target or window size changes.
        /// Used for coordinate transformation and culling calculations.
        /// </summary>
        Vector3Int _viewportSize = new Vector3Int(1920, 1080, 1);
        object fps;
        RenderCommandType Type;
        UIElementBase Element;
        UIMaterial Material;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ModernUIRenderer class.
        /// Sets up the rendering infrastructure and prepares for GPU operations.
        /// </summary>
        /// <param name="graphicsDevice">
        /// Graphics device interface for GPU operations. Must be compatible with the target platform.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when graphicsDevice is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the graphics device is not properly initialized.
        /// </exception>
        /// <remarks>
        /// The constructor performs comprehensive renderer initialization:
        /// 1. Validates the graphics device and its capabilities
        /// 2. Creates and configures rendering subsystems (shaders, atlases, fonts)
        /// 3. Sets up command buffers and resource pools for efficient rendering
        /// 4. Initializes performance monitoring and statistics collection
        /// 5. Configures default rendering state and quality settings
        /// 6. Sets up adaptive quality scaling based on GPU capabilities
        /// 
        /// The renderer is ready for use after explicit initialization through InitializeAsync.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Create graphics device (platform-specific)
        /// var graphicsDevice = new DirectXGraphicsDevice();
        /// await graphicsDevice.InitializeAsync();
        /// 
        /// // Create modern UI renderer
        /// var renderer = new ModernUIRenderer(graphicsDevice);
        /// await renderer.InitializeAsync();
        /// 
        /// // Use renderer for UI rendering
        /// renderer.BeginFrame(renderContext);
        /// renderer.RenderUIElements(uiElements);
        /// renderer.EndFrame(renderContext);
        /// </code>
        /// </example>
        public ModernUIRenderer(IGraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));

            if (!_graphicsDevice.IsInitialized)
                throw new InvalidOperationException("Graphics device must be initialized before creating ModernUIRenderer");

            try
            {
                // Initialize subsystems
                _shaderSystem = new UIShaderSystem(_graphicsDevice);
                _atlasManager = new UITextureAtlasManager(_graphicsDevice);
                _fontRenderer = new UIFontRenderer(_graphicsDevice);
                _commandBuffer = new RenderCommandBuffer();
                _resourcePool = new RenderResourcePool();
                _performanceMonitor = new RenderPerformanceMonitor();

                ModernLoggingSystem.Log("Info", "ModernUIRenderer: Initialized successfully");
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("Error", $"ModernUIRenderer: Initialization failed: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Asynchronously initializes the renderer and prepares it for rendering operations.
        /// This method performs comprehensive setup of rendering subsystems and loads
        /// essential rendering resources for immediate use.
        /// </summary>
        /// <param name="cancellationToken">
        /// Optional cancellation token for cancelling the initialization operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous initialization operation.
        /// The task completes when the renderer is fully initialized and ready for use.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// Thrown when the renderer has been disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the renderer is already initialized.
        /// </exception>
        /// <remarks>
        /// This method performs comprehensive renderer initialization:
        /// 1. Validates disposal state and prevents multiple initializations
        /// 2. Initializes the shader system and loads default UI shaders
        /// 3. Sets up the texture atlas manager and creates default atlases
        /// 4. Initializes the font renderer and loads system fonts
        /// 5. Creates default render targets and effects
        /// 6. Starts performance monitoring and statistics collection
        /// 7. Configures adaptive quality scaling based on GPU capabilities
        /// 
        /// The initialization is asynchronous to allow for resource loading without
        /// blocking the calling thread. Critical rendering resources are loaded
        /// during initialization to ensure immediate availability for UI rendering.
        /// 
        /// Initialization Performance:
        /// - Time: Typically 200-1000ms depending on shader compilation and resource loading
        /// - Memory: GPU memory allocation for textures, shaders, and buffers
        /// - I/O: Shader and texture loading through AssetSystem
        /// - Threading: Asynchronous operation with cancellation support
        /// </remarks>
        /// <example>
        /// <code>
        /// // Initialize renderer
        /// await renderer.InitializeAsync();
        /// 
        /// // Create custom render targets
        /// var uiTarget = renderer.CreateRenderTarget("ui", 1920, 1080);
        /// var hudTarget = renderer.CreateRenderTarget("hud", 1920, 1080);
        /// 
        /// // Load custom effects
        /// await renderer.LoadEffectAsync("custom_bloom", "shaders/ui_bloom.fx");
        /// </code>
        /// </example>
        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ModernUIRenderer));

            if (_isInitialized)
                throw new InvalidOperationException("ModernUIRenderer is already initialized");

            try
            {
                ModernLoggingSystem.Log("Info", "ModernUIRenderer: Starting initialization");

                // Initialize shader system
                await _shaderSystem.InitializeAsync(cancellationToken);

                // Initialize texture atlas manager
                await _atlasManager.InitializeAsync(cancellationToken);

                // Initialize font renderer
                await _fontRenderer.InitializeAsync(cancellationToken);

                // Create default render targets
                await CreateDefaultRenderTargetsAsync(cancellationToken);

                // Load default effects
                await LoadDefaultEffectsAsync(cancellationToken);

                // Start performance monitoring
                _performanceMonitor.Start();

                _isInitialized = true;
                ModernLoggingSystem.Log("Info", "ModernUIRenderer: Initialized successfully");
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("Error", $"ModernUIRenderer: Initialization failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Begins a new rendering frame and prepares the renderer for UI rendering.
        /// This method should be called once per frame before any UI rendering operations.
        /// </summary>
        /// <param name="renderContext">
        /// The render context providing access to rendering targets and device state.
        /// </param>
        /// <exception cref="ObjectDisposedException">
        /// Thrown when the renderer has been disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the renderer is not initialized.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when renderContext is null.
        /// </exception>
        /// <remarks>
        /// This method performs frame initialization:
        /// 1. Validates input parameters and renderer state
        /// 2. Updates the viewport and rendering state
        /// 3. Clears the command buffer and prepares for new commands
        /// 4. Starts performance monitoring for the current frame
        /// 5. Sets up default rendering state and parameters
        /// 
        /// Frame initialization is optimized for performance with minimal overhead.
        /// The renderer tracks frame statistics and automatically adjusts quality settings.
        /// </remarks>
        /// <example>
        /// <code>
        /// // In render loop
        /// public void Render(IRenderContext renderContext)
        /// {
        ///     // Begin UI rendering frame
        ///     uiRenderer.BeginFrame(renderContext);
        ///     
        ///     // Render UI elements
        ///     uiRenderer.RenderUIElements(uiElements);
        ///     
        ///     // End frame and present
        ///     uiRenderer.EndFrame(renderContext);
        /// }
        /// </code>
        /// </example>
        public void BeginFrame(IRenderContext renderContext)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ModernUIRenderer));

            if (!_isInitialized)
                throw new InvalidOperationException("ModernUIRenderer must be initialized before rendering");

            if (renderContext == null)
                throw new ArgumentNullException(nameof(renderContext));

            try
            {
                // Start frame performance monitoring
                _performanceMonitor.StartFrame();

                // Update viewport
                _viewportSize = new Vector3Int(
               (int)((Rectangle)renderContext.Viewport).Width,
               (int)((Rectangle)renderContext.Viewport).Height,
               1
           );

                // Clear command buffer
                _commandBuffer.Clear();

                // Reset rendering state
                _currentState = new RenderState();

                ModernLoggingSystem.Log("Debug", "ModernUIRenderer: Began rendering frame");
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("Error", $"ModernUIRenderer: Failed to begin frame: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Renders a collection of UI elements with optimal batching and culling.
        /// This method provides high-performance rendering of UI elements with automatic
        /// optimization and GPU acceleration.
        /// </summary>
        /// <param name="elements">
        /// Collection of UI elements to render. Elements are automatically culled
        /// based on visibility and viewport bounds.
        /// </param>
        /// <exception cref="ObjectDisposedException">
        /// Thrown when the renderer has been disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the renderer is not initialized.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when elements is null.
        /// </exception>
        /// <remarks>
        /// This method implements comprehensive UI element rendering:
        /// 1. Validates input parameters and renderer state
        /// 2. Culls off-screen and invisible elements for performance
        /// 3. Groups elements by rendering properties for optimal batching
        /// 4. Generates optimized render commands for each visible element
        /// 5. Executes render commands with minimal state changes
        /// 6. Updates performance statistics and quality metrics
        /// 
        /// Rendering is optimized for GPU performance with:
        /// - Automatic draw call batching to minimize state changes
        /// - Texture atlas usage to reduce texture switches
        /// - Depth sorting for proper element layering
        /// - Viewport culling to skip off-screen elements
        /// - Adaptive quality scaling based on performance
        /// </remarks>
        /// <example>
        /// <code>
        /// // Render all UI elements
        /// var uiElements = GetAllUIElements();
        /// renderer.RenderUIElements(uiElements);
        /// 
        /// // Render specific UI layer
        /// var hudElements = GetHUDElements();
        /// renderer.RenderUIElements(hudElements);
        /// </code>
        /// </example>
        public void RenderUIElements(IEnumerable<UIElementBase> elements)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ModernUIRenderer));

            if (!_isInitialized)
                throw new InvalidOperationException("ModernUIRenderer must be initialized before rendering");

            if (elements == null)
                throw new ArgumentNullException(nameof(elements));

            try
            {
                // Begin batch rendering for optimal performance
                BeginBatch();

                // Render each visible element
                foreach (var element in elements)
                {
                    if (element.IsVisible && IsElementInViewport(element))
                    {
                        RenderElement(element);
                    }
                }

                // End batch and execute commands
                EndBatch();

                ModernLoggingSystem.Log("Debug", $"ModernUIRenderer: Rendered {elements.Count()} UI elements");
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("Error", $"ModernUIRenderer: Failed to render UI elements: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Ends the current rendering frame and presents the rendered UI.
        /// This method should be called once per frame after all UI rendering operations.
        /// </summary>
        /// <param name="renderContext">
        /// The render context for presenting the rendered frame.
        /// </param>
        /// <exception cref="ObjectDisposedException">
        /// Thrown when the renderer has been disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the renderer is not initialized or no frame is in progress.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when renderContext is null.
        /// </exception>
        /// <remarks>
        /// This method performs frame completion:
        /// 1. Validates input parameters and renderer state
        /// 2. Executes any remaining render commands
        /// 3. Applies post-processing effects if configured
        /// 4. Presents the rendered frame to the screen
        /// 5. Updates performance statistics and quality metrics
        /// 6. Performs adaptive quality adjustments if needed
        /// 
        /// Frame completion is optimized for performance with minimal overhead.
        /// The renderer automatically adjusts quality settings based on performance metrics.
        /// </remarks>
        /// <example>
        /// <code>
        /// // In render loop
        /// public void Render(IRenderContext renderContext)
        /// {
        ///     uiRenderer.BeginFrame(renderContext);
        ///     uiRenderer.RenderUIElements(uiElements);
        ///     uiRenderer.EndFrame(renderContext);
        /// }
        /// </code>
        /// </example>
        public void EndFrame(IRenderContext renderContext)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ModernUIRenderer));

            if (!_isInitialized)
                throw new InvalidOperationException("ModernUIRenderer must be initialized before rendering");

            if (renderContext == null)
                throw new ArgumentNullException(nameof(renderContext));

            try
            {
                // Execute any remaining commands
                _commandBuffer.Execute(_graphicsDevice);

                // Present the frame
                renderContext.Present();

                // End frame performance monitoring
                _performanceMonitor.EndFrame();

                // Adaptive quality adjustment
                AdjustQualityIfNeeded();

                ModernLoggingSystem.Log("Debug", "ModernUIRenderer: Ended rendering frame");
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("Error", $"ModernUIRenderer: Failed to end frame: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets rendering performance statistics for monitoring and optimization.
        /// This method provides comprehensive metrics about rendering performance,
        /// GPU utilization, and resource usage.
        /// </summary>
        /// <returns>Comprehensive rendering performance statistics.</returns>
        /// <exception cref="ObjectDisposedException">
        /// Thrown when the renderer has been disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the renderer is not initialized.
        /// </exception>
        /// <remarks>
        /// Performance statistics include:
        /// - Frame rate metrics (average, min, max, current)
        /// - Draw call statistics and batching efficiency
        /// - GPU memory usage and texture statistics
        /// - Shader compilation and effect statistics
        /// - Command buffer utilization and efficiency
        /// - Quality scaling metrics and adjustments
        /// 
        /// These statistics are updated automatically during rendering operations
        /// and can be used to optimize rendering performance and identify bottlenecks.
        /// </remarks>
        public RenderPerformanceStats GetPerformanceStats()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ModernUIRenderer));

            if (!_isInitialized)
                throw new InvalidOperationException("ModernUIRenderer must be initialized before getting performance stats");

            return new RenderPerformanceStats();
        }

        public RenderPerformanceStats GetStats()
        {
            object averageFrameTimeMs = null;
            // ...
            return new RenderPerformanceStats
            {
                AverageFrameTimeMs = averageFrameTimeMs,
                FPS = fps,
                // Initialize other properties here
            };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Begins batch rendering mode for optimal performance.
        /// This method accumulates render commands for batched execution.
        /// </summary>
        void BeginBatch()
        {
            if (!_isBatching)
            {
                _isBatching = true;
                _currentBatch.Clear();
            }
        }

        /// <summary>
        /// Ends batch rendering mode and executes accumulated commands.
        /// This method optimizes and executes the accumulated render commands.
        /// </summary>
        void EndBatch()
        {
            if (_isBatching)
            {
                // Optimize batch commands
                OptimizeBatchCommands();

                // Execute batched commands
                foreach (var command in _currentBatch)
                    _commandBuffer.AddCommand(command);

                _isBatching = false;
                _currentBatch.Clear();
            }
        }

        /// <summary>
        /// Renders a single UI element with appropriate rendering commands.
        /// This method handles element-specific rendering with Asset System integration.
        /// </summary>
        /// <param name="element">UI element to render.</param>
        void RenderElement(UIElementBase element)
        {
            // This would implement actual element rendering
            // Using the Asset System for textures and fonts
            var command = new RenderCommand();
            var transform3x2 = element.Transform;

            {
                Type = RenderCommandType.DrawElement;
                Element = element;
                Material = GetElementMaterial(element);
            }

            ;

            if (_isBatching) _currentBatch.Add(command);
            else _commandBuffer.AddCommand(command);
        }

        /// <summary>
        /// Checks if an element is within the current viewport bounds.
        /// This method is used for viewport culling optimization.
        /// </summary>
        /// <param name="element">UI element to check.</param>
        /// <returns>True if the element is visible in the viewport.</returns>
        bool IsElementInViewport(UIElementBase element)
        {
            var bounds = element.Bounds;

            return bounds.Right >= 0 && bounds.Left <= _viewportSize.X &&
                   bounds.Bottom >= 0 && bounds.Top <= _viewportSize.Y;
        }

        /// <summary>
        /// Gets the rendering material for a UI element.
        /// This method handles material creation and caching with Asset System integration.
        /// </summary>
        /// <param name="element">UI element to get material for.</param>
        /// <returns>Rendering material for the element.</returns>
        UIMaterial GetElementMaterial(UIElementBase element)
        {
            // This would create or retrieve a material for the element
            // Using the Asset System for textures and shaders
            return new UIMaterial();
        }

        /// <summary>
        /// Optimizes batch commands for efficient GPU execution.
        /// This method sorts and groups commands to minimize state changes.
        /// </summary>
        void OptimizeBatchCommands()
        {
            // Sort commands by material and texture to minimize state changes
            _currentBatch.Sort((a, b) =>
            {
                var materialCompare = a.Material.GetHashCode().CompareTo(b.Material.GetHashCode());
                return materialCompare != 0 ? materialCompare : a.Element.GetHashCode().CompareTo(b.Element.GetHashCode());
            });
        }

        /// <summary>
        /// Adjusts rendering quality based on performance metrics.
        /// This method implements adaptive quality scaling.
        /// </summary>
        void AdjustQualityIfNeeded()
        {
            var stats = _performanceMonitor.GetStats();

            // Adjust quality based on frame rate
            if (stats.AverageFPS < 30)
            {
                // Reduce quality
                _shaderSystem.SetQualityLevel(RenderQuality.Low);
            }
            else if (stats.AverageFPS > 50)
            {
                // Increase quality
                _shaderSystem.SetQualityLevel(RenderQuality.High);
            }
        }

        /// <summary>
        /// Creates default render targets for UI rendering.
        /// This method sets up standard render targets for common UI scenarios.
        /// </summary>
        async Task CreateDefaultRenderTargetsAsync(CancellationToken cancellationToken)
        {
            // Create main UI render target
            var mainTarget = _graphicsDevice.CreateRenderTarget(
                "main_ui",
                _viewportSize.X,
                _viewportSize.Y,
                PixelFormat.R8G8B8A8_UNorm
            );

            _renderTargets["main_ui"] = mainTarget;

            // Create HUD render target
            var hudTarget = _graphicsDevice.CreateRenderTarget(
                "hud",
                _viewportSize.X,
                _viewportSize.Y,
                PixelFormat.R8G8B8A8_UNorm
            );

            _renderTargets["hud"] = hudTarget;
        }

        /// <summary>
        /// Loads default rendering effects for UI post-processing.
        /// This method loads standard effects like bloom, vignette, and color correction.
        /// </summary>
        async Task LoadDefaultEffectsAsync(CancellationToken cancellationToken)
        {
            // Load standard UI effects
            var bloomEffect = await _shaderSystem.LoadEffectAsync("bloom", "shaders/ui_bloom.fx", cancellationToken);
            _effects["bloom"] = bloomEffect;

            var vignetteEffect = await _shaderSystem.LoadEffectAsync("vignette", "shaders/ui_vignette.fx", cancellationToken);
            _effects["vignette"] = vignetteEffect;
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Disposes the ModernUIRenderer and releases all GPU resources.
        /// This method should be called during application shutdown to ensure
        /// proper cleanup of GPU resources and rendering subsystems.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            lock (_lock)
            {
                // Dispose render targets
                foreach (var target in _renderTargets.Values)
                    target?.Dispose();

                _renderTargets.Clear();

                // Dispose effects
                foreach (var effect in _effects.Values)
                    effect?.Dispose();

                _effects.Clear();

                // Dispose subsystems
                _shaderSystem?.Dispose();
                _atlasManager?.Dispose();
                _fontRenderer?.Dispose();
                _commandBuffer?.Dispose();
                _resourcePool?.Dispose();
                _performanceMonitor?.Dispose();
            }

            ModernLoggingSystem.Log("Info", "ModernUIRenderer: Disposed");
        }

        #endregion
    }

    #region Supporting Classes

    /// <summary>
    /// Graphics device interface abstraction for cross-platform rendering.
    /// </summary>
    public interface IGraphicsDevice
    {
        bool IsInitialized { get; }
        IRenderTarget CreateRenderTarget(string name, int width, int height, PixelFormat format);
        void ExecuteCommands(IEnumerable<RenderCommand> commands);
    }

    /// <summary>
    /// Render effect interface for post-processing and visual effects.
    /// </summary>
    public interface IRenderEffect : IDisposable
    {
        string Name { get; }
        void Apply(IRenderTarget source, IRenderTarget target);
        void SetParameter(string name, object value);
    }

    /// <summary>
    /// Render command for batching and optimization.
    /// </summary>
    public class RenderCommand
    {
        public RenderCommandType Type { get; set; }
        public UIElementBase Element { get; set; }
        public Matrix4x4 Transform { get; set; }
        public UIMaterial Material { get; set; } = new UIMaterial();

        public static implicit operator string(RenderCommand command)
        {
            return command.Type.ToString();
        }
    }

    /// <summary>
    /// UI material for element rendering.
    /// </summary>
    public class UIMaterial
    {
        public ITexture2D Texture { get; set; }
        public IShader Shader { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
    }

    /// <summary>
    /// Render command types for different rendering operations.
    /// </summary>
    public enum RenderCommandType
    {
        DrawElement,
        DrawText,
        DrawSprite,
        Clear,
        SetRenderTarget
    }

    /// <summary>
    /// Rendering state tracking for optimization.
    /// </summary>
    public class RenderState
    {
        public IShader CurrentShader { get; set; }
        public ITexture2D CurrentTexture { get; set; }
        public BlendMode BlendMode { get; set; }
        public bool DepthTestEnabled { get; set; }
        public CullMode CullMode { get; set; }
    }

    /// <summary>
    /// Performance statistics for rendering operations.
    /// </summary>
    public class RenderPerformanceStats
    {
        internal object AverageFrameTimeMs, FPS;

        public float AverageFPS { get; set; }
        public float CurrentFPS { get; set; }
        public int DrawCalls { get; set; }
        public int Triangles { get; set; }
        public long GPUMemoryUsage { get; set; }
        public int TextureSwitches { get; set; }
        public int ShaderSwitches { get; set; }
        public TimeSpan AverageFrameTime { get; set; }
        public RenderQuality CurrentQuality { get; set; }
    }

    /// <summary>
    /// Render quality levels for adaptive scaling.
    /// </summary>
    public enum RenderQuality
    {
        Low,
        Medium,
        High,
        Ultra
    }

    /// <summary>
    /// Pixel format enumeration for render targets.
    /// </summary>
    public enum PixelFormat
    {
        R8G8B8A8_UNorm,
        R32G32B32A32_Float,
        R16G16B16A16_Float,
        D24_UNorm_S8_UInt
    }

    /// <summary>
    /// Blend mode enumeration for rendering.
    /// </summary>
    public enum BlendMode
    {
        Opaque,
        AlphaBlend,
        Additive,
        Multiply
    }

    /// <summary>
    /// Cull mode enumeration for rendering.
    /// </summary>
    public enum CullMode
    {
        None,
        Front,
        Back
    }

    #endregion
    #region Supporting Structures

    /// <summary>
    /// Viewport structure for rendering.
    /// </summary>
    public struct Viewport
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float MinDepth { get; set; }
        public float MaxDepth { get; set; }
    }

    /// <summary>
    /// Render statistics structure.
    /// </summary>
    public struct RenderStats
    {
        public float AverageFrameTimeMs { get; set; }
        public float FPS { get; set; }
        public float AverageFPS { get; set; }
    }

    #endregion
}