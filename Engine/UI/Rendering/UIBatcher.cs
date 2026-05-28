using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    /// <summary>
    /// Batching logic for UI draw calls to optimize rendering performance
    /// P80-03-02: UIBatcher providing batching logic for UI draw calls
    /// </summary>
    public class UIBatcher
    {
        private readonly List<UIDrawCall> _drawCalls;
        private readonly Dictionary<string, UIBatch> _batches;
        private bool _needsSorting = true;

        /// <summary>
        /// Gets the number of draw calls in the batcher
        /// </summary>
        public int DrawCallCount => _drawCalls.Count;

        /// <summary>
        /// Gets the number of batches
        /// </summary>
        public int BatchCount => _batches.Count;

        /// <summary>
        /// Initializes a new UIBatcher
        /// </summary>
        public UIBatcher()
        {
            _drawCalls = new List<UIDrawCall>();
            _batches = new Dictionary<string, UIBatch>();
            System.Diagnostics.Debug.WriteLine("UIBatcher: Initialized");
        }

        /// <summary>
        /// Adds a draw call to the batcher
        /// </summary>
        /// <param name="drawCall">Draw call to add</param>
        public void AddDrawCall(UIDrawCall drawCall)
        {
            try
            {
                if (drawCall == null)
                {
                    System.Diagnostics.Debug.WriteLine("UIBatcher: Cannot add null draw call");
                    return;
                }

                _drawCalls.Add(drawCall);
                _needsSorting = true;

                System.Diagnostics.Debug.WriteLine($"UIBatcher: Added draw call, total: {_drawCalls.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIBatcher: Error adding draw call - {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a rectangle draw call
        /// </summary>
        /// <param name="rect">Rectangle to draw</param>
        /// <param name="color">Color to use</param>
        /// <param name="texture">Texture to use (optional)</param>
        public void DrawRectangle(System.Drawing.RectangleF rect, System.Drawing.Color color, string? texture = null)
        {
            try
            {
                var drawCall = new UIDrawCall
                {
                    Type = UIDrawCallType.Rectangle,
                    Rectangle = rect,
                    Color = color,
                    Texture = texture,
                    Depth = GetNextDepth()
                };

                AddDrawCall(drawCall);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIBatcher: Error drawing rectangle - {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a text draw call
        /// </summary>
        /// <param name="text">Text to draw</param>
        /// <param name="position">Position to draw at</param>
        /// <param name="font">Font to use</param>
        /// <param name="color">Color to use</param>
        public void DrawText(string text, System.Drawing.PointF position, string font, System.Drawing.Color color)
        {
            try
            {
                var drawCall = new UIDrawCall
                {
                    Type = UIDrawCallType.Text,
                    Text = text,
                    Position = position,
                    Font = font,
                    Color = color,
                    Depth = GetNextDepth()
                };

                AddDrawCall(drawCall);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIBatcher: Error drawing text - {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a line draw call
        /// </summary>
        /// <param name="start">Start point</param>
        /// <param name="end">End point</param>
        /// <param name="color">Color to use</param>
        /// <param name="thickness">Line thickness</param>
        public void DrawLine(System.Drawing.PointF start, System.Drawing.PointF end, System.Drawing.Color color, float thickness = 1.0f)
        {
            try
            {
                var drawCall = new UIDrawCall
                {
                    Type = UIDrawCallType.Line,
                    Start = start,
                    End = end,
                    Color = color,
                    Thickness = thickness,
                    Depth = GetNextDepth()
                };

                AddDrawCall(drawCall);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIBatcher: Error drawing line - {ex.Message}");
            }
        }

        /// <summary>
        /// Batches all draw calls for efficient rendering
        /// </summary>
        public void Batch()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"UIBatcher: Batching {_drawCalls.Count} draw calls");

                // Clear existing batches
                _batches.Clear();

                // Sort draw calls by depth if needed
                if (_needsSorting)
                {
                    _drawCalls.Sort((a, b) => a.Depth.CompareTo(b.Depth));
                    _needsSorting = false;
                }

                // Group draw calls by batch key
                foreach (var drawCall in _drawCalls)
                {
                    var batchKey = GetBatchKey(drawCall);

                    if (!_batches.TryGetValue(batchKey, out var batch))
                    {
                        batch = new UIBatch
                        {
                            Type = drawCall.Type,
                            Texture = drawCall.Texture,
                            Font = drawCall.Font,
                            DrawCalls = new List<UIDrawCall>()
                        };
                        _batches[batchKey] = batch;
                    }

                    batch.DrawCalls.Add(drawCall);
                }

                System.Diagnostics.Debug.WriteLine($"UIBatcher: Created {_batches.Count} batches");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIBatcher: Error batching draw calls - {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all batches for rendering
        /// </summary>
        /// <returns>Collection of batches</returns>
        public IEnumerable<UIBatch> GetBatches()
        {
            try
            {
                return _batches.Values;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIBatcher: Error getting batches - {ex.Message}");
                return new List<UIBatch>();
            }
        }

        /// <summary>
        /// Clears all draw calls and batches
        /// </summary>
        public void Clear()
        {
            try
            {
                _drawCalls.Clear();
                _batches.Clear();
                _needsSorting = true;

                System.Diagnostics.Debug.WriteLine("UIBatcher: Cleared all draw calls and batches");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIBatcher: Error clearing - {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the next depth value for draw calls
        /// </summary>
        /// <returns>Next depth value</returns>
        private float GetNextDepth()
        {
            // Simple depth increment - in a real implementation this would be more sophisticated
            return _drawCalls.Count * 0.001f;
        }

        /// <summary>
        /// Gets the batch key for a draw call
        /// </summary>
        /// <param name="drawCall">Draw call to get key for</param>
        /// <returns>Batch key</returns>
        private string GetBatchKey(UIDrawCall drawCall)
        {
            try
            {
                // Create a batch key based on draw call properties
                var key = $"{drawCall.Type}_{drawCall.Texture}_{drawCall.Font}";
                return key;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIBatcher: Error getting batch key - {ex.Message}");
                return "default";
            }
        }
    }

    /// <summary>
    /// Represents a single UI draw call
    /// </summary>
    public class UIDrawCall
    {
        public UIDrawCallType Type { get; set; }
        public System.Drawing.RectangleF Rectangle { get; set; }
        public System.Drawing.PointF Position { get; set; }
        public System.Drawing.PointF Start { get; set; }
        public System.Drawing.PointF End { get; set; }
        public System.Drawing.Color Color { get; set; }
        public string Texture { get; set; }
        public string Font { get; set; }
        public string Text { get; set; }
        public float Thickness { get; set; }
        public float Depth { get; set; }
    }

    /// <summary>
    /// Types of UI draw calls
    /// </summary>
    public enum UIDrawCallType
    {
        Rectangle,
        Text,
        Line,
        Circle,
        Triangle
    }

    /// <summary>
    /// Represents a batch of UI draw calls
    /// </summary>
    public class UIBatch
    {
        public UIDrawCallType Type { get; set; }
        public string Texture { get; set; }
        public string Font { get; set; }
        public List<UIDrawCall> DrawCalls { get; set; }

        /// <summary>
        /// Gets the number of draw calls in this batch
        /// </summary>
        public int Count => DrawCalls?.Count ?? 0;
    }
}




