using System;
using SASZombieAssaultTD.Engine.Rendering;
using RenderSystem = SASZombieAssaultTD.Engine.Rendering;

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    /// <summary>
    /// Rendering methods for drawing UI elements using the engine's rendering system
    /// P80-03-01: UIRenderer providing rendering methods for drawing UI elements using the engine's rendering system
    /// </summary>
    public class UIRenderer
    {
        private readonly UIBatcher _batcher;
        private UIRenderContext _context;
        private bool _isInitialized = false;

        /// <summary>
        /// Gets whether the renderer is initialized
        /// </summary>
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// Gets the current render context
        /// </summary>
        public UIRenderContext Context => _context;

        /// <summary>
        /// Initializes the UI renderer
        /// </summary>
        public UIRenderer()
        {
            _batcher = new UIBatcher();
            _context = new UIRenderContext();
            Console.WriteLine("UIRenderer: Initialized");
        }

        /// <summary>
        /// Begins a new rendering frame
        /// </summary>
        public void BeginFrame()
        {
            try
            {
                if (!_isInitialized)
                {
                    Console.WriteLine("UIRenderer: Cannot begin frame - not initialized");
                    return;
                }

                _context.Reset();
                _batcher.Clear();

                Console.WriteLine("UIRenderer: Began new render frame");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIRenderer: Error beginning frame - {ex.Message}");
            }
        }

        /// <summary>
        /// Ends the current rendering frame and submits batches
        /// </summary>
        public void EndFrame()
        {
            try
            {
                if (!_isInitialized)
                {
                    Console.WriteLine("UIRenderer: Cannot end frame - not initialized");
                    return;
                }

                // Batch all draw calls
                _batcher.Batch();

                // Submit batches to rendering system
                SubmitBatches();

                Console.WriteLine("UIRenderer: Ended render frame");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIRenderer: Error ending frame - {ex.Message}");
            }
        }

        /// <summary>
        /// Renders a UI element
        /// </summary>
        /// <param name="element">UI element to render</param>
        public void RenderElement(UIElement element)
        {
            try
            {
                if (element == null)
                {
                    Console.WriteLine("UIRenderer: Cannot render null element");
                    return;
                }

                if (!element.IsVisible)
                {
                    Console.WriteLine("UIRenderer: Element is not visible, skipping render");
                    return;
                }

                Console.WriteLine($"UIRenderer: Rendering element at {element.AbsolutePosition}");

                // Set up render context for this element
                _context.SetTransform(RenderSystem.Matrix.CreateTranslation(element.AbsolutePosition.X, element.AbsolutePosition.Y, 0f));
                _context.SetAlpha(1.0f);

                // Render the element based on its type
                RenderElementByType(element);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIRenderer: Error rendering element - {ex.Message}");
            }
        }

        /// <summary>
        /// Renders an element based on its specific type
        /// </summary>
        /// <param name="element">Element to render</param>
        private void RenderElementByType(UIElement element)
        {
            try
            {
                // This would be implemented with specific rendering logic
                // For now, render as a simple rectangle
                var rect = new System.Drawing.RectangleF(0, 0, element.Size.Width, element.Size.Height);
                _batcher.DrawRectangle(rect, System.Drawing.Color.White);

                Console.WriteLine($"UIRenderer: Rendered element as rectangle {rect}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIRenderer: Error rendering element by type - {ex.Message}");
            }
        }

        /// <summary>
        /// Renders text
        /// </summary>
        /// <param name="text">Text to render</param>
        /// <param name="position">Position to render at</param>
        /// <param name="font">Font to use</param>
        /// <param name="color">Color to use</param>
        public void RenderText(string text, System.Drawing.PointF position, string font, System.Drawing.Color color)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                {
                    Console.WriteLine("UIRenderer: Cannot render null or empty text");
                    return;
                }

                _batcher.DrawText(text, position, font, color);
                Console.WriteLine($"UIRenderer: Rendered text '{text}' at {position}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIRenderer: Error rendering text - {ex.Message}");
            }
        }

        /// <summary>
        /// Renders a rectangle
        /// </summary>
        /// <param name="rect">Rectangle to render</param>
        /// <param name="color">Color to use</param>
        /// <param name="texture">Texture to use (optional)</param>
        public void RenderRectangle(System.Drawing.RectangleF rect, System.Drawing.Color color, string? texture = null)
        {
            try
            {
                _batcher.DrawRectangle(rect, color, texture);
                Console.WriteLine($"UIRenderer: Rendered rectangle {rect} with color {color}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIRenderer: Error rendering rectangle - {ex.Message}");
            }
        }

        /// <summary>
        /// Renders a line
        /// </summary>
        /// <param name="start">Start point</param>
        /// <param name="end">End point</param>
        /// <param name="color">Color to use</param>
        /// <param name="thickness">Line thickness</param>
        public void RenderLine(System.Drawing.PointF start, System.Drawing.PointF end, System.Drawing.Color color, float thickness = 1.0f)
        {
            try
            {
                _batcher.DrawLine(start, end, color, thickness);
                Console.WriteLine($"UIRenderer: Rendered line from {start} to {end}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIRenderer: Error rendering line - {ex.Message}");
            }
        }

        /// <summary>
        /// Sets the clipping rectangle
        /// </summary>
        /// <param name="rect">Clipping rectangle</param>
        public void SetClipRect(System.Drawing.RectangleF rect)
        {
            try
            {
                // TODO: Implement SetClipRect method
                // _context.SetClipRect(rect);
                Console.WriteLine($"UIRenderer: Set clip rect {rect}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIRenderer: Error setting clip rect - {ex.Message}");
            }
        }

        /// <summary>
        /// Clears the clipping rectangle
        /// </summary>
        public void ClearClipRect()
        {
            try
            {
                _context.ClearClipRect();
                Console.WriteLine("UIRenderer: Cleared clip rect");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIRenderer: Error clearing clip rect - {ex.Message}");
            }
        }

        /// <summary>
        /// Sets the global alpha for rendering
        /// </summary>
        /// <param name="alpha">Alpha value (0.0 to 1.0)</param>
        public void SetGlobalAlpha(float alpha)
        {
            try
            {
                _context.SetAlpha(alpha);
                Console.WriteLine($"UIRenderer: Set global alpha to {alpha}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIRenderer: Error setting global alpha - {ex.Message}");
            }
        }

        /// <summary>
        /// Submits all batches to the rendering system
        /// </summary>
        private void SubmitBatches()
        {
            try
            {
                foreach (var batch in _batcher.GetBatches())
                {
                    // This would integrate with the engine's actual rendering system
                    // For now, just log the batch submission
                    Console.WriteLine($"UIRenderer: Submitting batch with {batch.Count} draw calls");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIRenderer: Error submitting batches - {ex.Message}");
            }
        }

        /// <summary>
        /// Initializes the renderer
        /// </summary>
        public void Initialize()
        {
            try
            {
                if (_isInitialized)
                {
                    Console.WriteLine("UIRenderer: Already initialized");
                    return;
                }

                _isInitialized = true;
                Console.WriteLine("UIRenderer: Initialized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIRenderer: Error during initialization - {ex.Message}");
            }
        }

        /// <summary>
        /// Shuts down the renderer
        /// </summary>
        public void Shutdown()
        {
            try
            {
                if (!_isInitialized)
                {
                    Console.WriteLine("UIRenderer: Already shut down");
                    return;
                }

                _isInitialized = false;
                _batcher.Clear();
                _context.Reset();

                Console.WriteLine("UIRenderer: Shut down successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIRenderer: Error during shutdown - {ex.Message}");
            }
        }
    }
}




